using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private TextMeshProUGUI currentPointsText;

    [SerializeField] private Image hardnesBar;
    [SerializeField] private Color easyColor;
    [SerializeField] private Color mediumColor;
    [SerializeField] private Color hardColor;

    [SerializeField] private RawImage questionRawImage;

    private Card currentCard;

    private bool showsQuestion;

    public void ShowCard(Card card)
    {
        if(card == null)
        {
            currentCard = new Card(new System.Guid(), "No Cards in this Category.", "This is a test card. Please create your own cards in the Create Card menu.", null, null, 0, new System.Guid());
        }
        else
        {
            currentCard = card;
        }
        LeanTween.moveLocalX(gameObject, 1000, 0.25f).setOnComplete(MoveBack);
    }

    private void MoveBack()
    {
        currentPointsText.SetText("Points: " + currentCard.CurrentPoints);
        if (currentCard.CurrentPoints >= 70)
        {
            hardnesBar.color = hardColor;
        }
        else if (currentCard.CurrentPoints >= 30)
        {
            hardnesBar.color = mediumColor;
        }
        else
        {
            hardnesBar.color = easyColor;
        }

        ShowQuestion();
        LeanTween.moveLocalX(gameObject, -1000, 0);
        LeanTween.moveLocalX(gameObject, 0, 0.25f);
    }

    public void Turn()
    {
        LeanTween.rotateLocal(gameObject, new Vector3(0, 90, 0), 0.25f).setOnComplete(RotateBack);
    }

    private void RotateBack()
    {
        if (showsQuestion)
        {
            ShowAnswear();
        }
        else
        {
            ShowQuestion();
        }
        LeanTween.rotateLocal(gameObject, new Vector3(0, 0, 0), 0.5f);
    }

    private void ShowQuestion()
    {
        showsQuestion = true;
        textBox.SetText(currentCard.Question);

        if(currentCard.ImageBytesQuestion != null)
        {
            textBox.alignment = TextAlignmentOptions.TopLeft;
            questionRawImage.gameObject.SetActive(true);

            Texture2D tex = new Texture2D(1600, 900);
            tex.LoadImage(currentCard.ImageBytesQuestion);
            questionRawImage.texture = tex;
        }
        else
        {
            textBox.alignment = TextAlignmentOptions.MidlineLeft;
            questionRawImage.gameObject.SetActive(false);
        }
    }

    private void ShowAnswear()
    {
        showsQuestion = false;
        textBox.SetText(currentCard.Answear);

        if (currentCard.ImageBytesAnswear != null)
        {
            textBox.alignment = TextAlignmentOptions.TopLeft;
            questionRawImage.gameObject.SetActive(true);

            Texture2D tex = new Texture2D(1600, 900);
            tex.LoadImage(currentCard.ImageBytesAnswear);
            questionRawImage.texture = tex;
        }
        else
        {
            textBox.alignment = TextAlignmentOptions.MidlineLeft;
            questionRawImage.gameObject.SetActive(false);
        }
    }
}
