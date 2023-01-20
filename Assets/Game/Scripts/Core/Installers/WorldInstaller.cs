using DS.Core.Projectiles;
using DS.Core.Weapons;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

public class WorldInstaller : MonoInstaller
{
	[field: SerializeField]
	private ProjectileManager ProjectileManager { get; [UsedImplicitly] set; }

	[field: SerializeField]
	private Transform ProjectileParentForPool { get; [UsedImplicitly] set; }

	[field: SerializeField]
	private ProjectileBehaviour Projectile { get; [UsedImplicitly] set; }

	[field: SerializeField]
	private Transform ProjectileDecalParentForPool { get; [UsedImplicitly] set; }

	[field: SerializeField]
	private ProjectileDecalBehaviour ProjectileDecal { get; [UsedImplicitly] set; }

	public override void InstallBindings()
	{
		Container
			.Bind<IProjectileManager>()
			.To<ProjectileManager>()
			.FromInstance(ProjectileManager)
			.AsSingle()
			.NonLazy();
		
		Container
			.Bind<PlayerInputActions>()
			.FromNew()
			.AsSingle()
			.NonLazy();

		Container
			.BindMemoryPool<ProjectileBehaviour, ProjectileBehaviour.ProjectilePool>()
			.WithInitialSize(10)
			.FromComponentInNewPrefab(Projectile)
			.UnderTransform(ProjectileParentForPool);

		Container.BindMemoryPool<ProjectileDecalBehaviour, ProjectileDecalBehaviour.DecalPool>()
			.WithInitialSize(10)
			.FromComponentInNewPrefab(ProjectileDecal)
			.UnderTransform(ProjectileDecalParentForPool);
	}
}