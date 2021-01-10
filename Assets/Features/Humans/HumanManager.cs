using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : MonoBehaviour
{
    [Header("Prefabs")]
    public HumanController civilian;

    public void PickHuman()
    {
        HumanController civ = Instantiate(civilian);
        Hive.live.activeHumans.Add(civ);
    }
}
