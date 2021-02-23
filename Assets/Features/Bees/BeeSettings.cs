using System;
using UnityEngine;

public class BeeSettings : MonoBehaviour
{
    [Header("Stats")] ////////////////////////////
    [SerializeField]private int _capacity = 5;
    public static int Capacity => Instance._capacity;
    
    [SerializeField]private int _damage = 20;
    public static int Damage => Instance._damage;
    
    [SerializeField]private float _health = 100;
    public static float Health => Instance._health;

    [Header("Movement")] ////////////////////////////
    [SerializeField] private float _speed = 50.0f;
    public static float Speed => Instance._speed;

    [SerializeField] private float _rotSpeed = 50.0f;
    public static float RotSpeed => Instance._rotSpeed;

    [SerializeField] private float _targetRadius = 5.0f;
    public static float TargetRadius => Instance._targetRadius;

    [Header("Boiding")] ////////////////////////////
    [SerializeField] private float _sepDist = 25.0f;
    public static float SeparationDistance => Instance._sepDist;

    [SerializeField] private float _cohesDist = 50.0f;
    public static float CohesionDistance => Instance._cohesDist;

    [SerializeField] private float _sepStr = 250.0f;
    public static float SeparationStrength => Instance._sepStr;

    [SerializeField] private float _cohesStr = 25.0f;
    public static float CohesionStrength => Instance._cohesStr;

    [Header("Foraging")] ////////////////////////////
    [SerializeField]private int _collTime = 6;
    public static int CollectionTime => Instance._collTime;

    [SerializeField] private float _forageRad = 20.0f;
    public static float ForageRadius => Instance._forageRad;

    [Header("Detection")] //////////////////////////
    [SerializeField]
    private float _detectionTime = 0.25f;
    public static float DetectTime => Instance._detectionTime;
    
    [SerializeField]private float _detectionRad = 20.0f;
    public static float DetectionRad => Instance._detectionRad;
    
    [SerializeField]private float _scoutRadiusX = 40.0f;
    public static float ScoutRadiusX => Instance._scoutRadiusX;
    
    [SerializeField]private float _scoutRadiusY = 20.0f;
    public static float ScoutRadiusY => Instance._scoutRadiusY;
    
    [SerializeField]private float _scoutRadiusZ = 70.0f;
    public static float ScoutRadiusZ => Instance._scoutRadiusZ;
    
    [SerializeField]private float _scoutTime = 5.0f;
    public static float ScoutTime => Instance._scoutTime;

    [SerializeField]private Vector3 _idlePosition = FindObjectOfType<HiveController>().transform.position;
    public static Vector3 IdlePosition => Instance._idlePosition;
    
    public static BeeSettings Instance { get; private set; }
    private void Awake() => Instance = this;
}
