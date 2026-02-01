using System;
using intheclouds;
using UnityEngine;
using FMODUnity;


public class JumpPad : MonoBehaviour
{
    public float JumpPadPower = 10f;

    //sound
    
    [SerializeField] private EventReference jump;


    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerManager>();
        if (player)
        {
            player.Controller.OnEnteredJumpPad(JumpPadPower);

            RuntimeManager.PlayOneShot(jump, transform.position);
        }
    }
}
