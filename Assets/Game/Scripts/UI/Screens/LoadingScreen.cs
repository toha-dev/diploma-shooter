using System;
using DS.UI.Base.Gui;
using DS.UI.Base.ViewModels;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DS.UI.Screens
{
	public class LoadingScreen : ScreenView<LoadingScreenViewModel>
	{
		[field: SerializeField]
		private Slider LoadingProgressBar { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private TextMeshProUGUI LoadingProgressText { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private GameObject PressAnyKeyText { get; [UsedImplicitly] set; }

		[Inject]
		private UiInputActions _uiInputActions;

		protected override void Show()
		{
			PressAnyKeyText.SetActive(false);

			_uiInputActions.UI.AnyKey.performed += ViewModel.HandleAnyButtonClick;

			ViewModel.LoadingProgress.Skip(1).Subscribe(x =>
			{
				LoadingProgressBar.value = x;
				LoadingProgressText.text = $"{(int)(x * 100)}%";

				if (Math.Abs(x - 1) < 0.001)
				{
					PressAnyKeyText.SetActive(true);
					_uiInputActions.Enable();
				}
			}).AddTo(Disposables);
		}

		protected override void Hide()
		{
			base.Hide();

			_uiInputActions.UI.AnyKey.performed -= ViewModel.HandleAnyButtonClick;
			_uiInputActions.Disable();
		}
	}
}