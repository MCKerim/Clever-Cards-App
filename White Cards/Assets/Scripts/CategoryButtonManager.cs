using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CategoryButtonManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    private Category category;

    private CardManager cardManager;

    private void Start()
    {
        cardManager = GameObject.FindObjectOfType<CardManager>();
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
        cardManager.ShowConfirmDeleteCategoryPopup(category);
    }

    public void EditThisCategory()
    {
        cardManager.EditCategoryNameButtonClicked(category);
    }

    public void ShareThisCategory()
    {
        cardManager.ShareCategory(category);
    }
}
