using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSprite : MonoBehaviour
{
    private Transform player;   

    private SpriteRenderer thisSprite;
    private SpriteRenderer playerSprite;

    private Color color;

    [Header("ʱ����Ʋ���")]
    public float activeTime;    //shadow��ʾ��ʱ��
    public float activeStart;   //��ʼ��ʾ��ʱ���

    [Header("��͸���ȿ���")]
    public float alphaSet;          //��ʼֵ
    public float alphaMultiplier;   //��С���������𽥱��͸��
    private float alpha;            //ʵ��alphaֵ


    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();

        //��ʼ��alphaֵ
        alpha = alphaSet;

        //��ȡ���ǵ�Sprite��ͼ
        thisSprite.sprite = playerSprite.sprite;

        //�������ǵ�transform����Ϣ
        transform.position = player.position;
        transform.localScale = player.localScale;
        transform.rotation = player.rotation;

        //��¼��ʼ��ʾ��ʱ���
        activeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //�𽥱��͸��ֱ����ʧ
        alpha *= alphaMultiplier;

        //color.a = alpha;
        //������ɫ��alphaֵ
        color = new Color(0.5f, 0.5f, 1, alpha);
        //Ϊ��ͼ������ɫ
        thisSprite.color = color;
        
        //ʱ��������������ض���صȴ��´�ʹ��
        if(Time.time >= activeStart + activeTime)
        {
            ShadowPool.instance.IntoPool(gameObject);
        }
    }
}
