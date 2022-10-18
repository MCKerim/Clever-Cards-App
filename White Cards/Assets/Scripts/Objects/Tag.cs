using System;
using System.Collections.Generic;

[System.Serializable]
public class Tag
{
    private Guid uuid;
    private string name;

    public Tag(String name){
        this.uuid = Guid.NewGuid();
        this.name = name;
    }

    public Guid Uuid { get => uuid; }
    public string Name { get => name; }

    public override bool Equals(object obj)
    {
        var tag = obj as Tag;
        return tag != null &&
               uuid.Equals(tag.Uuid);
    }

    public override int GetHashCode()
    {
        return 1907758594 + EqualityComparer<Guid>.Default.GetHashCode(uuid);
    }
}