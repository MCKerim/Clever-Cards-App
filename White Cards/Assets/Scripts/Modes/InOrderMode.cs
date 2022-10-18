using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InOrderMode : Mode, IMode
{
    [SerializeField] private GameObject cardNumberHolder;
    [SerializeField] private TextMeshProUGUI cardNumberText;
    private int counter = 0;

    public void StartMode(){
        counter = 0;
        cardNumberHolder.SetActive(true);
        cardNumberText.SetText("");
    }

    public List<Card> PrepareCardSet(List<Card> cardSet){
        counter = 0;
        return cardSet;
    }

    public Card GetCard(List<Card> cardSet, Card lastCard){
        if(cardSet.Count == 0){
            UpdateCounterText(0, 0);
            return null;
        }

        if(counter >= cardSet.Count)
        {
            counter = 0;
        }

        Card nextCard = cardSet[counter];
        
        counter++;
        UpdateCounterText(counter, cardSet.Count);
        return nextCard;
    }

    public void EndMode(){
        cardNumberHolder.SetActive(false);
    }

    private void UpdateCounterText(int currentIndex, int maxIndex){
        cardNumberText.SetText(currentIndex + " / " + maxIndex);
    }
}
