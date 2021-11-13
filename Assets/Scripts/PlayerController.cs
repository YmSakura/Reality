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


    private bool isHurt = false;        //�Ƿ�����
    private bool checkHead = false;     //�¶׼��
    public bool isJoyStick = false;     //�Ƿ�����ҡ��


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

    //�ƶ�����
    void Movement()
    {
        //�ƶ��豸
        if (isJoyStick)
        {
            //���ˮƽ�����
            horizontalAxis = joystick.Horizontal;

            //ת��
            if (horizontalAxis > 0f)
                transform.localScale = new Vector3(1, 1, 1);
            if (horizontalAxis < 0f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            //PC��
            //GetAxis ��-1 -> 0 ->1 ƽ������
            horizontalAxis = Input.GetAxis("Horizontal");
            //GetAxisRaw ��ֱ�ӷ���-1 0 1��û���м�ֵ
            faceDirection = Input.GetAxisRaw("Horizontal");

            //��ɫת��
            if (faceDirection != 0)
                transform.localScale = new Vector3(faceDirection, 1, 1);
        }
   
        //��ɫ�ƶ�
        rb.velocity = new Vector2(horizontalAxis * speed * Time.fixedDeltaTime, rb.velocity.y);
        //running�����Ĺ���
        animator.SetFloat("running", Mathf.Abs(faceDirection));
    }

    //�л�����Ч��
    void SwitchAnimation()
    {
        //Debug.Log(rb.velocity.y);
        //�Ӹߴ�����ҲҪ����Ϊfalling״̬
        if (rb.velocity.y < 2f && !onGround())
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
            //Debug.Log(rb.velocity.y);
            //y�����ϵ��ٶ���ʧ����ʼ����
            if (rb.velocity.y <= 0)
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }
        }
        else if (bigCollider.IsTouchingLayers(ground))
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
            Invoke("Restart", 1.5f);
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
            }//�������������ˣ�������
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
        if (Input.GetButtonDown("Crouch") && onGround())
        {
            animator.SetBool("crouching", true);
            //�л�Collider
            bigCollider.enabled = false;
            smallCollider.enabled = true;
        }
        else if (Input.GetButtonUp("Crouch"))
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
                //�л�Collider
                bigCollider.enabled = true;
                smallCollider.enabled = false;
                //�رռ��
                checkHead = false;
            }
        }
    }

    //���¼��س���
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //�����
    void ExtraJump()
    {
        //��������ͨ��debug���֣���һ����Ծ������extraJump��ֵ��û��ģ���ʵִ����extraJump--
        //�������ڳ��ٶ�̫С�����¸��������У���Ȼ���Լ�⵽���棬����extraJump�ָֻ���
        if (onGround())
            extraJump = 1;

        if (isJoyStick)
        {
            //�ƶ���
            if (joystick.Vertical > 0.5f && extraJump > 0 && !isHurt)
                Jump();
        }
        else
        {
            //PC��
            if (Input.GetButtonDown("Jump") && extraJump > 0 && !isHurt)
                Jump();
        }
    }

    //��Ծ
    private void Jump()
    {
        //ͨ���޸��ٶ���ʵ����Ծ��x���䣬y���
        rb.velocity = Vector2.up * jumpForce * Time.fixedDeltaTime;
        extraJump--;
        SoundManager.instance.JumpAudio();
        animator.SetBool("jumping", true);
    }

    //������
    private bool onGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
    }

    //ӣ�Ҽ���
    public void CherryCount()
    {
        score++;
        cherryNumber.text = score.ToString();
    }

}
