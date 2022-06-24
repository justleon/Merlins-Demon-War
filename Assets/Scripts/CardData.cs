using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public enum DamageType
    {
        Fire,
        Ice,
        Both,
        Destruct
    }

    public string cardName;
    public string cardDescription;
    public int cost;
    public int damage;
    public DamageType damageType;
    public Sprite cardImage;
    public Sprite frameImage;
    public int numberInDeck;
    
    public bool isDefenceCard = false;
    public bool isMirrorCard = false;
    public bool isMulti = false;
    public bool isDestruct = false;
}