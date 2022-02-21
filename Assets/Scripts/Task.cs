using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
    public enum Type
    {
        Bug,
        Feat,
        Improvement
    }

    public Type TaskType;

    public int DeadlineTime;
    public int DurationTime;

    public Sprite GetSprite() {
        switch(TaskType) {
            default:
            case Type.Bug: return TaskAssets.Instance.BugSprite;
            case Type.Feat: return TaskAssets.Instance.FeatSprite;
            case Type.Improvement: return TaskAssets.Instance.ImprovementSprite;
        }
    }

    public Color GetColor()
    {
        switch (TaskType)
        {
            default:
            case Type.Bug: return TaskAssets.Instance.BugColor;
            case Type.Feat: return TaskAssets.Instance.FeatColor;
            case Type.Improvement: return TaskAssets.Instance.ImprovementColor;
        }
    }
}
