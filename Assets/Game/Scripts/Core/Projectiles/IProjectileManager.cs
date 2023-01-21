using DS.Core.Weapons;
using UnityEngine;

namespace DS.Core.Projectiles
{
	public interface IProjectileManager
	{
		bool IsSpawned(ProjectileBehaviour projectileBehaviour);

		void RegisterShot(Vector3 initialPosition, Quaternion quaternion, float force);
		void RegisterHit(ProjectileBehaviour projectileBehaviour, Collision other);
		void Unregister(ProjectileBehaviour projectileBehaviour);
	}
}