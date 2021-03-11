using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Condition {
    WHEN_ONGOING_BUILDING
}

public class BlockOnCondition : MonoBehaviour
{
    public List<Condition> conditions;

    void Start()
    {
        foreach (var condition in conditions)
        {
            ApplyCondition(condition);
        }
    }

    private void ApplyCondition(Condition condition)
    {
        if (condition == Condition.WHEN_ONGOING_BUILDING)
        {
            EventBus.instance.onBuildingInitiated += (arg) => gameObject.SetActive(false);
            //EventBus.instance.onBulidingComplete
        }
    }
}
