using System;
using intheclouds;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float JumpPadPower = 10f;
    
    
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerManager>();
        if (player)
        {
            player.Controller.OnEnteredJumpPad(JumpPadPower);
        }
    }
}
