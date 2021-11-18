using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    //����
    private static InventoryManager instance;

    public Inventory myBag;
    public GameObject slotGrid;
    //public Slot slotPrefab;
    public GameObject emptySlot;
    public Text itemInfo;
    
    //������е�Slotʵ��
    public List<GameObject> slots = new List<GameObject>();

    private void Awake()
    {
        //����������Ѿ����ڸö��󣬾������Ѿ����ڵĶ���
        if(instance != null)
            Destroy(this);
        instance = this;
    }
    
    private void OnEnable()
    {
        //ÿ��������Ϸ����ʱ���±������item��Ϣ
        RefreshItem();
    }
    
    public static void UpdateItemInfo(string itemDescription)
    {
        instance.itemInfo.text = itemDescription;
    }

    //����slot��UI
    /*public static void CreateNewSlot(Item item)
    {
        //����һ���µ�Slot����slotGridΪparent�����ᰴ��slotGrid�Ĳ��ְڷ�
        Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(instance.slotGrid.transform);
        
        //����Ҽ񵽵�item�����Դ����������ɵ�Slot
        newItem.slotItem = item;
        newItem.slotImage.sprite = item.itemImage;
        newItem.slotItemNum.text = item.itemHeld.ToString();
    }*/

    //����item����Ϸ�տ�ʼ��ÿ����Ҽ��µ�Item������
    public static void RefreshItem()
    {
        //��slotGrid��������slotȫ������
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            //���������slots���飬������ظ������ܶ�Slot
            instance.slots.Clear();
        }

        //�ٸ���myBag�е�item���´���slot
        for (int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            //��һ��д������������Ԫ�صĸ������������´����µ�Slot
            //CreateNewSlot(instance.myBag.itemList[i]);
            
            //�ڶ���д������slots���鱣�����е�slot
            //��ΪitemList�Ѿ���ʼ��������ÿ�ζ�������ʵ��������Ԫ�أ�Ȼ���ٸ���ÿ��slot������
            //ʵ����Slot����ӵ�slots����
            instance.slots.Add(Instantiate(instance.emptySlot));
            //��slotGrid����Ϊÿ��slot��parent
            instance.slots[i].transform.SetParent(instance.slotGrid.transform);
            //����ÿ��slot����Ϣ
            instance.slots[i].GetComponent<Slot>().slotID = i;
            instance.slots[i].GetComponent<Slot>().SetupSlot(instance.myBag.itemList[i]);
        }
    }

}
