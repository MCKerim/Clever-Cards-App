using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QueryMode : Mode, IMode
{
    [SerializeField] private GameObject cardNumberHolder;
    [SerializeField] private TextMeshProUGUI cardNumberText;
    private List<Card> askedCards = new List<Card>();

    public void StartMode()
    {
        askedCards.Clear();
        cardNumberHolder.SetActive(true);
    }

    public List<Card> PrepareCardSet(List<Card> cardSet)
    {
        return cardSet;
    }

    public Card GetCard(List<Card> cardSet, Card lastCard)
    {
        List<Card> notAskedCards = new List<Card>();

        foreach(Card c in cardSet){
            if(!askedCards.Contains(c)){
                notAskedCards.Add(c);
            }
        }

        if (notAskedCards.Count == 0)
        {
            UpdateCounterText(askedCards.Count, cardSet.Count);
            return CardBuilder.InfoCard("We asked you all questions in this deck.", "Please restart this mode.");
        }

        Card nextCard = ModeHelperFunctions.GetRandomCardFromList(notAskedCards);
        askedCards.Add(nextCard);
        UpdateCounterText(askedCards.Count, cardSet.Count);
        return nextCard;
    }

    public void EndMode()
    {
        cardNumberHolder.SetActive(false);
    }

    private void UpdateCounterText(int currentIndex, int maxIndex){
        cardNumberText.SetText(currentIndex + " / " + maxIndex);
    }
}

