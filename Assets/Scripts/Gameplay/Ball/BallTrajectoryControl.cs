using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Manager;
using UnityEngine;
using VContainer;

namespace Gameplay.Ball
{
    public class BallTrajectoryControl : MonoBehaviour
    {
        [Inject] private GameplayManager gameplayManager { get; set; }

        [SerializeField] private BallControl ballControl;
        [SerializeField] private LineRenderer trajectory;
        [SerializeField] private int segmentCount;
        [SerializeField] private float force;
        
        private List<Vector3> trajectoryPoints = new List<Vector3>();
        private void Awake()
        {
            gameplayManager.OnBallTrajectoryChanged += OnTrajectoryChanged;
        }

        private void OnTrajectoryChanged(Vector3 velocity)
        {
            UpdateTrajectory(velocity, ballControl.Rb, ballControl.Rb.position);
        }

        private void UpdateTrajectory(Vector3 forceVector, Rigidbody rb, Vector3 startingPoint)
        {
            var velocity = forceVector / rb.mass * Time.fixedDeltaTime;
            var flightDuration = 2 * velocity.y / Physics.gravity.y;
            var stepTime = flightDuration / segmentCount;
            trajectoryPoints.Clear();
            for (var i = 0; i < segmentCount; i++)
            {
                var stepTimePassed = stepTime * i;
                var movementVector = new Vector3(
                    velocity.x * stepTimePassed,
                    velocity.y * stepTimePassed - 0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed,
                    velocity.z * stepTimePassed);
                
                if (Physics.Raycast(startingPoint, -movementVector, out _, movementVector.magnitude))
                {
                    break;
                }
                trajectoryPoints.Add(-movementVector + startingPoint);
            }

            trajectory.positionCount = trajectoryPoints.Count;
            trajectory.SetPositions(trajectoryPoints.ToArray());
        }
    }
}
