using DS.Utils.DataStructures;

namespace DS.UI.Base.Gui
{
	public abstract class ScreenView<TViewModel> : ScreenViewBase
		where TViewModel : ScreenViewModel, new()
	{
		protected TViewModel ViewModel;

		protected readonly DisposableList Disposables = new();

		public sealed override void Initialize(ScreenViewModel viewModel)
		{
			ViewModel = (TViewModel)viewModel;

			base.Initialize(viewModel);
		}

		protected override void Hide()
		{
			Disposables.Dispose();
		}
	}
}