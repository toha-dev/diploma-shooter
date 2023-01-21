using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Projectiles;
using DS.Utils.Attributes;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace DS.Core.Weapons
{
	public class WeaponBehaviour : MonoBehaviour
	{
		[field: SerializeField]
		private WeaponConfig Config { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private Transform ProjectileSpawnPoint { get; [UsedImplicitly] set; }

		[field: SerializeField, ReadOnly]
		private double LastShootTime { get; set; } = double.MinValue;

		[field: SerializeField, ReadOnly]
		private bool IsReloading { get; [UsedImplicitly] set; }

		[Inject]
		private IProjectileManager _projectileManager;

		private int _magazineAmmo;
		private int _ammoLeft;

		private CancellationTokenSource _cancellationToken;

		private void Awake()
		{
			_magazineAmmo = Config.MagazineAmmoCount;
			_ammoLeft = Config.MaximumAmmoCount - _magazineAmmo;
		}

		private void OnEnable()
		{
			_cancellationToken = new CancellationTokenSource();
		}

		private void OnDisable()
		{
			_cancellationToken.Cancel();
			_cancellationToken = null;
		}

		public bool TryShootToPosition(Vector3 shootPosition)
		{
			var nextShootTime = LastShootTime + Config.FireDelay;

			if (Time.realtimeSinceStartupAsDouble < nextShootTime
				|| _magazineAmmo == 0
				|| IsReloading)
			{
				return false;
			}

			ShootToPosition(shootPosition);
			return true;

		}

		public bool TryReload()
		{
			if (_magazineAmmo == Config.MagazineAmmoCount
				|| _ammoLeft == 0
				|| IsReloading)
			{
				return false;
			}

			Reload().Forget();
			return true;
		}

		private async UniTaskVoid Reload()
		{
			IsReloading = true;
			Debug.LogError("RELOAD STARTED");

			var needToReload = Math.Min(Config.MagazineAmmoCount - _magazineAmmo, _ammoLeft);

			await UniTask.Delay(
				(int)(Config.ReloadTime * 1000),
				DelayType.DeltaTime,
				PlayerLoopTiming.FixedUpdate,
				_cancellationToken.Token);

			_ammoLeft -= needToReload;
			_magazineAmmo += needToReload;

			Debug.LogError($"RELOAD END {_magazineAmmo}/{_ammoLeft}");
			IsReloading = false;
		}

		private void ShootToPosition(Vector3 shootPosition)
		{
			_magazineAmmo--;
			LastShootTime = Time.realtimeSinceStartupAsDouble;

			var projectilePosition = ProjectileSpawnPoint.position;
			var targetDirection = shootPosition - projectilePosition;

			var newDirection = Vector3.RotateTowards(
				transform.forward,
				targetDirection,
				float.MaxValue,
				float.MaxValue);

			var rotation = Quaternion.LookRotation(newDirection);

			_projectileManager.RegisterShot(
				projectilePosition,
				rotation,
				Config.InitialProjectileForce);
		}
	}
}