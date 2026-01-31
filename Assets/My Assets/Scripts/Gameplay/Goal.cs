using System;
using intheclouds;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    public UnityEvent ReachedGoal;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerManager>())
        {
            ReachedGoal?.Invoke();
        }
    }
}
