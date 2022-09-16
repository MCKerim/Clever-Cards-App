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

        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        AndroidNotificationCenter.CancelAllNotifications();
    }

    private void OnApplicationPause(bool pauseStatus) 
    {
        if(!pauseStatus)
        {
            AndroidNotificationCenter.CancelAllNotifications();
            return;
        }

        SedNotification();
    }

    private void OnApplicationQuit() 
    {
        AndroidNotificationCenter.CancelAllNotifications();

        SedNotification();
    }

    private void SedNotification()
    {
        var notification = new AndroidNotification();

        string notificationTitel = "";
        string notificationText = "";

        List<Category> categorys = cardManager.GetAllCategories();
        if(categorys.Count > 0)
        {
            int randomNumber = Random.Range(0, categorys.Count);
            Category randomCategory = categorys[randomNumber];
            notificationTitel = randomCategory.Name;

            List<Card> cards = cardManager.LoadCardsOfCategoryFromFile(randomCategory);
            if(cards.Count > 0)
            {
                randomNumber = Random.Range(0, cards.Count);
                Card randomCard = cards[randomNumber];
                notificationText = randomCard.Question;
            }
            else
            {
                notificationText = "Create your first card now!";
            }
        }
        else
        {
            notificationTitel = "Start now and learn!";
        }

        
        notification.Title = notificationTitel;
        notification.Text = notificationText;

        float randomMinutes = Random.Range(5f, 60f);
        float randomHours = Random.Range(0f, 24f);
        notification.FireTime = System.DateTime.Now.AddHours(randomHours).AddMinutes(randomMinutes);

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
}
