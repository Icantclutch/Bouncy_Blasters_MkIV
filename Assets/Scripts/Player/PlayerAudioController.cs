using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAudioController : NetworkBehaviour
{
    [SerializeField]
    private List<AudioClip> _sounds;
    public Dictionary<string, AudioClip> soundsD;

    [ClientRpc]
    public void RpcOnAllClients(int soundIndex)
    {
        if(soundIndex < _sounds.Count)
        {
            GetComponent<AudioSource>().PlayOneShot(_sounds[soundIndex]);
        }
    }

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
