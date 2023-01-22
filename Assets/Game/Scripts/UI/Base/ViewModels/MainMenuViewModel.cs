using System;
using DS.UI.Base.Gui;

namespace DS.UI.Base.ViewModels
{
	public class MainMenuViewModel : ScreenViewModel
	{
		private Action _onPlayClicked;
		private Action _onExitClicked;

		public void SetData(Action onPlayClicked, Action onExitClicked)
		{
			_onPlayClicked = onPlayClicked;
			_onExitClicked = onExitClicked;
		}
		
		public void HandlePlayClicked()
		{
			_onPlayClicked?.Invoke();
		}

		public void HandleExitClicked()
		{
			_onExitClicked?.Invoke();
		}
	}
}