using System.Collections.Generic;

namespace DS.UI.Base.Gui
{
	internal abstract class LayerHandler
	{
		internal abstract IEnumerable<ScreenViewBase> FindScreensToHide(ScreenType screenType);

		internal abstract void HandleShow(ScreenViewBase screenView);
		internal abstract void HandleHide(ScreenViewBase screenView);
	}
}