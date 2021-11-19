using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioMixer audioMixer;
    
    //开始游戏，进入下一个场景
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    //退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }

    //渐变动画结束后启动UI
    private void UIEnable()
    {
        //新学的函数Find，通过Hierachy窗口的目录来查找物体
        GameObject.Find("Canvas/Menu/UI").SetActive(true);
    }

    //暂停游戏
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    //回到游戏
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    //用Slider控制音量
    public void SetVolume(float value)
    {
        audioMixer.SetFloat("MainVolume", value);
    }

    //死后点击按钮重新加载场景
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
