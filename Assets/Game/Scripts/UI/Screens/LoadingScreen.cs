using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DS.UI.Screens
{
	public class LoadingScreen : MonoBehaviour
	{
		[field: SerializeField]
		private Slider LoadingProgressBar { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private TextMeshProUGUI LoadingProgressText { get; [UsedImplicitly] set; }

		private void Update()
		{
			LoadingProgressBar.value = SceneLoader.LoadingProgress;
			LoadingProgressText.text = $"{(int)(SceneLoader.LoadingProgress * 100)}%";

			Debug.LogError($"Loading progress {SceneLoader.LoadingProgress}");
		}
	}
}