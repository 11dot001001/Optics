﻿using Assets.Scripts.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Optics
{
	public class LightRay : MonoBehaviour
	{
		private List<Vector3> _controlPoints = new List<Vector3>();
		private Vector3 _initialPosition;
		private Vector3 _initialDirection;

		public BeamRenderer LightRenderer;

		public void Initialize(Vector3 position, Vector3 direction)
		{
			_initialPosition = position;
			_initialDirection = direction;
		}

		private void Start()
		{
			Wall[] walls = FindObjectsOfType<Wall>();
			GeneratePathPosition(_initialPosition, _initialDirection, walls);
			LightRenderer.RenderTrajectory(_controlPoints);
		}

		private void GeneratePathPosition(Vector3 startPosition, Vector3 direction, Wall[] walls)
		{
			_controlPoints.Add(startPosition);
			Ray ray = new Ray(startPosition, direction);
			RaycastHit hit = default;
			Wall wall = walls.FirstOrDefault(x => x.Colliders.First().Raycast(ray, out hit, float.MaxValue));
			if (wall == null)
			{
				_controlPoints.Add(startPosition + direction * 10);
				return;
			}

			Wall.WallPoint wallPoint = new Wall.WallPoint
			{
				Position = hit.point,
				Normal = hit.normal
			};

			float opticalDensityOfIncidentBeam = 1.5F;
			float opticalDensityOfRefractedBeam = OpticalDensities.Air;

			Vector3 newBeamDirection;
			if (opticalDensityOfIncidentBeam <= opticalDensityOfRefractedBeam)
			{
				newBeamDirection = GetRefractedBeamDirection(
					startPosition,
					wallPoint.Position,
					wallPoint.Normal,
					opticalDensityOfIncidentBeam,
					opticalDensityOfRefractedBeam
				);
			}
			else
			{
				float incidentBeamAngleDegrees = GetIncidentBeamAngleDegrees(
					startPosition,
					wallPoint.Position,
					wallPoint.Normal
				);
				float ultimateRefractionAngleDegrees = (float)ConvertRadiansToDegrees(
					Math.Asin(opticalDensityOfRefractedBeam / opticalDensityOfIncidentBeam)
				);

				if (incidentBeamAngleDegrees < ultimateRefractionAngleDegrees)
				{
					newBeamDirection = GetRefractedBeamDirection(
						startPosition,
						wallPoint.Position,
						wallPoint.Normal,
						opticalDensityOfIncidentBeam,
						opticalDensityOfRefractedBeam
					);
				}
				else
				{
					newBeamDirection = GetReflectedBeamDirectionViaRelativePosition(
						startPosition,
						wallPoint.Position,
						wallPoint.Normal
					);
				}
			}

			GeneratePathPosition(wallPoint.Position, newBeamDirection, walls);
		}

		private float GetIncidentBeamAngleDegrees(Vector3 startPosition, Vector3 surfacePosition, Vector3 surfaceNormal)
		{
			Vector3 incidentBeamDirection = surfacePosition - startPosition;
			Vector3 reverseIncidentBeamDirection = incidentBeamDirection * -1;
			Vector3 rotationNormalDirection = Vector3.Cross(reverseIncidentBeamDirection, surfaceNormal).normalized;
			return Vector3.SignedAngle(reverseIncidentBeamDirection, surfaceNormal, rotationNormalDirection);
		}

		/// <summary>
		/// Rotates surface normal vector on incidence angle in plane of incidence.
		/// </summary>
		private Vector3 GetReflectedBeamDirectionViaRotation(Vector3 startPosition, Vector3 surfacePosition, Vector3 surfaceNormal)
		{
			Vector3 incidentBeamDirection = surfacePosition - startPosition;
			Vector3 reverseLightDirection = incidentBeamDirection * -1;
			Vector3 rotationNormal = Vector3.Cross(reverseLightDirection, surfaceNormal).normalized;

			float incidentBeamAndNormalAngleDegrees = Vector3.SignedAngle(reverseLightDirection, surfaceNormal, rotationNormal); // equal to reflection
			Quaternion rotation = Quaternion.AngleAxis(incidentBeamAndNormalAngleDegrees, rotationNormal);
			return (rotation * surfaceNormal).normalized;
		}
		/// <summary>
		/// Returns reflected direction via relative start position regarding surface.
		/// </summary>
		private Vector3 GetReflectedBeamDirectionViaRelativePosition(Vector3 startPosition, Vector3 surfacePosition, Vector3 surfaceNormal)
		{
			float startPositionToSurfaceMagnitude = Math.Abs(Vector3.Dot(surfaceNormal, startPosition) - Vector3.Dot(surfaceNormal, surfacePosition));

			Vector3 relativeStartPositionViaSurface = startPosition + surfaceNormal.normalized * startPositionToSurfaceMagnitude * -2;
			return (surfacePosition - relativeStartPositionViaSurface).normalized;
		}

		private Vector3 GetRefractedBeamDirection(
			Vector3 startPosition,
			Vector3 surfacePosition,
			Vector3 surfaceNormal,
			float opticalDensityOfIncidentBeam,
			float opticalDensityOfRefractedBeam
		)
		{
			Vector3 incidentBeamDirection = surfacePosition - startPosition;
			Vector3 reverseIncidentBeamDirection = incidentBeamDirection * -1;
			Vector3 rotationNormalDirection = Vector3.Cross(reverseIncidentBeamDirection, surfaceNormal).normalized;
			float incidentBeamAngleDegrees = Vector3.SignedAngle(reverseIncidentBeamDirection, surfaceNormal, rotationNormalDirection);

			float refractedBeamAndReverseNormalAngleDegrees = (float)ConvertRadiansToDegrees(Math.Asin(
				Math.Sin(ConvertDegreesToRadians(incidentBeamAngleDegrees)) * opticalDensityOfIncidentBeam
				/
				opticalDensityOfRefractedBeam
			));

			Vector3 reverseNormal = surfaceNormal * -1;
			Quaternion rotation = Quaternion.AngleAxis(-refractedBeamAndReverseNormalAngleDegrees, rotationNormalDirection);
			return (rotation * reverseNormal).normalized;
		}

		private double ConvertDegreesToRadians(double degrees) => Math.PI / 180 * degrees;
		private double ConvertRadiansToDegrees(double radians) => radians * 180 / Math.PI;
	}
}