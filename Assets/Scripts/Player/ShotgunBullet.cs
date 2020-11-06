using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShotgunBullet : Bullet
{
    //The actual bullet used
    public string BulletName;
    //How many pellets are fired
    public int pellets;

    [Server]
    public override void Vel(Vector3 vel, float speed)
    {
        //Get the network bullet
        GameObject bulletPrefab = NetworkManager.singleton.spawnPrefabs.Find(bu => bu.name.Equals(BulletName));
        //Loop through each pellet
        for (int i = 0; i < pellets; i++)
        {
            //Get the offset
            Vector3 newDir = new Vector3(
                transform.eulerAngles.x + Random.Range(-10f, 10f),
                transform.eulerAngles.y + Random.Range(-10f, 10f),
                transform.eulerAngles.z
            );
            //Spawn and rotate fired bullet
            GameObject b = Instantiate(bulletPrefab, transform.position, transform.rotation);
            b.transform.eulerAngles = newDir;
            //Spawn on server
            NetworkServer.Spawn(b);
            //Transfer shot info and initialize
            b.GetComponent<Bullet>().Initialize(myShot.damage, myShot.maxBounces, speed, myShot.playerID);
        }
        //Destroy this object
        DestroyBullet();
    }
}
