using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gameplay.Interactables;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Manager
{
    public class GameplayManager : IAsyncStartable
    {
        [Inject] private GameplayUIManager uiManager { get; set; }
        [Inject] private GameplaySoundManager soundManager { get; set; }
        
        public Action OnBallPressed;
        public Action<Vector3> OnBallTrajectoryChanged;
        public Action<Vector3> ShootBall;
        public Action ShotCanceled;
        public Action ResetBall;
        public Action<BallControl> BallSetUp;

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
                    SetState(GameplayState.Failed);
                }
            }
        }

        public void BallSelectShot()
        {
            OnBallPressed?.Invoke();
        }

        public void BallShotFinalized(Vector2 dragDelta)
        {
            var velocity = CalculateBallVelocity(dragDelta);
            soundManager.PlaySound(GameplaySoundManager.Sounds.Shoot);
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
                    soundManager.PlaySound(GameplaySoundManager.Sounds.ShotIn);
                    break;
                case HoleControl.HoleState.Negative:
                    LevelTime -= TimeSpan.FromSeconds(10);
                    soundManager.PlaySound(GameplaySoundManager.Sounds.ShotMissed);
                    break;
                case HoleControl.HoleState.Bonus:
                    LevelTime += TimeSpan.FromSeconds(10);
                    soundManager.PlaySound(GameplaySoundManager.Sounds.ShotIn);
                    break;
            }
            ResetBall?.Invoke();
        }

        public void SetState(GameplayState newState)
        {
            if (newState == state) return;
            state = newState;
            ProcessStateChange();
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
                    uiManager.SetPause(true);
                    break;
                case GameplayState.Won:
                    uiManager.SetWinScreen(true);
                    break;
                case GameplayState.Failed:
                    uiManager.SetLoseScreen(true);
                    break;
            }
        }

        public void BallMissed()
        {
            soundManager.PlaySound(GameplaySoundManager.Sounds.ShotMissed);
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
