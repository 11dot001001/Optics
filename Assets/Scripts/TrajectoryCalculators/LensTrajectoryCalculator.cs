using Assets.Scripts.Surfaces.Abstractions;
using Assets.Scripts.TrajectoryCalculators.Astractions;
using Assets.Scripts.TrajectoryCalculators.Models;
using Optics;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public IEnumerable<TrajectoryPoint> CalculateTrajectory(TrajectoryPoint startPoint)
		{
			List<TrajectoryPoint> trajectory = new List<TrajectoryPoint>();

			TrajectoryPoint calculatedPoint = startPoint;

			Collider incomingCollider = GetClosestCollider(calculatedPoint, _surface.Colliders, true, out RaycastHit surfaceRaycast);
			if (incomingCollider == null)
				return null;

			TrajectoryPoint incomingSurfacePoint = ConvertSurfaceRaycast(surfaceRaycast);
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
				Collider lastRefractedCollider = incomingCollider;
				for (; ; )
				{
					lastRefractedCollider = GetClosestCollider(
						calculatedPoint,
						_surface.Colliders.Where(x => x != lastRefractedCollider),
						false,
						out RaycastHit raycastHit
					);
					if (lastRefractedCollider == null)
						throw new Exception("Was not found collider in lens.");

					TrajectoryPoint colliderPoint = ConvertSurfaceRaycast(raycastHit);
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

		public float? GetMinimumDistanceToSurface(TrajectoryPoint startPoint)
		{
			if (GetClosestCollider(startPoint, _surface.Colliders, true, out RaycastHit raycastHit) != null)
			{
				return Vector3.Distance(startPoint.Position, raycastHit.point);
			}
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
		private Collider GetClosestCollider(
			TrajectoryPoint originPoint,
			IEnumerable<Collider> colliders,
			bool givenDirection,
			out RaycastHit closestRaycast
		)
		{
			Ray ray = new Ray(originPoint.Position, originPoint.Direction);
			Ray invertedRay = new Ray(
				originPoint.Position + originPoint.Direction * InvertedRayDistance,
				originPoint.Direction * -1
			);
			Collider closestCollider = null;
			float? minDistance = null;
			closestRaycast = new RaycastHit();

			foreach (Collider collider in colliders)
			{
				float? distanceForCollider = null;
				if (collider.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
					distanceForCollider = raycastHit.distance;
				else if (!givenDirection && collider.Raycast(invertedRay, out raycastHit, float.MaxValue))
					distanceForCollider = Vector3.Distance(originPoint.Position, raycastHit.point);

				if (!distanceForCollider.HasValue)
					continue;

				if (!minDistance.HasValue || distanceForCollider < minDistance)
				{
					minDistance = distanceForCollider;
					closestCollider = collider;
					closestRaycast = raycastHit;
				}
			}

			return closestCollider;
		}
	}
}
