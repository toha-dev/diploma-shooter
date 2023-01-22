using System.Collections.Generic;
using System.Linq;

namespace DS.UI.Base.Gui
{
	internal sealed class CommonLayerHandler : LayerHandler
	{
		private readonly List<ScreenViewBase> _screens = new();
		
		internal override IEnumerable<ScreenViewBase> FindScreensToHide(
			ScreenType screenType)
		{
			return new [] { _screens.Last(x => x.ScreenType == screenType) };
		}

		internal override void HandleShow(ScreenViewBase screenView)
		{
			_screens.Add(screenView);
		}

		internal override void HandleHide(ScreenViewBase screenView)
		{
			_screens.Remove(screenView);
		}
	}
}