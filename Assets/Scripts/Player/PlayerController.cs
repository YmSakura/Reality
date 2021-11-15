using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    
    public Collider2D smallCollider, bigCollider;
    public LayerMask ground;
    public TextMeshProUGUI cherryNumber;
    public Transform ceilingCheck, groundCheck;
    public Joystick joystick;
    public Image cdImage;

    [Header("速度参数")]
    public float speed = 10f;
    public float jumpForce = 10f;

    [Header("计时计分参数")]
    private int score;
    private int jumpCount;          
    private float hurtTime;

    [Header("条件判断参数")]
    private bool isHurt, isGround, isJoyStick, jumpPressed, crouchHeld, isDash, dashPressed;
    private bool checkHead = false;     //下蹲时的检测

    [Header("输入参数")]
    private float horizontalAxis, faceDirection;
    private string horizontal = "Horizontal";
    private string jump = "Jump";
    private string crouch = "Crouch";

    [Header("Dash参数")]
    public float dashTime;          //冲刺时间
    private float dashTimeLeft;      //冲刺剩余时间
    private float lastDash = -10;    //上一次冲刺时间
    public float dashSpeed;         //冲刺速度
    private float dashCoolDown = 2;  //冲刺CD


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //bigCollider = GetComponent<Collider2D>();
        //smallCollider = GetComponent<Collider2D>();
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
        }
    }
    
    //移动函数
    void Movement()
    {
        //手机端
        if (isJoyStick)
        {
            //转向要分开写，因为移动端没有GetAxisRaw
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
            //关闭下落
            animator.SetBool("falling", false);
        }

    }

    //触发器检测
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //吃到樱桃
        if (collision.tag == "Collection")
        {
            Cherry cherry = collision.GetComponent<Cherry>();
            cherry.Touch();
        }
        //碰到DeadLine
        if (collision.tag == "DeadLine")
        {
            SoundManager.instance.bgm.Pause();
            SoundManager.instance.DeathAudio();
            //延时
            //Invoke("Restart", 1.5f);
        }
    }

    //消灭敌人
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            //计算主角与敌人之间的距离
            float distance = transform.position.x - collision.gameObject.transform.position.x;
            //从敌人头顶下落，就消灭敌人
            if (animator.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, 10);
            }//从其他地方碰到敌人，就受伤
            else if (distance > 0)
            {
                Hurt(5f);
            }else if(distance < 0)
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
        if (checkHead)
        {
            //overlap是重叠的意思，函数的作用是检查是否有碰撞体进入了指定的圆形区域
            //当角色头顶没有ground的碰撞体时才可以结束下蹲状态
            if (!Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, ground))
            {
                animator.SetBool("crouching", false);
                //切换为大的胶囊体Collider
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
            jumpCount = 2;
        }

        if ( (jumpPressed || joystick.Vertical > 0.5f) && jumpCount > 0 && !isHurt)
        {
            Jump();
            jumpPressed = false;
        }

    }

    //冲刺CD图恢复
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
                //刷新冲刺剩余时间
                dashTimeLeft = dashTime;
                //记录冲刺时间
                lastDash = Time.time;
                isDash = true;
                //恢复CD图片
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
                isDash = false;
                //冲刺结束的时候再往上冲一下
                if(!isGround)
                {
                    rb.velocity = new Vector2(gameObject.transform.localScale.x * dashSpeed, jumpForce);
                }
            }
            else
            {
                //如果处于空中，每次冲刺都加一个向上的力，向上冲
                if (rb.velocity.y > 0 && !isGround)
                {
                    rb.velocity = new Vector2(gameObject.transform.localScale.x * dashSpeed, jumpForce);
                }
                    
                rb.velocity = new Vector2(gameObject.transform.localScale.x * dashSpeed, rb.velocity.y);
                ShadowPool.instance.GetFromPool();
                dashTimeLeft -= Time.deltaTime;
            }
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

    //地面检测
    private bool OnGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
    }

    //樱桃计数
    public void CherryCount()
    {
        score++;
        cherryNumber.text = score.ToString();
    }
    
}
