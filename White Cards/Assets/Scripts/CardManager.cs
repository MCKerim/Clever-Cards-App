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
    private Card lastCard;
    private Card currentCard;
    private Category currentCategory;

    private List<Card> currentCardSet = new List<Card>();
    private List<Card> filteredCardSet = new List<Card>();

    [SerializeField] private List<Category> categories;
    [SerializeField] private List<Mode> modeObjects;
    private List<IMode> modes;

    [SerializeField] private CardUIManager cardUIManager;

    [SerializeField] private UIManager uIManager;

    [SerializeField] private TextMeshProUGUI currentModeText;
    
    private int currentModeIndex = 0;
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

    private Category categorySelectedToDelete;
    [SerializeField] private GameObject confirmCategoryDeletionPopup;
    [SerializeField] private TextMeshProUGUI confirmCategoryDeletionText;
    
    [SerializeField] private GameObject createCategoryPopup;
    [SerializeField] private TMP_InputField createCategoryPopupInputField;
    [SerializeField] private ColorButtonsManager createCategoryColorSelection;
    private Category currentEditedCategory = null;

    public delegate void CategoryUpdateAction();
    public static event CategoryUpdateAction OnCategorysChanged;

    // Start is called before the first frame update
    void Awake()
    {
        categories = FileManager.LoadCategoriesFromFile();

        modes = new List<IMode>();
        foreach(Mode m in modeObjects){
            modes.Add((IMode) m);
        }

        currentModeIndex = 0;
        currentModeText.SetText(modes[currentModeIndex].GetName());
        modes[currentModeIndex].StartMode();
    }

    public void SelectCategory(Category category)
    {
        uIManager.HideCategoriesUI();
        uIManager.ShowGameUI();

        currentCategory = category;
        currentCategoryText.SetText(currentCategory.Name);
        currentCardSet = FileManager.LoadCardsOfCategoryFromFile(currentCategory);

        PrepareFilteredCardSet();
        lastCard = null;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
        cardUIManager.ShowFirstCard(currentCard);
    }

    public void ResetCurrentCardSetPoints()
    {
        foreach(Card c in currentCardSet)
        {
            c.CurrentPoints = CardBuilder.startpointsForCard;
        }
        PrepareFilteredCardSet();
        lastCard = null;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    public void ShowLastCard(){
        if(currentCard == null || lastCard == null)
        {
            return;
        }

        Card temp = currentCard;
        currentCard = lastCard;
        lastCard = temp;

        cardUIManager.MoveCardUp(currentCard);
    }

    public void RateCardEasy()
    {
        if(currentCard == null)
        {
            return;
        }

        if(modes[currentModeIndex].GetName().Equals("Smart Mode") || modes[currentModeIndex].GetName().Equals("Hard Mode")){
            RateCard(-10, currentCard);
            if(modes[currentModeIndex].GetName().Equals("Hard Mode") && currentCard.CurrentPoints < 70)
            {
                filteredCardSet.Remove(currentCard);
            }
        }

        lastCard = currentCard;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
        cardUIManager.MoveCardLeft(currentCard);
    }

    public void RateCardMedium()
    {
        if(currentCard == null)
        {
            return;
        }

        if(modes[currentModeIndex].GetName().Equals("Smart Mode") || modes[currentModeIndex].GetName().Equals("Hard Mode")){
            RateCard(0, currentCard);
        }

        lastCard = currentCard;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
        cardUIManager.MoveCardDown(currentCard);
    }

    public void RateCardHard()
    {
        if(currentCard == null)
        {
            return;
        }

        if(modes[currentModeIndex].GetName().Equals("Smart Mode") || modes[currentModeIndex].GetName().Equals("Hard Mode")){
            RateCard(10, currentCard);
        }
        
        lastCard = currentCard;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
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
        lastCard = null;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
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
        lastCard = null;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    public void NextMode()
    {
        modes[currentModeIndex].EndMode();

        currentModeIndex++;
        if(currentModeIndex >= modes.Count){
            currentModeIndex = 0;
        }

        modes[currentModeIndex].StartMode();
        currentModeText.SetText(modes[currentModeIndex].GetName());
        PrepareFilteredCardSet();
        lastCard = null;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
        cardUIManager.ShowCardWithoutAnim(currentCard);
    }

    public void PreviousMode()
    {
        modes[currentModeIndex].EndMode();

        currentModeIndex--;
        if(currentModeIndex < 0){
            currentModeIndex = modes.Count-1;
        }

        modes[currentModeIndex].StartMode();
        currentModeText.SetText(modes[currentModeIndex].GetName());
        PrepareFilteredCardSet();
        lastCard = null;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
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

        filteredCardSet = modes[currentModeIndex].PrepareCardSet(filteredCardSet);
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

    public Card GetNextCard()
    {
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

        return modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
    }

    public void AddCardToCurrentCategory(Card newCard)
    {
        currentCardSet.Add(newCard);
        PrepareFilteredCardSet();
        
        lastCard = currentCard;
        currentCard = newCard;
        UpdateCurrentCardUI();
    }

    public void SaveCard(Card card, Category category)
    {
        List<Card> cardsOfCategory =  FileManager.LoadCardsOfCategoryFromFile(category);
        cardsOfCategory.Add(card);
        FileManager.SaveCardsOfCategory(category, cardsOfCategory);
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
        lastCard = null;
        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
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

        if(currentCard.Equals(lastCard)){
            lastCard = null;
        }

        currentCard = modes[currentModeIndex].GetCard(filteredCardSet, currentCard);
        cardUIManager.MoveCardRight(currentCard);
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
        FileManager.SaveCategories(categories);
    }

    public void ShowCategorySettingsPopup()
    {
        categorySettingsPopup.SetActive(true);
    }

    public void HideCategorySettingsPopup()
    {
        categorySettingsPopup.SetActive(false);
    }

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
            FileManager.SaveCategories(categories);
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
        FileManager.SaveCategories(categories);
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
            FileManager.SaveCardsOfCategory(currentCategory, currentCardSet);
        }
    }

    ////////////////Sharable Category Code
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
            FileManager.SaveCardsOfCategory(sharebleCategory, sharebleCards);
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

