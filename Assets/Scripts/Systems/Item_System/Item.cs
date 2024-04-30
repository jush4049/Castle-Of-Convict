using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public Sprite itemImage;
    public string itemValue;
    public string itemName;
    [TextArea(8, 8)]
    public string itemDescription;
}