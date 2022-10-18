using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMode : Mode, IMode
{
    public void StartMode(){

    }

    public List<Card> PrepareCardSet(List<Card> cardSet){
        return cardSet;
    }

    public Card GetCard(List<Card> cardSet, Card lastCard){
        if(cardSet.Count == 0){
            return null;
        }

        return GetDifferentRandomCardFromList(cardSet, lastCard);
    }

    public void EndMode(){

    }

    private Card GetDifferentRandomCardFromList(List<Card> cards, Card lastCard)
    {
        if(cards.Count == 0){
            return null;
        }

        Card nextCard;
        do{
            nextCard = GetRandomCardFromList(cards);
        }while(nextCard.Equals(lastCard) && cards.Count > 1);

        return nextCard;
    }

    private Card GetRandomCardFromList(List<Card> cards)
    {
        if(cards.Count == 0)
        {
            return null;
        }
        int randomIndex = UnityEngine.Random.Range(0, cards.Count);
        return cards[randomIndex];
    }
}