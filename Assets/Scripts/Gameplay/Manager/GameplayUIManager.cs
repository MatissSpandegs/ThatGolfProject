using UnityEngine;

namespace Gameplay.Manager
{
    public class GameplayUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject loseUI;

        public void SetLoseScreen(bool active)
        {
            loseUI.SetActive(active);
        }
    }
}
