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

		public bool TryShoot()
		{
			var nextShootTime = LastShootTime + Config.FireDelay;

			if (Time.realtimeSinceStartupAsDouble < nextShootTime
				|| _magazineAmmo == 0
				|| IsReloading)
			{
				return false;
			}

			Shoot();
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

		private void Shoot()
		{
			_magazineAmmo--;
			LastShootTime = Time.realtimeSinceStartupAsDouble;

			_projectileManager.RegisterShot(ProjectileSpawnPoint.position, ProjectileSpawnPoint.rotation, Config.InitialProjectileForce);
			//var projectile = Instantiate(Config.Projectile, ProjectileSpawnPoint.position, ProjectileSpawnPoint.rotation);
			//projectile.Rigidbody.AddForce(projectile.transform.forward * Config.InitialProjectileForce);

			//Debug.LogError($"SHOOT {_magazineAmmo}/{_ammoLeft}");
		}
	}
}