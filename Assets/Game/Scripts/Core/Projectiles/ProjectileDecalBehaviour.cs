using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace DS.Core.Projectiles
{
	public class ProjectileDecalBehaviour : MonoBehaviour
	{
		[UsedImplicitly]
		public class DecalPool : MonoMemoryPool<ProjectileDecalBehaviour>
		{
		}
	}
}