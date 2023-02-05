using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;
using DS.UI.Base.Configs;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace DS.UI.Base.Gui
{
	[UsedImplicitly]
	internal class GuiService : MonoBehaviour, IGuiService
	{
		[Serializable]
		private struct LayerData
		{
			[field: SerializeField]
			public GuiLayer GuiLayer { get; private set; }

			[field: SerializeField]
			public Transform Parent { get; private set; }
		}

		[field: SerializeField]
		private GuiServiceConfig Config { get; [UsedImplicitly] set; }

		[field: SerializeField][SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
		private List<LayerData> Layers { get; [UsedImplicitly] set; }

		[Inject]
		private DiContainer _container;

		private readonly Dictionary<ScreenType, AssetReferenceGameObject> _screens = new();
		private readonly Dictionary<GuiLayer, Transform> _layers = new();

		private readonly Dictionary<AssetReferenceGameObject, (GameObject prefab, int count)> _loadedScreens = new();

		private readonly Dictionary<GuiLayer, LayerHandler> _layerHandlers = new()
		{
			{ GuiLayer.Overlay, new CommonLayerHandler() },
			{ GuiLayer.Stack, new StackLayerHandler() },
		};

		private readonly List<ScreenType> _inProgress = new();
		private readonly List<ScreenType> _hideRequested = new();

		private bool _waitingPreload;

		private async void Awake()
		{
			foreach (var screen in Config.ScreensAssets)
			{
				_screens.Add(screen.ScreenType, screen.AssetReference);
			}

			foreach (var layer in Layers)
			{
				_layers.Add(layer.GuiLayer, layer.Parent);
			}

			if (!Config.PreloadAllScreens)
			{
				return;
			}

			_waitingPreload = true;

			foreach (var screen in Config.ScreensAssets)
			{
				var asyncOperationHandle = screen.AssetReference.LoadAssetAsync<GameObject>();

				await asyncOperationHandle;

				if (asyncOperationHandle.Status != AsyncOperationStatus.Succeeded)
				{
					throw new ArgumentException($"Can't preload a screen {screen.ScreenType}!");
				}

				_loadedScreens[screen.AssetReference] = (asyncOperationHandle.Result, 1);
			}

			_waitingPreload = false;
		}

		public async UniTask<T> ShowScreenAsync<T>(
			ScreenType screenType,
			GuiLayer guiLayer,
			Action<T> callback = null)
			where T : ScreenViewModel
		{
			if (_waitingPreload)
			{
				await UniTask.WaitWhile(() => _waitingPreload);
			}

			_inProgress.Add(screenType);

			var screenToLoad = _screens[screenType];
			var layerParent = _layers[guiLayer];
			GameObject loadedPrefab;

			if (!_loadedScreens.ContainsKey(screenToLoad))
			{
				var asyncOperationHandle = screenToLoad.LoadAssetAsync<GameObject>();

				await asyncOperationHandle;

				if (asyncOperationHandle.Status != AsyncOperationStatus.Succeeded)
				{
					throw new ArgumentException($"Can't load a screen {screenType}!");
				}

				loadedPrefab = asyncOperationHandle.Result;
				_loadedScreens[screenToLoad] = (loadedPrefab, 1);
			}
			else
			{
				var (prefab, count) = _loadedScreens[screenToLoad];
				loadedPrefab = prefab;

				_loadedScreens[screenToLoad] = (prefab, count + 1);
			}

			var viewModel = _container.Instantiate<T>();

			callback?.Invoke(viewModel);

			var screenPrefab = _container.InstantiatePrefab(
				loadedPrefab,
				layerParent);

			if (screenPrefab == null)
			{
				throw new ArgumentException($"Can't instantiate a screen {screenType}!");
			}

			if (!screenPrefab.TryGetComponent<ScreenViewBase>(out var screenView))
			{
				throw new ArgumentException($"Can't get view component on screen {screenType}!");
			}

			screenView.Initialize(viewModel);

			_layerHandlers[guiLayer].HandleShow(screenView);

			try
			{
				return viewModel;
			}
			finally
			{
				_inProgress.Remove(screenType);

				if (_hideRequested.Contains(screenType))
				{
					HideScreen(screenType, guiLayer);
				}
			}
		}

		public void HideScreen(ScreenType screenType, GuiLayer guiLayer)
		{
			var layerHandler = _layerHandlers[guiLayer];
			var screens = layerHandler.FindScreensToHide(screenType).ToArray();

			if (!screens.Any() && _inProgress.Contains(screenType))
			{
				_hideRequested.Add(screenType);
			}

			foreach (var screen in screens)
			{
				layerHandler.HandleHide(screen);
				Destroy(screen.gameObject);

				var (key, (prefab, count)) = _loadedScreens.FirstOrDefault(x => x.Value.prefab == screen.gameObject);

				if (prefab == null)
				{
					continue;
				}

				var nextItemCount = count - 1;
				_loadedScreens[key] = (prefab, nextItemCount);

				if (nextItemCount == 0)
				{
					Addressables.Release(key);
				}
			}
		}
	}
}