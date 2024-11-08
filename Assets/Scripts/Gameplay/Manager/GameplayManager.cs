using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gameplay.Interactables;
using UnityEngine;
using VContainer.Unity;

namespace Gameplay.Manager
{
    public class GameplayManager : IAsyncStartable
    {
        public Action<Vector3> OnBallPressed;
        public Action<Vector3> OnBallTrajectoryChanged;
        public Action<Vector3> ShootBall;
        public Action ShotCanceled;
        public Action ResetBall;

        public float ForceMultiplier = 2;
        public float LevelStartTime = 60f;

        private TimeSpan levelTime;

        public TimeSpan LevelTime
        {
            get { return levelTime; }
            set
            {
                levelTime = value;
                if (levelTime < TimeSpan.Zero)
                {
                    levelTime = TimeSpan.Zero;
                }
            }
        }

        private GameplayState state;


        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            LevelTime = TimeSpan.FromSeconds(LevelStartTime);
            state = GameplayState.Active;
            ProgressLevelTime(cancellation).Forget();
        }

        private async UniTask ProgressLevelTime(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (state != GameplayState.Active)
                {
                    await UniTask.Yield();
                    continue;
                }

                await UniTask.WaitForSeconds(1, cancellationToken: token);
                if (state != GameplayState.Active)
                {
                    await UniTask.Yield();
                    continue;
                }
                LevelTime -= TimeSpan.FromSeconds(1);
                if (LevelTime <= TimeSpan.Zero)
                {
                    state = GameplayState.Failed;
                    ProcessStateChange();
                }
            }
        }

        private void ProcessStateChange()
        {
            switch (state)
            {
                case GameplayState.PreLevel:
                    break;
                case GameplayState.Active:
                    break;
                case GameplayState.Paused:
                    break;
                case GameplayState.Won:
                    break;
                case GameplayState.Failed:
                    break;
            }
        }
        
        public void BallSelectShot()
        {
            
        }

        public void BallShotFinalized(Vector2 dragDelta)
        {
            var velocity = CalculateBallVelocity(dragDelta);
            ShootBall?.Invoke(velocity);
        }

        public void BallShotChanged(Vector2 dragDelta)
        {
            var velocity = CalculateBallVelocity(dragDelta);
            OnBallTrajectoryChanged?.Invoke(velocity);
        }

        private Vector3 CalculateBallVelocity(Vector2 dragDelta)
        {
            var forceVector = new Vector3(-dragDelta.x, -dragDelta.y, -dragDelta.y);
            return forceVector * ForceMultiplier;
        }

        public void TargetHit(HoleControl.HoleState holeState)
        {
            switch (holeState)
            {
                case HoleControl.HoleState.Positive:
                    LevelTime += TimeSpan.FromSeconds(5);
                    break;
                case HoleControl.HoleState.Negative:
                    LevelTime -= TimeSpan.FromSeconds(10);
                    break;
                case HoleControl.HoleState.Bonus:
                    LevelTime += TimeSpan.FromSeconds(10);
                    break;
            }
            ResetBall?.Invoke();
        }
    }

    public enum GameplayState
    {
        PreLevel,
        Active,
        Paused,
        Won,
        Failed
    }
}
