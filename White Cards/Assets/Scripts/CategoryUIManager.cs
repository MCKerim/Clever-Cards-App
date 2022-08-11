using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryUIManager : MonoBehaviour
{
    [SerializeField] private CategoryButtonManager categoryButtonManagerPrefab;
    [SerializeField] private Transform contentTransform;

    [SerializeField] private CardManager cardManager;

    private void OnEnable()
    {
        UpdateCategoryUI();
    }

    private void InstantiateAllCategoryButtons()
    {
        List<Category> categories = cardManager.GetAllCategories();

        float spaceBetween = 225;

        for(int i=0; i < categories.Count; i++)
        {
            CategoryButtonManager c = Instantiate(categoryButtonManagerPrefab, contentTransform);
            c.SetCategory(categories[i]);
            c.transform.LeanSetLocalPosY(-(spaceBetween * i));
        }

        RectTransform rt = contentTransform.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (categories.Count) * spaceBetween);
    }

    public void UpdateCategoryUI()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        InstantiateAllCategoryButtons();
    }
}
