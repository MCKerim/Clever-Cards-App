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

        return ModeHelperFunctions.GetDifferentRandomCardFromList(cardSet, lastCard);
    }

    public void EndMode(){

    }
}