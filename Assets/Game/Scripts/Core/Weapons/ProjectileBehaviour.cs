using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DS.Core.Weapons
{
	public class ProjectileBehaviour : MonoBehaviour
	{
		[field: SerializeField]
		public Rigidbody Rigidbody { get; [UsedImplicitly] private set; }

		private void Update()
		{
			Debug.LogError($"SPEED {Rigidbody.velocity}");
		}

		private void OnCollisionEnter(Collision other)
		{
			Debug.LogError($"BULLET COLLISION");
		}
	}
}