using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(Shooting))]
public class PlayerReference : MonoBehaviour
{
    [HideInInspector]
    public PlayerHealth playerHealth;
    [HideInInspector]
    public Shooting playerShooting;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerShooting = GetComponent<Shooting>();
    }
}
