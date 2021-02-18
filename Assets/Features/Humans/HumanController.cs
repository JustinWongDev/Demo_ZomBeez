using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class HumanController : MonoBehaviour
{
    [SerializeField] private HumanSettings _settings = null;
    [SerializeField] private GameObject _modelHolder = null;
    [SerializeField] private GameObject[] _abilityPrefabs = null;
    
    private HumanBrain _brain;
    private HumanAnimController _animController;
    private HumanInventory _inventory;

    public event Action OnHumanDeath;
    public HumanInventory Inventory => _inventory;
    public HumanSettings Settings {get {return _settings;}}

    public void Initialise(HumanSO so)
    {
        //References
        _animController = GetComponent<HumanAnimController>();
        _inventory = GetComponent<HumanInventory>();
        _brain = GetComponent<HumanBrain>();
        
        //Instantiate model
        GameObject go = Instantiate(so.model, _modelHolder.transform);

        GetComponent<Droppable>().Initialise();
        _animController.Initialise();
        _brain.Initialise(so);
    }

    public void UseAbility()
    {
        
    }

    public void LoseBrains(int val)
    {
        _settings.AddBrains(-val);

        if (_settings.Brains <= 0)
        {
            Death();
        }
    }

    public void ReceiveDamage(float initialDamage)
    {
        float remainingDamage = Settings.Armour - initialDamage;
        if (remainingDamage >= 0)
        {
            Settings.SetArmour(remainingDamage);
            return;
        }
        else if (remainingDamage < 0)
        {
            Settings.SetArmour(0);
            Settings.SetHealth(Settings.Health + remainingDamage);
        
            if (Settings.Health <= 0)
            {
                Death();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HiveResources>())
        {
            other.gameObject.GetComponent<HiveResources>().LoseJelly();
            Settings.SetHasJelly(true);
        }
    }

    private void Death()
    {
        Settings.SetIsDead(true);
        _animController.Bool_IsDead(true);
        OnHumanDeath?.Invoke();
        HiveController.live.RemoveHuman(this);
        Destroy(this.gameObject, 5.0f);
    }
}
