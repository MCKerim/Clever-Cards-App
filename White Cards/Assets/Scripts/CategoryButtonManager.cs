using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CategoryButtonManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    private Category category;

    private CardManager cardManager;
    private EditCategoryManager editCategoryManager;

    private void Start()
    {
        cardManager = GameObject.FindObjectOfType<CardManager>();
        editCategoryManager = GameObject.FindObjectOfType<EditCategoryManager>();
    }

    public void SetCategory(Category category)
    {
        this.category = category;
        nameText.SetText(category.Name);
    }

    public void Select()
    {
        cardManager.SelectCategory(category);
    }

    public void DeleteThisCategory()
    {
        cardManager.DeleteCategory(category);
        GameObject.FindObjectOfType<CategoryUIManager>().UpdateCategoryUI();
    }

    public void EditThisCategory()
    {
        editCategoryManager.StartEditingCategory(category);
    }
}
