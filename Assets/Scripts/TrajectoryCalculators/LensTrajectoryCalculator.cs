using Assets.Scripts.Surfaces.Abstractions;
using Assets.Scripts.TrajectoryCalculators.Astractions;
using Assets.Scripts.TrajectoryCalculators.Models;
using Optics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TrajectoryCalculators
{
	public class LensTrajectoryCalculator : ISurfaceTrajectoryCalculator
	{
		private const float InvertedRayDistance = 100F;

		private readonly ISurface _surface;

		public LensTrajectoryCalculator(ISurface surface)
		{
			_surface = surface ?? throw new ArgumentNullException(nameof(surface));
		}

		public IEnumerable<TrajectoryPoint> CalculateTrajectory(TrajectoryPoint originPoint)
		{
			List<TrajectoryPoint> trajectory = new List<TrajectoryPoint>();

			TrajectoryPoint calculatedPoint = originPoint;

			Ray ray = new Ray(originPoint.Position, originPoint.Direction);
			if (!_surface.Collider.Raycast(ray, out RaycastHit incomingRaycast, float.MaxValue))
				return null;

			TrajectoryPoint incomingSurfacePoint = ConvertSurfaceRaycast(incomingRaycast);
			Vector3 modifiedBeamDirection = BeamCalculator.IncidentBeam(
				calculatedPoint, 
				incomingSurfacePoint,
				OpticalDensities.Air,
				_surface.OpticalDensity,
				out BeamType modifiedBeamType
			);
			PainterPovider.Painter.DrawHelpPoint(incomingSurfacePoint.Position, true);

			trajectory.Add(calculatedPoint = new TrajectoryPoint
			{
				Position = incomingSurfacePoint.Position,
				Direction = modifiedBeamDirection
			});

			if (modifiedBeamType == BeamType.Refracted)
			{
				for (; ; )
				{
					Ray invertedRay = new Ray(
						calculatedPoint.Position + calculatedPoint.Direction * InvertedRayDistance,
						calculatedPoint.Direction * -1
					);
					if(!_surface.Collider.Raycast(invertedRay, out RaycastHit raycastHit, float.MaxValue))
						throw new Exception("Was not found hit in lens.");

					TrajectoryPoint colliderPoint = ConvertSurfaceRaycast(raycastHit);
					colliderPoint.Direction *= -1;
					modifiedBeamDirection = BeamCalculator.IncidentBeam(
						calculatedPoint,
						colliderPoint,
						_surface.OpticalDensity,
						OpticalDensities.Air,
						out modifiedBeamType
					);

					trajectory.Add(calculatedPoint = new TrajectoryPoint
					{
						Position = colliderPoint.Position,
						Direction = modifiedBeamDirection
					});
					PainterPovider.Painter.DrawHelpPoint(calculatedPoint.Position, true);

					if (modifiedBeamType == BeamType.Refracted)
						break;
				}
			}

			return trajectory;
		}

		public float? GetMinimumDistanceToSurface(TrajectoryPoint originPoint)
		{
			Ray ray = new Ray(originPoint.Position, originPoint.Direction);
			if (_surface.Collider.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
				return raycastHit.distance;

			return null;
		}

		private TrajectoryPoint ConvertSurfaceRaycast(RaycastHit surfaceRaycast)
		{
			return new TrajectoryPoint
			{ 
				Position = surfaceRaycast.point,
				Direction = surfaceRaycast.normal
			};
		}
	}
}
