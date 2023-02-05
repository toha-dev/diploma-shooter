using Cysharp.Threading.Tasks;
using DS.Core.Player;
using DS.UI.Base.Gui;
using DS.UI.Base.ViewModels;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace DS.Simulation
{
	public class PlayerController : IPlayerController
	{
		[Inject]
		private PlayerEntity.Factory _playerFactory;

		[Inject]
		private PlayerInputActions _playerInputActions;

		[Inject]
		private IGuiService _guiService;

		public IPlayerEntity Player { get; private set; }

		private void HandlePlayerShootStart(InputAction.CallbackContext obj)
		{
			Player.StartShooting();
		}

		private void HandlePlayerShootEnd(InputAction.CallbackContext obj)
		{
			Player.EndShooting();
		}

		private void HandlePlayerReload(InputAction.CallbackContext obj)
		{
			Player.Reload();
		}

		public async void SpawnPlayer()
		{
			Player = _playerFactory.Create();

			_playerInputActions.Weapon.Shoot.started += HandlePlayerShootStart;
			_playerInputActions.Weapon.Shoot.canceled += HandlePlayerShootEnd;
			_playerInputActions.Weapon.Reload.started += HandlePlayerReload;

			await UniTask.WaitWhile(() => Application.State.Value != ApplicationState.Running);

			_playerInputActions.Enable();
			_guiService.ShowScreenAsync<HudViewModel>(ScreenType.HudScreen, GuiLayer.Stack);
		}

		public void DestroyPlayer()
		{
			Object.Destroy(Player.GameObject);
			Player = null;

			_playerInputActions.Disable();

			_playerInputActions.Weapon.Shoot.started -= HandlePlayerShootStart;
			_playerInputActions.Weapon.Shoot.canceled -= HandlePlayerShootEnd;
			_playerInputActions.Weapon.Reload.started -= HandlePlayerReload;
		}
	}
}