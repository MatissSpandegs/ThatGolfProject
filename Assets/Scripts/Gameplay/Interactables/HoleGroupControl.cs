using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Manager;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = System.Random;

namespace Gameplay.Interactables
{
    public class HoleGroupControl : MonoBehaviour
    {
        [Inject] private GameplayManager gameplayManager { get; set; }
        [Inject] private IObjectResolver objectResolver { get; set; }
        
        [SerializeField] private Transform[] validHolePositions;
        [SerializeField] private Transform holeParent;
        
        private List<HoleControl> holes;
        
        private void Awake()
        {
            SpawnHoles();
            SetHoleStates();
            gameplayManager.ResetBall += OnBallReset;
        }

        private void SpawnHoles()
        {
            var requiredHoles = validHolePositions.Length;
            holes = new List<HoleControl>(requiredHoles);
            var holeObject = Resources.Load<GameObject>("Prefabs/Gameplay/Hole");
            for (var i = 0; i < requiredHoles; i++)
            {
                var hole = objectResolver.Instantiate(holeObject, holeParent);
                var position = validHolePositions[i].localPosition;
                hole.transform.localPosition = position;
                var holeControl = hole.GetComponent<HoleControl>();
                holes.Add(holeControl);
            }
        }

        private void SetHoleStates()
        {
            var addBonusHole = UnityEngine.Random.Range(0, 100) > 80;
            var totalHoles = UnityEngine.Random.Range(2, holes.Count+1);
            var activeHoleCount = UnityEngine.Random.Range(1, totalHoles);
            var rng = new Random();
            var randomHoles = holes.OrderBy(x => rng.Next());
            var holesAdded = 0;
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

                if (holesAdded >= totalHoles)
                {
                    holeControl.transform.gameObject.SetActive(false);
                }
                else
                {
                    holesAdded++;
                    holeControl.transform.gameObject.SetActive(true);
                    holeControl.SetState(state);
                }
            }
        }

        private void OnBallReset()
        {
            SetHoleStates();
        }

        public void BallLanded(HoleControl.HoleState currentState)
        {
            gameplayManager.TargetHit(currentState);
        }
    }
}
