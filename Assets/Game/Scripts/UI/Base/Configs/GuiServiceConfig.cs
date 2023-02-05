using System;
using System.Collections.Generic;
using DS.UI.Base.Gui;
using DS.Utils.Attributes;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DS.UI.Base.Configs
{
	[CreateAssetMenu(menuName = "Configs/UI/GuiServiceConfig")]
	internal class GuiServiceConfig : ScriptableObject
	{
		[Serializable]
		internal struct ScreenAssetReference
		{
			[field: SerializeField]
			public ScreenType ScreenType { get; private set; }

			[field: SerializeField]
			public AssetReferenceGameObject AssetReference { get; private set; }

			internal ScreenAssetReference(
				ScreenType screenType,
				AssetReferenceGameObject assetReference)
			{
				ScreenType = screenType;
				AssetReference = assetReference;
			}
		}

		[field: SerializeField, ReadOnly]
		private string ScreensFolder { get; [UsedImplicitly] set; }

		[field: SerializeField, ReadOnly]
		internal List<ScreenAssetReference> ScreensAssets { get; [UsedImplicitly] private set; } = new();

		[field: SerializeField]
		internal bool PreloadAllScreens { get; [UsedImplicitly] private set; }

#if UNITY_EDITOR
		private void OnValidate()
		{
			ScreensAssets.Clear();

			var prefabGuids = AssetDatabase.FindAssets("t:prefab", new[] { ScreensFolder });

			foreach (var guid in prefabGuids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

				if (prefab.TryGetComponent<ScreenViewBase>(out var screenView))
				{
					ScreensAssets.Add(
						new ScreenAssetReference(
							screenView.ScreenType,
							new AssetReferenceGameObject(guid)));
				}
			}
		}
#endif
	}
}