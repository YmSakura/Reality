using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //拖拽Item的原始Parent
    public Transform originalParent;
    public Inventory myBag;
    public int currentItemID;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        //保存Parent
        originalParent = transform.parent;

        //获取slotId
        currentItemID = originalParent.GetComponent<Slot>().slotID;
        
        //拖拽过程中将parent从Slot改为Grid
        transform.SetParent(originalParent.parent);
        //获取鼠标位置
        transform.position = eventData.position;
        //取消射线遮挡（如果打开的话拖拽过程中鼠标射线只会检测到当前拖拽的物体）
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //获取鼠标位置
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject pointerGameObject = eventData.pointerCurrentRaycast.gameObject;

        //如果鼠标射线返回物体为空 或 返回物体tag不是Slot 就让拖拽物体回到原味
        if (pointerGameObject == null || !pointerGameObject.CompareTag("Slot"))
        {
            transform.SetParent(originalParent);
            transform.position = originalParent.position;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            return;
        }
        
        //如果是Item Image说明当前Slot格内有Item，则与当前拖拽的Item交换位置
        if (pointerGameObject.name == "Item Image")
        {
             //获取鼠标射线指向的Item和Slot
             Transform item = pointerGameObject.transform.parent;
             Transform slot = item.parent;
             //GameObject itemObject = pointerGameObject.transform.parent.gameObject;
             //GameObject slotObject = pointerGameObject.transform.parent.parent.gameObject;
             Slot slotObject = slot.GetComponent<Slot>();
             
             //将鼠标射线所指的物体与当前物体交换
             transform.SetParent(slot);
             transform.position = slot.position;
             
             //为了数据持久化保存，itemList也要更新
             (myBag.itemList[currentItemID], myBag.itemList[slotObject.slotID]) =
                 (myBag.itemList[slotObject.slotID], myBag.itemList[currentItemID]);

             item.transform.SetParent(originalParent);
             item.position = originalParent.position;
             
             GetComponent<CanvasGroup>().blocksRaycasts = true;
             return;
        }
        
        //如果当前格内没有Item，鼠标射线返回的物体就是Slot
        Transform emptySlot = pointerGameObject.transform;
        
        transform.SetParent(emptySlot);
        transform.position = emptySlot.position;
        //更新itemList的数据
        myBag.itemList[pointerGameObject.GetComponent<Slot>().slotID] = myBag.itemList[currentItemID];
        //解决自己放在自己位置的问题，如果放回原位不设为Null
        if(pointerGameObject.GetComponent<Slot>().slotID != currentItemID)
            myBag.itemList[currentItemID] = null;
        
        
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
