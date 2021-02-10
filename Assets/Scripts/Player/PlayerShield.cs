using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerShield : HitInteraction
{
    [SerializeField]
    private float _shieldVisibilityTime = 2f;
    private float _timer = 0;
    public override void Hit(Bullet.Shot shot)
    {
        transform.parent.SendMessage("Hit", shot, SendMessageOptions.DontRequireReceiver);
        Rpc_ShowShield();
    }

    [ClientRpc]
    public void Rpc_ShowShield()
    {
        GetComponent<MeshRenderer>().enabled = true;
        _timer = _shieldVisibilityTime;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        _timer = 0;
        GetComponent<MeshRenderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(_timer > 0)
        {
            _timer -= Time.deltaTime;
            if(_timer <= 0)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
