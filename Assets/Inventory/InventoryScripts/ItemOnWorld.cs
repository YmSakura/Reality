using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这是实际游戏场景中item的脚本
public class ItemOnWorld : MonoBehaviour
{
    //将自身的item拖过来
    public Item thisItem;
    //背包
    public Inventory myBag;

    //玩家触发
    private void OnTriggerEnter2D(Collider2D other)
    {
        //人物吃到了item就将当前item加入背包
        if (other.gameObject.CompareTag("Player"))
        {
            AddNewItem();
            Destroy(gameObject);
        }
    }

    //将Item加入Inventory
    public void AddNewItem()
    {
        //如果背包里没有，就加入背包
        if (!myBag.itemList.Contains(thisItem))
        {
            //第一种写法，每次捡到新的item才将其加入itemList，并为其更新UI显示
            //MyBag.itemList.Add(thisItem);
            //InventoryManager.CreateNewItem(thisItem);
            
            //初始化item数量为1
            thisItem.itemHeld = 1;
            //第二种写法，itemList在游戏开始时已经初始化，只需判断当前元素是否为空即可
            for (int i = 0; i < myBag.itemList.Count; i++)
            {
                //遍历myBag的itemList数组，如果当前元素为空就把item放进去
                if (myBag.itemList[i] == null)
                {
                    myBag.itemList[i] = thisItem;
                    break;
                }
            }
        }
        else
        {
            //如果背包里已经有了，Item的数量++
            thisItem.itemHeld++;
        }
        
        //每次添加完都刷新一次背包
        InventoryManager.RefreshItem();
    }
}
