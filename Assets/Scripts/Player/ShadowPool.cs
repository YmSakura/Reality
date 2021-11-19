using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//对象池
public class ShadowPool : MonoBehaviour
{
    //单例模式
    public static ShadowPool instance;

    //shadow的预制体，用来生成实例
    public GameObject shadowPrefab;

    //对象池中对象数量
    public int shadowCount;     

    //shadow队列
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    private void Awake()
    {
        //初始化单例
        if(instance != null)
            Destroy(this);
        instance = this;
        //初始化对象池
        FillPool();
    }

    //实例化一定数量的ShadowPrefab的Object出来，进入队列等待使用
    public void FillPool()
    {
        for (int i = 0; i < shadowCount; i++)
        {
            var newShadow = Instantiate(shadowPrefab);
            //使其成为ShadowPool的子物体
            newShadow.transform.SetParent(transform);

            //这里只是初始化，无需启用，进入对象池即可
            IntoPool(newShadow);
        }
    }

    //进入对象池（初始化以及冲刺结束后调用）
    public void IntoPool(GameObject shadowObj)
    {
        //默认不显示
        shadowObj.SetActive(false);
        //入队等待下次使用
        availableObjects.Enqueue(shadowObj);
    }

    //从对象池中取出一个作为残影
    public void GetFromPool()
    {
        //队列里的shadow用光了，就扩充一下
        if (availableObjects.Count == 0)
            FillPool();
        
        //出队一个拿来用
        var outShadow = availableObjects.Dequeue();
        outShadow.SetActive(true);
    }
}
