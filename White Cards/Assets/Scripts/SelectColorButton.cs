using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectColorButton : MonoBehaviour
{
    [SerializeField] private Image colorImage;
    private int index;
    private bool isSelected;
    private ColorButtonsManager colorButtonsManager;

    public void Initialise(ColorButtonsManager colorButtonsManager, int index)
    {
        this.colorButtonsManager = colorButtonsManager;
        this.index = index;
    }

    public void ButtonPressed()
    {
        if(!isSelected){
            colorButtonsManager.SelectButton(index);
        }
    }

    public void Select()
    {
        isSelected = true;
        LeanTween.scale(gameObject, new Vector3(1.25f, 1.25f, 1), 0.25f);
    }

    public void Deselect()
    {
        isSelected = false;
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.25f);
    }

    public Color GetColor()
    {
        return colorImage.color;
    }
}
