using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public static class TutorialSusre
{
    public static List<string> AllStep = new List<string>()
    {
        "Tutorial/Start",
        "Tutorial/1",
        "Tutorial/2",
        "Tutorial/3",
        "Tutorial/4",
        "Tutorial/5",
        "Tutorial/6",
        "Tutorial/7",
        "Tutorial/8",
        "Tutorial/9",
        "Tutorial/10",
        "Tutorial/11",
        "Tutorial/12",
        "Tutorial/13",
        "Tutorial/PreFinish",
        "Tutorial/PrePreFinish",
        "Tutorial/End"
    };
    public static Sprite surse = string.IsNullOrEmpty(AllStep[0]) ? null : Resources.Load<Sprite>(AllStep[0]);
}

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager Instance { get; set; }

    public GameObject Tutorial;
    public Image IMG;


    public int Navigation = 0;

    public GameObject MainMenu;
    public GameObject ServerMenu;
    public GameObject ConnectMenu;


    public GameObject Next;
    public GameObject Prew;
    public GameObject Exit;

    public GameObject serverPrefab;
    public GameObject clientPrefab;
    public TMPro.TMP_InputField nameInput;

    private void Start()
    {
        Instance = this;
        ChangePage("MainMenu");
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Animatrion());
        Tutorial.SetActive(false);
        Next.SetActive(false);
        Prew.SetActive(false);
        Exit.SetActive(false);
        IEnumerator Animatrion()
        {
            yield return new WaitForSeconds(1);
        }

    }

    /// <summary>
    /// Включение обучения
    /// </summary>
    public void StartTutorial()
    {
        Navigation = 0;
        Tutorial.SetActive(true);
        Next.SetActive(true);
        Prew.SetActive(true);
        Exit.SetActive(true);
        IMG.sprite = string.IsNullOrEmpty(TutorialSusre.AllStep[0]) ? null : Resources.Load<Sprite>(TutorialSusre.AllStep[0]);
    }

    /// <summary>
    /// След. шаг обучения
    /// </summary>
    public void NextStep()
    {
        Tutorial.SetActive(true);
        Next.SetActive(true);
        Prew.SetActive(true);
        Exit.SetActive(true);
        Navigation++;
        if (Navigation == TutorialSusre.AllStep.Count - 1)
        {
            Exit.SetActive(false);
        }
        if (Navigation == TutorialSusre.AllStep.Count)
        {
            ExitButton();
        }
        else
        {
            TutorialSusre.surse = Resources.Load<Sprite>(TutorialSusre.AllStep[Navigation]);
            IMG.sprite = TutorialSusre.surse;
        }
    }

    /// <summary>
    /// Пред. шаг обучения
    /// </summary>
    public void PrewStep()
    {
        Tutorial.SetActive(true);
        Next.SetActive(true);
        Prew.SetActive(true);
        Exit.SetActive(true);
        Navigation--;

        if (Navigation == -1)
        {
            ExitButton();
        }
        else
        {
            TutorialSusre.surse = Resources.Load<Sprite>(TutorialSusre.AllStep[Navigation]);
            IMG.sprite = TutorialSusre.surse;
        }
    }

    /// <summary>
    /// Выключение обучения
    /// </summary>
    public void ExitButton()
    {
        Navigation = 0;
        Tutorial.SetActive(false);
        Next.SetActive(false);
        Prew.SetActive(false);
        Exit.SetActive(false);
    }



    /// <summary>
    /// Кнопка перехода в меню "клиента"
    /// </summary>
    public void ConnectButton()
    {
        ChangePage("ConnectMenu");
    }

    /// <summary>
    /// Отключение всех связей
    /// </summary>
    public void DestroyAll()
    {
        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);

        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }

    /// <summary>
    /// Кнопка перехода в меню "хоста"
    /// </summary>
    public void HostButton()
    {
        try
        {
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();

            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            c.isHost = true;
            if (c.clientName == "")
                c.clientName = "Host";
            c.ConnectToServer("127.0.0.1", 6321);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        ChangePage("ServerMenu");
    }

    /// <summary>
    /// Кнопка подкдючения клиента к серверу
    /// </summary>
    public void ConnectToServerButton()
    {
        ChangePage("ConnectMenu");
        string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (hostAddress == "")
            hostAddress = "127.0.0.1";

        try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "Client";
            c.ConnectToServer(hostAddress, 6321);
            ConnectMenu.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }


    /// <summary>
    /// Кнопка перехода в главную меню
    /// </summary>
    public void BackButton()
    {
        ChangePage("MainMenu");

        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);

        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);


    }

    /// <summary>
    /// Обработчик перехода между окнами меню
    /// </summary>
    /// <param name="PageName">Имя окна</param>
    public void ChangePage(string PageName)
    {
        switch (PageName)
        {
            case "MainMenu":
                ConnectMenu.SetActive(false);
                ServerMenu.SetActive(false);
                MainMenu.SetActive(true);
                break;
            case "ServerMenu":
                ConnectMenu.SetActive(false);
                ServerMenu.SetActive(true);
                MainMenu.SetActive(false);
                break;
            case "ConnectMenu":
                ConnectMenu.SetActive(true);
                ServerMenu.SetActive(false);
                MainMenu.SetActive(false);
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// Запуск игры
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("GameWin");
    }

    /// <summary>
    /// Отключение игры
    /// </summary>
    public void EndGame()
    {
        SceneManager.LoadScene("Menu");
    }
}
