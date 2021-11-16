using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("UI组件")]
    public Text textContent;
    public Image faceImage;

    [Header("文本文件")]
    public TextAsset textFile;
    public int index;
    public float textSpeed;

    [Header("头像")] public Sprite fox, sensei;

    //取消打字效果
    private bool cancelTyping;
    //当前行是否结束
    private bool textFinished;

    List<string> textList = new List<string>();

    void Awake()
    {
        GetTextFromFile(textFile);
    }
    
    void GetTextFromFile(TextAsset file)
    {
        //清空list以及index
        textList.Clear();

        //按行切割文本文件，用临时变量保存
        var lineDate = file.text.Split('\n');
        //填入list
        foreach (var line in lineDate)
        {
            textList.Add(line);
        }
    }

    
    
    //对话框每次打开自动播放第一句话
    private void OnEnable()
    {
        //textContent.text = textList[index++];
        textFinished = true;
        StartCoroutine(SetTextUI());
    }

    // Update is called once per frame
    void Update()
    {
        SetText();
    }

    //协程
    IEnumerator SetTextUI()
    {
        textFinished = false;
        textContent.text = "";

        switch (textList[index])
        {
            case "A\r":
                faceImage.sprite = fox;
                index++;
                break;
            case "B\r":
                faceImage.sprite = sensei;
                index++;
                break;
        }

        int letter = 0;
        //当cancelTyping为true时结束循环
        while (!cancelTyping && letter < textList[index].Length - 1)
        {
            textContent.text += textList[index][letter++];
            //这个作用就是每隔textSpeed秒执行一次上面的语句，并且不会执行循环后面的内容，直到循环结束
            yield return new WaitForSeconds(textSpeed);
        }

        textContent.text = textList[index];
        cancelTyping = false;
        textFinished = true;
        index++;
    }
    
    void SetText()
    {
        //文本结束关闭对话框
        if (Input.GetKeyDown(KeyCode.R) && index == textList.Count)
        {
            gameObject.SetActive(false);
            index = 0;
            return;
        }
        
        //按R更新文本内容，只有一行结束才可以更新到下一行
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (textFinished && !cancelTyping)
            {
                //如果上一行已经结束并且没有取消打字效果就开启协程
                StartCoroutine(SetTextUI());
            }
            else if (!textFinished)
            {
                //如果正在输入一行的文本，此时按下R可以取消打字效果
                cancelTyping = !cancelTyping;
            }
        }
    }
}
