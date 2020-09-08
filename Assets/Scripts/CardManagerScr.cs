using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Card
{
    public int Id { get; set; }
    public string Name;
    public int Gold;
    public Sprite Logo => string.IsNullOrEmpty(LogoPath) ? null : Resources.Load<Sprite>(LogoPath);
    public Sprite DropLogo => string.IsNullOrEmpty(DroplogoPath) ? null : Resources.Load<Sprite>(DroplogoPath);
    public int Attack;
    public int Defence;
    public bool CanAttack;
    public string LogoPath;
    public string DroplogoPath;

    /// <summary>
    /// Игровая карта
    /// </summary>
    /// <param name="name">Имя карты</param>
    /// <param name="logoPath">Путь к изображению мини-иконки</param>
    /// <param name="defence">Параметр защиты</param>
    /// <param name="dropLogoPath">Путь к изображению карты</param>
    /// <param name="gold">Параметр стоимости</param>
    /// <param name="id">Идентификатор</param>
    public Card(string name, string logoPath, int attack, int defence, string dropLogoPath, int gold, int id)
    {
        LogoPath = logoPath;
        DroplogoPath = dropLogoPath;
        Name = name;
        Attack = attack;
        Defence = defence;
        CanAttack = false;
        Gold = gold;
        Id = id;
    }

}

/// <summary>
/// Список всех существующих карт
/// </summary>
public static class CardManager
{
    public static List<Card> AllCards = new List<Card>();
}


/// <summary>
/// Список всех имен карты
/// </summary>
public static class CardName
{
    public const string knight = "knight";
    public const string archer = "archer";
    public const string worker = "worker";
}

/// <summary>
/// Инициализация всех существующих карт 
/// </summary>
public class CardManagerScr : MonoBehaviour
{
    public void Awake()
    {
        CardManager.AllCards.Add(new Card("knight", "Sprites/CardUPDT/Knight", 1, 10, "Sprites/CardUPDT/KnightDrop", 5, 0));
        CardManager.AllCards.Add(new Card("worker", "Sprites/CardUPDT/worker", 1, 3, "Sprites/CardUPDT/WorkerDrop", 2, 0));
        CardManager.AllCards.Add(new Card("archer", "Sprites/CardUPDT/Archer", 1, 7, "Sprites/CardUPDT/ArcherDrop", 4, 0));
    }

}
