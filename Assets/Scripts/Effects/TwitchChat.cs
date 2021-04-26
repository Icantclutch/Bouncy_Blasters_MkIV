using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;


//BY DEFAULT THE "OWNERSHIP" OF THE TWITCH CHAT IN GAME
//WILL FALL TO THE HOST OF THE LOBBY
public class TwitchChat : MonoBehaviour
{
    public TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    //Secure this!
    //TwitchPassword Link https://twitchapps.com/tmi
    //WHEN the player enters this make sure its set to lowercase! [Upper Case DOES NOT WORK]
    public string username, password, channelName;

    //Where to put chat output
    public Text chatBox;
    private float timer;

    //--------------------
    //Project Review Addition! (makes showing what I did easy)
    public GameObject objectSpawner;
    public Camera camMain;
    public Transform camPosSpawner;
    public Transform camLookSpawner;
    private float timeToMoveCam = 5;

    //Camera Test
    public Transform camPosTest;
    public Transform camLookTest;
    private float timeToMoveCamTest = 5;

    //Grenade Test Prefab
    public GameObject GrenadePrefab;
    public Transform camPosGrenade;
    public Transform camLookGrenade;
    private float timeGrenade = 3;
    //--------------------

    //CONNECT ON START
    //For testing this is fine / prompt the password in the future
    void Start()
    {
        print("Connecting");
        Connect();
    }

    void Update()
    {
        //Create a connection if its not connected
<<<<<<< HEAD
        if (twitchClient != null) {
            if (!twitchClient.Connected)
            {
                Connect();
            }
        }

        if (!_lobbyManager)
=======
        if (!twitchClient.Connected)
>>>>>>> parent of 64a14082 (Merge branch 'main' of https://github.com/Icantclutch/Bouncy_Blasters_MkIV into main)
        {
            Connect();
        }
        ReadChat();
        //Runs to prevent a disconnect
        //KeepConnectionAlive();
    }

    private void Connect()
<<<<<<< HEAD
=======
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        //LOGGIN IN
        //Do not change (Sends to Twitch Servers) / From Twitch.API
        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();
    }

    public void ReadChat()
    {
        //If messages to read
        if (twitchClient.Available > 0)
        {
            var message = reader.ReadLine();
            //Returns only custom messages
            if (message.Contains("PRIVMSG"))
            {
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(0, splitPoint);
                chatName = chatName.Substring(1);

                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);

                chatBox.text = chatBox.text + "\n" + String.Format("{0}: {1}", chatName, message);
                Input(chatName, message);
            }

        }

    }

    private void Input(string viewerName, string viewerMessage)
>>>>>>> parent of 64a14082 (Merge branch 'main' of https://github.com/Icantclutch/Bouncy_Blasters_MkIV into main)
    {
        if (viewerMessage.ToLower() == "join")
        {
            print("JOIN EVENT");
        }

        if (viewerMessage.ToLower() == "pool")
        {
            print("Creating Particle Pooler");
            objectSpawner.SetActive(true);

            //Move the camera if the camera exists
            if (camMain != null)
            {
                CameraShaking toUseShake = camMain.GetComponent<CameraShaking>();
                if (toUseShake != null)
                {
                    toUseShake.AddCameraTween(camPosSpawner, camLookSpawner, timeToMoveCam);
                }
            }
        }

<<<<<<< HEAD
    }

    public void SetTwitchInfo(string name, string oauth)
    {
        //If host then set
        if (_lobbyManager.isServer)
        {
            username = name;
            password = oauth;
            //channelName = name;
            Connect();
        }
    }

    //Twitch Events
    //Loading screens
    //

    private void Input(string viewerName, string viewerMessage)
    {
        if (viewerMessage.ToLower() == "test")
        {
            print("JOIN EVENT");
        }

        if (viewerMessage.ToLower() == "hitmarker")
        {
            foreach (PlayerData Data in _lobbyManager.players)
            {
                Data.GetComponent<PlayerEffects>().CreateHitmarker(5);
            }
        }

        if (viewerMessage.ToLower() == "reload")
        {
            foreach (PlayerData Data in _lobbyManager.players)
            {
                Data.GetComponent<Shooting>().Rpc_FullReload();
=======
        //Camera Movement
        if (viewerMessage.ToLower() == "camtest")
        {
            print("Camera Test");

            //Move the camera if the camera exists
            if (camMain != null)
            {
                CameraShaking toUseShake = camMain.GetComponent<CameraShaking>();
                if (toUseShake != null)
                {
                    toUseShake.AddCameraTween(camPosTest, camLookTest, timeToMoveCamTest);
                }
            }
        }

        //Camera Movement
        if (viewerMessage.ToLower() == "grenade")
        {
            print("Running grenade Test");

            //Move the camera if the camera exists
            if (camMain != null)
            {
                CameraShaking toUseShake = camMain.GetComponent<CameraShaking>();
                if (toUseShake != null)
                {
                    Instantiate(GrenadePrefab);
                    toUseShake.AddCameraTween(camPosGrenade, camLookGrenade, timeGrenade);
                }
>>>>>>> parent of 64a14082 (Merge branch 'main' of https://github.com/Icantclutch/Bouncy_Blasters_MkIV into main)
            }
        }

        //EVENTS
        //Low Gravity (change PlayerMovement.JumpHeight)

        //Super speed (sprint increase too) (change PlayerMovement.Speed)

        //Earth Quake

        //Team randomize

        //if (viewerMessage.ToLower() == "pool")
        //{
        //    print("Creating Particle Pooler");
        //    objectSpawner.SetActive(true);

        //    //Move the camera if the camera exists
        //    if (camMain != null)
        //    {
        //        CameraShaking toUseShake = camMain.GetComponent<CameraShaking>();
        //        if (toUseShake != null)
        //        {
        //            toUseShake.AddCameraTween(camPosSpawner, camLookSpawner, timeToMoveCam);
        //        }
        //    }
        //}

        //Camera Movement
        //if (viewerMessage.ToLower() == "camtest")
        //{
        //    print("Camera Test");

        //    //Move the camera if the camera exists
        //    if (camMain != null)
        //    {
        //        CameraShaking toUseShake = camMain.GetComponent<CameraShaking>();
        //        if (toUseShake != null)
        //        {
        //            toUseShake.AddCameraTween(camPosTest, camLookTest, timeToMoveCamTest);
        //        }
        //    }
        //}

        //Camera Movement
        //if (viewerMessage.ToLower() == "grenade")
        //{
        //    print("Running grenade Test");

        //    //Move the camera if the camera exists
        //    if (camMain != null)
        //    {
        //        CameraShaking toUseShake = camMain.GetComponent<CameraShaking>();
        //        if (toUseShake != null)
        //        {
        //            Instantiate(GrenadePrefab);
        //            toUseShake.AddCameraTween(camPosGrenade, camLookGrenade, timeGrenade);
        //        }
        //    }
        //}
    }

    private void KeepConnectionAlive()
    {
        timer -= Time.deltaTime;

        if (timer < 0f)
        {
            if (writer != null)
            {
                writer.WriteLine("ping");
                writer.Flush();
            }
            timer = 5f;
        }
    }
}