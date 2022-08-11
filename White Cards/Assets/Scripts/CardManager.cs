using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CardManager : MonoBehaviour
{
    [SerializeField] private List<Card> allCards;
    private Card currentCard;
    private Category currentCategory;

    private List<Card> currentCardSet;

    [SerializeField] private List<Category> categories;

    [SerializeField] private CardUIManager cardUIManager;

    [SerializeField] private UIManager uIManager;

    // Start is called before the first frame update
    void Start()
    {
        categories = new List<Category>();
        allCards = new List<Card>();

        Data data = LoadData();
        if(data != null)
        {
            allCards.AddRange(data.allCards);
            categories.AddRange(data.categories);
        }
        else
        {
            CreateTestCategories(3);
            CreateTestCards(5);
            SaveData();
        }

        foreach (Card c in allCards)
        {
            Debug.Log("All Cards: " + c.Question);
        }
    }

    public void SelectCategory(Category category)
    {
        uIManager.HideCategorysUI();
        uIManager.ShowGameUI();

        currentCategory = category;
        currentCardSet = GetCardSetForCategory(currentCategory);

        ShowNextCard();
    }

    private List<Card> GetCardSetForCategory(Category category)
    {
        List<Card> cards = new List<Card>();

        foreach(Card c in allCards)
        {
            if (c.CategoryUuid.Equals(category.Uuid))
            {
                cards.Add(c);
            }
        }
        return cards;
    }

    public void RateCard(int addPoints)
    {
        if(currentCard == null)
        {
            return;
        }

        if (currentCard.CurrentPoints + addPoints >= 10 && currentCard.CurrentPoints + addPoints <= 100)
        {
            currentCard.CurrentPoints += addPoints;
        }
        else if (currentCard.CurrentPoints + addPoints > 100)
        {
            currentCard.CurrentPoints = 100;
        }
        else
        {
            currentCard.CurrentPoints = 10;
        }
        ShowNextCard();
    }

    public void ShowNextCard()
    {
        Card newCard = GetRandomCardFromListBasedOfChance(currentCardSet);

        while (currentCardSet.Count >= 2 && newCard.Equals(currentCard))
        {
            newCard = GetRandomCardFromListBasedOfChance(currentCardSet);
        }

        currentCard = newCard;
        cardUIManager.ShowCard(currentCard);
    }

    private Card GetRandomCardFromList(List<Card> cards)
    {
        if(cards.Count <= 0)
        {
            return null;
        }
        int randomIndex = UnityEngine.Random.Range(0, cards.Count);
        return cards[randomIndex];
    }

    private Card GetRandomCardFromListBasedOfChance(List<Card> cards)
    {
        int sum = SumPoints(cards);
        int randomChance = UnityEngine.Random.Range(0, sum);

        float currentChance = 0;
        foreach (Card c in cards)
        {
            currentChance += c.CurrentPoints;
            if (currentChance >= randomChance)
            {
                return c;
            }
        }
        return null;
    }

    private int SumPoints(List<Card> cards)
    {
        int sum = 0;
        cards.ForEach(c => sum += c.CurrentPoints);
        return sum;
    } 

    public void SafeCard(Card newCard)
    {
        allCards.Add(newCard);
    }

    public void DeleteCurrentCard()
    {
        DeleteCard(currentCard);
        ShowNextCard();
    }

    public void DeleteCard(Card card)
    {
        currentCardSet.Remove(currentCard);
        allCards.Remove(card);
    }

    public void CreateTestCards(int amount)
    {
        for(int i=0; i < amount; i++)
        {
            Guid uuid = Guid.NewGuid();
            SafeCard(new Card(uuid, "Question " + i, "Answear " + i, null, null, 50, categories[0].Uuid));
        }
    }

    public void SafeCategory(Category newCategory)
    {
        categories.Add(newCategory);
    }

    public void DeleteCategory(Category category)
    {
        allCards.RemoveAll(c => c.CategoryUuid.Equals(category.Uuid));
        categories.Remove(category);
    }

    public List<Category> GetAllCategories()
    {
        return categories;
    }

    public void CreateTestCategories(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Guid uuid = Guid.NewGuid();
            SafeCategory(new Category(uuid, "Category " + i));
        }
    }



    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }

    private void SaveData()
    {
        Debug.Log("Try to save..");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/whiteCardsData.MCKerimData";
        FileStream stream = new FileStream(path, FileMode.Create);

        Data data = new Data(allCards, categories);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Data Saved in: " + path);

        foreach (Card c in allCards)
        {
            Debug.Log("All Cards: " + c.Question);
        }
    }

    public Data LoadData()
    {
        string path = Application.persistentDataPath + "/whiteCardsData.MCKerimData";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Data data = formatter.Deserialize(stream) as Data;
            stream.Close();
            Debug.Log("Save File Found in: " + path);
            return data;
        }
        else
        {
            Debug.Log("Save File not found in " + path);
            return null;
        }
    }
}

[System.Serializable]
public class Data
{
    public List<Card> allCards;
    public List<Category> categories;

    public Data(List<Card> allCards, List<Category> categories)
    {
        this.allCards = allCards;
        this.categories = categories;
    }
}
