using UnityEngine;

public enum HumanType { civilian, keeper, sadist };


[CreateAssetMenu(fileName = "HumanSO", menuName = "ScriptableObjects/HumanSO")]
public class HumanSO : ScriptableObject
{
    [Header("General")]
    public GameObject model;
    
    [Header("Stats")]
    public float max_Health = 100.0f;
    public float max_Armour = 0.0f;
    public int max_Resource = 20;
    public float max_Speed = 20;
    public float max_Damage = 2;

    [Header("AI")]
    public HumanType currentType;
    public Dfa dfa;
}

[System.Serializable]
public class Dfa
{
    [System.Serializable]
    public struct RowData
    {
        public int[] row;
    }

    public RowData[] rows = new RowData[5];
}