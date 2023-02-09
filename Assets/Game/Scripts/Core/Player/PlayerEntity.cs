using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Weapons;
using DS.Utils.Attributes;
using JetBrains.Annotations;
using StarterAssets;
using UniRx;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace DS.Core.Player
{
	public class PlayerEntity : MonoBehaviour, IPlayerEntity
	{
		[field: SerializeField]
		private FirstPersonController FirstPersonController { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private Camera FirstPersonCamera { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private WeaponBehaviour PrimaryWeaponSlot { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private WeaponBehaviour SecondaryWeaponSlot { get; [UsedImplicitly] set; }

		[field: SerializeField, ReadOnly]
		private bool IsShooting { get; set; }

		public GameObject GameObject => gameObject;

		public IReadOnlyReactiveProperty<WeaponBehaviour> SelectedWeapon => _selectedWeapon;

		public event Action EventPlayerSpawned;
		public event Action EventPlayerDespawned;
		public event Action EventPlayerDead;

		private readonly ReactiveProperty<WeaponBehaviour> _selectedWeapon = new();
		private CancellationTokenSource _cancellationToken;

		private readonly RecoilSmoother _recoilSmoother = new();

		private void Awake()
		{
			_selectedWeapon.Value = PrimaryWeaponSlot ? PrimaryWeaponSlot : SecondaryWeaponSlot;
			EventPlayerSpawned?.Invoke();
		}

		private void OnDestroy()
		{
			EventPlayerDespawned?.Invoke();
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

		private void LateUpdate()
		{
			if (!_recoilSmoother.IsRecoilActive)
			{
				return;
			}

			_recoilSmoother.CalculateStep();

			FirstPersonController.RotateX(_recoilSmoother.RotationAngle.y);
			transform.Rotate(new Vector3(0, _recoilSmoother.RotationAngle.x, 0));
		}

		public void StartShooting()
		{
			IsShooting = true;

			Shooting().Forget();

			async UniTaskVoid Shooting()
			{
				while (!_cancellationToken.IsCancellationRequested
						&& IsShooting)
				{
					_selectedWeapon.Value.TryShootToPosition(GetShootPosition(), HandleRecoil);
					await UniTask.Yield(PlayerLoopTiming.FixedUpdate, _cancellationToken.Token);
				}
			}

			Vector3 GetShootPosition()
			{
				var cameraTransform = FirstPersonCamera.transform;

				if (!Physics.Raycast(
					cameraTransform.position,
					GetAccuracy(),
					out var hit,
					Mathf.Infinity))
				{
					return cameraTransform.forward * 10f;
				}

				Debug.DrawRay(
					cameraTransform.position,
					cameraTransform.forward * hit.distance,
					Color.red);

				return hit.point;

				Vector3 GetAccuracy()
				{
					var accuracy = Vector3.one;

					accuracy.x *= UnityEngine.Random.Range(-_selectedWeapon.Value.Config.Accuracy, _selectedWeapon.Value.Config.Accuracy);
					accuracy.y *= UnityEngine.Random.Range(-_selectedWeapon.Value.Config.Accuracy, _selectedWeapon.Value.Config.Accuracy);
					accuracy.z *= UnityEngine.Random.Range(-_selectedWeapon.Value.Config.Accuracy, _selectedWeapon.Value.Config.Accuracy);

					Debug.LogWarning(accuracy);
					
					return Vector3.Normalize(cameraTransform.forward + accuracy);
				}
			}
		}

		private void HandleRecoil(Vector2 recoilOffset)
		{
			_recoilSmoother.SetRotation(
				recoilOffset,
				_selectedWeapon.Value.Config.RecoilSmoothness);
		}

		public void EndShooting()
		{
			IsShooting = false;
		}

		public void Reload()
		{
			_selectedWeapon.Value.TryReload();
		}

		private class RecoilSmoother
		{
			public bool IsRecoilActive => _rotation.x > 0.01f || _rotation.y > 0.01f;

			public Vector2 RotationAngle { get; private set; }

			private Vector2 _rotation;
			private float _speed;

			public void SetRotation(Vector2 rotation, float speed)
			{
				_rotation = rotation;
				_speed = speed;
			}

			public void CalculateStep()
			{
				if (!IsRecoilActive)
				{
					return;
				}

				RotationAngle = Vector2.Lerp(
					Vector2.zero,
					_rotation,
					_speed * Time.deltaTime);

				_rotation -= RotationAngle;

				//CinemachineCameraTarget.transform.Rotate(-target);
				//_cinemachineTargetPitch -= target.x;
				//Debug.LogWarning($"TARGET ROTATION OFFSET {RotationAngle}");
			}
		}

		public class Factory : PlaceholderFactory<PlayerEntity>
		{
		}
	}
}