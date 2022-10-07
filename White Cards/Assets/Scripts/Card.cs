using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class Card
{
    private Guid uuid;
    private string question;
    private string answear;
    private byte[] imageBytesQuestion;
    private byte[] imageBytesAnswear;
    private int currentPoints;
    private Guid categoryUuid;
    private bool isFavorite;
    private List<String> tags;

    public Card(string question, string answear, byte[] imageBytesQuestion, byte[] imageBytesAnswear, int currentPoints, Guid categoryID, bool isFavorite, List<String> tags)
    {
        this.uuid = Guid.NewGuid();
        this.question = question;
        this.answear = answear;

        if(imageBytesQuestion != null)
        {
            this.imageBytesQuestion = new byte[imageBytesQuestion.Length];
            imageBytesQuestion.CopyTo(this.imageBytesQuestion, 0);
        }
        
        if(imageBytesAnswear != null)
        {
            this.imageBytesAnswear = new byte[imageBytesAnswear.Length];
            imageBytesAnswear.CopyTo(this.imageBytesAnswear, 0);
        }

        this.currentPoints = currentPoints;
        this.categoryUuid = categoryID;
        this.isFavorite = isFavorite;

        
    }

    public List<String> Tags { get => tags; set => tags = value; }

    public void AddTag(string tag)
    {
        tags.Add(tag);
    }

    public bool HasTag(List<String> tagsToFilter)
    {
        bool hasTag = false;
        tagsToFilter.ForEach(tag => {
            if(tags.Contains(tag))
            {
                hasTag = true;
            }
        });
        return hasTag;
    }
    public Guid Uuid { get => uuid; }
    public string Question { get => question; set => question = value; }
    public string Answear { get => answear; set => answear = value; }
    public byte[] ImageBytesQuestion { get => imageBytesQuestion; set => imageBytesQuestion = value; }
    public byte[] ImageBytesAnswear { get => imageBytesAnswear; set => imageBytesAnswear = value; }
    public int CurrentPoints { get => currentPoints; set => currentPoints = value; }
    public Guid CategoryUuid { get => categoryUuid; set => categoryUuid = value; }
    public bool IsFavorite { get => isFavorite; set => isFavorite = value; }

    public override bool Equals(object obj)
    {
        var card = obj as Card;
        return card != null &&
               uuid.Equals(card.Uuid);
    }

    public override int GetHashCode()
    {
        return 1907758594 + EqualityComparer<Guid>.Default.GetHashCode(uuid);
    }
}
