using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttributes
{
    #region Base
    float Health { get; set; }
    float Stamina { get; set; }
    float Poise { get; set; }
    #endregion

    void ApplyItemAttributes(Item item);
    void RemoveItemAttributes(Item item);
}
