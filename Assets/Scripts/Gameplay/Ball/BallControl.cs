using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Manager;
using UnityEngine;
using VContainer;

public class BallControl : MonoBehaviour
{
    [Inject] private GameplayManager gameplayManager { get; set; }
    [SerializeField] private float minVelocityThreshold = 0.3f;
    public Rigidbody Rb;
    private bool inShot;
    private CancellationTokenSource source;
    private void Awake()
    {
        gameplayManager.ShootBall += Shoot;
        gameplayManager.ResetBall += Reset;
        gameplayManager.BallSetUp?.Invoke(this);
    }

    private void Reset()
    {
        Rb.velocity = Vector3.zero;
        Rb.angularVelocity = Vector3.zero;
        
        source?.Cancel();
        source?.Dispose();
        source = null;
    }

    private void Shoot(Vector3 velocity)
    {
        Rb.AddForce(velocity);
        
        source?.Cancel();
        source?.Dispose();
        source = null;

        source = new CancellationTokenSource();
        CheckIfBallStopped(source.Token).Forget();
    }

    private async UniTask CheckIfBallStopped(CancellationToken token)
    {
        await UniTask.NextFrame(timing: PlayerLoopTiming.LastFixedUpdate, cancellationToken: token);
        while (!token.IsCancellationRequested)
        {
            //Check OOB
            if (Rb.transform.position.y < -30f)
            {
                gameplayManager.BallMissed();
                break;
            }
            if (Rb.velocity.magnitude > minVelocityThreshold)
            {
                await UniTask.NextFrame(timing: PlayerLoopTiming.FixedUpdate, cancellationToken: token);
                continue;
            }

            //Delay to account for possible ball bounce
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: token);
            if (Rb.velocity.magnitude > minVelocityThreshold)
            {
                continue;
            }
            gameplayManager.BallMissed();
            break;
        }
    }
}
