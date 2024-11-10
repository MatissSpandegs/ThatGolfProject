using System;
using Gameplay.Manager;
using TMPro;
using UnityEngine;
using VContainer;

namespace Gameplay.UI
{
    public class LevelTimer : MonoBehaviour
    {
        [Inject] private GameplayManager gameplayManager { get; set; }

        [SerializeField] private TextMeshProUGUI timer;

        private void Update()
        {
            timer.SetText($"Time: {gameplayManager.LevelTime:mm\\:ss}");
        }
    }
}
