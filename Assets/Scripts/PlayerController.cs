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

    public float speed = 400.0f;
    public float jumpForce = 650.0f;
    public int score;
    private float horizontalAxis;
    private float faceDirection;
    private int extraJump;
    private float hurtTime;


    private bool isHurt = false;        //是否受伤
    private bool checkHead = false;     //下蹲检测
    public bool isJoyStick = false;     //是否启动摇杆


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (!isHurt) Movement();
        ExtraJump();
        Crouch();
        SwitchAnimation();
    }

    //移动函数
    void Movement()
    {
        //移动设备
        if (isJoyStick)
        {
            //获得水平轴参数
            horizontalAxis = joystick.Horizontal;

            //转向
            if (horizontalAxis > 0f)
                transform.localScale = new Vector3(1, 1, 1);
            if (horizontalAxis < 0f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            //PC端
            //GetAxis ：-1 -> 0 ->1 平滑过度
            horizontalAxis = Input.GetAxis("Horizontal");
            //GetAxisRaw ：直接返回-1 0 1，没有中间值
            faceDirection = Input.GetAxisRaw("Horizontal");

            //角色转向
            if (faceDirection != 0)
                transform.localScale = new Vector3(faceDirection, 1, 1);
        }
   
        //角色移动
        rb.velocity = new Vector2(horizontalAxis * speed * Time.fixedDeltaTime, rb.velocity.y);
        //running参数的关联
        animator.SetFloat("running", Mathf.Abs(faceDirection));
    }

    //切换动画效果
    void SwitchAnimation()
    {
        //Debug.Log(rb.velocity.y);
        //从高处下落也要调整为falling状态
        if (rb.velocity.y < 2f && !onGround())
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
            //Debug.Log(rb.velocity.y);
            //y轴向上的速度消失，开始下落
            if (rb.velocity.y <= 0)
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }
        }
        else if (bigCollider.IsTouchingLayers(ground))
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
            Invoke("Restart", 1.5f);
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
            }//从左右碰到敌人，就受伤
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
        if (Input.GetButtonDown("Crouch") && onGround())
        {
            animator.SetBool("crouching", true);
            //切换Collider
            bigCollider.enabled = false;
            smallCollider.enabled = true;
        }
        else if (Input.GetButtonUp("Crouch"))
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
                //切换Collider
                bigCollider.enabled = true;
                smallCollider.enabled = false;
                //关闭检测
                checkHead = false;
            }
        }
    }

    //重新加载场景
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //多段跳
    void ExtraJump()
    {
        //二段跳，通过debug发现，第一次跳跃到空中extraJump的值是没变的，其实执行了extraJump--
        //但是由于初速度太小，导致刚跳到空中，依然可以检测到地面，所以extraJump又恢复了
        if (onGround())
            extraJump = 1;

        if (isJoyStick)
        {
            //移动端
            if (joystick.Vertical > 0.5f && extraJump > 0 && !isHurt)
                Jump();
        }
        else
        {
            //PC端
            if (Input.GetButtonDown("Jump") && extraJump > 0 && !isHurt)
                Jump();
        }
    }

    //跳跃
    private void Jump()
    {
        //通过修改速度来实现跳跃，x不变，y变大
        rb.velocity = Vector2.up * jumpForce * Time.fixedDeltaTime;
        extraJump--;
        SoundManager.instance.JumpAudio();
        animator.SetBool("jumping", true);
    }

    //地面检测
    private bool onGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
    }

    //樱桃计数
    public void CherryCount()
    {
        score++;
        cherryNumber.text = score.ToString();
    }

}
