using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimController : MonoBehaviour
{
    private Animator _animator;

    public void Initialise()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public bool IsAnimOnBlendTree()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("2D Blend Tree");
    }

    public void Bool_IsDead(bool val)
    {
        _animator.SetBool("isDead", val);
    }

    public void Float_VelocityZ(float val)
    {
        _animator.SetFloat("Velocity Z", val);
    }

    public void Trig_Dropped()
    {
        _animator.SetTrigger("isDropped");
    }

    public void Trig_Grounded()
    {
        _animator.SetTrigger("isGrounded");
    }

    public void Trig_Collect()
    {
        _animator.SetTrigger("isCollecting");
    }
    
    public void Trig_Deposit()
    {
        _animator.SetTrigger("isDepositing");
    }
}
