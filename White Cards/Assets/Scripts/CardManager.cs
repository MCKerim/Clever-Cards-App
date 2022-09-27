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
    [SerializeField] private CreateCardManager createCardManager;
    [SerializeField] private GameObject categorySettingsPopup;
    [SerializeField] private TextMeshProUGUI currentCategoryText;


    // Start is called before the first frame update
    void Awake()
    {
        categories = LoadCategoriesFromFile();
        SelectGameMode(GameMode.Smart);
    }

    public void SelectCategory(Category category)
    {
        uIManager.HideCategoriesUI();
        uIManager.ShowGameUI();

        currentCategory = category;
        currentCategoryText.SetText(currentCategory.Name);
        currentCardSet = LoadCardsOfCategoryFromFile(currentCategory);

        currentCard = GetNextCard(currentGameMode);
        cardUIManager.ShowFirstCard(currentCard);
    }

    public void ResetCurrentCardSetPoints()
    {
        foreach(Card c in currentCardSet)
        {
            c.CurrentPoints = startpointsForCard;
        }
        currentCard = GetNextCard(currentGameMode);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    public void RateCardEasy()
    {
        if(currentCard == null)
        {
            return;
        }

        if(currentGameMode == GameMode.Smart){
            RateCard(-10, currentCard);
        }

        currentCard = GetNextCard(currentGameMode);
        cardUIManager.MoveCardLeft(currentCard);
    }

    public void RateCardMedium()
    {
        if(currentCard == null)
        {
            return;
        }

        if(currentGameMode == GameMode.Smart){
            RateCard(0, currentCard);
        }

        currentCard = GetNextCard(currentGameMode);
        cardUIManager.MoveCardDown(currentCard);
    }

    public void RateCardHard()
    {
        if(currentCard == null)
        {
            return;
        }

        if(currentGameMode == GameMode.Smart){
            RateCard(10, currentCard);
        }
        
        currentCard = GetNextCard(currentGameMode);
        cardUIManager.MoveCardRight(currentCard);
    }

    private void RateCard(int addPoints, Card card)
    {
        if (card.CurrentPoints + addPoints >= 10 && card.CurrentPoints + addPoints <= 100)
        {
            card.CurrentPoints += addPoints;
        }
        else if (card.CurrentPoints + addPoints > 100)
        {
            card.CurrentPoints = 100;
        }
        else
        {
            card.CurrentPoints = 10;
        }
    }

    public void StartEditingCurrentCard()
    {
        if(currentCard == null)
        {
            return;
        }
        uIManager.HideGameUI();
        createCardManager.StartEditingCurrentCard();
    }

    public void StartCreatingNewCard()
    {
        uIManager.HideGameUI();
        createCardManager.StartCreatingNewCard();
    }

    public void PreviousGameMode()
    {
        GameMode[] gameModes = (GameMode[])Enum.GetValues(typeof(GameMode));

        int currentEnumValue = (int) currentGameMode;
        currentEnumValue--;
        if(currentEnumValue < 0){
            currentEnumValue = gameModes.Length-1;
        }

        SelectGameMode((GameMode) currentEnumValue);

        currentCard = GetNextCard(currentGameMode);
        cardUIManager.ShowCardWithoutAnim(currentCard);
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

        currentCard = GetNextCard(currentGameMode);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    private void SelectGameMode(GameMode gameMode)
    {
        currentGameMode = gameMode;
        currentGameModeText.SetText(currentGameMode + " Mode");

        counterForInOrderMode = 0;
    }

    public Card GetNextCard(GameMode mode)
    {
        Card nextCard = null;
        switch(mode){
            case GameMode.Smart:
                nextCard = GetNextCardSmartMode();
            break;
            
            case GameMode.Random:
                nextCard = GetNextCardRandomMode();
            break;

            case GameMode.InOrder:
                nextCard = GetNextCardInOrderMode();
            break;

            case GameMode.Hard:
                nextCard = GetNextCardHardMode();
            break;

            default:
                Debug.LogError("Game Mode not implemented.");
            break;
        }
        return nextCard;
    }

    int counterForInOrderMode = 0;
    private Card GetNextCardInOrderMode()
    {
        if(currentCardSet.Count == 0){
            return null;
        }

        if(counterForInOrderMode >= currentCardSet.Count)
        {
            counterForInOrderMode = 0;
        }

        Card nextCard = currentCardSet[counterForInOrderMode];

        counterForInOrderMode++;
        return nextCard;
    }

    private Card GetNextCardRandomMode()
    {
        if(currentCardSet.Count == 0){
            return null;
        }

        return GetDifferentRandomCardFromList(currentCardSet);
    }

    private Card GetNextCardHardMode()
    {
        if(currentCardSet.Count == 0){
            return null;
        }

        List<Card> hardCards = new List<Card>();
        foreach(Card c in currentCardSet){
            if(c.CurrentPoints >= 70)
            {
                hardCards.Add(c);
            }
        }

        if(hardCards.Count == 0){
            return new Card("No Hard rated cards in this deck.", "Please choose another mode.", null, null, 100, Guid.Empty);
        }

        return GetDifferentRandomCardFromList(hardCards);
    }

    private Card GetDifferentRandomCardFromList(List<Card> cards)
    {
        if(cards.Count == 0){
            return null;
        }

        Card nextCard;
        do{
            nextCard = GetRandomCardFromList(cards);
        }while(nextCard.Equals(currentCard) && cards.Count > 1);

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

    private Card GetNextCardSmartMode()
    {
        if(currentCardSet.Count == 0){
            return null;
        }

        Card nextCard;
        do{
            nextCard = GetRandomCardFromListBasedOfChance(currentCardSet);
        }while (currentCardSet.Count > 1 && nextCard.Equals(currentCard));
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

    public void AddCardToCurrentCategory(Card newCard)
    {
        currentCardSet.Add(newCard);
        SetCurrentCard(newCard);
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
        currentCard = GetNextCard(currentGameMode);
        cardUIManager.MoveCardRight(currentCard);
    }

    public void SetCurrentCard(Card card)
    {
        currentCard = card;
        UpdateCurrentCardUI();
    }

    public void UpdateCurrentCardUI()
    {
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

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

    public void ShowCategorySettingsPopup()
    {
        categorySettingsPopup.SetActive(true);
    }

    public void HideCategorySettingsPopup()
    {
        categorySettingsPopup.SetActive(false);
    }

    private Category categorySelectedToDelete;
    [SerializeField] private GameObject confirmCategoryDeletionPopup;
    [SerializeField] private TextMeshProUGUI confirmCategoryDeletionText;
    public void ShowConfirmDeleteCategoryPopup(Category category)
    {
        categorySelectedToDelete = category;
        confirmCategoryDeletionPopup.SetActive(true);
        confirmCategoryDeletionText.SetText("Sure you want to delete category: " + category.Name + "?");
    }

    public void HideConfirmDeleteCategoryPopup()
    {
        categorySelectedToDelete = null;
        confirmCategoryDeletionPopup.SetActive(false);
    }

    [SerializeField] private GameObject createCategoryPopup;
    [SerializeField] private TMP_InputField createCategoryPopupInputField;
    private Category currentEditedCategory = null;

    public delegate void CategoryUpdateAction();
    public static event CategoryUpdateAction OnCategorysChanged;

    public void CreateCategoryButtonClicked()
    {
        createCategoryPopup.SetActive(true);
    }

    public void EditCategoryNameButtonClicked(Category category)
    {
        currentEditedCategory = category;
        createCategoryPopup.SetActive(true);
        createCategoryPopupInputField.text = currentEditedCategory.Name;
    }

    public void CategoryPopupConfirmButtonClicked()
    {
        string categoryName = createCategoryPopupInputField.text;
        if(categoryName == ""){
            return;
        }
        createCategoryPopupInputField.text = "";

        if(currentEditedCategory == null)
        {
            Category category = new Category(Guid.NewGuid(), categoryName);
            SaveCategory(category);
        }
        else
        {
            currentEditedCategory.Name = categoryName;
            SaveCategories();
            currentEditedCategory = null;
        }
        
        if(OnCategorysChanged != null){
            OnCategorysChanged();
        }

        HideCreateCategoryPopup();
    }

    public void CategoryPopupCancelButtonClicked()
    {
        currentEditedCategory = null;
        createCategoryPopupInputField.text = "";
        HideCreateCategoryPopup();
    }

    private void HideCreateCategoryPopup()
    {
        createCategoryPopup.SetActive(false);
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
        HideConfirmDeleteCategoryPopup();
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
		.SetSubject( category.Name + " Category" ).SetText( "Import this file in the White Cards App to start learning!" ).SetUrl( "https://MCKerim.com" )
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
        importErrorText.SetText("Please give this app permission to read files.");
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
                importErrorText.SetText("Something went wrong while loading this file. Please select another File. " + e.Message);
                stream.Close();
                return;
            }
            if(shareableCategoryData == null){
                importErrorPanel.SetActive(true);
                importErrorText.SetText("Something went wrong while loading this file. Please select another file or try again.");
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
