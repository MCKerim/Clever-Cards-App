using System.Collections.Generic;

public interface IMode
{
    public string GetName();
    public void StartMode();

    public List<Card> PrepareCardSet(List<Card> cardSet);
    public Card GetCard(List<Card> cardSet, Card lastCard);

    public void EndMode();
}
