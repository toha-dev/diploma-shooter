using UnityEngine;

namespace DS.Core
{
	public class FpsLimiter : MonoBehaviour
	{
		private void Start()
		{
			Application.targetFrameRate = 100;
		}
	}
}
