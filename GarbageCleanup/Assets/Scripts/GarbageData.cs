using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Garbage { Waste, Recyclable, Textile, Electronic }

[Serializable]
public struct GarbageData
{
    public Garbage garbageType;

    [Header("UI")]
    public Sprite icon;
}