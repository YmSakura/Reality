using System.Collections;
using System.Collections.Generic;
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
        Destroy(gameObject);
    }

    public void JumpOn()
    {
        animator.SetTrigger("death");
        deathAudio.Play();
    }


}
