using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace DS.Core.Player
{
	[RequireComponent(typeof(PlayerBehaviour))]
	public class PlayerController : MonoBehaviour
	{
		[field: SerializeField, ReadOnly]
		private PlayerBehaviour PlayerBehaviour { get; [UsedImplicitly] set; }

		[Inject]
		private PlayerInputActions _playerInputActions;

		private void OnValidate()
		{
			PlayerBehaviour = GetComponent<PlayerBehaviour>();
		}

		private void OnEnable()
		{
			_playerInputActions.Enable();

			_playerInputActions.Weapon.Shoot.started += HandlePlayerShootStart;
			_playerInputActions.Weapon.Shoot.canceled += HandlePlayerShootEnd;
			_playerInputActions.Weapon.Reload.started += HandlePlayerReload;
		}

		private void OnDisable()
		{
			_playerInputActions.Weapon.Shoot.started -= HandlePlayerShootStart;
			_playerInputActions.Weapon.Shoot.canceled -= HandlePlayerShootEnd;
			_playerInputActions.Weapon.Reload.started -= HandlePlayerReload;

			_playerInputActions.Disable();
		}

		private void HandlePlayerShootStart(InputAction.CallbackContext obj)
		{
			PlayerBehaviour.StartShooting();
		}

		private void HandlePlayerShootEnd(InputAction.CallbackContext obj)
		{
			PlayerBehaviour.EndShooting();
		}

		private void HandlePlayerReload(InputAction.CallbackContext obj)
		{
			PlayerBehaviour.Reload();
		}
	}
}