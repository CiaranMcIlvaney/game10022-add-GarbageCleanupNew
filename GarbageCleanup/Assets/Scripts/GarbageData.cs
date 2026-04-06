using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Garbage { Waste, Recyclable, Textile, Electronic }

[Serializable]
public struct GarbageData
{
    [Header("Garbage Type")]
    public Garbage garbageType;

    [Header("UI")]
    public Sprite icon;

    [Header("Info")]
    public string displayName;
    [TextArea(2, 5)]
    public string wrongBinExplanation;
}