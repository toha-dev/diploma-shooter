using System;
using DS.Core.Weapons;
using UniRx;
using UnityEngine;

namespace DS.Core.Player
{
	public interface IPlayerEntity
	{
		GameObject GameObject { get; }

		IReadOnlyReactiveProperty<WeaponBehaviour> SelectedWeapon { get; }

		event Action EventPlayerSpawned;
		event Action EventPlayerDespawned;
		event Action EventPlayerDead;

		void StartShooting();
		void EndShooting();
		void Reload();
	}
}