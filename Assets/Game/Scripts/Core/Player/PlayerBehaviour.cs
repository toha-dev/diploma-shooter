using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Weapons;
using DS.Utils.Attributes;
using JetBrains.Annotations;
using ModestTree;
using UnityEngine;

namespace DS.Core.Player
{
	public class PlayerBehaviour : MonoBehaviour
	{
		[field: SerializeField]
		private WeaponBehaviour PrimaryWeaponSlot { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private WeaponBehaviour SecondaryWeaponSlot { get; [UsedImplicitly] set; }

		[field: SerializeField, ReadOnly]
		private bool IsShooting { get; set; }

		private WeaponBehaviour _selectedWeapon;
		private CancellationTokenSource _cancellationToken;

		private void Awake()
		{
			_selectedWeapon = PrimaryWeaponSlot ? PrimaryWeaponSlot : SecondaryWeaponSlot;
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
					_selectedWeapon.TryShoot();
					await UniTask.Yield(PlayerLoopTiming.FixedUpdate, _cancellationToken.Token);
				}
			}
		}

		public void EndShooting()
		{
			IsShooting = false;
		}

		public void Reload()
		{
			_selectedWeapon.TryReload();
		}
	}
}