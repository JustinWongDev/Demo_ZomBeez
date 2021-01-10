using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : MonoBehaviour
{
    [Header("Prefabs")]
    public HumanController humanPrefab;

    [Header("Scriptables")]
    public HumanSO[] scriptables;

    public void PickHuman()
    {
        HumanController civ = Instantiate(humanPrefab);
        Hive.live.activeHumans.Add(civ);
    }

    public void PickKeeper()
    {
        HumanController civ = Instantiate(humanPrefab);
        Hive.live.activeHumans.Add(civ);
    }

    public void PickSadist()
    {
        HumanController civ = Instantiate(humanPrefab);
        Hive.live.activeHumans.Add(civ);
    }
}
