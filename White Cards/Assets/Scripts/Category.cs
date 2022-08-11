using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Category
{
    private Guid uuid;
    private string name;

    public Category(Guid uuid, string name)
    {
        this.uuid = uuid;
        this.name = name;
    }

    public Guid Uuid { get => uuid;}
    public string Name { get => name; set => name = value; }

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
