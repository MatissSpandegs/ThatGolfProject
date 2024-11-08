using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Manager;
using UnityEngine;
using VContainer;

public class BallControl : MonoBehaviour
{
    [Inject] private GameplayManager gameplayManager { get; set; }
    
    public Rigidbody Rb;

    private void Awake()
    {
        gameplayManager.ShootBall += Shoot;
    }

    private void Shoot(Vector3 velocity)
    {
        Rb.AddForce(velocity);
    }
}
