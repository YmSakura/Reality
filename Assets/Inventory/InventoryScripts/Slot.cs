using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Slot��ӦUnity�е�Slot prefab
public class Slot : MonoBehaviour
{
    //Item�������Լ�������С�����ݿ��࣬������Ҫ��ȡ������е�itemInfo
    //public Item slotItem;

    public int slotID;
    
    //����item����ʾ
    public GameObject itemInSlot;
    
    //Item����Ϣ
    public Image slotImage;
    public Text slotItemNum;
    public string slotInfo;
    
    //�����imageʱ����description
    public void ItemOnClicked()
    {
        InventoryManager.UpdateItemInfo(slotInfo);
    }

    //����ÿ��Slot�����ԣ���ʵ��������Item��
    public void SetupSlot(Item item)
    {
        if (item == null)
        {
            //���itemΪ�վͲ���ʾ���item
            itemInSlot.SetActive(false);
            return;
        }
        //�����Ϊ�վ͸���ͼƬ���ֺ�������Ϣ
        slotImage.sprite = item.itemImage;
        slotItemNum.text = item.itemHeld.ToString();
        slotInfo = item.itemInfo;
    }
}
