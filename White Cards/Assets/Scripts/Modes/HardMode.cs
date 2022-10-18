using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardMode : Mode, IMode
{
    public void StartMode(){

    }

    public List<Card> PrepareCardSet(List<Card> cardSet){
        List<Card> onlyHardCards = new List<Card>();
        foreach (Card c in cardSet)
        {
            if (c.CurrentPoints >= 70)
            {
                onlyHardCards.Add(c);
            }
        }
        return onlyHardCards;
    }

    public Card GetCard(List<Card> cardSet, Card lastCard){
        if(cardSet.Count == 0){
            return CardBuilder.InfoCard("There are no 'HARD' rated cards in this category.", "Please choose another mode.");
        }
        return ModeHelperFunctions.GetDifferentRandomCardFromList(cardSet, lastCard);
    }

    public void EndMode(){

    }
}
