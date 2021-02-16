using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class HumanController : MonoBehaviour
{
    [SerializeField] private HumanSettings _settings = null;
    [SerializeField] private GameObject _modelHolder = null;
    [SerializeField] private GameObject _cam = null;
    
    private HumanBrain _brain;
    private HumanAnimController _animController;
    private HumanInventory _inventory;

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

    public void Attack()
    {
        
    }

    public void UseAbility()
    {
        
    }

    public void LoseBrains(int val)
    {
        _settings.AddBrains(-val);

        if (_settings.Brains <= 0)
        {
            Settings.SetIsDead(true);
            _animController.Bool_IsDead(true);
            Destroy(this.gameObject, 5.0f);
        }
    }

    public void TakeDamage(float dmg)
    {
    //Armour calc
        float val = Settings.Armour - dmg;
        if (val >= 0)
        {
            Settings.SetArmour(val);
            return;
        }
        else if (val < 0)
        {
            Settings.SetArmour(0);
            
    //Health calc
        Settings.SetHealth(Settings.Health + val);
            
    //Death logic
            if (Settings.Health <= 0)
            {
                Settings.SetIsDead(true);
                _animController.Bool_IsDead(true);
                Destroy(this.gameObject, 5.0f);
            }
        }
    }

    public GameObject ReturnCamPos()
    {
        return _cam;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Aware of bees?
        if (Settings.GetIsAware() == false)
        {
            Settings.SetIsAware(other.GetComponent<Worker>());
        }
        
        //Jelly?
        if (other.gameObject.GetComponent<Hive>())
        {
            other.gameObject.GetComponent<Hive>().LoseJelly();
            Settings.SetHasJelly(true);
        }
    }
}
