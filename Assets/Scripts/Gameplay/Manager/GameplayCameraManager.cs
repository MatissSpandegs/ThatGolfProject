using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Gameplay.Manager
{
    public class GameplayCameraManager : MonoBehaviour
    {
        [Inject] private GameplayManager gameplayManager { get; set; }
        [Inject] private GameplaySoundManager soundManager { get; set; }
        
        [SerializeField] private PrelevelCameraData prelevelCameraOne;
        [SerializeField] private PrelevelCameraData prelevelCameraTwo;
        [SerializeField] private BallCamera ballCamera;
        [SerializeField] private ShotCamera shotCamera;
        

        private GameObject activeCamera;

        private void Awake()
        {
            gameplayManager.ResetBall += ResetBall;
            gameplayManager.ShootBall += ShotBall;
            gameplayManager.BallSetUp += BallSetUp;
            PlayIntroSequence().Forget();
        }

        private async UniTask PlayIntroSequence()
        {
            // gameplayManager.SetState(GameplayState.PreLevel);
            // soundManager.PlaySound(GameplaySoundManager.Sounds.Cheer);
            // await ShowPrelevel();
            gameplayManager.SetState(GameplayState.Active);
            SetActiveCamera(ballCamera.Cam.gameObject);
        }


        public async UniTask ShowPrelevel()
        {
            var cameraOneTime = 0f;
            var cameraOneMaxTime = prelevelCameraOne.TrackDuration;
            var cameraOne = prelevelCameraOne.Dolly;
            SetActiveCamera(cameraOne.gameObject);
            var cameraOneDolly = cameraOne.GetCinemachineComponent<CinemachineTrackedDolly>();
            while (cameraOneTime < cameraOneMaxTime)
            {
                cameraOneDolly.m_PathPosition = cameraOneTime / cameraOneMaxTime;
                cameraOneTime += Time.deltaTime;
                await UniTask.NextFrame();
            }

            var cameraTwoTime = 0f;
            var cameraTwoMaxTime = prelevelCameraTwo.TrackDuration;
            var cameraTwo = prelevelCameraTwo.Dolly;
            SetActiveCamera(cameraTwo.gameObject);
            var cameraTwoDolly = cameraTwo.GetCinemachineComponent<CinemachineTrackedDolly>();
            while (cameraTwoTime < cameraTwoMaxTime)
            {
                cameraTwoDolly.m_PathPosition = cameraTwoTime / cameraTwoMaxTime;
                cameraTwoTime += Time.deltaTime;
                await UniTask.NextFrame();
            }
        }

        private void ResetBall()
        {
            SetActiveCamera(ballCamera.Cam.gameObject);
        }

        private void ShotBall(Vector3 velocity)
        {
            SetActiveCamera(shotCamera.Cam.gameObject);
        }
        
        private void BallSetUp(BallControl ball)
        {
            var virtualCamera = shotCamera.Cam;
            var rbTransform = ball.Rb.transform;
            virtualCamera.Follow = rbTransform;
        }

        private void SetActiveCamera(GameObject newActiveCamera)
        {
            if (activeCamera != null)
            {
                activeCamera.SetActive(false);
            }

            activeCamera = newActiveCamera;
            activeCamera.SetActive(true);
        }
    }

    [Serializable]
    public class PrelevelCameraData
    {
        public CinemachineVirtualCamera Dolly;
        public float TrackDuration;
    }

    [Serializable]
    public class BallCamera
    {
        public CinemachineVirtualCamera Cam;
    }

    [Serializable]
    public class ShotCamera
    {
        public CinemachineVirtualCamera Cam;
    }
    
}
