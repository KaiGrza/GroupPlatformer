using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupUnit : MonoBehaviour
{
    public void Pickup()
    {
        PlayerMovement.TotalPickups++;
    }
}
