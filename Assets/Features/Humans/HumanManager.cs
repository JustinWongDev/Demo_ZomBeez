using System;
using UnityEngine;

public class HumanManager : MonoBehaviour
{
    #region Static Singleton
    public static HumanManager Instance { get; private set; }
    private void Awake() => Instance = this;
    #endregion

    [Header("Prefabs")]
    [SerializeField]
    private HumanController humanPrefab = null;

    [Header("Scriptables")]
    [SerializeField]
    private HumanSO[] scriptables= null;

    public event Action<HumanController> OnNewHuman;

    public void PickVariant(int index)
    {
        HumanController prefab = Instantiate(humanPrefab);
        prefab.name = scriptables[index].name;
        prefab.Initialise(scriptables[index]);
        HiveController.live.AddActiveHuman(prefab);
        OnNewHuman?.Invoke(prefab);
    }
}
