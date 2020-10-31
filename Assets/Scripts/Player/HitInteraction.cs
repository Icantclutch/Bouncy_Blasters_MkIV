using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class HitInteraction : NetworkBehaviour
{
    public abstract void Hit(Bullet.Shot shot);
}
