using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagButtonsManager : MonoBehaviour
{
    private List<TagButton> tagButtons;
    [SerializeField] private TagButton tagButtonPrefab;
    [SerializeField] private Transform contentTransform;

    private void Start() {
        tagButtons = new List<TagButton>();
    }

    public void UpdateTagList(List<Tag> allTags)
    {
        Debug.Log("All Tags num: " + allTags.Count);
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        tagButtons.Clear();
        float spaceBetween = 200;

        for(int i=0; i < allTags.Count; i++)
        {
            TagButton t = Instantiate(tagButtonPrefab, contentTransform);
            t.Initialise(this, allTags[i]);
            t.transform.LeanSetLocalPosY(-(spaceBetween * i));
            tagButtons.Add(t);
        }

        RectTransform rt = contentTransform.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (allTags.Count) * spaceBetween);
    }

    public void SetSelectedTags(List<Tag> selectedTags){
        Debug.Log("num: " + selectedTags.Count);
        if(selectedTags.Count == 0){
            return;
        }

        foreach(TagButton tagButton in tagButtons){
            if(selectedTags.Contains(tagButton.Tag)){
                Debug.Log("Select " + tagButton.Tag.Name);
                Debug.Log("UUID: " + tagButton.Tag.Uuid);
                tagButton.Select();
            }
            else
            {
                tagButton.Deselect();
            }
        }
    }

    public List<Tag> GetSelectedTags()
    {
        List<Tag> selectedTags = new List<Tag>();
        foreach(TagButton t in tagButtons){
            if(t.IsSelected()){
                selectedTags.Add(t.Tag);
            }
        }
        return selectedTags;
    }
}
