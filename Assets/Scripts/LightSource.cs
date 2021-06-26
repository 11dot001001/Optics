using UnityEngine;

namespace Optics
{
	public class LightSource : MonoBehaviour
	{
		private Transform _transform;

		public Transform Transform => _transform;
		
		private void Start()
		{
			_transform = GetComponent<Transform>();
		}
	}
}