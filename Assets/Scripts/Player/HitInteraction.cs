using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitInteraction : MonoBehaviour
{
    public abstract void Hit(Bullet.Shot shot);
}
