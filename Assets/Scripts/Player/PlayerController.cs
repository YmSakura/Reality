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

    [Header("�ٶȲ���")]
    public float speed = 10f;
    public float jumpForce = 10f;

    [Header("��ʱ�Ʒֲ���")]
    private int score;
    private int jumpCount;          
    private float hurtTime;

    [Header("�����жϲ���")]
    private bool isHurt, isGround, isJoyStick, jumpPressed, crouchHeld, isDash, dashPressed;
    private bool checkHead = false;     //�¶�ʱ�ļ��

    [Header("�������")]
    private float horizontalAxis, faceDirection;
    private string horizontal = "Horizontal";
    private string jump = "Jump";
    private string crouch = "Crouch";

    [Header("Dash����")]
    public float dashTime;          //���ʱ��
    private float dashTimeLeft;      //���ʣ��ʱ��
    private float lastDash = -10;    //��һ�γ��ʱ��
    public float dashSpeed;         //����ٶ�
    private float dashCoolDown = 2;  //���CD


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

    //��ȡ��Ҹ�������
    void GetInput()
    {
        if(isJoyStick)
        {
            horizontalAxis = joystick.Horizontal;
        }
        else
        {
            //�ƶ�����
            horizontalAxis = Input.GetAxis(horizontal);
            faceDirection = Input.GetAxisRaw(horizontal);
            //��Ծ����
            jumpPressed = Input.GetButtonDown(jump);
            //�¶�
            crouchHeld = Input.GetButton(crouch);
            //���
            dashPressed = Input.GetKeyDown(KeyCode.J);
        }
    }
    
    //�ƶ�����
    void Movement()
    {
        //�ֻ���
        if (isJoyStick)
        {
            //ת��Ҫ�ֿ�д����Ϊ�ƶ���û��GetAxisRaw
            if (horizontalAxis > 0f)
                transform.localScale = new Vector3(1, 1, 1);
            if (horizontalAxis < 0f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            //PC��
            if (faceDirection != 0)
                transform.localScale = new Vector3(faceDirection, 1, 1);
        }
        
        //�ƶ�
        rb.velocity = new Vector2(horizontalAxis * speed, rb.velocity.y);
        
    }

    //�л�����Ч��
    private void SwitchAnimation()
    {
        //running�Ĳ���
        animator.SetFloat("running", Mathf.Abs(horizontalAxis));

        //�Ӹߴ�����ҲҪ����Ϊfalling״̬
        if (rb.velocity.y < -2f && !isGround)
            animator.SetBool("falling", true);

        //���ж��Ƿ����ˣ���Ϊ����״̬������������
        if (isHurt)
        {
            animator.SetBool("hurting", true);
            //����ʱ��Ϊ0.5s
            if (Time.time -  hurtTime > 0.5f)
            {
                animator.SetBool("hurting", false);
                isHurt = false;
            }
        }
        else if (animator.GetBool("jumping"))
        {
            //y���ٶ����¼�Ϊfalling
            if (rb.velocity.y <= 0)
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }
        }
        else if (isGround)
        {
            //�ر�����
            animator.SetBool("falling", false);
        }

    }

    //���������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�Ե�ӣ��
        if (collision.tag == "Collection")
        {
            Cherry cherry = collision.GetComponent<Cherry>();
            cherry.Touch();
        }
        //����DeadLine
        if (collision.tag == "DeadLine")
        {
            SoundManager.instance.bgm.Pause();
            SoundManager.instance.DeathAudio();
            //��ʱ
            //Invoke("Restart", 1.5f);
        }
    }

    //�������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            //�������������֮��ľ���
            float distance = transform.position.x - collision.gameObject.transform.position.x;
            //�ӵ���ͷ�����䣬���������
            if (animator.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, 10);
            }//�������ط��������ˣ�������
            else if (distance > 0)
            {
                Hurt(5f);
            }else if(distance < 0)
            {
                Hurt(-5f);
            }
        }
    }

    //���˺���
    private void Hurt(float hurtForce)
    {
        isHurt = true;
        SoundManager.instance.HurtAudio();
        rb.velocity = new Vector2(hurtForce, rb.velocity.y);
        //��¼���˵�ʱ��
        hurtTime = Time.time;
    }

    //�¶�
    private void Crouch()
    {
        //�ƶ���
        if (isJoyStick)
        {
            if (joystick.Vertical < -0.5f)
            {
                animator.SetBool("crouching", true);
                //�л�Collider
                bigCollider.enabled = false;
                smallCollider.enabled = true;
            }
            else
            {
                checkHead = true;
            }
        }
        
        //PC��
        if (crouchHeld && isGround)
        {
            animator.SetBool("crouching", true);
            //�л�ΪС��Բ��Collider
            bigCollider.enabled = false;
            smallCollider.enabled = true;
        }
        else if (!crouchHeld)
        {
            //���¶׼��ɿ�ʱ���ͷ����û�ж���
            checkHead = true;
        }
        
        //ͷ�����
        if (checkHead)
        {
            //overlap���ص�����˼�������������Ǽ���Ƿ�����ײ�������ָ����Բ������
            //����ɫͷ��û��ground����ײ��ʱ�ſ��Խ����¶�״̬
            if (!Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, ground))
            {
                animator.SetBool("crouching", false);
                //�л�Ϊ��Ľ�����Collider
                bigCollider.enabled = true;
                smallCollider.enabled = false;
                //�رռ��
                checkHead = false;
            }
        }
    }


    //�����
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

    //���CDͼ�ָ�
    void UpdateCdImage()
    {
        cdImage.fillAmount -= 1.0f / dashCoolDown * Time.deltaTime;
    }
    
    //��������ж�
    void DashCondition()
    {
        if(dashPressed)
        {
            //�ж�CD�Ƿ����
            if ((Time.time - lastDash) >= dashCoolDown)
            {
                //ˢ�³��ʣ��ʱ��
                dashTimeLeft = dashTime;
                //��¼���ʱ��
                lastDash = Time.time;
                isDash = true;
                //�ָ�CDͼƬ
                cdImage.fillAmount = 1.0f;
            }
        }
    }

    //���
    private void Dash()
    {
        if(isDash)
        {
            if(dashTimeLeft <= 0)
            {
                isDash = false;
                //��̽�����ʱ�������ϳ�һ��
                if(!isGround)
                {
                    rb.velocity = new Vector2(gameObject.transform.localScale.x * dashSpeed, jumpForce);
                }
            }
            else
            {
                //������ڿ��У�ÿ�γ�̶���һ�����ϵ��������ϳ�
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

    //������Ծ
    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
        animator.SetBool("jumping", true);
        SoundManager.instance.JumpAudio();
        jumpCount--;
    }

    //������
    private bool OnGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);
    }

    //ӣ�Ҽ���
    public void CherryCount()
    {
        score++;
        cherryNumber.text = score.ToString();
    }
    
}
