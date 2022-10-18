using System.Collections.Generic;

[System.Serializable]
public class ShareableCategoryData
{
    public Category category;
    public List<Card> cards;

    public ShareableCategoryData(Category category, List<Card> cards){
        this.category = category;
        this.cards = cards;
    }
}
