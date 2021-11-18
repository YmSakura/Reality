using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Slot对应Unity中的Slot prefab
public class Slot : MonoBehaviour
{
    //Item是我们自己创建的小型数据库类，我们需要获取这个类中的itemInfo
    //public Item slotItem;

    public int slotID;
    
    //控制item的显示
    public GameObject itemInSlot;
    
    //Item的信息
    public Image slotImage;
    public Text slotItemNum;
    public string slotInfo;
    
    //当点击image时更新description
    public void ItemOnClicked()
    {
        InventoryManager.UpdateItemInfo(slotInfo);
    }

    //设置每个Slot的属性（其实就是设置Item）
    public void SetupSlot(Item item)
    {
        if (item == null)
        {
            //如果item为空就不显示这个item
            itemInSlot.SetActive(false);
            return;
        }
        //如果不为空就更新图片数字和描述信息
        slotImage.sprite = item.itemImage;
        slotItemNum.text = item.itemHeld.ToString();
        slotInfo = item.itemInfo;
    }
}
