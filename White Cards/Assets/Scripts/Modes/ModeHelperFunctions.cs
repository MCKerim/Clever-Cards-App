using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ModeHelperFunctions
{
    public static Card GetDifferentRandomCardFromList(List<Card> cards, Card lastCard)
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

    public static Card GetRandomCardFromList(List<Card> cards)
    {
        if(cards.Count == 0)
        {
            return null;
        }
        int randomIndex = UnityEngine.Random.Range(0, cards.Count);
        return cards[randomIndex];
    }
}
