using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskAssets : MonoBehaviour
{
    public static TaskAssets Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public Sprite BugSprite;
    public Sprite ImprovementSprite;
    public Sprite FeatSprite;

    public Color BugColor;
    public Color ImprovementColor;
    public Color FeatColor;
}
