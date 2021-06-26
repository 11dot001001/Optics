using System;
using UnityEngine;

namespace Optics
{
	public class Wall : MonoBehaviour 
	{
		private Transform _transform;

		public Transform Transform => _transform;
		
		private void Start()
		{
			_transform = GetComponent<Transform>();
		}
	}
}