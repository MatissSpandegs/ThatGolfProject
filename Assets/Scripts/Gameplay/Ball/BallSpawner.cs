using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Ball
{
    public class BallSpawner : MonoBehaviour
    {
        [Inject] private IObjectResolver objectResolver;
    
        private void Awake()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Gameplay/Ball");
            var ball = objectResolver.Instantiate(prefab, transform);
            objectResolver.Inject(ball);
        }
    }
}
