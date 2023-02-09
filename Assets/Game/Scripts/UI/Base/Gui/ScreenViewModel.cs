using System;
using DS.Utils.DataStructures;
using Zenject;

namespace DS.UI.Base.Gui
{
	public abstract class ScreenViewModel : IDisposable
	{
		[Inject]
		protected IGuiService GuiService;

		protected readonly DisposableList Disposables = new();

		public virtual void Dispose()
		{
			Disposables.Dispose();
		}
	}
}