using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject categorysUI;

    public void ShowGameUI()
    {
        gameUI.SetActive(true);
    }

    public void HideGameUI()
    {
        gameUI.SetActive(false);
    }

    public void ShowCategoriesUI()
    {
        categorysUI.SetActive(true);
    }

    public void HideCategoriesUI()
    {
        categorysUI.SetActive(false);
    }
}
