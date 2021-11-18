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
    public float activeTime;    //shadow显示的时长
    public float activeStart;   //开始显示的时间点

    [Header("不透明度控制")]
    public float alphaSet;          //初始值
    public float alphaMultiplier;   //乘小数，让它逐渐变得透明
    private float alpha;            //实际alpha值


    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();

        //初始化alpha值
        alpha = alphaSet;

        //获取主角的Sprite贴图
        thisSprite.sprite = playerSprite.sprite;

        //复制主角的transform等信息
        transform.position = player.position;
        transform.localScale = player.localScale;
        transform.rotation = player.rotation;

        //记录开始显示的时间点
        activeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //逐渐变得透明直至消失
        alpha *= alphaMultiplier;

        //color.a = alpha;
        //设置颜色和alpha值
        color = new Color(0.5f, 0.5f, 1, alpha);
        //为贴图加上颜色
        thisSprite.color = color;
        
        //时间结束就让它返回对象池等待下次使用
        if(Time.time >= activeStart + activeTime)
        {
            ShadowPool.instance.IntoPool(gameObject);
        }
    }
}
