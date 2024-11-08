using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Manager;
using UnityEngine;
using VContainer;
using Random = System.Random;

namespace Gameplay.Interactables
{
    public class HoleGroupControl : MonoBehaviour
    {
        [Inject] private GameplayManager gameplayManager { get; set; }
        
        [SerializeField] private HoleControl[] holes;
        [SerializeField] private bool trig;
        private void Awake()
        {
            SetHoleStates();
        }

        private void Update()
        {
            if (trig)
            {
                trig = false;
                SetHoleStates();
            }
        }

        public void SetHoleStates()
        {
            var addBonusHole = UnityEngine.Random.Range(0, 100) > 80;
            var activeHoleCount = UnityEngine.Random.Range(1, holes.Length);
            var rng = new Random();
            var randomHoles = holes.OrderBy(x => rng.Next());
            foreach (var holeControl in randomHoles)
            {
                var state = HoleControl.HoleState.Negative;
                var isActiveHole = activeHoleCount > 0;
                if (isActiveHole)
                {
                    if (addBonusHole)
                    {
                        addBonusHole = false;
                        state = HoleControl.HoleState.Bonus;
                    }
                    else
                    {
                        state = HoleControl.HoleState.Positive;
                    }
                    activeHoleCount--;
                }

                holeControl.SetState(state);

            }
        }

        public void BallLanded(HoleControl.HoleState currentState)
        {
            gameplayManager.TargetHit(currentState);
            SetHoleStates();
        }
    }
}
