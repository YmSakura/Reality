using UnityEngine;

public class Enemy : MonoBehaviour
{
    //protected ����͸��๲��
    protected Animator animator;
    protected AudioSource deathAudio;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        deathAudio = GetComponent<AudioSource>();
    }
    void Death()
    {
        //ȡ����ײ�����д��Death������ǰȡ���Ļ�������Ӱ�죬��߱�ը������
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject);
    }

    public void JumpOn()
    {
        animator.SetTrigger("death");
        deathAudio.Play();
    }

}
