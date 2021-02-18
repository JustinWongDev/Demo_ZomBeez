using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class HumanSettings
{
    [Header("Stats")]
    [SerializeField]
    [Range (80, 100)]
    private float _health = 100.0f;
    public float Health { get { return _health; }}
    public void SetHealth(float val)
    { _health = val; }

    [SerializeField]
    [Range (0, 200)]
    private float _armour = 0.0f;
    public float Armour { get { return _armour; }}
    public void SetArmour(float val)
    { _armour = val; }

    [SerializeField] 
    [Range (1, 100)]
    private float _damage = 10.0f;
    public float Damage { get { return _damage; }}

    [SerializeField] 
    [Range (5, 100)]
    private int _brains = 20;
    public int Brains {get { return _brains;}}
    public void AddBrains(int val)
    { _brains += val; }

    [Header("Movement")]
    [SerializeField] 
    [Range (0.01f, 1.0f)]
    private float _minDistance = 0.01f;
    public float MinDistance { get { return _minDistance; }}
    
    [SerializeField] 
    [Range (1.0f, 10.0f)]
    private float _acceleration = 5.0f;
    public float Acceleration { get { return _acceleration; }}
    
    [SerializeField] 
    [Range (5.0f, 50.0f)]
    private float _deceleration = 25.0f;
    public float Deceleration { get { return _deceleration; }}
    
    [SerializeField] 
    [Range (5, 30)]
    private float _maxSpeed = 20.0f;
    public float MaxSpeed { get { return _maxSpeed; }}
    
    [SerializeField]
    private float _currentSpeed = 20.0f;
    public float GetCurrentSpeed() 
    {  return _currentSpeed; }
    public void AddCurrentSpeed(float value)
    { _currentSpeed += value; }
    
    [SerializeField] 
    [Range (1, 5)]
    private float _attackTime = 1.0f;
    public float AttackTime { get { return _attackTime; }}
    
    [SerializeField] 
    [Range (5, 15)]
    private float _abilityTime = 5.0f;
    public float AbilityTime { get { return _abilityTime; }}
    
    [SerializeField] 
    [Range (0.5f, 3.0f)]
    private float _collectTime = 3.0f;
    public float CollectTime { get { return _collectTime; }}
    
    [Header("Status")]
    [SerializeField]
    private bool _isDead = false;
    public bool GetIsDead() 
    {  return _isDead; }
    public void SetIsDead(bool value)
    { _isDead = value; }
    
    [SerializeField]
    private bool _hasJelly = false;
    public bool GetHasJelly() 
    {  return _hasJelly; }
    public void SetHasJelly(bool value)
    { _hasJelly = value; }

    public bool HealthPercentCheck(float percentage)
    {
        return (Health / 100) <= percentage;
    }
}
