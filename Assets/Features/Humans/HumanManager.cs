using System.Collections;
using System.Collections.Generic;
using Features.Humans;
using UnityEngine;

public class HumanManager : MonoBehaviour
{
    [Header("Prefabs")]
    public HumanController humanPrefab;

    [Header("Scriptables")]
    public HumanSO[] scriptables;

    public void PickHuman()
    {
        HumanController prefab = Instantiate(humanPrefab);
        prefab.Initialise(scriptables[0]);
        Hive.live.activeHumans.Add(prefab);
    }

    public void PickKeeper()
    {
        HumanController prefab = Instantiate(humanPrefab);
        prefab.Initialise(scriptables[1]);
        Hive.live.activeHumans.Add(prefab);
    }

    public void PickSadist()
    {
        HumanController prefab = Instantiate(humanPrefab);
        prefab.Initialise(scriptables[2]);
        Hive.live.activeHumans.Add(prefab);
    }
}
