using UnityEngine;

namespace Optics
{
	public class Wall : MonoBehaviour
	{
		public class WallPoint
		{
			public Vector3 Position { get; set; }
			public Vector3 Normal { get; set; }
		}
		
		private BoxCollider _boxCollider;

		public WallPoint LastSelectedPoint { get; private set; }
		public BoxCollider BoxCollider => _boxCollider;

		private void OnMouseDown()
		{
			Camera camera = Camera.main;
			Vector3 globalMousePosition = camera.ScreenToWorldPoint(Input.mousePosition);

			_boxCollider.Raycast(new Ray(globalMousePosition, camera.transform.forward), out RaycastHit hit, float.MaxValue);
			LastSelectedPoint =  new WallPoint
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