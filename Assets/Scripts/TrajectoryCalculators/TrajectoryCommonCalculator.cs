using Assets.Scripts.Surfaces.Abstractions;
using Assets.Scripts.TrajectoryCalculators.Astractions;
using Assets.Scripts.TrajectoryCalculators.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.TrajectoryCalculators
{
	public class TrajectoryCommonCalculator : ITrajectoryCalculator
	{
		public class SurfaceComponent
		{
			public ISurface Surface { get; set; }
			public ISurfaceTrajectoryCalculator TrajectoryCalculator { get; set; }
		}

		private readonly IEnumerable<SurfaceComponent> _surfaceComponents;

		public TrajectoryCommonCalculator(IEnumerable<SurfaceComponent> surfaceComponents)
		{
			_surfaceComponents = surfaceComponents ?? throw new ArgumentNullException(nameof(surfaceComponents));
		}

		public IEnumerable<TrajectoryPoint> CalculateTrajectory(TrajectoryPoint originPoint)
		{
			List<TrajectoryPoint> trajectory = new List<TrajectoryPoint>();
			trajectory.Add(originPoint);

			TrajectoryPoint calculatedPoint = originPoint;
			SurfaceComponent lastClosestSurface = null;
			for (; ; )
			{
				lastClosestSurface = GetClosestSurface(
					calculatedPoint, 
					_surfaceComponents.Where(x => x != lastClosestSurface)
				);
				if (lastClosestSurface == null)
					break;

				IEnumerable<TrajectoryPoint> surfaceTrajectory = lastClosestSurface.TrajectoryCalculator.CalculateTrajectory(calculatedPoint);
				trajectory.AddRange(surfaceTrajectory);
				calculatedPoint = surfaceTrajectory.Last();
			}

			float exitBeamLength = 40F;
			trajectory.Add(new TrajectoryPoint
			{
				Position = calculatedPoint.Position + calculatedPoint.Direction * exitBeamLength,
				Direction = calculatedPoint.Direction
			});

			return trajectory;
		}

		private SurfaceComponent GetClosestSurface(TrajectoryPoint originPoint, IEnumerable<SurfaceComponent> surfaceComponents)
		{
			SurfaceComponent closestSurface = null;
			float? minDistance = null;

			foreach (SurfaceComponent surfaceComponent in surfaceComponents)
			{
				float? distanceToSurface = surfaceComponent.TrajectoryCalculator.GetMinimumDistanceToSurface(originPoint);
				if (!distanceToSurface.HasValue)
					continue;

				if (!minDistance.HasValue || distanceToSurface < minDistance)
				{
					minDistance = distanceToSurface;
					closestSurface = surfaceComponent;
				}
			}

			return closestSurface;
		}
	}
}
