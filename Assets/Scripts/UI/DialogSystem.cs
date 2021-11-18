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
    //文本数组的下标即行数
    public int index;
    //打字机的速度
    public float textSpeed;

    [Header("头像")] public Sprite fox, sensei;

    //是否取消打字机效果
    private bool cancelTyping;
    //是否完成打字
    private bool textFinished;

    //用来存放每一行的文本内容
    List<string> textList = new List<string>();

    void Awake()
    {
        GetTextFromFile(textFile);
    }
    
    void GetTextFromFile(TextAsset file)
    {
        //清空list以及index
        textList.Clear();

        //按行切割文本文件，用临时的String数组保存，每一行为数组中一个元素
        var lineDate = file.text.Split('\n');
        foreach (var line in lineDate)
        {
            //复制文本内容，因为数组没法直接赋值，所以采用for循环的方式
            textList.Add(line);
        }
    }

    
    
    //对话框每次打开自动播放第一句话
    private void OnEnable()
    {
        //不使用协程的做法，直接输出每一行的文本内容
        //textContent.text = textList[index++];
        
        //初始状态默认上一行文本已经打完
        textFinished = true;
        //开启协程
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
        //进入协程就代表处于typing状态，当协程结束时textFinished=true
        textFinished = false;
        //清空Text的内容
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

        //letter即每一行文本String数组的下标，为了和index区分
        int letter = 0;
        //当玩家按下R键将cancelTyping置为true时结束循环，否则逐字显示每一行的文本内容
        while (!cancelTyping && letter < textList[index].Length - 1)
        {
            textContent.text += textList[index][letter++];
            //这个作用就是每隔textSpeed秒执行一次循环，并且不会执行循环后面的内容，直到循环结束
            yield return new WaitForSeconds(textSpeed);
        }
        //如果循环未结束就退出循环，则直接输出此行文本内容
        textContent.text = textList[index];
        
        //此行输出结束，切换状态，index更新到下一行
        cancelTyping = false;
        textFinished = true;
        index++;
    }
    
    //更新文本内容
    void SetText()
    {
        //文本结束再按R则关闭对话框
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
                //如果上一行已经finished并且没有取消打字效果就开启协程
                StartCoroutine(SetTextUI());
            }
            else if (!textFinished)
            {
                //如果正在输出一行的文本，此时按下R可以取消打字机效果
                cancelTyping = !cancelTyping;
            }
        }
    }
}
