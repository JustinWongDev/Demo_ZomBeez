using System;
using UnityEngine;

public class DepotAnimController : MonoBehaviour
{
    [SerializeField] private Animator animator = null;

    public void PlayDepositAnim()
    {
        animator.SetTrigger("useDepot");
    }
}
