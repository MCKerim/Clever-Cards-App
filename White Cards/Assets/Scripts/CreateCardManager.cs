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
    [SerializeField] private int startpointsForCard;

    [SerializeField] private CardManager cardManager;

    [SerializeField] private TMP_InputField questionInputField;
    [SerializeField] private TMP_InputField answearInputField;
    [SerializeField] private TMP_Dropdown categoryDropdown;

    [SerializeField] private GameObject createCategoryPanel;
    [SerializeField] private TMP_InputField createCategoryInputField;
    [SerializeField] private Transform dropdownContentTransform;

    private byte[] currentImageAsBytesQuestion;
    private byte[] currentImageAsBytesAnswear;

    private List<Category> categories;

    private void OnEnable()
    {
        UpdateCategoryDropdown();
    }

    public void CreateCard()
    {
        if (questionInputField.text == "" || answearInputField.text == "" || categories.Count == 0)
        {
            return;
        }

        Guid uuid = Guid.NewGuid();
        string question = questionInputField.text;
        questionInputField.text = "";
        string answear = answearInputField.text;
        answearInputField.text = "";
        int categoryDropdownValue = categoryDropdown.value;
        Guid categoryUuid = categories[categoryDropdownValue].Uuid;

        Card card = new Card(uuid, question, answear, currentImageAsBytesQuestion, currentImageAsBytesAnswear, startpointsForCard, categoryUuid);
        cardManager.SaveCard(card, categories[categoryDropdownValue]);

        currentImageAsBytesQuestion = null;
        currentImageAsBytesAnswear = null;
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

        HideCreateCategoryPanel();
    }

    private void UpdateCategoryDropdown()
    {
        categories = cardManager.GetAllCategories();

        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();
        foreach (Category c in categories)
        {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = c.Name;
            dropdownOptions.Add(newOption);
        }

        categoryDropdown.options = dropdownOptions;

        float spaceBetween = 100;
        RectTransform rt = dropdownContentTransform.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (categories.Count-1) * spaceBetween);

        //Can happen when last category was deleted
        if(categoryDropdown.value > categoryDropdown.options.Count-1){
            categoryDropdown.value = categoryDropdown.options.Count-1;
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
