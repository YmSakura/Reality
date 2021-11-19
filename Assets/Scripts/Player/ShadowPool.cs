using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����
public class ShadowPool : MonoBehaviour
{
    //����ģʽ
    public static ShadowPool instance;

    //shadow��Ԥ���壬��������ʵ��
    public GameObject shadowPrefab;

    //������ж�������
    public int shadowCount;     

    //shadow����
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    private void Awake()
    {
        //��ʼ������
        if(instance != null)
            Destroy(this);
        instance = this;
        //��ʼ�������
        FillPool();
    }

    //ʵ����һ��������ShadowPrefab��Object������������еȴ�ʹ��
    public void FillPool()
    {
        for (int i = 0; i < shadowCount; i++)
        {
            var newShadow = Instantiate(shadowPrefab);
            //ʹ���ΪShadowPool��������
            newShadow.transform.SetParent(transform);

            //����ֻ�ǳ�ʼ�����������ã��������ؼ���
            IntoPool(newShadow);
        }
    }

    //�������أ���ʼ���Լ���̽�������ã�
    public void IntoPool(GameObject shadowObj)
    {
        //Ĭ�ϲ���ʾ
        shadowObj.SetActive(false);
        //��ӵȴ��´�ʹ��
        availableObjects.Enqueue(shadowObj);
    }

    //�Ӷ������ȡ��һ����Ϊ��Ӱ
    public void GetFromPool()
    {
        //�������shadow�ù��ˣ�������һ��
        if (availableObjects.Count == 0)
            FillPool();
        
        //����һ��������
        var outShadow = availableObjects.Dequeue();
        outShadow.SetActive(true);
    }
}
