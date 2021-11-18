using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����DeadLine���ϵĽű�
public class GameOver : MonoBehaviour
{
    //gameOver��UI
    public GameObject gameOverPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameOverPanel.SetActive(true);
        }
    }
}
