using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用ScriptableObject创建出来的一个“文件”，用来离线存储Item数据
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    
    //持有数
    public int itemHeld;
    
    //描述信息
    [TextArea]
    public string itemInfo;
    
    //是否可装备
    public bool equip;

}
