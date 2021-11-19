using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioMixer audioMixer;
    
    //��ʼ��Ϸ��������һ������
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    //�˳���Ϸ
    public void QuitGame()
    {
        Application.Quit();
    }

    //���䶯������������UI
    private void UIEnable()
    {
        //��ѧ�ĺ���Find��ͨ��Hierachy���ڵ�Ŀ¼����������
        GameObject.Find("Canvas/Menu/UI").SetActive(true);
    }

    //��ͣ��Ϸ
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    //�ص���Ϸ
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    //��Slider��������
    public void SetVolume(float value)
    {
        audioMixer.SetFloat("MainVolume", value);
    }

    //��������ť���¼��س���
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
