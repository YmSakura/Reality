using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDialog : MonoBehaviour
{
    //�Ի����UI
    public GameObject enterDialog;

    //�������ǲų���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enterDialog.SetActive(true);
        }
    }

    //�뿪����ʧ
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enterDialog.SetActive(false);
        }
    }
}
