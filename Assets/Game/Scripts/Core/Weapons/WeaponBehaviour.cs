using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Projectiles;
using DS.Utils.Attributes;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Zenject;

namespace DS.Core.Weapons
{
	public class WeaponBehaviour : MonoBehaviour
	{
		[field: SerializeField]
		public WeaponConfig Config { get; [UsedImplicitly] private set; }

		[field: SerializeField]
		private Animator Animator { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private Transform BarrelLocation { get; [UsedImplicitly] set; }

		[field: SerializeField, ReadOnly]
		private double LastShootTime { get; set; } = double.MinValue;

		[field: SerializeField, ReadOnly]
		private bool IsReloading { get; [UsedImplicitly] set; }

		[Inject]
		private IProjectileManager _projectileManager;

		public IReadOnlyReactiveProperty<int> MagazineAmmo => _magazineAmmo;
		public IReadOnlyReactiveProperty<int> AmmoLeft => _ammoLeft;

		private readonly ReactiveProperty<int> _magazineAmmo = new();
		private readonly ReactiveProperty<int> _ammoLeft = new();

		private int _shootCount;

		private CancellationTokenSource _cancellationToken;

		private void Awake()
		{
			_magazineAmmo.Value = Config.MagazineAmmoCount;
			_ammoLeft.Value = Config.MaximumAmmoCount - Config.MagazineAmmoCount;
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

		public bool TryShootToPosition(Vector3 shootPosition, Action<Vector2> recoilCallback = null)
		{
			var nextShootTime = LastShootTime + Config.FireDelay;

			if (Time.realtimeSinceStartupAsDouble < nextShootTime
				|| _magazineAmmo.Value == 0
				|| IsReloading)
			{
				return false;
			}

			var recoilReset = LastShootTime + Config.RecoilCooldown;

			if (Time.realtimeSinceStartupAsDouble > recoilReset)
			{
				_shootCount = 0;
			}

			ShootToPosition(shootPosition);
			recoilCallback?.Invoke(Config.GetRecoilOffset(_shootCount++));
			return true;
		}

		public bool TryReload()
		{
			if (_magazineAmmo.Value == Config.MagazineAmmoCount
				|| _ammoLeft.Value == 0
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

			var needToReload = Math.Min(Config.MagazineAmmoCount - _magazineAmmo.Value, _ammoLeft.Value);

			await UniTask.Delay(
				(int)(Config.ReloadTime * 1000),
				DelayType.DeltaTime,
				PlayerLoopTiming.FixedUpdate,
				_cancellationToken.Token);

			_ammoLeft.Value -= needToReload;
			_magazineAmmo.Value += needToReload;

			Debug.LogError($"RELOAD END {_magazineAmmo}/{_ammoLeft}");
			IsReloading = false;
		}

		private void ShootToPosition(Vector3 shootPosition)
		{
			_magazineAmmo.Value--;
			LastShootTime = Time.realtimeSinceStartupAsDouble;

			var position = BarrelLocation.position;
			var projectilePosition = position;
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

			Animator.SetTrigger("Fire");
		}
	}
}