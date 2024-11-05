using System;
using Gameplay.Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Gameplay.UI
{
    public class BallShootingControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Inject] private GameplayManager gamePlayManager { get; set; }
        
        [SerializeField] private Camera cam;
        [SerializeField] private RectTransform screen;
        [SerializeField] private RectTransform inputHolder;

        private Vector2 dragDelta;

        private void Awake()    
        {
            inputHolder.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            inputHolder.gameObject.SetActive(true);
            var position = eventData.position;
            MoveInput(position);
            dragDelta = Vector2.zero;
            gamePlayManager.BallSelectShot();
        }
        

        public void OnPointerUp(PointerEventData eventData)
        {
            inputHolder.gameObject.SetActive(false);
            gamePlayManager.BallShotFinalized(dragDelta);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var position = eventData.position;
            MoveInput(position);
            dragDelta += eventData.delta;
            gamePlayManager.BallShotChanged(dragDelta);
        }

        private void MoveInput(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(screen, screenPosition, cam, out var transformPoint);
            inputHolder.anchoredPosition = transformPoint;
        }
    }
}
