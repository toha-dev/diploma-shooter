using System;
using Cysharp.Threading.Tasks;

namespace DS.UI.Base.Gui
{
	public enum ScreenType
	{
		None = 0,
		MainMenu = 1,
		Loading = 2,
		HudScreen = 3,
	}

	public enum GuiLayer
	{
		Stack = 0,
		Overlay = 1,
	}

	public interface IGuiService
	{
		UniTask<T> ShowScreenAsync<T>(
			ScreenType screenType,
			GuiLayer guiLayer,
			Action<T> callback = null)
			where T : ScreenViewModel;

		void HideScreen(ScreenType screenType, GuiLayer guiLayer);
	}
}