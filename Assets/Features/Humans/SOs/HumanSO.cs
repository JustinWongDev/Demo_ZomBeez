using UnityEngine;

public enum HumanType { civilian, keeper, sadist };


[CreateAssetMenu(fileName = "HumanSO", menuName = "ScriptableObjects/HumanSO")]
public class HumanSO : ScriptableObject
{
    public HumanType currentType;
    public GameObject model;
    public float max_Health = 100.0f;
    public int max_Resource = 20;
    public float max_Speed = 20;
}
