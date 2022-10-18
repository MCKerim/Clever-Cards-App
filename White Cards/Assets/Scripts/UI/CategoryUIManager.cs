using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryUIManager : MonoBehaviour
{
    [SerializeField] private CategoryButtonManager categoryButtonManagerPrefab;
    [SerializeField] private Transform contentTransform;

    [SerializeField] private CardManager cardManager;
    [SerializeField] private float spaceAtStart;
    [SerializeField] private float spaceAtEnd;

    private void OnEnable()
    {
        UpdateCategoryUI();
        CardManager.OnCategorysChanged += UpdateCategoryUI;
    }

    private void OnDisable() {
        CardManager.OnCategorysChanged -= UpdateCategoryUI;
    }

    public void UpdateCategoryUI()
    {
        DestroyAllCategoryButtons();
        InstantiateAllCategoryButtons();
    }

    private void InstantiateAllCategoryButtons()
    {
        List<Category> categories = cardManager.GetAllCategories();

        float spaceBetween = 225;

        for(int i=0; i < categories.Count; i++)
        {
            CategoryButtonManager c = Instantiate(categoryButtonManagerPrefab, contentTransform);
            c.SetCategory(categories[i]);
            c.transform.LeanSetLocalPosY(-(spaceBetween * i + spaceAtStart));
        }

        RectTransform rt = contentTransform.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (categories.Count) * spaceBetween + spaceAtEnd);
    }

    private void DestroyAllCategoryButtons()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
