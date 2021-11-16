using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkButton : MonoBehaviour
{
    //�����Ի���ť
    public GameObject talkButton;
    //�Ի���
    public GameObject talkUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            talkButton.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            talkButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(talkButton.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            talkUI.SetActive(true);
        }
        if(!talkButton.activeSelf)
        {
            talkUI.SetActive(false);
        }
    }
}
