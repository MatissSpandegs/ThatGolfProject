using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private TextMeshProUGUI addition;

        private void Awake()
        {
            gameplayManager.TimeChanged += OnTimeChanged;
        }

        private void OnTimeChanged(int seconds)
        {
            var token = gameplayManager.MainCancellationSource.Token;
            ShowAddedTime(seconds, token).Forget();
        }

        private void Update()
        {
            timer.SetText($"Time: {gameplayManager.LevelTime:mm\\:ss}");
        }

        private async UniTask ShowAddedTime(int seconds, CancellationToken token)
        {
            addition.gameObject.SetActive(true);
            addition.SetText($"{(seconds > 0 ? '+' : '-')}{Mathf.Abs(seconds)} seconds");
            await UniTask.WaitForSeconds(1, cancellationToken: token);
            addition.gameObject.SetActive(false);
        }
    }
}
