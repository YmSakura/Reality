using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("自身组件")]
    public Collider2D smallCollider, bigCollider; 
    private Rigidbody2D rb;
    private Animator animator;  

    [Header("检测相关组件")]
    public LayerMask ground;
    public Transform ceilingCheck, groundCheck;

    [Header("UI组件")]
    public Joystick joystick;
    public TextMeshProUGUI cherryNumber;
    public Image cdImage;
    public GameObject myBag;

    [Header("速度参数")]
    public float speed = 10f;
    public float jumpForce = 10f;

    [Header("计时计分参数")]
    private int score;
    private int jumpCount;          
    private float hurtTime;

    [Header("条件判断参数")] public bool isJoyStick;
    private bool jumpPressed, crouchHeld, dashPressed, openBagPressed;
    private bool isHurt, isGround, isDash, checkHead;     //下蹲时的检测

    [Header("输入参数")]
    private float horizontalAxis, faceDirection;
    private string horizontal = "Horizontal";
    private string jump = "Jump";
    private string crouch = "Crouch";

    [Header("Dash参数")]
    public float dashTime;          //冲刺总时间
    private float dashTimeLeft;      //冲刺剩余时间
    private float lastDash = -10;    //上一次冲刺时间点
    public float dashSpeed;         //冲刺速度
    private float dashCoolDown = 2;  //冲刺CD
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        isGround = OnGround();
        UpdateCdImage();
        DashCondition();
        Crouch();
        SwitchAnimation();
        ExtraJump();
        OpenMyBag();
    }

    private void FixedUpdate()
    {
        if (!isHurt) Movement();
        Dash();
    }

    //获取玩家各种输入
    void GetInput()
    {
        if(isJoyStick)
        {
            horizontalAxis = joystick.Horizontal;
        }
        else
        {
            //移动参数
            horizontalAxis = Input.GetAxis(horizontal);
            faceDirection = Input.GetAxisRaw(horizontal);
            //跳跃参数
            jumpPressed = Input.GetButtonDown(jump);
            //下蹲
            crouchHeld = Input.GetButton(crouch);
            //冲刺
            dashPressed = Input.GetKeyDown(KeyCode.J);
            //打开背包
            openBagPressed = Input.GetKeyDown(KeyCode.B);
        }
    }
    
    //移动
    void Movement()
    {
        //转向要分开写，因为移动端没有GetAxisRaw
        if (isJoyStick)
        {
            //手机端
            if (horizontalAxis > 0f)
                transform.localScale = new Vector3(1, 1, 1);
            if (horizontalAxis < 0f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            //PC端
            if (faceDirection != 0)
                transform.localScale = new Vector3(faceDirection, 1, 1);
        }
        
        //移动
        rb.velocity = new Vector2(horizontalAxis * speed, rb.velocity.y);
        
    }

    //切换动画效果
    private void SwitchAnimation()
    {
        //running的参数
        animator.SetFloat("running", Mathf.Abs(horizontalAxis));

        //从高处下落也要调整为falling状态
        if (rb.velocity.y < -2f && !isGround)
            animator.SetBool("falling", true);

        //先判断是否受伤，因为受伤状态不能做其他事
        if (isHurt)
        {
            animator.SetBool("hurting", true);
            //受伤时间为0.5s
            if (Time.time -  hurtTime > 0.5f)
            {
                animator.SetBool("hurting", false);
                isHurt = false;
            }
        }
        else if (animator.GetBool("jumping"))
        {
            //y轴速度向下即为falling
            if (rb.velocity.y <= 0)
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }
        }
        else if (isGround)
        {
            //接触地面关闭下落
            animator.SetBool("falling", false);
        }

    }

    //触发器检测
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //吃到樱桃
        if (collision.CompareTag("Collection"))
        {
            Cherry cherry = collision.GetComponent<Cherry>();
            cherry.Touch();
        }
        
        //碰到DeadLine
        if (collision.CompareTag("DeadLine"))
        {
            Destroy(gameObject);
            SoundManager.instance.bgm.Pause();
            SoundManager.instance.DeathAudio();
            //延时重启游戏
            //Invoke("Restart", 1.5f);
        }
    }

    //碰撞器检测
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //碰到敌人
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            Vector3 enemyPos = collision.gameObject.transform.position;
            
            //计算主角与敌人之间的水平距离
            float xDistance = transform.position.x - enemyPos.x;
            //垂直距离
            float yDistancce = groundCheck.position.y - enemyPos.y;
            
            //从敌人头顶下落，就消灭敌人
            if (animator.GetBool("falling") && yDistancce > 0.3f)
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, 10);
            }//从其他地方碰到敌人，受伤并被击退
            else if (xDistance > 0)
            {
                Hurt(5f);
            }else if(xDistance < 0)
            {
                Hurt(-5f);
            }
        }
    }

    //受伤函数
    private void Hurt(float hurtForce)
    {
        isHurt = true;
        SoundManager.instance.HurtAudio();
        rb.velocity = new Vector2(hurtForce, rb.velocity.y);
        //记录受伤的时间
        hurtTime = Time.time;
    }

    //下蹲
    private void Crouch()
    {
        //移动端
        if (isJoyStick)
        {
            if (joystick.Vertical < -0.5f)
            {
                animator.SetBool("crouching", true);
                //切换Collider
                bigCollider.enabled = false;
                smallCollider.enabled = true;
            }
            else
            {
                checkHead = true;
            }
        }
        
        //PC端
        if (crouchHeld && isGround)
        {
            animator.SetBool("crouching", true);
            //切换为小的圆形Collider
            bigCollider.enabled = false;
            smallCollider.enabled = true;
        }
        else if (!crouchHeld)
        {
            //当下蹲键松开时检测头顶有没有东西
            checkHead = true;
        }
        
        //头部检测
        HeadCheck();
        
    }
    void HeadCheck()
    {
        //头部检测
        if (checkHead)
        {
            //overlap是重叠的意思，函数的作用是检查是否有碰撞体进入了指定的圆形区域
            //当角色头顶没有ground的碰撞体时才可以结束下蹲状态
            if (!Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, ground))
            {
                animator.SetBool("crouching", false);
                //切换为大的Capsule Collider
                bigCollider.enabled = true;
                smallCollider.enabled = false;
                //关闭检测
                checkHead = false;
            }
        }
    }

    //多段跳
    void ExtraJump()
    {
        if (isGround)
        {
            //接触地面时恢复跳跃次数
            jumpCount = 1;
        }

        if ( (jumpPressed || joystick.Vertical > 0.5f) && jumpCount > 0 && !isHurt)
        {
            //如果按下跳跃键并且还有跳跃次数就可以跳跃
            Jump();
            jumpPressed = false;
        }
    }
    
    //单次跳跃
    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
        animator.SetBool("jumping", true);
        SoundManager.instance.JumpAudio();
        jumpCount--;
    }

    //冲刺CD UI恢复
    void UpdateCdImage()
    {
        cdImage.fillAmount -= 1.0f / dashCoolDown * Time.deltaTime;
    }
    
    //冲刺条件判断
    void DashCondition()
    {
        if(dashPressed)
        {
            //判断CD是否结束
            if ((Time.time - lastDash) >= dashCoolDown)
            {
                isDash = true;
                dashPressed = false;
                
                //重置冲刺剩余时间
                dashTimeLeft = dashTime;
                //记录本次冲刺时间点
                lastDash = Time.time;
                //技能图标开始冷却
                cdImage.fillAmount = 1.0f;
            }
        }
    }

    //冲刺
    private void Dash()
    {
        if(isDash)
        {
            if(dashTimeLeft <= 0)
            {
                //冲刺结束
                isDash = false;
                //冲刺结束的时候再往上冲一下
                if(!isGround)
                {
                    rb.velocity = new Vector2(gameObject.transform.localScale.x * dashSpeed, jumpForce);
                }
            }
            else
            {
                //从对象池中获取对象，作为残影
                ShadowPool.instance.GetFromPool();
                
                //如果处于空中，每次冲刺都加一个向上的力，向上冲
                if (rb.velocity.y > 0 && !isGround)
                {
                    rb.velocity = new Vector2(gameObject.transform.localScale.x * dashSpeed, jumpForce);
                }
                
                //水平冲刺
                rb.velocity = new Vector2(gameObject.transform.localScale.x * dashSpeed, rb.velocity.y);
                
                //刷新冲刺剩余时间
                dashTimeLeft -= Time.deltaTime;
            }
        }
        
    }

    
    //地面检测
    private bool OnGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
    }

    //樱桃计数
    public void CherryCount()
    {
        score++;
        //更新文本
        cherryNumber.text = score.ToString();
    }

    //打开背包
    void OpenMyBag()
    {
        if (openBagPressed)
        {
            //如果背包打开就关闭，如果关闭就打开
            myBag.SetActive(!myBag.activeSelf);
            openBagPressed = false;
        }
    }
    
}
