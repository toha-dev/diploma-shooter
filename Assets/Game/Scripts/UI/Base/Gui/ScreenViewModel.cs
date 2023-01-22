using Zenject;

namespace DS.UI.Base.Gui
{
	public abstract class ScreenViewModel
	{
		[Inject]
		protected IGuiService GuiService;
	}
}