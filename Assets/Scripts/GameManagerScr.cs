using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

/// <summary>
/// Экземпляр игры содержащий в себе колоду для игры
/// </summary>
public class Game
{
    public List<Card> Deck;
    public List<int> DeckInfoToSync;
    public static System.Random rand = new System.Random();


    public Game()
    {
        DeckInfoToSync = new List<int>();
        Deck = new List<Card>();
    }

    /// <summary>
    /// Генерация новой колоды
    /// </summary>
    /// <returns>Колода для игры</returns>
    public List<Card> GiveDeckCard()
    {
        List<Card> list = new List<Card>();
        for (int i = 0; i < 30; i++)
        {
            var cardNumber = rand.Next(0, CardManager.AllCards.Count);

            var newCard = CardManager.AllCards[cardNumber];
            newCard.Id = i;
            list.Add(newCard);
            DeckInfoToSync.Add(cardNumber);
        }
        return list;
    }

    /// <summary>
    /// Копирование колоды для клиента с колоды хоста
    /// </summary>
    /// <returns>Колода для игры</returns>
    public List<Card> CreateDeckFromNumberList(List<int> info)
    {
        List<Card> list = new List<Card>();
        for (int i = 0; i < 30; i++)
        {
            var cardNumber = info[i];
            var newCard = CardManager.AllCards[cardNumber];
            newCard.Id = i;
            list.Add(newCard);
            DeckInfoToSync.Add(cardNumber);
        }
        return list;
    }
}

/// <summary>
/// Игровые сообщения
/// </summary>
public enum SendType
{
    /// <summary>
    /// Начало новой игры
    /// </summary>
    startNewGame = 1,
    /// <summary>
    /// Взятие карты из колоды
    /// </summary>
    giveOneCard = 2,
    /// <summary>
    /// Готовность игрока
    /// </summary>
    IAmReady = 3,
    /// <summary>
    /// Была выложена карта на боевое поле
    /// </summary>
    CardToWarField = 4,
    /// <summary>
    /// Была выложена карта на городское поле
    /// </summary>
    CardToTownField = 5,
    /// <summary>
    /// Была выложена карта на городское поле
    /// </summary>
    TakeStartHand = 6,
    /// <summary>
    /// Сообщение о готовности начать игру (завершение полной подготовки к старту)
    /// </summary>
    ReadyToStart = 7,
    /// <summary>
    /// Сообщение о передачи хода 
    /// </summary>
    YourTurn = 8,
    /// <summary>
    /// Сообщение о запросе взятии карты
    /// </summary>
    TakeCard = 9,
    /// <summary>
    /// Сообщение о реакции способности
    /// </summary>
    SkillReaction = 10,
    /// <summary>
    /// Сообщение о применении способности
    /// </summary>
    Skill = 11,
    /// <summary>
    /// Сообщение о конце игры
    /// </summary>
    EndGame = 12,

}

/// <summary>
/// Модель игровой ситации для отправки сообщения на сервер
/// </summary>
/// <typeparam name="T">Тип данных в сообщеии</typeparam>
public class SendBaseModel<T>
{
    /// <summary>
    /// Имя события
    /// </summary>
    public SendType Event;

    /// <summary>
    /// Данные события
    /// </summary>
    public T Data;

    public SendBaseModel(SendType _event, T data)
    {
        Event = _event;
        Data = data;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}


public class GameManagerScr : MonoBehaviour
{
    public static GameManagerScr Instanse { get; set; }
    public Game CurrentGame;
    public Transform EnemyHand,
                     PlayerHand,
                     EnemyField,
                     PlayerField,
                     EnemyTownField,
                     PlayerTownField;
    public GameObject CardPref;
    public const int StartGold = 13;
    int TurnTime = 30;
    public TextMeshProUGUI TurnTimeTxt,
                           EnemyAllPoint,
                           PlayerAllPoint,
                           EnemyWarPoint,
                           PlayerWarPoint,
                           EnemyCivilPoint,
                           PlayerCivilPoint,
                           Count;
    public Button EndTurnBtn;
    
    public GameObject ResursesError;
    public GameObject FieldError;
    public GameObject CardError;
    public GameObject Win;
    public GameObject NoOne;
    public GameObject YourTurnImg;
    public GameObject Lose;
    public bool FirstPlayer;
    public Client client;
    public bool IsHost;
    public const int HandCardsCount = 3;
    public int EnemyGold = 0;
    public int PlayerGold = 0;
    public TextMeshProUGUI PlayerGoldCountTxt, EnemyGoldCountTxt;
    public List<CardInfoScr> PlayerHandCards = new List<CardInfoScr>(),
                             PlayerFieldCards = new List<CardInfoScr>(),
                             PlayerField_TownCards = new List<CardInfoScr>(),
                             EnemyHandCards = new List<CardInfoScr>(),
                             EnemyField_TownCards = new List<CardInfoScr>(),
                             EnemyFieldCards = new List<CardInfoScr>();
    public bool IsPlayerTurn;
    public bool IsWin;


    /// <summary>
    /// Обработчик первого кадра
    /// </summary>
    void Start()
    {
        Instanse = this;
        TurnTimeTxt.text = "";
        CurrentGame = new Game();
        client = FindObjectOfType<Client>();
        IsPlayerTurn = client.isHost;
        EndTurnBtn.interactable = IsPlayerTurn;

        if (client.isHost)
            StartNewGame(true);
        ShowResources();
        PointCounter(true);

    }

    /// <summary>
    /// Создание новой игры
    /// </summary>
    /// <param name="isHost"></param>
    /// <param name="deckInfo"></param>
    public void StartNewGame(bool isHost, List<int> deckInfo = null)
    {
        CurrentGame = new Game();
        IsHost = isHost;
        if (isHost)
        {
            CurrentGame.Deck = CurrentGame.GiveDeckCard();
        }
        else
        {
            CurrentGame.Deck = CurrentGame.CreateDeckFromNumberList(deckInfo);
        }


        ResursesError.SetActive(false);
        FieldError.SetActive(false);
        Win.SetActive(false);
        NoOne.SetActive(false);
        Lose.SetActive(false);
        YourTurnImg.SetActive(false);
        CardError.SetActive(false);
        PlayerHandCards.Clear();
        PlayerFieldCards.Clear();
        PlayerField_TownCards.Clear();
        EnemyHandCards.Clear();
        EnemyField_TownCards.Clear();
        EnemyFieldCards.Clear();
        EnemyGold = StartGold;
        PlayerGold = StartGold;
        ShowResources();
        if (isHost)
        {
            var data = new SendBaseModel<List<int>>(SendType.startNewGame, CurrentGame.DeckInfoToSync);
            client.Send(data.ToString());
        }

    }

    /// <summary>
    /// Обработчик своего хода
    /// </summary>
    /// <param name="id">Идентификатор карты</param>
    /// <param name="card">Обект иформации о карте</param>
    /// <param name="Event">Событие к обработке</param>
    public void Step(int id, CardInfoScr card, SendType Event)
    {
        var tmp = new SendBaseModel<int>(SendType.CardToWarField, id);

        switch (Event)
        {
            case SendType.CardToWarField:
                tmp = new SendBaseModel<int>(SendType.CardToWarField, id);
                break;
            case SendType.CardToTownField:
                tmp = new SendBaseModel<int>(SendType.CardToTownField, id);
                break;
            default:
                break;
        }
        PointCounter(false);
        client.Send(JsonUtility.ToJson(tmp));
    }

    /// <summary>
    /// Обработчик чужого хода
    /// </summary>
    /// <param name="id">Идентификатор карты</param>
    /// <param name="Event">Событие к обработке</param>
    public void StepReaction(int id, SendType Event)
    {
        var ItCard = EnemyHandCards.FirstOrDefault(o => o.SelfCard.Id == id);

        List<CardInfoScr> tempCardList = new List<CardInfoScr> { ItCard };
        EnemyHandCards.Remove(ItCard);

        var Parent = transform.parent;
        switch (Event)
        {
            case SendType.CardToWarField:
                EnemyFieldCards.Add(ItCard);
                ItCard.ShowCardInfoEnemy(ItCard.SelfCard);
                ItCard.Defence.text = CardManager.AllCards.Find(o => o.Name == ItCard.SelfCard.Name).Defence.ToString();
                ItCard.SelfCard.Defence = CardManager.AllCards.Find(o => o.Name == ItCard.SelfCard.Name).Defence;
                ItCard.transform.SetParent(EnemyField);
                break;
            case SendType.CardToTownField:
                EnemyField_TownCards.Add(ItCard);
                ItCard.ShowCardInfoEnemy(ItCard.SelfCard);
                ItCard.Defence.text = CardManager.AllCards.Find(o => o.Name == ItCard.SelfCard.Name).Defence.ToString();
                ItCard.SelfCard.Defence = CardManager.AllCards.Find(o => o.Name == ItCard.SelfCard.Name).Defence;
                ItCard.transform.SetParent(EnemyTownField);
                break;
            default:
                break;

        }
        EnemyGold -= ItCard.SelfCard.Gold;
        ShowResources();
        PointCounter(false);
    }

    /// <summary>
    /// Обработчик для подсчета очков на игровом поле
    /// </summary>
    /// <param name="start">Проверка на страт игры</param>
    public void PointCounter(bool start)
    {
        if (!start)
        {
            int PlayerAllPointData = 0;
            int PlayerCivilPointData = 0;
            int PlayerWarPointData = 0;

            int EnemyAllPointData = 0;
            int EnemyCivilPointData = 0;
            int EnemyWarPointData = 0;


            foreach (var item in PlayerFieldCards)
            {
                PlayerWarPointData += item.SelfCard.Defence;
            }

            foreach (var item in PlayerField_TownCards)
            {
                PlayerCivilPointData += item.SelfCard.Defence;
            }

            foreach (var item in EnemyFieldCards)
            {
                EnemyWarPointData += item.SelfCard.Defence;
            }

            foreach (var item in EnemyField_TownCards)
            {
                EnemyCivilPointData += item.SelfCard.Defence;
            }

            EnemyAllPointData = EnemyCivilPointData + EnemyWarPointData;
            PlayerAllPointData = PlayerCivilPointData + PlayerWarPointData;

            EnemyAllPoint.text = EnemyAllPointData.ToString();
            EnemyCivilPoint.text = EnemyCivilPointData.ToString();
            EnemyWarPoint.text = EnemyWarPointData.ToString();

            PlayerAllPoint.text = PlayerAllPointData.ToString();
            PlayerCivilPoint.text = PlayerCivilPointData.ToString();
            PlayerWarPoint.text = PlayerWarPointData.ToString();
        }
        else
        {
            EnemyAllPoint.text = "0";
            EnemyCivilPoint.text = "0";
            EnemyWarPoint.text = "0";

            PlayerAllPoint.text = "0";
            PlayerCivilPoint.text = "0";
            PlayerWarPoint.text = "0";
        }
    }

    /// <summary>
    /// Взятие начальной "руки" карт
    /// </summary>
    /// <param name="deck"></param>
    /// <param name="hand"></param>
    public void StartHand(List<Card> deck, Transform hand)
    {

        if (!IsHost)
        {
            //Добавление карт к руке противника у клиента
            for (int i = 0; i < HandCardsCount; i++)
            {
                if (deck.Count == 0)
                    return;
                Card card = deck[0];
                GameObject cardGO = Instantiate(CardPref, EnemyHand, false);
                if (EnemyHandCards.Count < 5)
                {
                    cardGO.GetComponent<CardInfoScr>().HideCardInfo(card);
                    EnemyHandCards.Add(cardGO.GetComponent<CardInfoScr>());
                }
                deck.RemoveAt(0);
            }

        }

        //Добавление карт к руке игрока
        for (int i = 0; i < HandCardsCount; i++)
        {
            if (deck.Count == 0)
                return;
            Card card = deck[0];
            GameObject cardGO = Instantiate(CardPref, PlayerHand, false);
            if (PlayerHandCards.Count < 5)
            {
                cardGO.GetComponent<CardInfoScr>().ShowCardInfo(card);
                PlayerHandCards.Add(cardGO.GetComponent<CardInfoScr>());
            }
            deck.RemoveAt(0);
        }

        if (IsHost)
        {
            //Добавление карт к руке противника у хоста
            for (int i = 0; i < HandCardsCount; i++)
            {
                if (deck.Count == 0)
                    return;
                Card card = deck[0];
                GameObject cardGO = Instantiate(CardPref, EnemyHand, false);
                if (EnemyHandCards.Count < 5)
                {
                    cardGO.GetComponent<CardInfoScr>().HideCardInfo(card);
                    EnemyHandCards.Add(cardGO.GetComponent<CardInfoScr>());
                }
                deck.RemoveAt(0);
            }
        }

        if (IsHost)
        {
            var messege = new SendBaseModel<object>(SendType.TakeStartHand, null);
            client.Send(JsonUtility.ToJson(messege));
            StartTimer();

        }
        else
        {
            var messege = new SendBaseModel<object>(SendType.ReadyToStart, null);
            client.Send(JsonUtility.ToJson(messege));
            StartTimer();
        }

    }

    /// <summary>
    /// "Запускатор" таймера хода
    /// </summary>
    public void StartTimer()
    {
        if (IsPlayerTurn)
            YourTurnImg.SetActive(true);
        StartCoroutine(TurnFunc());
    }

    /// <summary>
    /// Таймер хода
    /// </summary>
    /// <returns></returns>
    IEnumerator TurnFunc()
    {
        TurnTime = 30;
        TurnTimeTxt.text = "";
        while (TurnTime-- > 0)
        {
            if (CurrentGame.Deck.Count > 0)
            {
                if (TurnTime < 28)
                {
                    YourTurnImg.SetActive(false);
                }

                if (TurnTime < 5 && TurnTime > 0)
                {
                    TurnTimeTxt.text = TurnTime.ToString();
                }
            }

            yield return new WaitForSeconds(1);
        }
        if (IsPlayerTurn)
        {
            ChangeTurn();
        }
    }

    /// <summary>
    /// Обработчик смены хода
    /// </summary>
    public void ChangeTurn()
    {
        StopAllCoroutines();
        IsPlayerTurn = !IsPlayerTurn;
        var message = new SendBaseModel<object>(SendType.YourTurn, null);
        //CurrentGame.Deck.RemoveAt(0);
        client.Send(JsonUtility.ToJson(message));
        EndTurnBtn.interactable = IsPlayerTurn;
        StartTimer();
    }

    /// <summary>
    /// Добавление новой карты к руке игрока или противника
    /// </summary>
    /// <param name="player">Проверка на игрока или противника</param>
    /// <param name="Workers_count">Кол-во рабочих сыгранных на поле</param>
    public void GiveNewCard(bool player, int Workers_count = 0)
    {
        IsWin = CurrentGame.Deck.Count <= 0 ? true : false;
        if (IsWin)
        {
            var EnemyPoint = Convert.ToInt32(EnemyAllPoint.text);
            var PlayerPoint = Convert.ToInt32(PlayerAllPoint.text);
            if (EnemyPoint == PlayerPoint)
            {
                Lose.SetActive(false);
                Win.SetActive(false);
                NoOne.SetActive(true);
            }
            else
            {
                Lose.SetActive(IsWin = (EnemyPoint > PlayerPoint ? true : false));
                Win.SetActive(IsWin = (EnemyPoint > PlayerPoint ? false : true));
            }
            var data = new SendBaseModel<object>(SendType.EndGame, null);
            client.Send(JsonUtility.ToJson(data));
        }
        else
        {
            if (player)
            {
                Workers_count = PlayerField_TownCards.Count(o => o.Name.text == CardName.worker);

                if (IsPlayerTurn)
                {
                    if (CurrentGame.Deck.Count == 0)
                        return;


                    if (PlayerHandCards.Count < 5)
                    {
                        Card card = CurrentGame.Deck[0];
                        GameObject cardGO = Instantiate(CardPref, PlayerHand, false);
                        cardGO.GetComponent<CardInfoScr>().ShowCardInfo(card);
                        PlayerHandCards.Add(cardGO.GetComponent<CardInfoScr>());
                        CurrentGame.Deck.RemoveAt(0);
                        var data = new SendBaseModel<int>(SendType.TakeCard, Workers_count);
                        client.Send(data.ToString());
                    }
                    else
                    {
                        CardError.SetActive(true);
                        StartCoroutine(ErrorResursesDelay(3));
                        IEnumerator ErrorResursesDelay(float delayTime = 1)
                        {
                            yield return new WaitForSeconds(delayTime);
                        }
                        CardError.SetActive(false);
                        CurrentGame.Deck.RemoveAt(0);
                        var data = new SendBaseModel<int>(SendType.TakeCard, Workers_count);
                        client.Send(data.ToString());
                    }

                    ReduceGold(true, Workers_count * 2 + 1, false);
                }
            }
            else
            {
                if (EnemyHandCards.Count < 5)
                {
                    Card card = CurrentGame.Deck[0];
                    GameObject cardGO = Instantiate(CardPref, EnemyHand, false);
                    cardGO.GetComponent<CardInfoScr>().HideCardInfo(card);
                    EnemyHandCards.Add(cardGO.GetComponent<CardInfoScr>());
                    ReduceGold(false, Workers_count * 2 + 1, false);
                    CurrentGame.Deck.RemoveAt(0);
                    var data = new SendBaseModel<object>(SendType.Skill, null);
                    client.Send(/*data.ToString() */"");
                }
                else
                {
                    ReduceGold(false, Workers_count * 2 + 1, false);
                    CurrentGame.Deck.RemoveAt(0);
                    var data = new SendBaseModel<object>(SendType.Skill, null);
                    client.Send(/*data.ToString() */"");
                }
            }
            Count.text = CurrentGame.Deck.Count.ToString();
        }
    }

    /// <summary>
    /// Обработчик способности карты "Рыцаря"
    /// </summary>
    public void KnightSkill()
    {
        int hp;
        var knightsCount = 0;

        knightsCount = PlayerFieldCards.Count(o => o.Name.text == CardName.knight);
        for (int i = 0; i < knightsCount; i++)
        {
            var target = UnityEngine.Random.Range(0, EnemyFieldCards.Count);
            try
            {
                hp = Convert.ToInt32(EnemyFieldCards[target].Defence.text);
                hp -= 2;
                if (hp < 0)
                {
                    EnemyFieldCards.Remove(EnemyFieldCards[target]);
                    EnemyFieldCards[target].transform.SetParent(GameObject.Find("Canvas").transform);
                }
                else
                {
                    EnemyFieldCards[target].Defence.text = hp.ToString();
                }
                return;
            }
            catch (Exception)
            {
            }
        }
    }

    /// <summary>
    /// Обработчик способности карты "Лучник"
    /// </summary>
    public void ArcherSkill()
    {
    }

    /// <summary>
    /// Обработчик применения способностей карт
    /// </summary>
    public void Skill()
    {
        KnightSkill();
        ArcherSkill();
        var data = new SendBaseModel<List<CardInfoScr>>(SendType.SkillReaction, EnemyFieldCards);
        client.Send(data.ToString());
    }

    /// <summary>
    /// Обработчик реакции на способности карт от другого игрока
    /// </summary>
    /// <param name="data"></param>
    public void SkillReaction(List<CardInfoScr> data)
    {
        foreach (var item in data)
        {

        }
        PlayerFieldCards = data;
        client.Send("Got it");

    }

    /// <summary>
    /// Обработчик отображения ресурсов
    /// </summary>
    void ShowResources()
    {

        PlayerGoldCountTxt.text = PlayerGold.ToString();
        EnemyGoldCountTxt.text = EnemyGold.ToString();
    }

    /// <summary>
    /// Обработчик изменения кол-ва ресурсов 
    /// </summary>
    /// <param name="playerGold">Ресурсы игрока</param>
    /// <param name="goldCost">Стоимость карты</param>
    /// <param name="turn">Проверка на ход</param>
    public void ReduceGold(bool playerGold, int goldCost, bool turn)
    {
        if (turn)
        {
            if (playerGold)
                PlayerGold = Mathf.Clamp(PlayerGold - goldCost, 0, int.MaxValue);
            else
                EnemyGold = Mathf.Clamp(EnemyGold - goldCost, 0, int.MaxValue);
        }
        else
        {
            if (playerGold)
                PlayerGold += goldCost;
            else
                EnemyGold += goldCost;
        }
        ShowResources();
    }

}
