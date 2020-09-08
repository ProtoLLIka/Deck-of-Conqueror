using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;
using System.Linq;

public class Server : MonoBehaviour
{
    public int port = 6321;

    private List<ServerClient> clientsList;
    private List<ServerClient> disconnectionList;

    private TcpListener server;
    private bool serverStarted;
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        clientsList = new List<ServerClient>();
        disconnectionList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;

        }
        catch (Exception e)
        {

            Debug.Log($"Socket error: {e.Message} ");
        }
    }

    public void log(string text)
    {
        Debug.Log($"{text}");
    }

    /// <summary>
    /// Обработчик поступающей информации
    /// </summary>
    private void Update()
    {
        if (!serverStarted)
            return;
        foreach (ServerClient c in clientsList)
        {
            //Проверка "Подключен ли еще пользователь?"
            if (!IsConnected(c.tcp))
            {
                log("client is disconnect");
                c.tcp.Close();
                disconnectionList.Add(c);
                GameMenuManager.Instance.EndGame();
                GameMenuManager.Instance.DestroyAll();

                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();
                    log($"data: {data}");

                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }

        for (int i = 0; i < disconnectionList.Count - 1; i++)
        {

            //Игроку передается информация что кто то отключился
            clientsList.Remove(disconnectionList[i]);
            disconnectionList.RemoveAt(i);

        }
    }

    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTCPClient, server);
    }
    private void AcceptTCPClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        string allUsers = "";
        foreach (ServerClient i in clientsList)
        {
            allUsers += i.clientName + '|';
        }
        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clientsList.Add(sc);

        StartListening();

        Broadcast("SWHO|" + allUsers, clientsList[clientsList.Count - 1]);
    }

    /// <summary>
    /// Получение данных на сервер
    /// </summary>
    /// <param name="c"> От кого пришло сообщение </param>
    /// <param name="data"> Сообщение </param>
    private void OnIncomingData(ServerClient c, string data)
    {

        var adressClient = (c.clientName == "Host" ? clientsList.Where(num => num.clientName == "Client").ToList() : clientsList.Where(num => num.clientName == "Host").ToList());
       
        string[] aData = data.Split('|');
        if (aData.Length > 0)
        {
            switch (aData[0])
            {
                case "CWHO":
                    c.clientName = aData[1];
                    c.isHost = (aData[2] == "0") ? false : true;
                    Broadcast($"SCNN|" + c.clientName, clientsList);
                    break;
            }
        }
        Broadcast(data, adressClient);
    }

    /// <summary>
    /// Отправка сообщения от сервера
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cl"></param>
    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient sc in cl)
        {
            try
            {

                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();

            }
            catch (Exception e)
            {
                Debug.Log($"Write error: {e.Message}");
            }
        }
    }
    private void Broadcast(string data, ServerClient c)
    {
        List<ServerClient> sc = new List<ServerClient> { c };
        Broadcast(data, sc);
    }
}

public class ServerClient
{
    public string clientName;
    public TcpClient tcp;
    public bool isHost;

    public ServerClient(TcpClient tcp_ServerClient)
    {
        this.tcp = tcp_ServerClient;
    }
}