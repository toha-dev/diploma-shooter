using Zenject;

public class PlayerInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		Container
			.Bind<PlayerInputActions>()
			.FromNew()
			.AsSingle()
			.NonLazy();
	}
}