using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
    private CardManager cardManager;

    // Start is called before the first frame update
    void Start()
    {
        cardManager = GameObject.FindObjectOfType<CardManager>();

        RegisterQuestionNotificationChannel();
        RegisterRememberNotificationChannel();
        AndroidNotificationCenter.CancelAllNotifications();

        SendRememberNotification();
        SendQuestionNotifications();
    }

    private void RegisterRememberNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "Remember Channel",
            Name = "Remember Channel",
            Importance = Importance.High,
            Description = "Remember to learn",
            EnableVibration = true,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private void SendRememberNotification()
    {
        var notification = new AndroidNotification();
        notification.ShouldAutoCancel = true;
        notification.Style = NotificationStyle.BigTextStyle;
        notification.Title = "Hey Remember to learn!";
        
        SendNotification(notification, System.DateTime.Now.AddMinutes(30), "Remember Channel");
    }

    private void SendQuestionNotifications()
    {
        SendNotification(GetRandomQuestionNotification(), System.DateTime.Now.AddSeconds(5), "Question Channel");   
        SendNotification(GetRandomQuestionNotification(), System.DateTime.Now.AddSeconds(10), "Question Channel");   
        SendNotification(GetRandomQuestionNotification(), System.DateTime.Now.AddSeconds(15), "Question Channel");   
        SendNotification(GetRandomQuestionNotification(), System.DateTime.Now.AddSeconds(20), "Question Channel");   
    }

    private void RegisterQuestionNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "Question Channel",
            Name = "Question Channel",
            Importance = Importance.High,
            Description = "Asking Random Questions",
            EnableVibration = true,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private AndroidNotification GetRandomQuestionNotification()
    {
        var notification = new AndroidNotification();
        notification.ShouldAutoCancel = true;
        notification.Style = NotificationStyle.BigTextStyle;

        List<Category> categorys = cardManager.GetAllCategories();
        if(categorys.Count > 0)
        {
            Category randomCategory = categorys[Random.Range(0, categorys.Count)];
            notification.Title = randomCategory.Name;

            List<Card> cards = cardManager.LoadCardsOfCategoryFromFile(randomCategory);
            if(cards.Count > 0)
            {
                Card randomCard = cards[Random.Range(0, cards.Count)];
                notification.Text = randomCard.Question;
            }
        }
        return notification;
    }

    private void SendNotification(AndroidNotification notification, System.DateTime fireTime, string channelID)
    {
        notification.Text += " " + fireTime.Hour + ":" + fireTime.Minute + ":" + fireTime.Second + "Uhr";
        notification.FireTime = fireTime;
        notification.RepeatInterval =  new System.TimeSpan(0,1,0);
        AndroidNotificationCenter.SendNotification(notification, channelID);
    }
}
