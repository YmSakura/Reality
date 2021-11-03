using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    public AudioSource jumpAudio, collectionAudio, hurtAudio;
    public Collider2D coll;
    public LayerMask ground;
    public Text cherryNumber;

    public float speed = 400.0f;
    public float jumpForce = 400.0f;
    public int score;
    //Ĭ��Ϊfalse
    private bool isHurt;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isHurt)
        {
            Movement();
        }
        SwitchAnimation();
    }

    void Movement()
    {
        //GetAxis ��-1 -> 0 ->1 ƽ������
        float horizontalAxis = Input.GetAxis("Horizontal");
        //GetAxisRaw ��ֱ�ӷ���-1 0 1��û���м�ֵ
        float faceDirection = Input.GetAxisRaw("Horizontal");

        //Time.deltaTime����Update��FixedUpdate����Զ�����
        //Time.fixedDeltaTime���۷������ﶼ�ǹ̶���0.02
        //Debug.Log(Time.fixedDeltaTime);

        //��ɫ�ƶ�
        rb.velocity = new Vector2(horizontalAxis * speed * Time.fixedDeltaTime, rb.velocity.y);
        //�����˶�����
        animator.SetFloat("running", Mathf.Abs(faceDirection));

        //����ת��
        if (faceDirection != 0)
        {
            transform.localScale = new Vector3(faceDirection, 1, 1);
        }

        //��ɫ��Ծ
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            //ͨ���޸��ٶ���ʵ����Ծ��x���䣬y���
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            jumpAudio.Play();
            animator.SetBool("jumping", true);
        }
    }

    //�л�����Ч��
    void SwitchAnimation()
    {
        if(rb.velocity.y < 0.1f && !coll.IsTouchingLayers(ground))
        {
            animator.SetBool("falling", true);
        }
        
        //������Ծ
        if (animator.GetBool("jumping"))
        {
            Debug.Log(rb.velocity.y);
            //y���ٶ���ʧ����ʼ����
            if(rb.velocity.y <= 0)
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }
        }else if (isHurt)
        {
            animator.SetBool("hurting", true);
            if (Mathf.Abs(rb.velocity.x) < 1.0f)
            {
                animator.SetBool("hurting", false);
                isHurt = false;
            }
        }
        else if (coll.IsTouchingLayers(ground))
        {
            animator.SetBool("falling", false);
        }

    }

    //�ռ���Ʒ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collection")
        {
            collectionAudio.Play();
            Destroy(collision.gameObject);
            score++;
            cherryNumber.text = score.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //��������ʱ
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (animator.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, 5);
            }
            else if(transform.position.x < collision.gameObject.transform.position.x)
            {
                isHurt = true;
                hurtAudio.Play();
                rb.velocity = new Vector2(-10, rb.velocity.y);
            }
            else if(transform.position.x > collision.gameObject.transform.position.x)
            {
                isHurt = true;
                hurtAudio.Play();
                rb.velocity = new Vector2(10, rb.velocity.y);
            }
        }
    }
}
