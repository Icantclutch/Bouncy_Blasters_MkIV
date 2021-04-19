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
    public string username; //= "mrunholybus";
    public string password; //= "oauth:948xc4qk988hosacvahiunspztjpwz"; //Please dont steal this from me
    //public string channelName; //= "mrunholybus";
    private LobbyManager _lobbyManager;

    //Where to put chat output
    //public Text chatBox;
    private float timer;

    //--------------------
    //Project Review Addition! (makes showing what I did easy)
    //public GameObject objectSpawner;
    //public Camera camMain;
    //public Transform camPosSpawner;
    //public Transform camLookSpawner;
    //private float timeToMoveCam = 5;

    //Camera Test
    //public Transform camPosTest;
    //public Transform camLookTest;
    //private float timeToMoveCamTest = 5;

    //Grenade Test Prefab
    //public GameObject GrenadePrefab;
    //public Transform camPosGrenade;
    //public Transform camLookGrenade;
    //private float timeGrenade = 3;
    //--------------------
    private int Vote1 = 0;
    private int Vote2 = 0;
    private string Vote1Phrase = "";
    private string Vote2Phrase = "";
    private bool InVote = false;
    public Text twitchChat;
    public Text twitchChatVote1;
    public Text twitchChatVote2;
    private GameObject _gameManager = null;
    private List<string> alreadyVoted = new List<string>();

    //_gameManager.GetComponentInChildren<GameManagement>().MatchTimer.ToString()

    //CONNECT ON START
    //For testing this is fine / prompt the password in the future
    void Start()
    {
        //print("Connecting");
        //Connect();
    }

    void Update()
    {
        //Create a connection if its not connected
        if (twitchClient != null) {
            if (!twitchClient.Connected)
            {
                Connect();
            }
        }

        _gameManager = GameObject.FindGameObjectWithTag("Management");
        //_gameManager.GetComponentInChildren<GameManagement>().MatchTimer.ToString();

        if (!_lobbyManager)
        {
            _lobbyManager = GameObject.FindGameObjectWithTag("Management").GetComponent<LobbyManager>();
        }
        ReadChat();
        //Runs to prevent a disconnect
        //KeepConnectionAlive();
    }

    //Creates the vote event
    private void CreateVote()
    {
        if (InVote == false)
        {
            InVote = true;
            Vote1Phrase = "earthquake";
            Vote2Phrase = "max ammo";
            twitchChat.text = "Twitch Vote | Type In Chat!";
            twitchChatVote1.text = "'" + Vote1Phrase + "'";
            twitchChatVote2.text = "'" + Vote2Phrase + "'";
            StartCoroutine(StartVote());
        }
    }

    private void doWinning(string winningEvent)
    {
        if (winningEvent == "earthquake")
        {
            print("EARTHQUAKE BEING GIVEN");
            foreach (PlayerData Data in _lobbyManager.players)
            {
                Data.GetComponent<Shooting>().Rpc_FullReload();
                Data.GetComponent<PlayerEffects>().AlertText("EARTHQUAKE");
            }
        }
        else if (winningEvent == "max ammo")
        {
            print("MAX AMMO BEING GIVEN");
            foreach (PlayerData Data in _lobbyManager.players)
            {
                Data.GetComponent<Shooting>().Rpc_FullReload();
                Data.GetComponent<PlayerEffects>().AlertText("Max Ammo Granted");
            }
        }
    }

    private string DecideWinner()
    {
        if (Vote1 > Vote2)
        {
            return Vote1Phrase;
        }
        else
        {
            return Vote2Phrase;
        }
    }

    private void EndVote()
    {
        print("Ending");
        string winner = DecideWinner();
        doWinning(winner);
        Vote1Phrase = "CAPS";
        Vote2Phrase = "CAPS";
        alreadyVoted = new List<String>();
        twitchChat.text = "WINNER: " + winner + "!";
        twitchChatVote1.text = "";
        twitchChatVote2.text = "";
        //Do event
    }

    private void ResetVote()
    {
        print("Resetting vote");
        twitchChat.text = "";
        twitchChatVote1.text = "";
        twitchChatVote2.text = "";
        InVote = false;
    }

    IEnumerator StartVote()
    {
        //Set firing so you can't shoot while reloading
        yield return new WaitForSeconds(30);
        EndVote();

        yield return new WaitForSeconds(10);
        ResetVote();
        yield return null;
    }

    private void Connect()
    {
        //If host
        if (_lobbyManager.isServer)
        {
            if (password != null && username != null)
            {
                twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
                reader = new StreamReader(twitchClient.GetStream());
                writer = new StreamWriter(twitchClient.GetStream());

                //LOGGIN IN
                //Do not change (Sends to Twitch Servers) / From Twitch.API
                writer.WriteLine("PASS " + password);
                writer.WriteLine("NICK " + username);
                writer.WriteLine("USER " + username + " 8 * :" + username);
                writer.WriteLine("JOIN #" + username); //channelName
                writer.Flush();
            }
        }
    }

    public void ReadChat()
    {
        if (twitchClient != null)
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

                    //chatBox.text = chatBox.text + "\n" + String.Format("{0}: {1}", chatName, message);
                    Input(chatName, message);
                }

            }
        }

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

    private void Input(string viewerName, string viewerMessage)
    {
        print("MESSAGE SENT: " + viewerMessage.ToLower());
        if (viewerMessage.ToLower() == "start")
        {
            CreateVote();
        }

        if (viewerMessage.ToLower() == Vote1Phrase)
        {
            if (!alreadyVoted.Contains(viewerName))
            {
                print("Voted for 1");
                Vote1 = Vote1 + 1;
                alreadyVoted.Add(viewerName);
            }
        }

        if (viewerMessage.ToLower() == Vote2Phrase)
        {
            if (!alreadyVoted.Contains(viewerName))
            {
                print("Voted for 2");
                Vote2 = Vote2 + 1;
                alreadyVoted.Add(viewerName);
            }
        }
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