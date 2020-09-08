using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Типы игровых полей
/// </summary>
public enum FieldType
{
    SELF_HAND,
    SELF_FIELD,
    SELF_FIELD_TOWN,
    ENEMY_HAND,
    ENEMY_FIELD,
    ENEMY_FIELD_TOWN
}

public class DropPlaceScr : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public FieldType Type;
    public GameObject Player_count_war;
    public GameObject ResursesError;
    public GameObject FieldError;
    GameObject TempCardGO;
    public GameObject card1;
    bool FirstPlayer;
    public GameManagerScr GameManager;
    public CardMovementScr CardMovement;
    public List<CardInfoScr>
                     PlayerFieldCards = new List<CardInfoScr>(),
                     PlayerField_TownCards = new List<CardInfoScr>(),
                     EnemyField_TownCards = new List<CardInfoScr>(),
                     EnemyFieldCards = new List<CardInfoScr>();
    private Client client;

    public GameObject[] asd;

    /// <summary>
    /// Функция при появлении объекта 
    /// </summary>
    public void Start()
    {
        GameManager = FindObjectOfType<GameManagerScr>();
        PlayerFieldCards = GameManager.PlayerFieldCards;
        PlayerField_TownCards = GameManager.PlayerField_TownCards;
        EnemyField_TownCards = GameManager.EnemyField_TownCards;
        EnemyFieldCards = GameManager.EnemyFieldCards;
        FirstPlayer = GameManager.FirstPlayer;
    }

    /// <summary>
    /// Функция при выкладывании краты
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        bool exept = true;

        exept = Type == FieldType.ENEMY_FIELD || Type == FieldType.ENEMY_FIELD_TOWN || Type == FieldType.ENEMY_HAND;
        CardMovementScr card = eventData.pointerDrag.GetComponent<CardMovementScr>();
        if (exept)
            return;
        if (!CardMovementScr.IsDraggable)
            return;
        
        if (card.GameManager.PlayerGold < card.GetComponent<CardInfoScr>().SelfCard.Gold)
        {

            ResursesError.SetActive(true);
            StartCoroutine(ErrorResursesDelay(1));
            IEnumerator ErrorResursesDelay(float delayTime = 1)
            {
                yield return new WaitForSeconds(delayTime);
                ResursesError.SetActive(false);
            }
        }
        else
        {
            switch (card.GetComponent<CardInfoScr>().Name.text)
            {
                case CardName.knight:
                    ArmyCheck(card);
                    break;
                case CardName.archer:
                    ArmyCheck(card);
                    break;
                case CardName.worker:
                    CivilCheck(card);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Проверка карты военного типа при выкладывании на стол
    /// </summary>
    /// <param name="card">Выложенная карта</param>
    public void ArmyCheck(CardMovementScr card)
    {
        if (Type == FieldType.SELF_FIELD
                && card.GameManager.PlayerGold >= card.GetComponent<CardInfoScr>().SelfCard.Gold)
        {
            if (card && card.GameManager.PlayerFieldCards.Count < 6)
            {
                card.GameManager.PlayerHandCards.Remove(card.GetComponent<CardInfoScr>());
                card.GameManager.PlayerFieldCards.Add(card.GetComponent<CardInfoScr>());
                GameManagerScr.Instanse.Step(card.GetComponent<CardInfoScr>().SelfCard.Id, card.GetComponent<CardInfoScr>(), SendType.CardToWarField);
                card.DefaultParent = transform;
                card.GameManager.ReduceGold(true, card.GetComponent<CardInfoScr>().SelfCard.Gold, true);
                return;
            }
        }
        else
        {
            if (Type != FieldType.SELF_FIELD)
            {
                FieldError.SetActive(true);
                StartCoroutine(ErrorResursesDelay(1));
                IEnumerator ErrorResursesDelay(float delayTime = 1)
                {
                    yield return new WaitForSeconds(delayTime);
                    FieldError.SetActive(false);
                }
            }

        }
    }

    /// <summary>
    /// Проверка карты гражданского типа при выкладывании на стол
    /// </summary>
    /// <param name="card">Выложенная карта</param>
    public void CivilCheck(CardMovementScr card)
    {
        if (Type == FieldType.SELF_FIELD_TOWN
            && card.GameManager.PlayerGold >= card.GetComponent<CardInfoScr>().SelfCard.Gold)
        {
            if (card && card.GameManager.PlayerField_TownCards.Count < 3)
            {
                card.GameManager.PlayerHandCards.Remove(card.GetComponent<CardInfoScr>());
                card.GameManager.PlayerField_TownCards.Add(card.GetComponent<CardInfoScr>());
                card.DefaultParent = transform;

                GameManagerScr.Instanse.Step(card.GetComponent<CardInfoScr>().SelfCard.Id, card.GetComponent<CardInfoScr>(), SendType.CardToTownField);
                card.GameManager.ReduceGold(true, card.GetComponent<CardInfoScr>().SelfCard.Gold, true);
            }
        }
        else
        {
            if (Type != FieldType.SELF_FIELD)
            {

                FieldError.SetActive(true);
                StartCoroutine(ErrorResursesDelay(1));
                IEnumerator ErrorResursesDelay(float delayTime = 1)
                {
                    yield return new WaitForSeconds(delayTime);
                    FieldError.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Событие при наведении мыши от объекта
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        bool tmpFieldCheck, tmpHandCheck = true;
        tmpFieldCheck = (eventData.pointerDrag == null || Type == FieldType.ENEMY_FIELD || Type == FieldType.ENEMY_FIELD_TOWN || Type == FieldType.ENEMY_HAND || Type == FieldType.SELF_HAND);
        if (tmpFieldCheck)
            return;

        bool CountDeckCheck()
        {
            return (Type == FieldType.SELF_FIELD && PlayerFieldCards.Count >= 6 || Type == FieldType.SELF_FIELD_TOWN && PlayerField_TownCards.Count >= 3) ? true : false;
        }
        tmpHandCheck = Type != FieldType.SELF_HAND;
        CardMovementScr card = eventData.pointerDrag.GetComponent<CardMovementScr>();
        if (card && tmpHandCheck)
        {
            if (CountDeckCheck())
                return;
            card.DefaultTempCardParent = transform;
            TempCardGO = GameObject.Find("TempCardGO");
            TempCardGO.transform.SetParent(card.DefaultParent.parent);
        }
    }

    /// <summary>
    /// Событие при отведении мыши от объекта
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        CardMovementScr card = eventData.pointerDrag.GetComponent<CardMovementScr>();

        if (card && card.DefaultTempCardParent == transform)
            card.DefaultTempCardParent = card.DefaultParent;
    }
}
