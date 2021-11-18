using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������������item
[CreateAssetMenu(fileName = "New Inventory",menuName = "Inventory/New Inventory")]
public class Inventory : ScriptableObject
{
    public List<Item> itemList = new List<Item>();
}
