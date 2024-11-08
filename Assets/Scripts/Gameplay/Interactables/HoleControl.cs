using System;
using UnityEngine;
using VContainer;

namespace Gameplay.Interactables
{
    public class HoleControl : MonoBehaviour
    {
        [Inject] private HoleGroupControl holeGroup { get; set; }
        
        [SerializeField] private ColliderListener colliderListener;
        [SerializeField] private MeshRenderer meshRenderer;

        private HoleState currentState;
        private void Awake()
        {
            colliderListener.TriggerEnter += TriggerEntered;
        }

        public void SetState(HoleState state)
        {
            currentState = state;
            switch (state)
            {
                case HoleState.Positive:
                    meshRenderer.material.color = Color.green;
                    break;
                case HoleState.Negative:
                    meshRenderer.material.color = Color.red;
                    break;
                case HoleState.Bonus:
                    meshRenderer.material.color = Color.yellow;
                    break;
            }
        }

        private void TriggerEntered(Collider other)
        {
            holeGroup.BallLanded(currentState);
        }

        public enum HoleState
        {
            Positive,
            Negative,
            Bonus
        }
    }
}
