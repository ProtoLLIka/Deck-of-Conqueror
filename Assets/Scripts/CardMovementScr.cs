using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovementScr : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// Главная камера
    /// </summary>
    Camera MainCamera;
    /// <summary>
    /// Отступ от центра карты
    /// </summary>
    Vector3 offset;
    public Transform DefaultParent, DefaultTempCardParent;
    GameObject TempCardGO;
    public GameManagerScr GameManager;
    public static bool IsDraggable;
    public static bool IsWhichPlayer;
    public GameObject Logo;
    public GameObject DoropedIcon;
    public GameObject CardStyle;
    public int index = 0;


    public GameObject EnemyDefaultParent, PlayerDefaultParent;

    void Awake()
    {
        DefaultParent = DefaultTempCardParent = transform.parent;
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
        GameManager = FindObjectOfType<GameManagerScr>();
        DoropedIcon.SetActive(false);

    }


    /// <summary>
    /// Начало перетаскивания карты
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {

        index = transform.GetSiblingIndex();
        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);
        DefaultParent = DefaultTempCardParent = transform.parent;
        IsDraggable = false;
        if (GameManager.IsPlayerTurn)
            IsDraggable = (DefaultParent.GetComponent<DropPlaceScr>().Type == FieldType.SELF_HAND);

        if (!IsDraggable)
            return;

        DoropedIcon.SetActive(true);
        if (DefaultParent.GetComponent<DropPlaceScr>().Type == FieldType.SELF_HAND || DefaultParent.GetComponent<DropPlaceScr>().Type == FieldType.ENEMY_HAND)
        {
            TempCardGO.transform.SetParent(DefaultParent.parent);
            TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());
            transform.SetParent(DefaultParent.parent);
            transform.SetParent(DefaultParent.parent);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

    }

    /// <summary>
    /// Процесс перетаскивания карты
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;

        Logo.SetActive(false);
        CardStyle.SetActive(false);
        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position); 
        transform.position = newPos + offset; 

        if (TempCardGO.transform.parent != DefaultTempCardParent)
            TempCardGO.transform.SetParent(DefaultTempCardParent);

        CheckPosition();
    }

    /// <summary>
    /// Конец перетаскивания карты
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;

        transform.SetParent(DefaultParent);
        
        if (DefaultParent.GetComponent<DropPlaceScr>().Type == FieldType.SELF_HAND || DefaultParent.GetComponent<DropPlaceScr>().Type == FieldType.ENEMY_HAND)
        {
            Logo.SetActive(true);
            CardStyle.SetActive(true);
            DoropedIcon.SetActive(false);
        }
        else
        {
            DoropedIcon.SetActive(true);
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.SetSiblingIndex(index);
        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(2340, 0);
    }


    /// <summary>
    /// Проверка на соответствие места карты
    /// </summary>
    void CheckPosition()
    {
        int newIndex = DefaultTempCardParent.childCount;
        if (TempCardGO.transform.parent != GameObject.Find("SelfHand").transform && TempCardGO.transform.parent != GameObject.Find("EnemyHand").transform)
        {
            for (int i = 0; i < DefaultTempCardParent.childCount; i++)
            {
                if (transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
                {
                    newIndex = i;

                    if (TempCardGO.transform.GetSiblingIndex() < newIndex)
                        newIndex--;

                    break;
                }
            }
            TempCardGO.transform.SetSiblingIndex(newIndex);
        }
        else
        {
            TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
            TempCardGO.transform.localPosition = new Vector3(2340, 0);
        }
    }
}
