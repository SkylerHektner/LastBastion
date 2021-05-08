using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedSelector<T>
{
    T cur_item;
    int cur_weight = 0;

    public void AddItem( T item, int weight )
    {
        cur_weight += weight;
        if( Random.Range( 0, cur_weight ) < weight )
        {
            cur_item = item;
        }
    }

    public bool HasItem() { return cur_weight != 0; }

    public T GetItem()
    {
        return cur_item;
    }
}
