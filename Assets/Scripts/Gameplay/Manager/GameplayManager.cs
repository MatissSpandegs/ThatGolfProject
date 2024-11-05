using System;
using UnityEngine;

namespace Gameplay.Manager
{
    public class GameplayManager
    {
        public Action<Vector2> OnBallPressed;
        public Action<Vector2> OnBallTrajectoryChanged;
        public Action<Vector2> ShootBall;
        public Action ShotCanceled;
    

        public void BallSelectShot()
        {
            
        }

        public void BallShotFinalized(Vector2 dragDelta)
        {
            
        }

        public void BallShotChanged(Vector2 dragDelta)
        {
            OnBallTrajectoryChanged?.Invoke(dragDelta);
        }
    }
}
