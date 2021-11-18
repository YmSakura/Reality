using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ScriptableObject����������һ�����ļ������������ߴ洢Item����
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    
    //������
    public int itemHeld;
    
    //������Ϣ
    [TextArea]
    public string itemInfo;
    
    //�Ƿ��װ��
    public bool equip;

}
