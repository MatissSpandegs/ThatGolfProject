using Gameplay.Manager;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameplayManager>(Lifetime.Singleton);
        }
    }
}
