using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStation : HitInteraction
{
    public Weapon stationWeapon;

    public override void Hit(Bullet.Shot shot)
    {
        //create a new weapon instance
        Shooting.WeaponSlot newSlot = new Shooting.WeaponSlot();
        newSlot.weapon = stationWeapon;
        newSlot.currentAmmo = stationWeapon.ammoCount;

        //Give it to the player
        shot.playerSource.playerShooting.playerWeapons[0] = newSlot;
    }
}
