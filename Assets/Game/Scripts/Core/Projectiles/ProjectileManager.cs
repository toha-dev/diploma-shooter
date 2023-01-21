using System.Collections.Generic;
using DS.Core.Weapons;
using UnityEngine;
using Zenject;

namespace DS.Core.Projectiles
{
	public class ProjectileManager : MonoBehaviour, IProjectileManager
	{
		[Inject]
		private ProjectileBehaviour.ProjectilePool _projectilePool;

		[Inject]
		private ProjectileDecalBehaviour.DecalPool _projectileDecalPool;

		private readonly HashSet<ProjectileBehaviour> _projectiles = new();

		private void OnDisable()
		{
			foreach (var projectile in _projectiles)
			{
				_projectilePool.Despawn(projectile);
			}

			_projectiles.Clear();
		}

		public bool IsSpawned(ProjectileBehaviour projectileBehaviour)
		{
			return _projectiles.Contains(projectileBehaviour);
		}

		public void RegisterShot(Vector3 initialPosition, Quaternion quaternion, float force)
		{
			Debug.LogError($"Register at {initialPosition} | {quaternion.eulerAngles}");
			var projectile = _projectilePool.Spawn();
			var projectileTransform = projectile.transform;

			projectileTransform.position = initialPosition;
			projectileTransform.rotation = quaternion;

			projectile.Rigidbody.velocity = Vector3.zero;
			projectile.Rigidbody.angularVelocity = Vector3.zero;

			projectile.Rigidbody.AddForce(projectileTransform.forward * force);

			_projectiles.Add(projectile);
		}

		public void RegisterHit(ProjectileBehaviour projectileBehaviour, Collision other)
		{
			Debug.LogError($"Register hit at {projectileBehaviour.transform.position}");

			var decal = _projectileDecalPool.Spawn();
			var decalTransform = decal.transform;
			var projectileTransform = projectileBehaviour.transform;

			decalTransform.position = projectileTransform.position;
			decalTransform.rotation = projectileTransform.rotation;

			Unregister(projectileBehaviour);
		}

		public void Unregister(ProjectileBehaviour projectileBehaviour)
		{
			Debug.LogError($"Unregister");
			_projectilePool.Despawn(projectileBehaviour);
			_projectiles.Remove(projectileBehaviour);
		}
	}
}
