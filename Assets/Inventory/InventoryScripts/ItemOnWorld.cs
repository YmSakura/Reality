using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����ʵ����Ϸ������item�Ľű�
public class ItemOnWorld : MonoBehaviour
{
    //�������item�Ϲ���
    public Item thisItem;
    //����
    public Inventory myBag;

    //��Ҵ���
    private void OnTriggerEnter2D(Collider2D other)
    {
        //����Ե���item�ͽ���ǰitem���뱳��
        if (other.gameObject.CompareTag("Player"))
        {
            AddNewItem();
            Destroy(gameObject);
        }
    }

    //��Item����Inventory
    public void AddNewItem()
    {
        //���������û�У��ͼ��뱳��
        if (!myBag.itemList.Contains(thisItem))
        {
            //��һ��д����ÿ�μ��µ�item�Ž������itemList����Ϊ�����UI��ʾ
            //MyBag.itemList.Add(thisItem);
            //InventoryManager.CreateNewItem(thisItem);
            
            //��ʼ��item����Ϊ1
            thisItem.itemHeld = 1;
            //�ڶ���д����itemList����Ϸ��ʼʱ�Ѿ���ʼ����ֻ���жϵ�ǰԪ���Ƿ�Ϊ�ռ���
            for (int i = 0; i < myBag.itemList.Count; i++)
            {
                //����myBag��itemList���飬�����ǰԪ��Ϊ�վͰ�item�Ž�ȥ
                if (myBag.itemList[i] == null)
                {
                    myBag.itemList[i] = thisItem;
                    break;
                }
            }
        }
        else
        {
            //����������Ѿ����ˣ�Item������++
            thisItem.itemHeld++;
        }
        
        //ÿ������궼ˢ��һ�α���
        InventoryManager.RefreshItem();
    }
}
