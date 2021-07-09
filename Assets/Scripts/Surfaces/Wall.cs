using Assets.Scripts.Surfaces.Abstractions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Surfaces
{
	public class Wall : MonoBehaviour, ISurface
	{
		public class WallPoint
		{
			public Vector3 Position { get; set; }
			public Vector3 Normal { get; set; }
		}
		
		private BoxCollider _boxCollider;

		public WallPoint LastSelectedPoint { get; private set; }

		public float OpticalDensity => float.MaxValue;

		public IEnumerable<Collider> Colliders => new Collider[] { _boxCollider };

		private void OnMouseDown()
		{
			Camera camera = Camera.main;
			Vector3 globalMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

			_boxCollider.Raycast(new Ray(globalMousePosition, camera.transform.forward), out RaycastHit hit, float.MaxValue);
			LastSelectedPoint = new WallPoint
			{
				Position = hit.point,
				Normal = hit.normal
			};
		}

		private void Start()
		{
			_boxCollider = GetComponent<BoxCollider>();
			LastSelectedPoint = default;
		}
	}
}