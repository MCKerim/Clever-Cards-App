using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorButtonsManager : MonoBehaviour
{
    [SerializeField] private SelectColorButton[] colorButtons;
    private int selectedColorButton;

    private CardManager cardManager;

    private void Start() {
        cardManager = GameObject.FindObjectOfType<CardManager>();

        for(int i=0; i < colorButtons.Length; i++){
            colorButtons[i].Initialise(this, i);
            colorButtons[i].Deselect();
        }
        selectedColorButton = 0;
        colorButtons[selectedColorButton].Select();
    }

    private void OnEnable() {
        if(selectedColorButton == 0){
            return;
        }
        colorButtons[selectedColorButton].Deselect();
        selectedColorButton = 0;
        colorButtons[selectedColorButton].Select();
    }

    public void SelectButton(Color c)
    {
        for(int i=0; i < colorButtons.Length; i++){
            if(i != selectedColorButton && colorButtons[i].GetColor().Equals(c)){
                colorButtons[selectedColorButton].Deselect();
                selectedColorButton = i;
                colorButtons[selectedColorButton].Select();
            }
        }
    }

    public void SelectButton(int index)
    {
        colorButtons[selectedColorButton].Deselect();
        selectedColorButton = index;
        colorButtons[selectedColorButton].Select();
    }

    public Color GetSelectedColor()
    {
        return colorButtons[selectedColorButton].GetColor();
    }
}
