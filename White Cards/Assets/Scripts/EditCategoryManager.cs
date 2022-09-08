using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditCategoryManager : MonoBehaviour
{
    [SerializeField] private GameObject editCategoryPanel;
    [SerializeField] private TMP_InputField categoryNameInputField;
    [SerializeField] private CardManager cardManager;
    private Category currentEditedCategory;
    [SerializeField] private CategoryUIManager categoryUIManager;

    public void StartEditingCategory(Category category){
        currentEditedCategory = category;
        editCategoryPanel.SetActive(true);
        UpdateEditCategoryUI();
    }

    public void HideCategoryPanel(){
        editCategoryPanel.SetActive(false);
        currentEditedCategory = null;
    }

    private void UpdateEditCategoryUI(){
        categoryNameInputField.text = currentEditedCategory.Name;
    }

    public void SaveChanges(){
        if(categoryNameInputField.text == ""){
            return;
        }
        currentEditedCategory.Name = categoryNameInputField.text;
        cardManager.SaveCategories();
        categoryUIManager.UpdateCategoryUI();
        HideCategoryPanel();
    }

    private void OnDisable() {
        HideCategoryPanel();
    }
}
