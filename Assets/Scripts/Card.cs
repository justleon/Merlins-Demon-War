using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public CardData cardData = null;

    public TMP_Text nameText = null;
    public TMP_Text descriptionText = null;
    public Image damageImage = null;
    public Image costImage = null;
    public Image cardImage = null;
    public Image frameImage = null;
    public Image burnImage = null;

    public void Init()
    {
        if(cardData == null)
        {
            Debug.LogError("Card has no cardData!");
            return;
        }

        nameText.text = cardData.cardName;
        descriptionText.text = cardData.cardDescription;
        damageImage.sprite = GameController.instance.damageNumbers[cardData.damage];
        costImage.sprite = GameController.instance.healthNumbers[cardData.cost];
        cardImage.sprite = cardData.cardImage;
        frameImage.sprite = cardData.frameImage;
    }
}
