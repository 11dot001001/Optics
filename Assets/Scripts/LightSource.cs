using Assets.Scripts.Surfaces;
using Assets.Scripts.TrajectoryCalculators.Models;
using UnityEngine;

namespace Optics
{
	public class LightSource : MonoBehaviour
	{
		private Transform _transform;
		private BeamDistributor _beamDistributor;
		private Vector3 _initialLightDirection;
		[SerializeField]
		private Vector3 _direction  = Vector3.right;

		public Wall Wall;
		public LightRay LightRayPrefab;

		private void Start()
		{
			_transform = GetComponent<Transform>();
			_beamDistributor = FindObjectOfType<BeamDistributor>();
		}

		private void Update()
		{
			if (Wall.LastSelectedPoint == null)
				return;
			
			_initialLightDirection = (Wall.LastSelectedPoint.Position - _transform.position).normalized;

			_beamDistributor.Distribute(new TrajectoryPoint
			{
				Position = _transform.position,
				Direction = _direction //_initialLightDirection
			});

			//LightRay lightRay = Instantiate(LightRayPrefab);
			//lightRay.Initialize(_transform.position, _initialLightDirection);
			//Destroy(lightRay, 2);
		}
	}
}