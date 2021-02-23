using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("FX")]
    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private GameObject deathSound;

    [Header("Stats")]
    protected float health = 100;

    public virtual void takeDamage(float dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
            Instantiate(deathSound, transform.position, transform.rotation);

            Destroy(this.gameObject);
        }
    }
}
