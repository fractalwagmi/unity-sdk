using System;

[Serializable]
public class Item
{
    public string id;
    public string name;
    public bool isForSale;
    public ItemFile[] files;
    public Chain chain;
}
