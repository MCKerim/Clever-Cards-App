using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using NativeShareNamespace;
using NativeFilePickerNamespace;
using UnityEngine.Android;

public class CardManager : MonoBehaviour
{
    public int startpointsForCard;
    private Card currentCard;
    private Category currentCategory;

    private List<Card> currentCardSet;

    [SerializeField] private List<Category> categories;

    [SerializeField] private CardUIManager cardUIManager;

    [SerializeField] private UIManager uIManager;

    [SerializeField] private TextMeshProUGUI currentGameModeText;
    private GameMode currentGameMode;

    [SerializeField] private GameObject importErrorPanel;
    [SerializeField] private TextMeshProUGUI importErrorText;

    // Start is called before the first frame update
    void Start()
    {
        categories = LoadCategoriesFromFile();
        SelectGameMode(GameMode.Smart);
    }

    public void SelectCategory(Category category)
    {
        uIManager.HideCategoriesUI();
        uIManager.ShowGameUI();

        currentCategory = category;
        currentCardSet = LoadCardsOfCategoryFromFile(currentCategory);

        counterForInOrderMode = 0;
        ShowNextCard();
    }

    public void ResetCurrentCardSetPoints()
    {
        foreach(Card c in currentCardSet)
        {
            c.CurrentPoints = startpointsForCard;
        }
        ShowNextCard();
    }

    public void RateCard(int addPoints)
    {
        if(currentCard == null)
        {
            return;
        }

        if(currentGameMode == GameMode.Smart)
        {
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
        }
        
        ShowNextCard();
    }

    public void StartEditingCurrentCard()
    {
        if(currentCard == null)
        {
            return;
        }
        uIManager.HideGameUI();
        uIManager.ShowEditCardUI();
    }

    public void NextGameMode()
    {
        GameMode[] gameModes = (GameMode[])Enum.GetValues(typeof(GameMode));

        int currentEnumValue = (int) currentGameMode;
        currentEnumValue++;
        if(currentEnumValue >= gameModes.Length){
            currentEnumValue = 0;
        }

        SelectGameMode((GameMode) currentEnumValue);
    }

    private void SelectGameMode(GameMode gameMode)
    {
        currentGameMode = gameMode;
        currentGameModeText.SetText(currentGameMode + " Mode");
    }

    public void ShowNextCard()
    {
        switch(currentGameMode){
            case GameMode.Smart:
                currentCard = GetNextCardSmartMode();
            break;
            
            case GameMode.Random:
                currentCard = GetNextCardRandomMode();
            break;

            case GameMode.InOrder:
                currentCard = GetNextCardInOrderMode();
            break;

            default:
                Debug.LogError("Game Mode not implemented.");
            break;
        }
        cardUIManager.ShowCard(currentCard);
    }

    public void UpdateCurrentCardUI(){
        cardUIManager.ShowCard(currentCard);
    }

    int counterForInOrderMode = 0;
    private Card GetNextCardInOrderMode()
    {
        if(currentCardSet.Count == 0){
            return null;
        }

        Card nextCard = currentCardSet[counterForInOrderMode];

        counterForInOrderMode++;
        if(counterForInOrderMode >= currentCardSet.Count){
            counterForInOrderMode = 0;
        }
        return nextCard;
    }

    private Card GetNextCardRandomMode()
    {
        Card nextCard;
        do{
            nextCard = GetRandomCardFromList(currentCardSet);
        }while(nextCard.Equals(currentCard));

        return nextCard;
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

    private Card GetNextCardSmartMode()
    {
        Card nextCard;
        do{
            nextCard = GetRandomCardFromListBasedOfChance(currentCardSet);
        }while (currentCardSet.Count >= 2 && nextCard.Equals(currentCard));
        return nextCard;
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
        SaveCardsOfCategory(category, cardsOfCategory);
    }

    public void DeleteCurrentCard()
    {
        if(currentCard == null)
        {
            return;
        }

        currentCardSet.Remove(currentCard);
        ShowNextCard();
    }

    /*public void DeleteCard(Card card, Category category)
    {
        List<Card> cardsOfCategory =  LoadCardsOfCategoryFromFile(category);
        cardsOfCategory.Remove(card);
        SaveCardsOfCategory(cardsOfCategory, category);
    }*/

    public List<Category> GetAllCategories()
    {
        return categories;
    }

    public Card GetCurrentCard(){
        return currentCard;
    }

    public Category GetCurrentCategory(){
        return currentCategory;
    }

    public void SaveCategory(Category newCategory)
    {
        categories.Add(newCategory);
        SaveCategories();
    }

    private Category categorySelectedToDelete;
    [SerializeField] private GameObject confirmCategoryDeletionPanel;
    [SerializeField] private TextMeshProUGUI confirmCategoryDeletionText;
    public void ShowConfirmDeleteCategoryPanel(Category category)
    {
        categorySelectedToDelete = category;
        confirmCategoryDeletionPanel.SetActive(true);
        confirmCategoryDeletionText.SetText("Sure you want to delete category: " + category.Name + "?");
    }

    public void HideConfirmDeleteCategoryPanel()
    {
        categorySelectedToDelete = null;
        confirmCategoryDeletionPanel.SetActive(false);
    }

    public void DeleteSelectedCategory()
    {
        categories.Remove(categorySelectedToDelete);
        //Delete cards file for that category
        try
        {
            string path = Application.persistentDataPath + "/" + categorySelectedToDelete.Uuid + ".MCKerimData";
            File.Delete(path);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        SaveCategories();
        GameObject.FindObjectOfType<CategoryUIManager>().UpdateCategoryUI();
        HideConfirmDeleteCategoryPanel();
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
            SaveCardsOfCategory(currentCategory, currentCardSet);
        }
    }

    public void SaveCategories(){
        Debug.Log("Try to save Categories..");
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/categories.MCKerimData";
        FileStream stream = new FileStream(path, FileMode.Create);

        CategoriesData categoriesData = new CategoriesData(categories);

        formatter.Serialize(stream, categoriesData);
        stream.Close();
        Debug.Log("Categories Saved in: " + path);
    }

    private void SaveCardsOfCategory(Category category, List<Card> cards){
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

    public List<Card> LoadCardsOfCategoryFromFile(Category category)
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

    public void ShareCategory(Category category)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/" + category.Name + ".txt";
        FileStream stream = new FileStream(path, FileMode.Create);

        ShareableCategoryData shareableCategoryData = new ShareableCategoryData(category, LoadCardsOfCategoryFromFile(category));

        formatter.Serialize(stream, shareableCategoryData);
        stream.Close();

        new NativeShare().AddFile( path )
		.SetSubject( category.Name + " Category" ).SetText( "Import this file in White Cards App to learn with it!" ).SetUrl( "https://MCKerim.com" )
		.SetCallback( ( result, shareTarget ) => 
        {
            Debug.Log("Share result: " + result + ", selected app: " + shareTarget);
            File.Delete(path); 
        })
		.Share();
    }

    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        importErrorPanel.SetActive(true);
        importErrorText.SetText("Please give this app permission to read files. We cannot ask again.");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        PermissionGranted();
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        importErrorPanel.SetActive(true);
        importErrorText.SetText("Please give this app permission to read files.");
    }

    public void ImportCategory()
    {
        if(NativeFilePicker.IsFilePickerBusy()){
            return;
        }

        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            PermissionGranted();
        }
        else
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
            Permission.RequestUserPermission(Permission.ExternalStorageRead, callbacks);
        }
    }   

    private void PermissionGranted()
    {
        string fileType = NativeFilePicker.ConvertExtensionToFileType( "txt" );

        NativeFilePicker.PickFile( ( path ) =>
		{
			if( path == null ){
                Debug.Log( "Operation cancelled" );
            }
			else{
                LoadShareableCategory(path);
            }
		}, new string[] { fileType } );

    }

    private void LoadShareableCategory(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ShareableCategoryData shareableCategoryData = null;
            try{
                shareableCategoryData = formatter.Deserialize(stream) as ShareableCategoryData;
                stream.Close();
            }catch(Exception e){
                importErrorPanel.SetActive(true);
                importErrorText.SetText("Please select another file. " + e.Message);
                stream.Close();
                return;
            }
            if(shareableCategoryData == null){
                return;
            }

            Category newCategory = new Category(Guid.NewGuid(), shareableCategoryData.category.Name);
            SaveCategory(newCategory);

            List<Card> cleanCards = new List<Card>();
            foreach(Card c in shareableCategoryData.cards){
                Card card = new Card(c.Question, c.Answear, c.ImageBytesQuestion, c.ImageBytesAnswear, startpointsForCard, newCategory.Uuid);
                cleanCards.Add(card);
            }
            SaveCardsOfCategory(newCategory, cleanCards);
            GameObject.FindObjectOfType<CategoryUIManager>().UpdateCategoryUI();
        }
        else
        {
            importErrorPanel.SetActive(true);
            importErrorText.SetText("File not found.");
        }
    }

    public void HideImportErrorPanel()
    {
        importErrorText.SetText("");
        importErrorPanel.SetActive(false);
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

[System.Serializable]
public class ShareableCategoryData
{
    public Category category;
    public List<Card> cards;

    public ShareableCategoryData(Category category, List<Card> cards){
        this.category = category;
        this.cards = cards;
    }
}
