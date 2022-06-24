using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    public List<CardData> cardDatas = new List<CardData>();

    public void Create()
    {
        //create a list of cardData for the deck
        List<CardData> cardDataInOrder = new List<CardData>();
        foreach(CardData cardData in GameController.instance.cards)
        {
            for(int i = 0; i < cardData.numberInDeck; i++)
                cardDataInOrder.Add(cardData);
        }

        //randomize the deck
        while(cardDataInOrder.Count > 0)
        {
            int randomIndex = Random.Range(0, cardDataInOrder.Count);
            cardDatas.Add(cardDataInOrder[randomIndex]);
            cardDataInOrder.RemoveAt(randomIndex);
        }
    }

    private CardData RandomCard()
    {
        CardData result = null;
        
        if(cardDatas.Count == 0)
            Create();

        result = cardDatas[0];
        cardDatas.RemoveAt(0);

        return result;
    }

    private Card CreateNewCard(Vector3 pos, string animName)
    {
        GameObject newCard = GameObject.Instantiate(GameController.instance.cardPrefab, GameController.instance.canvas.transform);

        newCard.transform.position = pos;
        Card card = newCard.GetComponent<Card>();
        if(card)
        {
            card.cardData = RandomCard();
            card.Init();

            Animator animator = newCard.GetComponentInChildren<Animator>();
            if(animator)
            {
                animator.CrossFade(animName, 0);
            }
            else
            {
                Debug.LogError("No animator found!");
            }

            return card;
        }
        else
        {
            Debug.LogError("No card component found!");
            return null;
        }
    }

    public void DealCard(Hand hand)
    {
        for(int i = 0; i < 3; i++)
        {
            if(hand.cards[i] == null)
            {
                if(hand.playerHand)
                    GameController.instance.player.PlayCardSound();
                else
                    GameController.instance.enemy.PlayCardSound();
                    
                hand.cards[i] = CreateNewCard(hand.positions[i].position, hand.animations[i]);
                return;
            }
        }
    }
}
