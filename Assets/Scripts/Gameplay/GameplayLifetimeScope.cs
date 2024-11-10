using Gameplay.Interactables;
using Gameplay.Manager;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameplayUIManager uiManager;
        [SerializeField] private GameplaySoundManager soundManager;
        [SerializeField] private HoleGroupControl holeGroupControl;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameplayManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.RegisterComponent(uiManager);
            builder.RegisterComponent(soundManager);
            builder.RegisterComponent(holeGroupControl);
        }
    }
}
