using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    public AudioSource jumpAudio, collectionAudio, hurtAudio;
    public Collider2D coll;
    public Collider2D discoll;
    public LayerMask ground;
    public Text cherryNumber;
    public Transform ceilingCheck, floorCheck;

    public float speed = 400.0f;
    public float jumpForce = 400.0f;
    public int score;

    //�Ƿ�Ϊ����״̬
    private bool isHurt = false;
    //�¶׼��
    private bool checkHead = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Crouch();
        SwitchAnimation();
        if (!isHurt) Movement();
    }

    void Movement()
    {
        //GetAxis ��-1 -> 0 ->1 ƽ������
        float horizontalAxis = Input.GetAxis("Horizontal");
        //GetAxisRaw ��ֱ�ӷ���-1 0 1��û���м�ֵ
        float faceDirection = Input.GetAxisRaw("Horizontal");

        //��ɫ�ƶ�
        rb.velocity = new Vector2(horizontalAxis * speed * Time.fixedDeltaTime, rb.velocity.y);
        //running�����Ĺ���
        animator.SetFloat("running", Mathf.Abs(faceDirection));

        //��ɫת��
        if (faceDirection != 0)
            transform.localScale = new Vector3(faceDirection, 1, 1);

    }

    //�л�����Ч��
    void SwitchAnimation()
    {
        if (rb.velocity.y < 0.1f && !coll.IsTouchingLayers(ground))
        {
            animator.SetBool("falling", true);
        }

        //������Ծ
        if (isHurt)
        {
            animator.SetBool("hurting", true);
            if (Mathf.Abs(rb.velocity.x) < 3.0f)
            {
                animator.SetBool("hurting", false);
                isHurt = false;
            }
        }
        else if (animator.GetBool("jumping"))
        {
            //Debug.Log(rb.velocity.y);
            //y���ٶ���ʧ����ʼ����
            if (rb.velocity.y <= 0)
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }
        }
        else if (coll.IsTouchingLayers(ground))
        {
            animator.SetBool("falling", false);
        }

    }

    //���������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�Ե�ӣ��
        if (collision.tag == "Collection")
        {
            collectionAudio.Play();
            collision.GetComponent<Animator>().Play("isGot");
        }
        //����DeadLine
        if (collision.tag == "DeadLine")
        {
            GetComponent<AudioSource>().enabled = false;
            //��ʱ2s
            Invoke("Restart", 2f);
        }
    }

    //�������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (animator.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, 10);
            }
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                isHurt = true;
                hurtAudio.Play();
                rb.velocity = new Vector2(-5, rb.velocity.y);
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                isHurt = true;
                hurtAudio.Play();
                rb.velocity = new Vector2(5, rb.velocity.y);
            }
        }
    }

    //�¶�
    private void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            animator.SetBool("crouching", true);
            //�ر�box collider
            discoll.enabled = false;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            //���¶׼�����ʱ�������
            checkHead = true;
        }
        //���ͷ���Ƿ���ground
        if (checkHead)
        {
            //overlap���ص�����˼�������������Ǽ���Ƿ�����ײ�������ָ����Բ������
            //����ɫͷ��û��ground����ײ��ʱ�ſ��Իָ�idle״̬
            if (!Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, ground))
            {
                animator.SetBool("crouching", false);
                discoll.enabled = true;
                //�رռ��
                checkHead = false;
            }
        }
    }

    //���¼��س���
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //��ɫ��Ծ
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && onGround() && !isHurt)
        {
            //ͨ���޸��ٶ���ʵ����Ծ��x���䣬y���
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            jumpAudio.Play();
            animator.SetBool("jumping", true);
        }
    }

    private bool onGround()
    {
        if (Physics2D.OverlapCircle(floorCheck.position, 0.2f, ground))
            return true;
        else
            return false;
    }

    public void CherryCount()
    {
        score++;
        cherryNumber.text = score.ToString();
    }

}
