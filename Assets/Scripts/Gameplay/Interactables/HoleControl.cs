using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Manager;
using UnityEngine;
using VContainer;

namespace Gameplay.Interactables
{
    public class HoleControl : MonoBehaviour
    {
        [Inject] private HoleGroupControl holeGroup { get; set; }
        [Inject] private GameplayManager gameplayManager { get; set; }
        
        [SerializeField] private ColliderListener colliderListener;
        [SerializeField] private MeshRenderer meshRenderer;

        private HoleState currentState;
        private void Awake()
        {
            colliderListener.TriggerEnter += TriggerEntered;
        }

        public void SetState(HoleState state)
        {
            var previousState = currentState;
            if (previousState == state)
            {
                return;
            }
            currentState = state;
            var materialColor = meshRenderer.material.color;
            var newColor = Color.white;
            switch (state)
            {
                case HoleState.Positive:
                    newColor = Color.blue;
                    break;
                case HoleState.Negative:
                    newColor = Color.red;
                    break;
                case HoleState.Bonus:
                    newColor = Color.yellow;
                    break;
            }

            if (previousState == HoleState.None)
            {
                meshRenderer.material.color = newColor;
                return;
            }
            SwitchHoleColor(materialColor,newColor, gameplayManager.MainCancellationSource.Token).Forget();
        }

        private async UniTask SwitchHoleColor(Color oldColor, Color newColor, CancellationToken token)
        {
            var time = 0f;
            var maxTime = 0.2f;
            while (time < maxTime)
            {
                var percentTime = time / maxTime;
                var color = Color.Lerp(oldColor, newColor, percentTime);
                meshRenderer.material.color = color;
                time += Time.deltaTime;
                await UniTask.NextFrame(token);
            }
        }

        private void TriggerEntered(Collider other)
        {
            holeGroup.BallLanded(currentState);
        }

        public enum HoleState
        {
            None,
            Positive,
            Negative,
            Bonus
        }
    }
}
