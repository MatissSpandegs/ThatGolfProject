using System;
using Cysharp.Threading.Tasks;
using Gameplay.Manager;
using UnityEngine;
using VContainer;

namespace Gameplay.Ball
{
    public class BallTrajectoryControl : MonoBehaviour
    {
        [Inject] private GameplayManager gameplayManager { get; set; }
        
        [SerializeField] private LineRenderer trajectory;
        [SerializeField] private int segmentCount;

        private Vector3[] trajectoryPoints;
        private void Awake()
        {
            trajectoryPoints = new Vector3[segmentCount];
            trajectory.positionCount = segmentCount;
        }

        private void UpdateTrajectory(Vector3 forceVector, Rigidbody rb, Vector3 startingPoint)
        {
            var velocity = forceVector / rb.mass * Time.fixedDeltaTime;
            var flightDuration = 2 * velocity.y / Physics.gravity.y;
            var stepTime = flightDuration / trajectoryPoints.Length;
            for (var i = 0; i < trajectoryPoints.Length; i++)
            {
                var stepTimePassed = stepTime * i;
                var movementVelocity = new Vector3(
                    velocity.x * stepTimePassed,
                    velocity.y * stepTimePassed - 0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed,
                    velocity.z * stepTimePassed);
                trajectoryPoints[i] = -movementVelocity + startingPoint;
            }
            trajectory.SetPositions(trajectoryPoints);
        }

        private async UniTask DrawTrajectory()
        {
            for (var i = 0; i < segmentCount; i++)
            {
                var t = i/((float)segmentCount - 1);
            }
            await UniTask.Yield();
        }

        private Vector3 GetQuadraticCoordinates(Vector3 start, Vector3 end, Vector3 middle, float position)
        {
            return Mathf.Pow(1-position,2)*start + 2*position*(1-position)*middle + Mathf.Pow(position,2)*end ;
        }
    }
}
