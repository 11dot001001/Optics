using Assets.Scripts.TrajectoryCalculators.Models;
using UnityEngine;

namespace Optics
{
	public class LightSource : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _direction = Vector3.right;

		private Transform _transform;
		private BeamDistributor _beamDistributor;

		private void Start()
		{
			_transform = GetComponent<Transform>();
			_beamDistributor = FindObjectOfType<BeamDistributor>();
		}

		private void Update()
		{
			_beamDistributor.Distribute(new TrajectoryPoint
			{
				Position = _transform.position,
				Direction = _direction
			});
		}
	}
}