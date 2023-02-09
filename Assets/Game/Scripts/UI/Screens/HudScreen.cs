using DS.UI.Base.Gui;
using DS.UI.Base.ViewModels;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UnityEngine;

namespace DS.UI.Screens
{
	public class HudScreen : ScreenView<HudViewModel>
	{
		[field: SerializeField]
		private TextMeshProUGUI AmmoCount { get; [UsedImplicitly] set; }

		protected override void Show()
		{
			ViewModel.MagazineAmmo.Subscribe(_ => UpdateAmmo()).AddTo(Disposables);
			ViewModel.AmmoLeft.Subscribe(_ => UpdateAmmo()).AddTo(Disposables);
		}

		private void UpdateAmmo()
		{
			AmmoCount.text = $"{ViewModel.MagazineAmmo.Value}/{ViewModel.AmmoLeft.Value}";
		}
	}
}