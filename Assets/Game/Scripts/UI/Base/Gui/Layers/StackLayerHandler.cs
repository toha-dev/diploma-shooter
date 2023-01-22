using System.Collections.Generic;
using System.Linq;

namespace DS.UI.Base.Gui
{
	internal sealed class StackLayerHandler : LayerHandler
	{
		private readonly Stack<ScreenViewBase> _screens = new();

		internal override IEnumerable<ScreenViewBase> FindScreensToHide(
			ScreenType screenType)
		{
			var result = new List<ScreenViewBase>();

			foreach (var screenView in _screens)
			{
				result.Add(screenView);

				if (screenView.ScreenType == screenType)
				{
					return result;
				}
			}

			return Enumerable.Empty<ScreenViewBase>();
		}

		internal override void HandleShow(ScreenViewBase screenView)
		{
			if (_screens.Any())
			{
				var screen = _screens.Peek();
				screen.gameObject.SetActive(false);
			}

			_screens.Push(screenView);
		}

		internal override void HandleHide(ScreenViewBase screenView)
		{
			_screens.Pop();

			if (_screens.Any())
			{
				_screens.Peek().gameObject.SetActive(true);
			}
		}
	}
}