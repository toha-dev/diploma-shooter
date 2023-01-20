using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Projectiles;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace DS.Core.Weapons
{
	public class ProjectileBehaviour : MonoBehaviour
	{
		[field: SerializeField]
		public Rigidbody Rigidbody { get; [UsedImplicitly] private set; }

		[field: SerializeField]
		private float DespawnTimer { get; [UsedImplicitly] set; }

		[Inject]
		private IProjectileManager _projectileManager;

		private void OnCollisionEnter(Collision other)
		{
			_projectileManager.RegisterHit(this, other);
		}

		private void FixedUpdate()
		{
			Debug.LogError($"PROJECTILE POSITION {transform.position}");
		}

		[UsedImplicitly]
		public class ProjectilePool : MonoMemoryPool<ProjectileBehaviour>
		{
			private CancellationTokenSource _cancellationToken;

			protected override void OnSpawned(ProjectileBehaviour item)
			{
				base.OnSpawned(item);

				_cancellationToken = new CancellationTokenSource();
				DespawnThroughTime(item.DespawnTimer).Forget();

				async UniTaskVoid DespawnThroughTime(float time)
				{
					await UniTask.Delay(
						(int)(time * 1000),
						cancellationToken: _cancellationToken.Token);

					if (item._projectileManager.IsRegistered(item))
					{
						item._projectileManager.Unregister(item);
					}
				}
			}

			protected override void OnDespawned(ProjectileBehaviour item)
			{
				base.OnDespawned(item);

				var itemTransform = item.transform;
				itemTransform.position = Vector3.zero;
				itemTransform.rotation = Quaternion.identity;

				_cancellationToken?.Cancel();
				_cancellationToken = null;
			}
		}
	}
}