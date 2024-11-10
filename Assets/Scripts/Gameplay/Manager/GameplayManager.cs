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
    public class GameplayManager : IInitializable, IDisposable
    {
        [Inject] private GameplayUIManager uiManager { get; set; }
        [Inject] private GameplaySoundManager soundManager { get; set; }
        
        public Action OnBallPressed;
        public Action<Vector3> OnBallTrajectoryChanged;
        public Action<Vector3> ShootBall;
        public Action ResetBall;
        public Action<BallControl> BallSetUp;
        public Action<int> TimeChanged { get; set; }

        public float ForceMultiplier = 2;
        public float LevelStartTime = 60f;

        private TimeSpan levelTime;

        public CancellationTokenSource MainCancellationSource;

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
        
        public GameplayState State { get; private set; }


        public void Initialize()
        {
            MainCancellationSource = new CancellationTokenSource();
            LevelTime = TimeSpan.FromSeconds(LevelStartTime);
            ProgressLevelTime(MainCancellationSource.Token).Forget();
        }

        private async UniTask ProgressLevelTime(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (State != GameplayState.Active)
                {
                    await UniTask.Yield();
                    continue;
                }

                await UniTask.WaitForSeconds(1, cancellationToken: token);
                if (State != GameplayState.Active)
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
            var addedTime = 0;
            switch (holeState)
            {
                case HoleControl.HoleState.Positive:
                    addedTime = 5;
                    soundManager.PlaySound(GameplaySoundManager.Sounds.ShotIn);
                    break;
                case HoleControl.HoleState.Negative:
                    addedTime = -10;
                    soundManager.PlaySound(GameplaySoundManager.Sounds.ShotMissed);
                    break;
                case HoleControl.HoleState.Bonus:
                    addedTime = 10;
                    soundManager.PlaySound(GameplaySoundManager.Sounds.ShotIn);
                    break;
            }
            LevelTime += TimeSpan.FromSeconds(addedTime);
            TimeChanged?.Invoke(addedTime);
            ResetBall?.Invoke();
        }

        public void SetState(GameplayState newState)
        {
            if (newState == State) return;
            State = newState;
            ProcessStateChange();
        }
        
        private void ProcessStateChange()
        {
            switch (State)
            {
                case GameplayState.PreLevel:
                    break;
                case GameplayState.Active:
                    SetPause(true);
                    break;
                case GameplayState.Failed:
                    SetPause(false);
                    uiManager.SetLoseScreen(true);
                    break;
            }
        }

        public void ResetGame()
        {
            LevelTime = TimeSpan.FromSeconds(LevelStartTime);
            ResetBall?.Invoke();
            SetState(GameplayState.Active);
        }

        private void SetPause(bool active)
        {
            Time.timeScale = active ? 1 : 0;
        }

        public void BallMissed()
        {
            soundManager.PlaySound(GameplaySoundManager.Sounds.ShotMissed);
            ResetBall?.Invoke();
        }

        public void Dispose()
        {
            MainCancellationSource?.Cancel();
            MainCancellationSource?.Dispose();
            MainCancellationSource = null;
        }
    }

    public enum GameplayState
    {
        PreLevel,
        Active,
        Failed
    }
}
