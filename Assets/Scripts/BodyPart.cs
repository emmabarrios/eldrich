using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public Part bodyPart;

    public enum Part {
        Head,
        Arm,
        Leg,
        Chest,
        Hip
    }
}
