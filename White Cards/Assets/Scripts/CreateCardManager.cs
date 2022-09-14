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

    [SerializeField] private GameObject createCategoryPanel;
    [SerializeField] private TMP_InputField createCategoryInputField;
    private byte[] currentImageAsBytesQuestion;
    private byte[] currentImageAsBytesAnswear;

    private List<Category> categories;

    private void OnEnable()
    {
        UpdateCategoryDropdown();
    }

    private void Start() {
        UpdateCategoryDropdown();
    }

    public void CreateCard()
    {
        if (questionInputField.text == "" || answearInputField.text == "" || categories.Count == 0)
        {
            return;
        }

        string question = questionInputField.text;
        questionInputField.text = "";
        string answear = answearInputField.text;
        answearInputField.text = "";
        int categoryDropdownValue = categoryDropdown.value;
        Guid categoryUuid = categories[categoryDropdownValue].Uuid;

        Card card = new Card(question, answear, currentImageAsBytesQuestion, currentImageAsBytesAnswear, cardManager.startpointsForCard, categoryUuid);
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
