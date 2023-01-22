using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DS.Simulation
{
	public class Launcher : MonoBehaviour
	{
		private void Start()
		{
			LoadMenuScene().Forget();
		}

		private static async UniTaskVoid LoadMenuScene()
		{
			await SceneLoader.LoadSceneAsync(
				SceneLoader.MainMenuSceneName,
				LoadSceneMode.Single,
				showLoadingScreen: false);
		}
	}
}