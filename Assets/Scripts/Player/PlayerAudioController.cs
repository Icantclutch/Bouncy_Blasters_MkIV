using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Audio;


public class PlayerAudioController : NetworkBehaviour
{
    [SerializeField]
    private List<AudioClip> _sounds;
    public AudioMixerGroup sfx;
    
    

    /// <summary>
    /// Plays a oneshot of the sound on all clients
    /// <para>
    /// 0: Bullet hits another player<br/>
    /// 1: Bullet hits local player<br/>
    /// 2: Shooting bullet<br/>
    /// 3: Autoblaster shooting<br/>
    /// 4: Pistol shooting<br/>
    /// 5: Rifle Reload<br/>
    /// 6: Pistol Reload<br/>
    /// 7: Teleport to Respawn Room<br/>
    /// 8: Teleport out of Respawn Room<br/>
    /// </para>
    /// </summary>
    [ClientRpc]
    public void RpcOnAllClients(int soundIndex)
    {
        if(soundIndex >= 0 && soundIndex < _sounds.Count)
        {
            GetComponent<AudioSource>().outputAudioMixerGroup = sfx;
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
    /// 5: Rifle Reload<br/>
    /// 6: Pistol Reload<br/>
    /// 7: Teleport to Respawn Room<br/>
    /// 8: Teleport out of Respawn Room<br/>
    /// </para>
    /// </summary>
    [TargetRpc]
    public void RpcOnPlayerClient(int soundIndex)
    {
        
        if (soundIndex >= 0 && soundIndex < _sounds.Count)
        {
            GetComponent<AudioSource>().outputAudioMixerGroup = sfx;
            GetComponent<AudioSource>().PlayOneShot(_sounds[soundIndex]);
        }
    }

    
}
