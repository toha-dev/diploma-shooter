using JetBrains.Annotations;
using UnityEngine;

namespace DS.UI.Base.Gui
{
	public abstract class ScreenViewBase : MonoBehaviour
	{
		[field: SerializeField]
		internal ScreenType ScreenType { get; [UsedImplicitly] set; }

		public virtual void Initialize(ScreenViewModel viewModel)
		{
			Show();
		}

		private void OnDestroy()
		{
			Hide();
		}

		protected virtual void Show()
		{
		}

		protected virtual void Hide()
		{
		}
	}
}