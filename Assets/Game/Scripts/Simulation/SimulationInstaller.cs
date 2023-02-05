using System;
using DS.Core.Player;
using DS.Simulation.GameModes;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace DS.Simulation.Installers
{
	public class SimulationInstaller : MonoInstaller
	{
		[field: SerializeField]
		private PlayerEntity PlayerEntity { get; [UsedImplicitly] set; }

		public override void InstallBindings()
		{
			Container.Bind<IPlayerController>().To<PlayerController>().FromNew().AsSingle().NonLazy();

			Container.BindFactory<PlayerEntity, PlayerEntity.Factory>()
				.FromComponentInNewPrefab(PlayerEntity);

			switch (Application.GameMode)
			{
				case GameModeType.Free:
				{
					Container.Bind<IGameMode>().To<FreeGameMode>().FromNew().AsSingle().NonLazy();
				}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}