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
    //�ı�������±꼴����
    public int index;
    //���ֻ����ٶ�
    public float textSpeed;

    [Header("ͷ��")] public Sprite fox, sensei;

    //�Ƿ�ȡ�����ֻ�Ч��
    private bool cancelTyping;
    //�Ƿ���ɴ���
    private bool textFinished;

    //�������ÿһ�е��ı�����
    List<string> textList = new List<string>();

    void Awake()
    {
        GetTextFromFile(textFile);
    }
    
    void GetTextFromFile(TextAsset file)
    {
        //���list�Լ�index
        textList.Clear();

        //�����и��ı��ļ�������ʱ��String���鱣�棬ÿһ��Ϊ������һ��Ԫ��
        var lineDate = file.text.Split('\n');
        foreach (var line in lineDate)
        {
            //�����ı����ݣ���Ϊ����û��ֱ�Ӹ�ֵ�����Բ���forѭ���ķ�ʽ
            textList.Add(line);
        }
    }

    
    
    //�Ի���ÿ�δ��Զ����ŵ�һ�仰
    private void OnEnable()
    {
        //��ʹ��Э�̵�������ֱ�����ÿһ�е��ı�����
        //textContent.text = textList[index++];
        
        //��ʼ״̬Ĭ����һ���ı��Ѿ�����
        textFinished = true;
        //����Э��
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
        //����Э�̾ʹ�����typing״̬����Э�̽���ʱtextFinished=true
        textFinished = false;
        //���Text������
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

        //letter��ÿһ���ı�String������±꣬Ϊ�˺�index����
        int letter = 0;
        //����Ұ���R����cancelTyping��Ϊtrueʱ����ѭ��������������ʾÿһ�е��ı�����
        while (!cancelTyping && letter < textList[index].Length - 1)
        {
            textContent.text += textList[index][letter++];
            //������þ���ÿ��textSpeed��ִ��һ��ѭ�������Ҳ���ִ��ѭ����������ݣ�ֱ��ѭ������
            yield return new WaitForSeconds(textSpeed);
        }
        //���ѭ��δ�������˳�ѭ������ֱ����������ı�����
        textContent.text = textList[index];
        
        //��������������л�״̬��index���µ���һ��
        cancelTyping = false;
        textFinished = true;
        index++;
    }
    
    //�����ı�����
    void SetText()
    {
        //�ı������ٰ�R��رնԻ���
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
                //�����һ���Ѿ�finished����û��ȡ������Ч���Ϳ���Э��
                StartCoroutine(SetTextUI());
            }
            else if (!textFinished)
            {
                //����������һ�е��ı�����ʱ����R����ȡ�����ֻ�Ч��
                cancelTyping = !cancelTyping;
            }
        }
    }
}
