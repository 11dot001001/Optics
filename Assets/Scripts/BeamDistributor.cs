using Assets.Scripts.Surfaces;
using Assets.Scripts.TrajectoryCalculators;
using Assets.Scripts.TrajectoryCalculators.Astractions;
using Assets.Scripts.TrajectoryCalculators.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.TrajectoryCalculators.TrajectoryCommonCalculator;

namespace Optics
{
	public static class PainterPovider
	{ 
		static public Painter Painter { get; set; }
	}

	public class BeamDistributor : MonoBehaviour
	{
		[SerializeField]
		private BeamRenderer _beamRenderer;
		[SerializeField]
		private Painter _painter;

		private IEnumerable<Lens> _lenses;
		private IEnumerable<Wall> _walls;
		private ITrajectoryCalculator _trajectoryCalculator;

		public void Start()
		{
			PainterPovider.Painter = _painter;
			_lenses = FindObjectsOfType<Lens>();

			List<SurfaceComponent> surfaceComponents = new List<SurfaceComponent>();
			surfaceComponents.AddRange(_lenses.Select(ConvertLensToSurfaceComponent));

			_trajectoryCalculator = new TrajectoryCommonCalculator(surfaceComponents);
		}

		public void Distribute(TrajectoryPoint startPoint)
		{
			IEnumerable<TrajectoryPoint> trajectoryPoints = _trajectoryCalculator.CalculateTrajectory(startPoint);
			_beamRenderer.RenderTrajectory(trajectoryPoints.Select(x => x.Position).ToList());
		}

		private SurfaceComponent ConvertLensToSurfaceComponent(Lens lens)
		{
			return new SurfaceComponent
			{
				Surface = lens,
				TrajectoryCalculator = new LensTrajectoryCalculator(lens)
			};
		}
	}
}