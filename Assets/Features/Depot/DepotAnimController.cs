using System;
using UnityEngine;

public class DepotAnimController : MonoBehaviour
{
    [SerializeField] private Animation animation;
    [SerializeField] private Animator animator;

    public void PlayDepositAnim()
    {
        animator.SetTrigger("useDepot");
    }
}
