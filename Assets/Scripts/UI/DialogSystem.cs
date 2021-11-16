using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("UI���")]
    public Text textContent;
    public Image faceImage;

    [Header("�ı��ļ�")]
    public TextAsset textFile;
    public int index;
    public float textSpeed;

    [Header("ͷ��")] public Sprite fox, sensei;

    //ȡ������Ч��
    private bool cancelTyping;
    //��ǰ���Ƿ����
    private bool textFinished;

    List<string> textList = new List<string>();

    void Awake()
    {
        GetTextFromFile(textFile);
    }
    
    void GetTextFromFile(TextAsset file)
    {
        //���list�Լ�index
        textList.Clear();

        //�����и��ı��ļ�������ʱ��������
        var lineDate = file.text.Split('\n');
        //����list
        foreach (var line in lineDate)
        {
            textList.Add(line);
        }
    }

    
    
    //�Ի���ÿ�δ��Զ����ŵ�һ�仰
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

    //Э��
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
        //��cancelTypingΪtrueʱ����ѭ��
        while (!cancelTyping && letter < textList[index].Length - 1)
        {
            textContent.text += textList[index][letter++];
            //������þ���ÿ��textSpeed��ִ��һ���������䣬���Ҳ���ִ��ѭ����������ݣ�ֱ��ѭ������
            yield return new WaitForSeconds(textSpeed);
        }

        textContent.text = textList[index];
        cancelTyping = false;
        textFinished = true;
        index++;
    }
    
    void SetText()
    {
        //�ı������رնԻ���
        if (Input.GetKeyDown(KeyCode.R) && index == textList.Count)
        {
            gameObject.SetActive(false);
            index = 0;
            return;
        }
        
        //��R�����ı����ݣ�ֻ��һ�н����ſ��Ը��µ���һ��
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (textFinished && !cancelTyping)
            {
                //�����һ���Ѿ���������û��ȡ������Ч���Ϳ���Э��
                StartCoroutine(SetTextUI());
            }
            else if (!textFinished)
            {
                //�����������һ�е��ı�����ʱ����R����ȡ������Ч��
                cancelTyping = !cancelTyping;
            }
        }
    }
}
