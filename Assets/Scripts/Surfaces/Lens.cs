using Assets.Scripts.Surfaces.Abstractions;
using UnityEngine;

namespace Assets.Scripts.Surfaces
{
	public class Lens : MonoBehaviour, ISurface
	{
		[SerializeField]
		private float _opticalDensity = OpticalDensities.Glass;
		[SerializeField]
		private Collider _collider;

		public float OpticalDensity => _opticalDensity;
		public Collider Collider => _collider;
	}
}
