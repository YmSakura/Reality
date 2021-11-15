using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPool : MonoBehaviour
{
    public static ShadowPool instance;

    public GameObject shadowPrefab;

    public int shadowCount;     //������ж�������

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    private void Awake()
    {
        instance = this;
        //��ʼ�������
        FillPool();
    }

    //����һ��������ShadowPrefab�����������������еȴ�ʹ��
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

    //��������
    public void IntoPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
        //���
        availableObjects.Enqueue(gameObject);
    }

    //�Ӷ������ȡ��һ����Ϊ��Ӱ
    public GameObject GetFromPool()
    {
        //�������shadow�ù��ˣ�������һ��
        if (availableObjects.Count == 0)
            FillPool();
        
        //����һ��������
        var outShadow = availableObjects.Dequeue();
        outShadow.SetActive(true);

        return outShadow;
    }
}
