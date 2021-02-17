using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private HumanController humanPrefab = null;

    [Header("Scriptables")]
    [SerializeField]
    private HumanSO[] scriptables= null;

    public void PickVariant(int index)
    {
        HumanController prefab = Instantiate(humanPrefab);
        prefab.name = scriptables[index].name;
        prefab.Initialise(scriptables[index]);
        Hive.live.AddActiveHuman(prefab);
    }
}
