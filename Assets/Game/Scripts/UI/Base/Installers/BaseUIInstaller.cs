using DS.UI.Base.Gui;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace DS.UI.Base
{
	internal class BaseUIInstaller : MonoInstaller
	{
		[field: SerializeField]
		private GuiService GuiService { get; [UsedImplicitly] set; }

		public override void InstallBindings()
		{
			Container
				.Bind<IGuiService>()
				.To<GuiService>()
				.FromInstance(GuiService)
				.AsSingle()
				.NonLazy();

			Container.Bind<UiInputActions>().FromNew().AsSingle().NonLazy();
		}
	}
}