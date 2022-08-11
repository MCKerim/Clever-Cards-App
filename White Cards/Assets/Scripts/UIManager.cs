using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject addCardUI;
    [SerializeField] private GameObject categorysUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowAddCardUI()
    {
        addCardUI.SetActive(true);
    }

    public void HideAddCardUI()
    {
        addCardUI.SetActive(false);
    }

    public void ShowGameUI()
    {
        gameUI.SetActive(true);
    }

    public void HideGameUI()
    {
        gameUI.SetActive(false);
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
    }

    public void HideMainMenu()
    {
        mainMenu.SetActive(false);
    }

    public void ShowCategorysUI()
    {
        categorysUI.SetActive(true);
    }

    public void HideCategorysUI()
    {
        categorysUI.SetActive(false);
    }
}
