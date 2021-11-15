using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSprite : MonoBehaviour
{
    private Transform player;   

    private SpriteRenderer thisSprite;
    private SpriteRenderer playerSprite;

    private Color color;

    [Header("时间控制参数")]
    public float activeTime;    //显示的时长
    public float activeStart;   //开始显示的时间点

    [Header("不透明度控制")]
    public float alphaSet; //初始值
    public float alphaMultiplier;   //乘小数，让它逐渐变得透明
    private float alpha;


    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();

        //初始化alpha值
        alpha = alphaSet;

        //获取主角的Sprite贴图
        thisSprite.sprite = playerSprite.sprite;

        //复制主角的位置等信息
        transform.position = player.position;
        transform.localScale = player.localScale;
        transform.rotation = player.rotation;

        //记录启动时间
        activeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //逐渐变得透明
        alpha *= alphaMultiplier;

        //color.a = alpha;
        color = new Color(0.5f, 0.5f, 1, alpha);
        //为贴图加上颜色
        thisSprite.color = color;

        if(Time.time >= activeStart + activeTime)
        {
            //时间结束就让它返回对象池等待下次使用
            ShadowPool.instance.IntoPool(this.gameObject);
        }
    }
}
