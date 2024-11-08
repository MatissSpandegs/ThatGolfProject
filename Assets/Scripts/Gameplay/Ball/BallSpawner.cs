using Gameplay.Manager;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Ball
{
    public class BallSpawner : MonoBehaviour
    {
        [Inject] private IObjectResolver objectResolver;
        [Inject] private GameplayManager gameplayManager;

        private GameObject ball;
        private void Awake()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Gameplay/Ball");
            ball = objectResolver.Instantiate(prefab, transform);
            objectResolver.Inject(ball);
            gameplayManager.ResetBall += ResetBall;
        }

        private void ResetBall()
        {
            ball.transform.localPosition = Vector3.zero;
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
