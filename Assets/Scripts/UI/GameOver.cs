using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//挂在DeadLine身上的脚本
public class GameOver : MonoBehaviour
{
    //gameOver的UI
    public GameObject gameOverPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameOverPanel.SetActive(true);
        }
    }
}
