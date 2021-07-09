using Assets.Scripts.Surfaces.Abstractions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Surfaces
{
	public class Lens : MonoBehaviour, ISurface
	{
		[SerializeField]
		private float _opticalDensity = OpticalDensities.Glass;
		[SerializeField]
		private Collider[] _colliders;

		public float OpticalDensity => _opticalDensity;
		public IEnumerable<Collider> Colliders => _colliders;
	}
}
