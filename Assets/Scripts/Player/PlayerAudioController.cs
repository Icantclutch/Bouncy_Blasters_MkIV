using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAudioController : NetworkBehaviour
{
    [SerializeField]
    private List<AudioClip> _sounds;
    public Dictionary<string, AudioClip> soundsD;

    /// <summary>
    /// Plays a oneshot of the sound on all clients
    /// <para>
    /// 0: Bullet hits another player<br/>
    /// 1: Bullet hits local player<br/>
    /// 2: Shooting bullet<br/>
    /// 3: Autoblaster shooting<br/>
    /// 4: Pistol shooting<br/>
    /// </para>
    /// </summary>
    [ClientRpc]
    public void RpcOnAllClients(int soundIndex)
    {
        if(soundIndex < _sounds.Count)
        {
            GetComponent<AudioSource>().PlayOneShot(_sounds[soundIndex]);
        }
    }

    /// <summary>
    /// Plays a oneshot of the sound only on the local client
    /// <para>
    /// 0: Bullet hits another player<br/>
    /// 1: Bullet hits local player<br/>
    /// 2: Shooting bullet<br/>
    /// 3: Autoblaster shooting<br/>
    /// 4: Pistol shooting<br/>
    /// </para>
    /// </summary>
    [TargetRpc]
    public void RpcOnPlayerClient(int soundIndex)
    {
        //Debug.Log(_sounds.Count);
        if (soundIndex < _sounds.Count)
        {
            //Debug.Log(soundIndex);
            GetComponent<AudioSource>().PlayOneShot(_sounds[soundIndex]);
        }
    }

    [ClientRpc]
    public void RpcOnAllClientsD(string soundName)
    {
        if (soundsD[soundName])
        {
            GetComponent<AudioSource>().PlayOneShot(soundsD[soundName]);
        }
    }

    [TargetRpc]
    public void RpcOnPlayerClientD(string soundName)
    {
        //Debug.Log(_sounds.Count);
        if (soundsD[soundName])
        {
            Debug.Log(soundName);
            GetComponent<AudioSource>().PlayOneShot(soundsD[soundName]);
        }
    }
}
