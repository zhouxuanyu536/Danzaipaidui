using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private Animator m_Animator;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public float blendValue;
    [HideInInspector] public bool isFreeFall;
    [HideInInspector] public bool isJump;
    [HideInInspector] public float animatorSpeed;

    private string IS_GROUNDED = "Grounded";
    private string BLEND = "Blend";
    private string FREEFALL = "FreeFall";
    private string JUMP = "Jump";

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        isGrounded = false;

    }
    private void Update()
    {
        m_Animator.speed = animatorSpeed;
        if (!IsOwner) return;
        m_Animator.SetBool(IS_GROUNDED, isGrounded);
        m_Animator.SetFloat(BLEND, blendValue);
        m_Animator.SetBool(FREEFALL, isFreeFall);
        if (isJump)
        {
            m_Animator.SetTrigger(JUMP);
            isJump = false;
        }
        

    }
}
