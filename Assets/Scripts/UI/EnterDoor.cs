using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterDoor : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            //bulidIndex是在Build Settings里设置的
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
