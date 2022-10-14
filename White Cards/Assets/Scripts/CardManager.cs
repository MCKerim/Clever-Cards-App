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
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    private Card currentCard;
    private Category currentCategory;

    private List<Card> currentCardSet = new List<Card>();
    private List<Card> filteredCardSet = new List<Card>();

    [SerializeField] private List<Category> categories;

    [SerializeField] private CardUIManager cardUIManager;

    [SerializeField] private UIManager uIManager;

    [SerializeField] private TextMeshProUGUI currentGameModeEnumText;
    private GameModeEnum currentGameModeEnum;
    private bool onlyFavorites;
    private bool filterCardsWithTags;
    private List<Tag> activeTags = new List<Tag>();

    [SerializeField] private GameObject importErrorPanel;
    [SerializeField] private TextMeshProUGUI importErrorText;
    [SerializeField] private CreateCardManager createCardManager;
    [SerializeField] private GameObject categorySettingsPopup;
    [SerializeField] private GameObject tagSettingsPopup;
    [SerializeField] private TagButtonsManager tagButtonsManager;

    [SerializeField] private TextMeshProUGUI onlyFavoritesToggleText;
    [SerializeField] private Image onlyFavoritesToggleBackground;
    [SerializeField] private TextMeshProUGUI filterCardsWithTagsToggleText;
    [SerializeField] private Image filterCardsWithTagsToggleBackground;
    [SerializeField] private Color toggleActiveColor;
    [SerializeField] private Color toggleDeactiveColor;

    [SerializeField] private TextMeshProUGUI currentCategoryText;

    // Start is called before the first frame update
    void Awake()
    {
        categories = LoadCategoriesFromFile();

        currentGameModeEnum = GameModeEnum.Smart;
        currentGameModeEnumText.SetText(currentGameModeEnum + " Mode");
    }

    public void SelectCategory(Category category)
    {
        uIManager.HideCategoriesUI();
        uIManager.ShowGameUI();

        currentCategory = category;
        currentCategoryText.SetText(currentCategory.Name);
        currentCardSet = LoadCardsOfCategoryFromFile(currentCategory);

        PrepareFilteredCardSet();
        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.ShowFirstCard(currentCard);
    }

    public void ResetCurrentCardSetPoints()
    {
        foreach(Card c in currentCardSet)
        {
            c.CurrentPoints = CardBuilder.startpointsForCard;
        }
        PrepareFilteredCardSet();
        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    public void RateCardEasy()
    {
        if(currentCard == null)
        {
            return;
        }

        if(currentGameModeEnum == GameModeEnum.Smart || currentGameModeEnum == GameModeEnum.Hard){
            RateCard(-10, currentCard);
            if(currentGameModeEnum == GameModeEnum.Hard && currentCard.CurrentPoints < 70)
            {
                filteredCardSet.Remove(currentCard);
            }
        }

        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.MoveCardLeft(currentCard);
    }

    public void RateCardMedium()
    {
        if(currentCard == null)
        {
            return;
        }

        if(currentGameModeEnum == GameModeEnum.Smart || currentGameModeEnum == GameModeEnum.Hard){
            RateCard(0, currentCard);
        }

        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.MoveCardDown(currentCard);
    }

    public void RateCardHard()
    {
        if(currentCard == null)
        {
            return;
        }

        if(currentGameModeEnum == GameModeEnum.Smart || currentGameModeEnum == GameModeEnum.Hard){
            RateCard(10, currentCard);
        }
        
        currentCard = GetNextCard(currentGameModeEnum);
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

    public void SetOnlyFavorites(bool onlyFavorites)
    {
        this.onlyFavorites = onlyFavorites;
        
        if(onlyFavorites)
        {
            onlyFavoritesToggleText.color = toggleActiveColor;
            onlyFavoritesToggleBackground.color = toggleActiveColor;
        }
        else
        {
            onlyFavoritesToggleText.color = toggleDeactiveColor;
            onlyFavoritesToggleBackground.color = toggleDeactiveColor;
        }

        PrepareFilteredCardSet();
        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    public void SetFilterCardsWithTags(bool filterCardsWithTags)
    {
        this.filterCardsWithTags = filterCardsWithTags;
        
        if(filterCardsWithTags)
        {
            filterCardsWithTagsToggleText.color = toggleActiveColor;
            filterCardsWithTagsToggleBackground.color = toggleActiveColor;
        }
        else
        {
            filterCardsWithTagsToggleText.color = toggleDeactiveColor;
            filterCardsWithTagsToggleBackground.color = toggleDeactiveColor;
        }

        PrepareFilteredCardSet();
        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    public void PreviousGameModeEnum()
    {
        GameModeEnum[] GameModeEnums = (GameModeEnum[])Enum.GetValues(typeof(GameModeEnum));

        int currentEnumValue = (int) currentGameModeEnum;
        currentEnumValue--;
        if(currentEnumValue < 0){
            currentEnumValue = GameModeEnums.Length-1;
        }

        SelectGameModeEnum((GameModeEnum) currentEnumValue);

        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    public void NextGameModeEnum()
    {
        GameModeEnum[] GameModeEnums = (GameModeEnum[])Enum.GetValues(typeof(GameModeEnum));

        int currentEnumValue = (int) currentGameModeEnum;
        currentEnumValue++;
        if(currentEnumValue >= GameModeEnums.Length){
            currentEnumValue = 0;
        }

        SelectGameModeEnum((GameModeEnum) currentEnumValue);

        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    private void SelectGameModeEnum(GameModeEnum GameModeEnum)
    {
        currentGameModeEnum = GameModeEnum;
        currentGameModeEnumText.SetText(currentGameModeEnum + " Mode");
        
        if(GameModeEnum == GameModeEnum.InOrder){
            counterForInOrderMode = 0;
            cardNumberHolder.SetActive(true);
            cardNumberText.SetText(0 + " / " + currentCardSet.Count);
        }
        else
        {
            cardNumberHolder.SetActive(false);
        }
        PrepareFilteredCardSet();
        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    private void PrepareFilteredCardSet()
    {
        filteredCardSet.Clear();
        filteredCardSet.AddRange(currentCardSet);

        if(onlyFavorites)
        {
            filteredCardSet = FilterCardSetFavorites(filteredCardSet);
        }

        if(filterCardsWithTags)
        {
            filteredCardSet = FilterCardSetTags(filteredCardSet, activeTags);
        }

        filteredCardSet = FilterCardSetGameModeEnum(filteredCardSet, currentGameModeEnum);
    }

    public void CurrentCardFavoriteStatusWasUpdated()
    {
        if(currentCard == null){
            return;
        }

        if(onlyFavorites && !currentCard.IsFavorite)
        {
            filteredCardSet.Remove(currentCard);
        }
    }

    public void CurrentCardsTagsWhereUpdated()
    {
        if(currentCard == null){
            return;
        }
        
        if (filterCardsWithTags)
        {
            if (!currentCard.HasTag(activeTags))
            {
                filteredCardSet.Remove(currentCard);
            }
        }
    }

    private List<Card> FilterCardSetFavorites(List<Card> cardSetToFilter)
    {
        List<Card> onlyFavoriteCards = new List<Card>();
        foreach (Card c in cardSetToFilter)
        {
            if (c.IsFavorite)
            {
                onlyFavoriteCards.Add(c);
            }
        }
        return onlyFavoriteCards;
    }

    private List<Card> FilterCardSetTags(List<Card> cardSetToFilter, List<Tag> activeTags)
    {
        List<Card> onlyCardsWithTag = new List<Card>();

        if(activeTags.Count == 0)
        {
            return onlyCardsWithTag;
        }

        foreach (Card c in cardSetToFilter)
        {
            if (c.HasTag(activeTags))
            {
                onlyCardsWithTag.Add(c);
            }
        }
        return onlyCardsWithTag;
    }

    private List<Card> FilterCardSetGameModeEnum(List<Card> cardSetToFilter, GameModeEnum mode)
    {
        switch (mode){
            case GameModeEnum.Hard:
                List<Card> onlyHardCards = new List<Card>();
                foreach (Card c in cardSetToFilter)
                {
                    if (c.CurrentPoints >= 70)
                    {
                        onlyHardCards.Add(c);
                    }
                }
                return onlyHardCards;
            default:
                    return cardSetToFilter;
        }
    }

    public Card GetNextCard(GameModeEnum mode)
    {
        Card nextCard = null;
        if(currentCardSet.Count == 0)
        {
            return null;
        }

        if(filteredCardSet.Count == 0){
            if(onlyFavorites){
                return CardBuilder.InfoCard("There are no favorite cards in this category.", "Please toggle 'ONLY FAVORITES' in the settings menu off.");
            }
            else if(filterCardsWithTags)
            {
                return CardBuilder.InfoCard("There are no cards with these tags in this category.", "Please select other tags.");
            }
        }
       
        switch(mode){
            case GameModeEnum.Smart:
                nextCard = GetNextCardSmartMode(filteredCardSet);
            break;
            
            case GameModeEnum.Random:
                nextCard = GetNextCardRandomMode(filteredCardSet);
            break;

            case GameModeEnum.InOrder:
                nextCard = GetNextCardInOrderMode(filteredCardSet);
            break;

            case GameModeEnum.Hard:
                nextCard = GetNextCardHardMode(filteredCardSet);
            break;

            default:
                Debug.LogError("Game Mode not implemented.");
            break;
        }
        return nextCard;
    }

    int counterForInOrderMode = 0;
    private Card GetNextCardInOrderMode(List<Card> cardSet)
    {
        if(cardSet.Count == 0){
            return null;
        }

        if(counterForInOrderMode >= cardSet.Count)
        {
            counterForInOrderMode = 0;
        }

        Card nextCard = cardSet[counterForInOrderMode];
        cardNumberText.SetText(counterForInOrderMode+1 + " / " + cardSet.Count);

        counterForInOrderMode++;
        return nextCard;
    }

    private Card GetNextCardRandomMode(List<Card> cardSet)
    {
        if(cardSet.Count == 0){
            return null;
        }

        return GetDifferentRandomCardFromList(cardSet);
    }

    private Card GetNextCardHardMode(List<Card> cardSet)
    {
        if(cardSet.Count == 0){
            return CardBuilder.InfoCard("There are no 'HARD' rated cards in this category.", "Please choose another mode.");
        }

        return GetDifferentRandomCardFromList(cardSet);
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

    private Card GetNextCardSmartMode(List<Card> cardSet)
    {
        if(cardSet.Count == 0){
            return null;
        }
        Card nextCard;
        do{
            nextCard = GetRandomCardFromListBasedOfChance(cardSet);
        }while (nextCard.Equals(currentCard) && cardSet.Count > 1);
        return nextCard;
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

    public void AddCardToCurrentCategory(Card newCard)
    {
        currentCardSet.Add(newCard);
        PrepareFilteredCardSet();
        SetCurrentCard(newCard);
    }

    public void SaveCard(Card card, Category category)
    {
        List<Card> cardsOfCategory =  LoadCardsOfCategoryFromFile(category);
        cardsOfCategory.Add(card);
        SaveCardsOfCategory(category, cardsOfCategory);
    }

    public void DeleteTagFromCurrentCategory(Tag tag)
    {
        currentCategory.DeleteTag(tag);
        foreach(Card c in currentCardSet){
            c.RemoveTag(tag);
        }
    }

    public void ShowTagSettingsPopup()
    {
        tagSettingsPopup.SetActive(true);
        tagButtonsManager.UpdateTagList(currentCategory.Tags);
        tagButtonsManager.SetSelectedTags(activeTags);
    }

    public void HideTagSettingsPopup()
    {
        activeTags = tagButtonsManager.GetSelectedTags();
        PrepareFilteredCardSet();
        currentCard = GetNextCard(currentGameModeEnum);
        cardUIManager.ShowCardWithoutAnim(currentCard);
        
        tagSettingsPopup.SetActive(false);
    }

    public void DeleteCurrentCard()
    {
        if(currentCard == null)
        {
            return;
        }

        currentCardSet.Remove(currentCard);
        filteredCardSet.Remove(currentCard);
        currentCard = GetNextCard(currentGameModeEnum);
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
    [SerializeField] private ColorButtonsManager createCategoryColorSelection;
    [SerializeField] private GameObject cardNumberHolder;
    [SerializeField] private TextMeshProUGUI cardNumberText;
    private Category currentEditedCategory = null;

    public delegate void CategoryUpdateAction();
    public static event CategoryUpdateAction OnCategorysChanged;

    public void CreateCategoryButtonClicked()
    {
        createCategoryPopup.SetActive(true);
    }

    public void EditCategoryButtonClicked(Category category)
    {
        currentEditedCategory = category;
        createCategoryPopup.SetActive(true);
        createCategoryPopupInputField.text = currentEditedCategory.Name;
        createCategoryColorSelection.SelectButton(currentEditedCategory.Color);
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
            Category category = new Category(Guid.NewGuid(), categoryName, createCategoryColorSelection.GetSelectedColor(), new List<Tag>());
            SaveCategory(category);
        }
        else
        {
            currentEditedCategory.Name = categoryName;
            currentEditedCategory.Color = createCategoryColorSelection.GetSelectedColor();
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
        if(currentCategory != null && currentCardSet != null){
            SaveCardsOfCategory(currentCategory, currentCardSet);
        }
    }

    public void SaveCategories(){
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/categories.MCKerimData";
        FileStream stream = new FileStream(path, FileMode.Create);

        CategoriesData categoriesData = new CategoriesData(categories);

        formatter.Serialize(stream, categoriesData);
        stream.Close();
        Debug.Log("Categories Saved in: " + path);
    }

    private void SaveCardsOfCategory(Category category, List<Card> cards){
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

            Category sharebleCategory = new Category(Guid.NewGuid(), shareableCategoryData.category.Name, shareableCategoryData.category.Color, shareableCategoryData.category.Tags);
            SaveCategory(sharebleCategory);

            List<Card> sharebleCards = new List<Card>();
            foreach(Card c in shareableCategoryData.cards){
                Card sharebleCard = CardBuilder.CopyCardToShare(c, sharebleCategory.Uuid);
                sharebleCards.Add(sharebleCard);
            }
            SaveCardsOfCategory(sharebleCategory, sharebleCards);
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
