using System;
using UnityEngine;

namespace DS.Core.Player
{
	public interface IPlayerEntity
	{
		GameObject GameObject { get; }

		event Action EventPlayerSpawned;
		event Action EventPlayerDespawned;
		event Action EventPlayerDead;

		void StartShooting();
		void EndShooting();
		void Reload();
	}
}