using DS.Utils.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace DS.Core.Weapons
{
	[CreateAssetMenu(menuName = "Configs/Weapon/WeaponConfig")]
	public class WeaponConfig : ScriptableObject
	{
		[field: SerializeField]
		public int MaximumAmmoCount { get; [UsedImplicitly] private set; }

		[field: SerializeField]
		public int MagazineAmmoCount { get; [UsedImplicitly] private set; }

		[field: SerializeField]
		private float Damage { get; [UsedImplicitly] set; }

		[field: SerializeField]
		public float InitialProjectileForce { get; [UsedImplicitly] private set; }

		[field: SerializeField, ReadOnly]
		private float MaxDamageDistance { get; [UsedImplicitly] set; } = 1000f;

		[field: SerializeField, ReadOnly]
		private float MinDamageDistance { get; [UsedImplicitly] set; } = 0f;

		[field: SerializeField, ReadOnly]
		private float DistanceToCurveTimeMultiplier { get; [UsedImplicitly] set; } = 1 / 1000f;

		[field: SerializeField]
		[field: Tooltip("How weapon damage will decrease with distance. 0.001 = 1 meter")]
		private AnimationCurve DamageThroughDistance { get; [UsedImplicitly] set; }

		[field: SerializeField]
		[field: Tooltip("Fire rate per minute")]
		private int FireRate { get; [UsedImplicitly] set; }

		[field: SerializeField]
		[field: Tooltip("Reload time in seconds")]
		public float ReloadTime { get; [UsedImplicitly] private set; }

		public double FireDelay => (60d / FireRate);

		public float CalculateDamage(float distanceInMeters)
		{
			distanceInMeters = Mathf.Clamp(
				distanceInMeters,
				MinDamageDistance,
				MaxDamageDistance);

			var curveTime = distanceInMeters * DistanceToCurveTimeMultiplier;
			return Damage * DamageThroughDistance.Evaluate(curveTime);
		}
	}
}