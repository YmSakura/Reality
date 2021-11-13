using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cherry : MonoBehaviour
{
    public AudioSource audioSource;
    public Animator animator;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Death()
    { 
        Destroy(gameObject);
    }

    public void Touch()
    {
        audioSource.Play();
        GetComponent<Collider2D>().enabled = false;
        FindObjectOfType<PlayerController>().CherryCount();
        animator.SetTrigger("death");
    }
}
