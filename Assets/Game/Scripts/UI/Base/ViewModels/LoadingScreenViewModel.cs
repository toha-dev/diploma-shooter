using System;
using DS.UI.Base.Gui;
using UniRx;
using UnityEngine.InputSystem;

namespace DS.UI.Base.ViewModels
{
	public class LoadingScreenViewModel : ScreenViewModel
	{
		public IReadOnlyReactiveProperty<float> LoadingProgress { get; private set; }
		private Action _callback;

		public void SetData(
			IReadOnlyReactiveProperty<float> loadingProgress,
			Action callback)
		{
			LoadingProgress = loadingProgress;
			_callback = callback;
		}

		public void HandleAnyButtonClick(InputAction.CallbackContext _)
		{
			_callback?.Invoke();
		}
	}
}