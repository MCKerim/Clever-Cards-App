using System;

public static class CardBuilder
{
    public static int startpointsForCard;

    public static Card InfoCard(string frontText, string backText)
    {
        return new Card(frontText, backText, null, null, 100, Guid.Empty, false, null);
    }    

    public static Card InfoCard(string frontText)
    {
        return InfoCard(frontText, "");
    }

    public static Card BasicInfoCard()
    {
        return InfoCard("Press + to add cards to this category.\nPress X to delete a card.\nPress the gearwheel to edit a card.\nTap for more information.", "Swipe left if a card was easy.\nSwipe down if it was medium.\nSwipe to the right if it was hard.");
    }

    public static Card NewCard(string question, string answear, byte[] imageBytesQuestion, byte[] imageBytesAnswear, Guid categoryID)
    {
        return new Card(question, answear, imageBytesQuestion, imageBytesAnswear, startpointsForCard, categoryID, false, null);
    }

    public static Card CopyCardToShare(Card c, Guid categoryUuid)
    {
        return new Card(c.Question, c.Answear, c.ImageBytesQuestion, c.ImageBytesAnswear, startpointsForCard, categoryUuid, c.IsFavorite, null);
    }
}