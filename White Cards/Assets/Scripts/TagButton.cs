using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TagButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject selectedImage;
    private TagButtonsManager tagButtonsManager;

    private Tag tag;
    private bool isSelected;

    public Tag Tag { get => tag; }

    public void Initialise(TagButtonsManager tagButtonsManager, Tag tag)
    {
        this.tagButtonsManager = tagButtonsManager;
        this.tag = tag;
        nameText.SetText(tag.Name);
        Deselect();
    }

    public void Press()
    {
        if(isSelected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void Select()
    {
        isSelected = true;
        selectedImage.SetActive(true);
    }

    public void Deselect()
    {
        isSelected = false;
        selectedImage.SetActive(false);
    }

    public void Edit()
    {

    }

    public void Delete()
    {

    }
}
