using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMode : Mode, IMode
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
        Card nextCard;
        do{
            nextCard = GetRandomCardFromListBasedOfChance(cardSet);
        }while (nextCard.Equals(lastCard) && cardSet.Count > 1);
        return nextCard;
    }

    public void EndMode(){

    }

    private Card GetRandomCardFromListBasedOfChance(List<Card> cardSet)
    {
        int sum = SumPoints(cardSet);
        int randomChance = UnityEngine.Random.Range(0, sum);

        float currentChance = 0;
        foreach (Card c in cardSet)
        {
            currentChance += c.CurrentPoints;
            if (currentChance >= randomChance)
            {
                return c;
            }
        }
        return null;
    }

    private int SumPoints(List<Card> cardSet)
    {
        int sum = 0;
        cardSet.ForEach(c => sum += c.CurrentPoints);
        return sum;
    } 
}
