using System;
using System.Collections.Generic;
using System.Threading;
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
        [SerializeField] private Gradient gradient;

        private List<Vector3> trajectoryPoints = new List<Vector3>();
        private void Awake()
        {
            gameplayManager.OnBallTrajectoryChanged += OnTrajectoryChanged;
            gameplayManager.OnBallPressed += OnBallPressed;
            gameplayManager.ResetBall += OnBallReset;
            gameplayManager.ShootBall += OnBallShot;
        }

        private void OnTrajectoryChanged(Vector3 velocity)
        {
            UpdateTrajectory(velocity, ballControl.Rb, ballControl.Rb.position);
        }

        private void OnBallPressed()
        {
            trajectory.gameObject.SetActive(true);
        }

        private void OnBallReset()
        {
            trajectory.gameObject.SetActive(false);
        }

        private void OnBallShot(Vector3 forceVector)
        {
            var token = gameplayManager.MainCancellationSource.Token;
            DecreaseTrajectory(token).Forget();
        }

        private async UniTask DecreaseTrajectory(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var applied = false;
                for (var i = 0; i < trajectoryPoints.Count-1; i++)
                {
                    var firstTrajectoryPoint = trajectoryPoints[i];
                    var secondTrajectoryPoint = trajectoryPoints[i+1];
                    var rbPosition = ballControl.Rb.transform.position;

                    var distance = Vector3.Distance(firstTrajectoryPoint, rbPosition);
                    var distancePoints = Vector3.Distance(firstTrajectoryPoint, secondTrajectoryPoint);

                    if (distance > distancePoints) continue;
                    var startIndex = i+1;
                    var positionCount = trajectoryPoints.Count - startIndex;
                    var positions = trajectoryPoints.GetRange(startIndex, positionCount);
                    positions.Insert(0, ballControl.Rb.transform.position);
                    trajectory.positionCount = positions.Count;
                    trajectory.SetPositions(positions.ToArray());
                    applied = true;
                }

                if (!applied)
                {
                    trajectory.gameObject.SetActive(false);
                    break;
                }
                await UniTask.WaitForFixedUpdate(token);
            }
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
            
            var distance = Vector3.Distance(trajectoryPoints[0], trajectoryPoints[^1]);
            var maxDistance = 30f;
            var percentDistance = distance / maxDistance;
            trajectory.material.color = gradient.Evaluate(percentDistance);
        }
    }
}
