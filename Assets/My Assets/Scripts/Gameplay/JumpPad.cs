using System;
using intheclouds;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float JumpPadPower = 10f;

    //sound
    [Header("FMOD Player SFX")]
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
