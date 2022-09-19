using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class EditCardManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField questionInputField;
    [SerializeField] private TMP_InputField answearInputField;
    [SerializeField] private TMP_Dropdown categoryDropdown;

    [SerializeField] private GameObject createCategoryPanel;
    [SerializeField] private TMP_InputField createCategoryInputField;

    private byte[] currentImageAsBytesQuestion;
    private byte[] currentImageAsBytesAnswear;

    private List<Category> categories;

    [SerializeField] private CardManager cardManager;
    private UIManager uIManager;

    private void Start() {
        uIManager = GameObject.FindObjectOfType<UIManager>();
        UpdateCategoryDropdown();
    }

    private void OnEnable()
    {
        UpdateCategoryDropdown();
        UpdateEditCardUI();
    }

    private void UpdateEditCardUI(){
        Card cardToEdit = cardManager.GetCurrentCard();

        questionInputField.text = cardToEdit.Question;
        answearInputField.text = cardToEdit.Answear;
        currentImageAsBytesQuestion = cardToEdit.ImageBytesQuestion;
        currentImageAsBytesAnswear = cardToEdit.ImageBytesAnswear;

        for(int i=0; i < categories.Count; i++){
            if(categories[i].Equals(cardManager.GetCurrentCategory())){
                categoryDropdown.value = i;
                return;
            }
        }
    }

    public void SaveEditedCard()
    {
        if (questionInputField.text == "" || answearInputField.text == "" || categories.Count == 0)
        {
            return;
        }

        Card cardToEdit = cardManager.GetCurrentCard();

        cardToEdit.Question = questionInputField.text;
        cardToEdit.Answear = answearInputField.text;
        
        cardToEdit.ImageBytesQuestion = currentImageAsBytesQuestion;
        cardToEdit.ImageBytesAnswear = currentImageAsBytesAnswear;

        int categoryDropdownValue = categoryDropdown.value;
        //Need to delete card from category and insert into another
        if(!cardManager.GetCurrentCategory().Equals(categories[categoryDropdownValue]))
        {
            cardToEdit.CategoryUuid = categories[categoryDropdownValue].Uuid;
            cardManager.SaveCard(cardToEdit, categories[categoryDropdownValue]);
            cardManager.DeleteCurrentCard();
        }
        else
        {
            cardManager.UpdateCurrentCardUI();
        }

        currentImageAsBytesQuestion = null;
        currentImageAsBytesAnswear = null;

        uIManager.ShowGameUI();
        uIManager.HideEditCardUI();
    }

    public void ShowCreateCategoryPanel()
    {
        createCategoryPanel.SetActive(true);
    }

    public void HideCreateCategoryPanel()
    {
        createCategoryPanel.SetActive(false);
    }

    public void CreateCategory()
    {
        string name = createCategoryInputField.text;
        if(name == ""){
            return;
        }

        Guid uuid = Guid.NewGuid();
        
        createCategoryInputField.text = "";

        Category category = new Category(uuid, name);
        cardManager.SaveCategory(category);

        UpdateCategoryDropdown();
        categoryDropdown.value = categoryDropdown.options.Count-1;
        if(categories.Count == 1)
        {
            categoryDropdown.captionText.SetText(categories[0].Name);
        }

        HideCreateCategoryPanel();
    }

    private void UpdateCategoryDropdown()
    {
        categories = cardManager.GetAllCategories();

        categoryDropdown.options.Clear();
        foreach (Category c in categories)
        {
            categoryDropdown.options.Add(new TMP_Dropdown.OptionData(){ text = c.Name});
        }

        //Can happen when last category was deleted
        if(categories.Count > 0 && categoryDropdown.value >= categories.Count-1)
        {
            categoryDropdown.value = categories.Count-1;
        }
        else if(categories.Count == 0)
        {
            categoryDropdown.captionText.SetText("Please create a category");
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
