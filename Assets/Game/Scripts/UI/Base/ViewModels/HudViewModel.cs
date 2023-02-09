using DS.Core.Player;
using DS.UI.Base.Gui;
using DS.Utils.DataStructures;
using UniRx;

namespace DS.UI.Base.ViewModels
{
	public class HudViewModel : ScreenViewModel
	{
		public IReadOnlyReactiveProperty<int> MagazineAmmo => _magazineAmmo;
		public IReadOnlyReactiveProperty<int> AmmoLeft => _ammoLeft;

		private readonly ReactiveProperty<int> _magazineAmmo = new();
		private readonly ReactiveProperty<int> _ammoLeft = new();

		public void SetData(IPlayerEntity playerEntity)
		{
			var localDisposables = new DisposableList();

			playerEntity.SelectedWeapon.Subscribe(x =>
			{
				DisposeLocal();

				x.MagazineAmmo.Subscribe(y =>
				{
					_magazineAmmo.Value = y;
				}).AddTo(localDisposables);

				x.AmmoLeft.Subscribe(y =>
				{
					_ammoLeft.Value = y;
				}).AddTo(localDisposables);
			}).AddTo(Disposables);

			void DisposeLocal()
			{
				localDisposables.Dispose();
			}
		}
	}
}