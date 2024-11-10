using UnityEngine;

namespace Gameplay.Manager
{
    public class GameplayUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject ballInput;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private GameObject victoryUI;
        [SerializeField] private GameObject loseUI;
            
        public void SetBallInput(bool active)
        {
            ballInput.SetActive(active);
        }

        public void SetPause(bool active)
        {
            pauseUI.SetActive(active);
        }

        public void SetWinScreen(bool active)
        {
            victoryUI.SetActive(active);
        }

        public void SetLoseScreen(bool active)
        {
            loseUI.SetActive(active);
        }
    }
}
