using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;
using static GameManagerScr;
using System.Linq;

public class Client : MonoBehaviour
{
    public string clientName;
    public bool isHost;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;
    private List<GameClient> players = new List<GameClient>();
    public int count1 = 0; 
    //Чтение сообщений от сервера 
    public bool StepIsOver = true;

    public void log(string data)
    {
        Debug.Log(data);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;

        }
        catch (Exception e)
        {

            Debug.Log($"Socket error: {e.Message}");
        }

        return socketReady;
    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    OnInComingData(data);
                }
                    
            }
        }
    }
    //Отправка сообщений серверу
    public void Send(string data)
    {

        if (!socketReady)
            return;
        //Debug.Log($"{(isHost ? "Host" : "Client")} send msg {data}" + " to " + (isHost ? "Client" : "Host").ToString());
        writer.WriteLine(data);
        writer.Flush();
    }
   
    private void OnInComingData(string data)
    {
        string[] aData = data.Split('|');
        if (aData.Length > 0)
        {
            switch (aData[0])
            {
                case "SWHO":
                    for (int i = 1; i < aData.Length - 1; i++)
                    {
                        UserConnected(aData[i]);
                    }
                    Send($"CWHO|{clientName}|" + ((isHost) ? 1 : 0).ToString());
                    break;
                case "SCNN":
                    UserConnected(aData[1]);
                    break;
            }
        }
        
        try
        {
            var sendModel = JsonUtility.FromJson<SendBaseModel<object>>(data);
            var currentModelInt = JsonUtility.FromJson<SendBaseModel<int>>(data);

            switch (sendModel.Event)
            {

                case SendType.startNewGame:
                    var currentModel = JsonUtility.FromJson<SendBaseModel<List<int>>>(data);
                    if (!isHost)
                    {   
                        GameManagerScr.Instanse.StartNewGame(false, currentModel.Data);
                        var messege = new SendBaseModel<object>(SendType.IAmReady, null);
                        Send(JsonUtility.ToJson(messege));
                    }
                    break;

                case SendType.IAmReady:
                    sendModel = JsonUtility.FromJson<SendBaseModel<object>>(data);
                    GameManagerScr.Instanse.StartHand(GameManagerScr.Instanse.CurrentGame.Deck, GameManagerScr.Instanse.PlayerHand);
                    break;

                case SendType.CardToWarField:
                    currentModelInt = JsonUtility.FromJson<SendBaseModel<int>>(data);
                    GameManagerScr.Instanse.StepReaction(currentModelInt.Data, currentModelInt.Event);
                    break;

                case SendType.CardToTownField:
                    currentModelInt = JsonUtility.FromJson<SendBaseModel<int>>(data);
                    GameManagerScr.Instanse.StepReaction(currentModelInt.Data, currentModelInt.Event);
                    break;

                case SendType.TakeStartHand:
                    GameManagerScr.Instanse.StartHand(GameManagerScr.Instanse.CurrentGame.Deck, GameManagerScr.Instanse.PlayerHand);
                    break;

                case SendType.YourTurn:
                    GameManagerScr.Instanse.IsPlayerTurn = true;
                    GameManagerScr.Instanse.EndTurnBtn.interactable = true;
                    GameManagerScr.Instanse.GiveNewCard(true);
                    GameManagerScr.Instanse.StopAllCoroutines();
                    GameManagerScr.Instanse.StartTimer();
                    break;

                case SendType.TakeCard:
                    GameManagerScr.Instanse.GiveNewCard(false, currentModelInt.Data);
                    break;

                case SendType.EndGame:
                    GameManagerScr.Instanse.GiveNewCard(false);
                    break;
                default:
                    break;
            }

        }
        catch (Exception)
        {
        }

    }

    private void UserConnected(string name)
    {
        GameClient c = new GameClient();
        c.name = name;
        players.Add(c);

        if (players.Count == 2)
        {
            GameMenuManager.Instance.StartGame();
        }
    }



    private void OnApplicationQuit()
    {
        CloseSocket();
        players.RemoveRange(0, players.Count);
    }
    private void OnDisable()
    {
        CloseSocket();
        players.RemoveRange(0, players.Count);

    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;
        players.RemoveRange(0, players.Count);
        Debug.Log($"players.Count - {players.Count}");
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}

public class GameClient
{
    public string name;
    public bool isHost;

}