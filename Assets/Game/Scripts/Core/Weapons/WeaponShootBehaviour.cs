using JetBrains.Annotations;
using UnityEngine;

namespace DS.Core.Weapons
{
	[RequireComponent(typeof(Animator))]
	public class WeaponShootBehaviour : MonoBehaviour
	{
		[field: Header("Instantiate Positions")]
		[field: SerializeField]
		private Transform BarrelLocation { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private Transform EjectionLocation { get; [UsedImplicitly] set; }

		[field: Header("Prefabs")]
		[field: SerializeField]
		private GameObject MuzzleFlash { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private GameObject BulletCasing { get; [UsedImplicitly] set; }

		[field: Header("Settings")]
		[field: SerializeField]
		private float EjectCasingForce { get; [UsedImplicitly] set; }

		[UsedImplicitly]
		public void Shoot()
		{
			Instantiate(
				MuzzleFlash,
				BarrelLocation.position,
				BarrelLocation.rotation,
				BarrelLocation);

			Debug.LogError($"Shot event");
		}

		[UsedImplicitly]
		public void CasingRelease()
		{
			var ejectionPosition = EjectionLocation.position;

			var instance = Instantiate(
				BulletCasing,
				ejectionPosition,
				EjectionLocation.rotation);

			instance.GetComponent<Rigidbody>().AddExplosionForce(
				Random.Range(EjectCasingForce * 0.7f, EjectCasingForce),
				(ejectionPosition - EjectionLocation.right * 0.3f - EjectionLocation.up * 0.6f), 1f);

			instance.GetComponent<Rigidbody>().AddTorque(
				new Vector3(
					0,
					Random.Range(100f, 500f),
					Random.Range(100f, 1000f)),
				ForceMode.Impulse);

			Debug.LogError($"Casing release event");
		}
	}
}