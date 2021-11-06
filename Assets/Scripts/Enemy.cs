using UnityEngine;

public class Enemy : MonoBehaviour
{
    //protected 子类和父类共享
    protected Animator animator;
    protected AudioSource deathAudio;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        deathAudio = GetComponent<AudioSource>();
    }
    void Death()
    {
        //取消碰撞体必须写在Death里，如果提前取消的话受重力影响，会边爆炸边下落
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject);
    }

    public void JumpOn()
    {
        animator.SetTrigger("death");
        deathAudio.Play();
    }

}
