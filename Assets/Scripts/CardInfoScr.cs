using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfoScr : MonoBehaviour
{
    public Card SelfCard;
    public Image Logo;
    public Image DropLogo;
    

    public TextMeshProUGUI Name, Attack, Defence, Cost;
    public GameObject HideObj, HighlightedObj, LogoPic, CardStyle, DropPic;

    /// <summary>
    /// Скрытие карты и её деталей
    /// </summary>
    /// <param name="card">Карта</param>
    public void HideCardInfo(Card card)
    {
        SelfCard = card;
        HideObj.SetActive(true);
        DropLogo.sprite = card.DropLogo;
        LogoPic.SetActive(false);
        CardStyle.SetActive(false);
        DropPic.SetActive(false);

    }

    /// <summary>
    /// Отображение карты и её деталей
    /// </summary>
    /// <param name="card">Карта</param>
    public void ShowCardInfo(Card card)
    {
        HideObj.SetActive(false);
        SelfCard = card;
        Cost.text = SelfCard.Gold.ToString();
        DropLogo.sprite = card.DropLogo;
        Logo.sprite = card.Logo;
        Logo.preserveAspect = false;
        Name.text = card.Name;
        Defence.text = SelfCard.Defence.ToString();
    }

    /// <summary>
    /// Отображение информации карт противника и её деталей
    /// </summary>
    /// <param name="card">Карта</param>
    public void ShowCardInfoEnemy(Card card)
    {
        SelfCard = card;
        Cost.text = SelfCard.Gold.ToString();
        HideObj.SetActive(false);
        DropLogo.sprite = card.DropLogo;
        LogoPic.SetActive(false);
        CardStyle.SetActive(false);
        DropPic.SetActive(true);
    }
}
