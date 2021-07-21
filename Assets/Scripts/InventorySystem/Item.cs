using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    food,
    material,
    weapon,
    armor,
}

public enum ItemId
{
    apple,
}
public class Item : ScriptableObject
{
    public ItemId itemId;
    public string name;
    [TextArea]public string desc;
    public Sprite icon;

    public bool stackAble;
    public bool maxStackNum;

    public virtual void Use()
    {
        
    }
}
