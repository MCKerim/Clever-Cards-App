using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Category
{
    private Guid uuid;
    private string name;
    private float[] colorValues;
    private List<Tag> tags;

    public Category(Guid uuid, string name, Color color, List<Tag> tags)
    {
        this.uuid = uuid;
        this.name = name;
        this.colorValues = new float[4] {color.r, color.g, color.b, color.a};
        this.tags = new List<Tag>();
        if(tags != null)
        {
            this.tags.AddRange(tags);
        }
    }

    public Guid Uuid { get => uuid;}
    public string Name { get => name; set => name = value; }
    public Color Color { get => new Color(colorValues[0], colorValues[1], colorValues[2], colorValues[3]); set => colorValues = new float[4] {value.r, value.g, value.b, value.a}; }
    public List<Tag> Tags { get => tags; set => tags = value; }

    public void AddTag(Tag tag)
    {
        tags.Add(tag);
    }

    public void DeleteTag(Tag tag)
    {
        tags.Remove(tag);
    }

    public override bool Equals(object obj)
    {
        var category = obj as Category;
        return category != null &&
               uuid.Equals(category.uuid);
    }

    public override int GetHashCode()
    {
        return 1907758594 + EqualityComparer<Guid>.Default.GetHashCode(uuid);
    }
}
