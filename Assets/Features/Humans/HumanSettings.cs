using System;
using UnityEngine;

[Serializable]
public class HumanSettings
{
    [SerializeField]
    [Range (80, 200)]
    private float _maxHealth = 100.0f;
    public float MaxHealth { get { return _maxHealth; }}

    [SerializeField]
    [Range (0, 200)]
    private float _maxArmour = 0.0f;
    public float MaxArmour { get { return _maxArmour; }}

    [SerializeField] 
    [Range (1, 100)]
    private float _maxDamage = 10.0f;
    public float MaxDamage { get { return _maxDamage; }}

    [SerializeField] 
    [Range (5, 100)]
    private int _maxResource = 20;
    public int MaxResource {get { return _maxResource;}}
    
}
