using System;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private float health;
    [SerializeField] private float stamina;
    [SerializeField] private float damage;
    [SerializeField] private float defense;



    public float Health { get { return health; } set { health = value; } }
    public float Stamina { get { return stamina; } set { stamina = value; } }
    public float Damage { get { return damage; } set { damage = value; } }
    public float Defense { get { return defense; } set { defense = value; } }
    

}
