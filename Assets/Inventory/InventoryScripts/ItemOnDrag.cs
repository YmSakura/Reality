using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //��קItem��ԭʼParent
    public Transform originalParent;
    public Inventory myBag;
    public int currentItemID;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        //����Parent
        originalParent = transform.parent;

        //��ȡslotId
        currentItemID = originalParent.GetComponent<Slot>().slotID;
        
        //��ק�����н�parent��Slot��ΪGrid
        transform.SetParent(originalParent.parent);
        //��ȡ���λ��
        transform.position = eventData.position;
        //ȡ�������ڵ�������򿪵Ļ���ק�������������ֻ���⵽��ǰ��ק�����壩
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //��ȡ���λ��
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject pointerGameObject = eventData.pointerCurrentRaycast.gameObject;

        //���������߷�������Ϊ�� �� ��������tag����Slot ������ק����ص�ԭζ
        if (pointerGameObject == null || !pointerGameObject.CompareTag("Slot"))
        {
            transform.SetParent(originalParent);
            transform.position = originalParent.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            return;
        }
        
        //�����Item Image˵����ǰSlot������Item�����뵱ǰ��ק��Item����λ��
        if (pointerGameObject.name == "Item Image")
        {
             //��ȡ�������ָ���Item��Slot
             Transform item = pointerGameObject.transform.parent;
             Transform slot = item.parent;
             //GameObject itemObject = pointerGameObject.transform.parent.gameObject;
             //GameObject slotObject = pointerGameObject.transform.parent.parent.gameObject;
             Slot slotObject = slot.GetComponent<Slot>();
             
             //�����������ָ�������뵱ǰ���彻��
             transform.SetParent(slot);
             transform.position = slot.position;
             
             //Ϊ�����ݳ־û����棬itemListҲҪ����
             (myBag.itemList[currentItemID], myBag.itemList[slotObject.slotID]) =
                 (myBag.itemList[slotObject.slotID], myBag.itemList[currentItemID]);

             item.transform.SetParent(originalParent);
             item.position = originalParent.position;
             
             GetComponent<CanvasGroup>().blocksRaycasts = true;
             return;
        }
        
        //�����ǰ����û��Item��������߷��ص��������Slot
        Transform emptySlot = pointerGameObject.transform;
        
        transform.SetParent(emptySlot);
        transform.position = emptySlot.position;
        //����itemList������
        myBag.itemList[pointerGameObject.GetComponent<Slot>().slotID] = myBag.itemList[currentItemID];
        //����Լ������Լ�λ�õ����⣬����Ż�ԭλ����ΪNull
        if(pointerGameObject.GetComponent<Slot>().slotID != currentItemID)
            myBag.itemList[currentItemID] = null;
        
        
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
