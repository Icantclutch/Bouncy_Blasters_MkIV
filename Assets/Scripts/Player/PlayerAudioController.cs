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
    /// 2: Small Rifle shooting<br/>
    /// 3: Rifle shooting<br/>
    /// 4: Large Rifle shooting<br/>
    /// 5: Low pistol shooting<br/>
    /// 6: pistol shooting<br/>
    /// 7: high pistol shooting<br/>
    /// 8: Teleport surge<br/>
    /// 9: Teleport surge<br/>
    /// 10: Empty magazine<br/>
    /// 11: Rifle reloading<br/>
    /// 12: Pistol reloading<br/>
    /// 13: Teleport to the respawn room<br/>
    /// 14: Teleport out of the respawn room<br/>
    /// 15: Run<br/>
    /// 16: Sprint<br/>
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
    /// 2: Small Rifle shooting<br/>
    /// 3: Rifle shooting<br/>
    /// 4: Large Rifle shooting<br/>
    /// 5: Low pistol shooting<br/>
    /// 6: pistol shooting<br/>
    /// 7: high pistol shooting<br/>
    /// 8: Teleport surge<br/>
    /// 9: Teleport surge<br/>
    /// 10: Empty magazine<br/>
    /// 11: Rifle reloading<br/>
    /// 12: Pistol reloading<br/>
    /// 13: Teleport to the respawn room<br/>
    /// 14: Teleport out of the respawn room<br/>
    /// 15: Run<br/>
    /// 16: Sprint<br/>
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

    [ClientRpc]
    public void RpcSetLoop(int soundIndex)
    {
        GetComponent<AudioSource>().loop = true;
        if (GetComponent<AudioSource>().clip != _sounds[soundIndex] || !GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().clip = _sounds[soundIndex];
            GetComponent<AudioSource>().Play();
        }
    }

    [ClientRpc]
    public void RpcStopLoop()
    {
        //GetComponent<AudioSource>().clip = _sounds[soundIndex];
        //GetComponent<AudioSource>().loop = true;
        if (GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Stop();
        }
    }
}
