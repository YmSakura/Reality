using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //单例
    private static InventoryManager instance;

    public Inventory myBag;
    public GameObject slotGrid;
    //public Slot slotPrefab;
    public GameObject emptySlot;
    public Text itemInfo;
    
    //存放所有的Slot实例
    public List<GameObject> slots = new List<GameObject>();

    private void Awake()
    {
        //如果场景中已经存在该对象，就销毁已经存在的对象
        if(instance != null)
            Destroy(this);
        instance = this;
    }
    
    private void OnEnable()
    {
        //每次随着游戏启动时更新背包里的item信息
        RefreshItem();
    }
    
    public static void UpdateItemInfo(string itemDescription)
    {
        instance.itemInfo.text = itemDescription;
    }

    //更新slot的UI
    /*public static void CreateNewSlot(Item item)
    {
        //生成一个新的Slot并以slotGrid为parent，它会按照slotGrid的布局摆放
        Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(instance.slotGrid.transform);
        
        //将玩家捡到的item的属性传给我们生成的Slot
        newItem.slotItem = item;
        newItem.slotImage.sprite = item.itemImage;
        newItem.slotItemNum.text = item.itemHeld.ToString();
    }*/

    //更新item，游戏刚开始和每次玩家捡到新的Item都更新
    public static void RefreshItem()
    {
        //将slotGrid的子物体slot全部销毁
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            //别忘了清空slots数组，否则会重复创建很多Slot
            instance.slots.Clear();
        }

        //再根据myBag中的item重新创建slot
        for (int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            //第一种写法，根据数组元素的个数遍历，重新创建新的Slot
            //CreateNewSlot(instance.myBag.itemList[i]);
            
            //第二种写法，用slots数组保存所有的slot
            //因为itemList已经初始化，所以每次都是重新实例化所有元素，然后再更新每个slot的属性
            //实例化Slot并添加到slots数组
            instance.slots.Add(Instantiate(instance.emptySlot));
            //将slotGrid设置为每个slot的parent
            instance.slots[i].transform.SetParent(instance.slotGrid.transform);
            //更新每个slot的信息
            instance.slots[i].GetComponent<Slot>().slotID = i;
            instance.slots[i].GetComponent<Slot>().SetupSlot(instance.myBag.itemList[i]);
        }
    }

}
