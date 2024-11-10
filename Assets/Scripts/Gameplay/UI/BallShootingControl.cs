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
        [SerializeField] private RectTransform ghostInputHolder;
        [SerializeField] private RectTransform inputHolder;
        [SerializeField] private LineRenderer lineRenderer;

        private Vector2 dragDelta;
        private bool canShoot = true;
        
        private void Awake()    
        {
            SetHelpState(false);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new[]{Vector3.zero, Vector3.zero });
            gamePlayManager.ResetBall += OnBallReset;
            gamePlayManager.ShootBall += OnBallShot;
        }

        private void OnBallReset()
        {
            SetHelpState(false);
            canShoot = true;
        }

        private void OnBallShot(Vector3 velocity)
        {
            canShoot = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanShoot())
            {
                return;
            }
            SetHelpState(true);
            var position = eventData.position;
            var inputPosition = GetInputPosition(position);
            inputHolder.anchoredPosition = inputPosition;
            ghostInputHolder.anchoredPosition = inputPosition;
            lineRenderer.SetPosition(0, inputPosition);
            dragDelta = Vector2.zero;
            gamePlayManager.BallSelectShot();
        }
        

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!CanShoot())
            {
                return;
            }
            
            SetHelpState(false);

            if (dragDelta.y > 0)
            {
                return;
            }
            gamePlayManager.BallShotFinalized(dragDelta);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!CanShoot())
            {
                return;
            }
            var position = eventData.position;
            var inputPosition = GetInputPosition(position);
            inputHolder.anchoredPosition = inputPosition;
            lineRenderer.SetPosition(1, inputPosition);
            dragDelta += eventData.delta;
            if (dragDelta.y > 0)
            {
                return;
            }
            gamePlayManager.BallShotChanged(dragDelta);
        }

        private void SetHelpState(bool active)
        {
            inputHolder.gameObject.SetActive(active);
            ghostInputHolder.gameObject.SetActive(active);
            lineRenderer.gameObject.SetActive(active);
        }

        private bool CanShoot()
        {
            var activeState = gamePlayManager.State == GameplayState.Active;
            return activeState && canShoot;
        }

        private Vector3 GetInputPosition(Vector2 screenPosition)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(screen, screenPosition, cam,
                    out var transformPoint))
            {
                return Vector3.zero;
            }
            return transformPoint;
        }
    }
}
