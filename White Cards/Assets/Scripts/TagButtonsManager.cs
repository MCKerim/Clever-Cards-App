using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagButtonsManager : MonoBehaviour
{
    private List<TagButton> tagButtons;
    [SerializeField] private TagButton tagButtonPrefab;
    [SerializeField] private Transform contentTransform;
    private CardManager cardManager;
    private float spaceBetween = 200;

    private void Start() {
        tagButtons = new List<TagButton>();
        cardManager = GameObject.FindObjectOfType<CardManager>();
    }

    public void DeleteTag(Tag tagToDelete)
    {
        cardManager.DeleteTagFromCurrentCategory(tagToDelete);

        TagButton tagButtonToDelete = null;
        foreach(TagButton tagButton in tagButtons){
            if(tagButton.Tag.Equals(tagToDelete)){
                tagButtonToDelete = tagButton;
                break;
            }
        }

        tagButtons.Remove(tagButtonToDelete);
        Destroy(tagButtonToDelete.gameObject);
        
        for(int i=0; i < tagButtons.Count; i++){
            tagButtons[i].transform.LeanSetLocalPosY(-(spaceBetween * i));
        }
    }

    public void UpdateTagList(List<Tag> allTags)
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        tagButtons.Clear();

        for(int i=0; i < allTags.Count; i++)
        {
            CreateTagButton(allTags[i], i);
        }

        RectTransform rt = contentTransform.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (tagButtons.Count) * spaceBetween);
    }

    public void AddTag(Tag tag)
    {
        CreateTagButton(tag, tagButtons.Count).Select();

        RectTransform rt = contentTransform.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (tagButtons.Count) * spaceBetween);
    }

    private TagButton CreateTagButton(Tag tag, int i)
    {
        TagButton newTagButton = Instantiate(tagButtonPrefab, contentTransform);
        newTagButton.Initialise(this, tag);
        newTagButton.transform.LeanSetLocalPosY(-(spaceBetween * i));
        tagButtons.Add(newTagButton);
        return newTagButton;
    }

    private void SelectTag(Tag tag)
    {
        foreach(TagButton tagButton in tagButtons){
            if(tag.Equals(tagButton.tag)){
                tagButton.Select();
                return;
            }
        }
    }

    public void SetSelectedTags(List<Tag> selectedTags){
        if(selectedTags.Count == 0){
            return;
        }

        foreach(TagButton tagButton in tagButtons){
            if(selectedTags.Contains(tagButton.Tag)){
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
