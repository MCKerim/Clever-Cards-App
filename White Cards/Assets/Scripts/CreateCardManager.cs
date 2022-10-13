using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class CreateCardManager : MonoBehaviour
{
    [SerializeField] private CardManager cardManager;

    [SerializeField] private TMP_InputField questionInputField;
    [SerializeField] private TMP_InputField answearInputField;
    [SerializeField] private TMP_Dropdown categoryDropdown;
    [SerializeField] private TextMeshProUGUI categoryLabelText;
    [SerializeField] private TMP_Dropdown tagsDropdown;
    [SerializeField] private TagButtonsManager tagButtonsManager;
    [SerializeField] private GameObject createTagPopup;
    [SerializeField] private GameObject tagPanel;
    [SerializeField] private TMP_InputField createTagInputField;

    private byte[] currentImageAsBytesQuestion;
    private byte[] currentImageAsBytesAnswear;

    private List<Category> categories;
    private UIManager uIManager;
    private Card currentEditedCard = null;
    [SerializeField] private GameObject createCardPanel;

    [SerializeField] private GameObject cardSavedPopup;

    private void Start() {
        uIManager = GameObject.FindObjectOfType<UIManager>();
    }

    public void StartCreatingNewCard()
    {
        createCardPanel.SetActive(true);
        UpdateCategoryDropdown();
        SelectCurrentCategory();
        UpdateCreateCardUI();
        PopulateTagsDropdown();
        
        CardManager.OnCategorysChanged += UpdateCategoryDropdownAndSelectLast;
    }

    public void StartEditingCurrentCard()
    {
        createCardPanel.SetActive(true);
        currentEditedCard = cardManager.GetCurrentCard();
        UpdateCategoryDropdown();
        SelectCurrentCategory();
        UpdateEditCardUI();
        PopulateTagsDropdown();

        CardManager.OnCategorysChanged += UpdateCategoryDropdownAndSelectLast;
    }

    public void CancelButtonClicked()
    {
        currentEditedCard = null;
        uIManager.ShowGameUI();
        createCardPanel.SetActive(false);

        CardManager.OnCategorysChanged -= UpdateCategoryDropdownAndSelectLast;
    }

    private void SelectCurrentCategory()
    {
        for(int i=0; i < categories.Count; i++){
            if(categories[i].Equals(cardManager.GetCurrentCategory())){
                categoryDropdown.value = i;
                return;
            }
        }
    }

    private void UpdateEditCardUI()
    {
        questionInputField.text = currentEditedCard.Question;
        answearInputField.text = currentEditedCard.Answear;
        currentImageAsBytesQuestion = currentEditedCard.ImageBytesQuestion;
        currentImageAsBytesAnswear = currentEditedCard.ImageBytesAnswear;
    }

    private void UpdateCreateCardUI()
    {
        questionInputField.text = "";
        answearInputField.text = "";
        currentImageAsBytesQuestion = null;
        currentImageAsBytesAnswear = null;
    }

    private bool IsValidInput()
    {
        return !(questionInputField.text == "" && answearInputField.text == "" && currentImageAsBytesQuestion == null && currentImageAsBytesAnswear == null) && categories.Count != 0;
    }

    public void SaveCard()
    {
        if (!IsValidInput())
        {
            return;
        }

        string question = questionInputField.text;
        questionInputField.text = "";
        string answear = answearInputField.text;
        answearInputField.text = "";
        int categoryDropdownValue = categoryDropdown.value;
        Category selectedCategory = categories[categoryDropdownValue];

        if(currentEditedCard == null)
        {
            Card newCard = CardBuilder.NewCard(question, answear, currentImageAsBytesQuestion, currentImageAsBytesAnswear, selectedCategory.Uuid, tagButtonsManager.GetSelectedTags());
            
            currentImageAsBytesQuestion = null;
            currentImageAsBytesAnswear = null;
            tagButtonsManager.SetSelectedTags(new List<Tag>());

            if(cardManager.GetCurrentCategory().Equals(selectedCategory))
            {
                cardManager.AddCardToCurrentCategory(newCard);
            }
            else
            {
                cardManager.SaveCard(newCard, selectedCategory);
            }

            PlayCardSavedAnim();
        }
        else
        {
            currentEditedCard.Question = question;
            currentEditedCard.Answear = answear;
            currentEditedCard.ImageBytesQuestion = currentImageAsBytesQuestion;
            currentEditedCard.ImageBytesAnswear = currentImageAsBytesAnswear;
            currentEditedCard.CategoryUuid = selectedCategory.Uuid;

            currentImageAsBytesQuestion = null;
            currentImageAsBytesAnswear = null;
            tagButtonsManager.SetSelectedTags(new List<Tag>());

            currentEditedCard.Tags = tagButtonsManager.GetSelectedTags();

            //Need to delete card from category and insert into another
            if(cardManager.GetCurrentCategory().Equals(selectedCategory))
            {
                cardManager.UpdateCurrentCardUI();
                cardManager.CurrentCardsTagsWhereUpdated();
            }
            else
            {
                cardManager.SaveCard(currentEditedCard, categories[categoryDropdownValue]);
                cardManager.DeleteCurrentCard();
            }

            CancelButtonClicked();
        }
    }

    private void PlayCardSavedAnim()
    {
        cardSavedPopup.SetActive(true);
        LeanTween.scale(cardSavedPopup, new Vector3(0, 0, 0), 0);
        LeanTween.scale(cardSavedPopup, new Vector3(1, 1, 1), 0.4f).setOnComplete(CardSavedAnimStep2).setEaseOutBack();
    }

    private void CardSavedAnimStep2()
    {
        LeanTween.scale(cardSavedPopup, new Vector3(0, 0, 0), 0.25f).setDelay(0.15f).setOnComplete(() => cardSavedPopup.SetActive(false)).setEaseInCirc();
    }

    public void UpdateCategoryDropdownAndSelectLast()
    {
        UpdateCategoryDropdown();
        SelectLastCategory();
    }

    private void UpdateCategoryDropdown()
    {
        categories = cardManager.GetAllCategories();

        categoryDropdown.options.Clear();
        foreach (Category c in categories)
        {
            categoryDropdown.options.Add(new TMP_Dropdown.OptionData(){ text = c.Name});
        }
    }

    private void SelectLastCategory()
    {
        categoryDropdown.value = categoryDropdown.options.Count-1;
        if(categories.Count == 1)
        {
            categoryDropdown.captionText.SetText(categories[0].Name);
        }
    }

    public void TagPanelButtonPressed()
    {
        ShowTagPanel();
    }   

    private void ShowTagPanel()
    {
        tagPanel.SetActive(true);
    }

    public void HideTagPanel()
    {
        tagPanel.SetActive(false);
    }

    public void NewTagButtonPressed()
    {
        ShowCreateTagPopup();
    }

    private void ShowCreateTagPopup()
    {
        createTagPopup.SetActive(true);
    }

    public void HideCreateTagPopup()
    {
        createTagPopup.SetActive(false);
    }

    public void CreateTagPopupConfirmButtonPressed()
    {
        string newTagName = createTagInputField.text;
        if(newTagName == "")
        {
            return;
        }

        Tag newTag = new Tag(newTagName);

        Category selectedCategory = categories[categoryDropdown.value];
        selectedCategory.AddTag(newTag);
        cardManager.SaveCategories();

        tagButtonsManager.AddTag(newTag);

        createTagInputField.text = "";
        HideCreateTagPopup();
    }

    private void PopulateTagsDropdown()
    {
        Category selectedCategory = categories[categoryDropdown.value];
        if(currentEditedCard == null){
            tagButtonsManager.UpdateTagList(selectedCategory.Tags);
            tagButtonsManager.SetSelectedTags(new List<Tag>());
            
        }else{
            tagButtonsManager.UpdateTagList(selectedCategory.Tags);
            tagButtonsManager.SetSelectedTags(currentEditedCard.Tags);
        }
    }

    public void SelectImageQuestion()
    {
        if (NativeFilePicker.IsFilePickerBusy())
        {
            return;
        }

        string[] fileTypes = { "image/*" };
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
           if (path == null)
           {
                Debug.Log("Operation cancelled");
            }
            else
            {
                currentImageAsBytesQuestion = File.ReadAllBytes(path);
            }
        }, fileTypes);

        Debug.Log("Permission result: " + permission);
    }

    public void SelectImageAnswear()
    {
        if (NativeFilePicker.IsFilePickerBusy())
        {
            return;
        }

        string[] fileTypes = { "image/*" };
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
            {
                Debug.Log("Operation cancelled");
            }
            else
            {
                currentImageAsBytesAnswear = File.ReadAllBytes(path);
            }
        }, fileTypes);

        Debug.Log("Permission result: " + permission);
    }

    public void DeleteImageQuestion()
    {
        currentImageAsBytesQuestion = null;
    }

    public void DeleteImageAnswear()
    {
        currentImageAsBytesAnswear = null;
    }

    public void ReplaceRichTextStyleInQuestion(string inputText)
    {
        questionInputField.text = FindAndReplace(inputText);
    }

    public void ReplaceRichTextStyleInAnswear(string inputText)
    {
        answearInputField.text = FindAndReplace(inputText);
    }

    private string FindAndReplace(string text)
    {
        string output;
        string patternTextStart = @"#(\d|\w)+";

        Regex regStart = new Regex(patternTextStart);
        output = regStart.Replace(text, "<style=\"$1\">");

        string patternTextEnd = @"#";

        Regex regEnd = new Regex(patternTextEnd);
        output = regEnd.Replace(output, "</style>");

        return output;
    }
}
