using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CardManager : MonoBehaviour
{
    private Card currentCard;
    private Category currentCategory;

    private List<Card> currentCardSet;

    [SerializeField] private List<Category> categories;

    [SerializeField] private CardUIManager cardUIManager;

    [SerializeField] private UIManager uIManager;

    // Start is called before the first frame update
    void Start()
    {
        categories = LoadCategoriesFromFile();
    }

    public void SelectCategory(Category category)
    {
        uIManager.HideCategoriesUI();
        uIManager.ShowGameUI();

        currentCategory = category;
        currentCardSet = LoadCardsOfCategoryFromFile(currentCategory);

        ShowNextCard();
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

    public void SaveCard(Card card, Category category)
    {
        List<Card> cardsOfCategory =  LoadCardsOfCategoryFromFile(category);
        cardsOfCategory.Add(card);
        SaveCardsOfCategory(cardsOfCategory, category);
    }

    public void DeleteCurrentCard()
    {
        currentCardSet.Remove(currentCard);
        ShowNextCard();
    }

    /*public void DeleteCard(Card card, Category category)
    {
        List<Card> cardsOfCategory =  LoadCardsOfCategoryFromFile(category);
        cardsOfCategory.Remove(card);
        SaveCardsOfCategory(cardsOfCategory, category);
    }*/

    public void CreateTestCards(int amount)
    {
        for(int i=0; i < amount; i++)
        {
            Guid uuid = Guid.NewGuid();
            SaveCard(new Card(uuid, "Question " + i, "Answear " + i, null, null, 50, categories[0].Uuid), categories[0]);
        }
    }

    public void CreateTestCategories(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Guid uuid = Guid.NewGuid();
            Category testCategory = new Category(uuid, "Category " + i);
            SaveCategory(testCategory);
        }
    }

    public List<Category> GetAllCategories()
    {
        return categories;
    }

    public void SaveCategory(Category newCategory)
    {
        categories.Add(newCategory);
        SaveCategories();
    }

    public void DeleteCategory(Category category)
    {
        categories.Remove(category);
        //Delete cards file for that category
        try
        {
            string path = Application.persistentDataPath + "/" + category.Uuid + ".MCKerimData";
            File.Delete(path);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        SaveCategories();
    }

    private void OnApplicationQuit()
    {
        SaveCurrentCardSet();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveCurrentCardSet();
        }
    }

    public void SaveCurrentCardSet(){
        if(currentCardSet != null){
            SaveCardsOfCategory(currentCardSet, currentCategory);
        }
    }

    /*private void SaveData()
    {
        Debug.Log("Try to Save..");
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
    }*/

    private void SaveCategories(){
        Debug.Log("Try to save Categories..");
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/categories.MCKerimData";
        FileStream stream = new FileStream(path, FileMode.Create);

        CategoriesData categoriesData = new CategoriesData(categories);

        formatter.Serialize(stream, categoriesData);
        stream.Close();
        Debug.Log("Categories Saved in: " + path);
    }

    private void SaveCardsOfCategory(List<Card> cards, Category category){
        Debug.Log("Try to save Cards of a Category..");
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/" + category.Uuid + ".MCKerimData";
        FileStream stream = new FileStream(path, FileMode.Create);

        CardsFromCategoryData cardsFromCategoryData = new CardsFromCategoryData(cards);

        formatter.Serialize(stream, cardsFromCategoryData);
        stream.Close();
        Debug.Log("Cards Saved in: " + path);
    }

    public List<Category> LoadCategoriesFromFile()
    {
        string path = Application.persistentDataPath + "/categories.MCKerimData";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            CategoriesData categoriesData = formatter.Deserialize(stream) as CategoriesData;

            stream.Close();
            Debug.Log("Save File Found in: " + path);
            return categoriesData.categories;
        }
        else
        {
            Debug.Log("Save File not found in " + path);
            List<Category> categories = new List<Category>();
            return categories;
        }
    }

    private List<Card> LoadCardsOfCategoryFromFile(Category category)
    {
        string path = Application.persistentDataPath + "/" + category.Uuid + ".MCKerimData";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            CardsFromCategoryData cardsFromCategoryData = formatter.Deserialize(stream) as CardsFromCategoryData;

            stream.Close();
            Debug.Log("Save File Found in: " + path);
            return cardsFromCategoryData.cards;
        }
        else
        {
            Debug.Log("Save File not found in " + path);
            List<Card> cards = new List<Card>();
            return cards;
        }
    }
}

[System.Serializable]
public class CategoriesData
{
    public List<Category> categories;

    public CategoriesData(List<Category> categories)
    {
        this.categories = categories;
    }
}

[System.Serializable]
public class CardsFromCategoryData
{
    public List<Card> cards;

    public CardsFromCategoryData(List<Card> cards){
        this.cards = cards;
    }
}
