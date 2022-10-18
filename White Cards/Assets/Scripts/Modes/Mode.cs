using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mode : MonoBehaviour
{
    [SerializeField] private string modeName;
    [SerializeField] private string description;

    public string GetName(){
        return modeName;
    }

    public string GetDescription(){
        return description;
    }
}
