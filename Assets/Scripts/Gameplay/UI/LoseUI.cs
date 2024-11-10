using System;
using Gameplay.Manager;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Gameplay.UI
{
    public class LoseUI : MonoBehaviour
    {
        [Inject] private GameplayManager gameplayManager { get; set; }
        [Inject] private GameplayUIManager uIManager { get; set; }
        
        [SerializeField] private Button retry;

        private void Awake()
        {
            retry.onClick.AddListener(OnRetry);
        }

        private void OnRetry()
        {
            gameplayManager.ResetGame();
            uIManager.SetLoseScreen(false);
        }
    }
}
