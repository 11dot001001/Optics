using UnityEngine;

namespace Optics
{
	public class LightSource : MonoBehaviour
	{
		private Transform _transform;
		private Vector3 _initialLightDirection;

		public Wall Wall;
		public LightRay LightRayPrefab;

		private void Start()
		{
			_transform = GetComponent<Transform>();
		}

		private void Update()
		{
			if (Wall.LastSelectedPoint == null)
				return;
			
			_initialLightDirection = (Wall.LastSelectedPoint.Position - _transform.position).normalized;
			
			LightRay lightRay = Instantiate(LightRayPrefab);
			lightRay.Initialize(_transform.position, _initialLightDirection);
			Destroy(lightRay, 2);
		}
	}
}