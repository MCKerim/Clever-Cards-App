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
    [SerializeField] private TMP_InputField notesInputField;

    private Card currentCard;

    private bool showsQuestion;

    private Vector3 startPos;
    private void OnEnable() {
        startPos = transform.localPosition;
    }

    public void ShowCardWithoutAnim(Card card)
    {
        currentCard = card;
        if(currentCard == null){
            currentCard = new Card("Press + to add cards to this category.\nPress X to delete a card.\nPress the gearwheel to edit a card.\nTap for more information.", "Swipe left if a card was easy.\nSwipe down if it was medium.\nSwipe to the right if it was hard.", null, null, 0, new System.Guid());
        }
        UpdatePointsUI();
        ShowQuestion();
    }

    public void ShowFirstCard(Card card)
    {
        currentCard = card;
        MoveCardBackToMiddel();
    }

    [SerializeField] private Image rateCardVisualPanel;
    [SerializeField] private CanvasGroup rateCardVisualcanvasGroup;

    private void PlayRateCardEffect(Color color)
    {
        rateCardVisualPanel.color = color;
        LeanTween.alphaCanvas(rateCardVisualcanvasGroup, 0.15f, 0.25f).setOnComplete(Back);
    }

    private void Back()
    {
        LeanTween.alphaCanvas(rateCardVisualcanvasGroup, 0f, 0.25f);
    }

    public void MoveCardLeft(Card card)
    {
        currentCard = card;
        LeanTween.moveLocalX(gameObject, -1000, 0.25f).setOnComplete(MoveCardBackToMiddel);
        PlayRateCardEffect(easyColor);
    }

    public void MoveCardRight(Card card)
    {
        currentCard = card;
        LeanTween.moveLocalX(gameObject, 1000, 0.25f).setOnComplete(MoveCardBackToMiddel);
        PlayRateCardEffect(hardColor);
    }

    public void MoveCardDown(Card card)
    {
        currentCard = card;
        LeanTween.moveLocalY(gameObject, -1500, 0.25f).setOnComplete(MoveCardBackToMiddel);
        PlayRateCardEffect(mediumColor);
    }
    [SerializeField] AnimationCurve cardAnimCurve;

    private void MoveCardBackToMiddel()
    {
        if(currentCard == null){
            currentCard = new Card("Press + to add cards to this category.\nPress X to delete a card.\nPress the gearwheel to edit a card.\nTap for more information.", "Swipe left if a card was easy.\nSwipe down if it was medium.\nSwipe to the right if it was hard.", null, null, 0, new System.Guid());
        }

        notesInputField.SetTextWithoutNotify("");
        UpdatePointsUI();
        ShowQuestion();

        gameObject.transform.localPosition = startPos;
        LeanTween.moveLocalX(gameObject, -1000, 0);
        LeanTween.moveLocalX(gameObject, 0, 0.25f).setEase(cardAnimCurve);
    }

    private void UpdatePointsUI()
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
}
