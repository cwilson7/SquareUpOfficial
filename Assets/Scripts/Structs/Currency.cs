using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Currency 
{
    public Money type;
    public int quantity;

    public Currency(Money _type, int _quantity)
    {
        quantity = _quantity;
        type = _type;
    }

    public int Quantity
    {
        get
        {
            return quantity;
        }
        set
        {
            quantity = value;
        }
    }
}

[System.Serializable]
public enum Money
{
    SquareBucks,
    CubeCoins
}
