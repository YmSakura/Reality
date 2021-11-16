using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDialog : MonoBehaviour
{
    //对话框的UI
    public GameObject enterDialog;

    //碰到主角才出现
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enterDialog.SetActive(true);
        }
    }

    //离开就消失
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enterDialog.SetActive(false);
        }
    }
}
