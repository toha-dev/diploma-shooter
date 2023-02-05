using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Weapons;
using DS.Utils.Attributes;
using JetBrains.Annotations;
using StarterAssets;
using UnityEngine;
using Zenject;

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

		public event Action EventPlayerSpawned;
		public event Action EventPlayerDespawned;
		public event Action EventPlayerDead;

		private WeaponBehaviour _selectedWeapon;
		private CancellationTokenSource _cancellationToken;

		private void Awake()
		{
			_selectedWeapon = PrimaryWeaponSlot ? PrimaryWeaponSlot : SecondaryWeaponSlot;
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

		public void StartShooting()
		{
			IsShooting = true;

			Shooting().Forget();

			async UniTaskVoid Shooting()
			{
				while (!_cancellationToken.IsCancellationRequested
						&& IsShooting)
				{
					_selectedWeapon.TryShootToPosition(GetShootPosition(), HandleRecoil);
					await UniTask.Yield(PlayerLoopTiming.FixedUpdate, _cancellationToken.Token);
				}
			}

			Vector3 GetShootPosition()
			{
				var cameraTransform = FirstPersonCamera.transform;

				if (!Physics.Raycast(
					cameraTransform.position,
					cameraTransform.forward,
					out var hit,
					Mathf.Infinity))
				{
					return cameraTransform.forward * 1000f;
				}

				Debug.DrawRay(
					cameraTransform.position,
					cameraTransform.forward * hit.distance,
					Color.red);

				return hit.point;
			}
		}

		private void HandleRecoil(Vector3 recoilOffset)
		{
			var xOffset = recoilOffset.x;
			var yOffset = recoilOffset.y;

			FirstPersonController.RotateX(-xOffset);
			FirstPersonCamera.transform.parent.transform.Rotate(new Vector3(-xOffset, 0, 0));
			gameObject.transform.Rotate(new Vector3(0, yOffset, 0));
		}

		public void EndShooting()
		{
			IsShooting = false;
		}

		public void Reload()
		{
			_selectedWeapon.TryReload();
		}

		public class Factory : PlaceholderFactory<PlayerEntity>
		{
		}
	}
}