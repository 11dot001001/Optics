using Assets.Scripts.TrajectoryCalculators.Models;
using System;
using UnityEngine;

namespace Assets.Scripts.TrajectoryCalculators
{
	public static class BeamCalculator
	{
		static public Vector3 IncidentBeam(
			TrajectoryPoint originPoint,
			TrajectoryPoint surfacePoint,
			float opticalDensityOfIncidentBeam,
			float opticalDensityOfRefractedBeam,
			out BeamType modifiedBeamType
		)
		{
			Vector3 modifiedBeamDirection;

			if (opticalDensityOfIncidentBeam <= opticalDensityOfRefractedBeam)
			{
				modifiedBeamDirection = GetRefractedBeamDirection(
					originPoint.Position,
					surfacePoint.Position,
					surfacePoint.Direction,
					opticalDensityOfIncidentBeam,
					opticalDensityOfRefractedBeam
				);
				modifiedBeamType = BeamType.Refracted;
			}
			else
			{
				float incidentBeamAngleDegrees = GetIncidentBeamAngleDegrees(
					originPoint.Position,
					surfacePoint.Position,
					surfacePoint.Direction
				);
				float ultimateRefractionAngleDegrees = (float)ConvertRadiansToDegrees(
					Math.Asin(opticalDensityOfRefractedBeam / opticalDensityOfIncidentBeam)
				);

				if (incidentBeamAngleDegrees < ultimateRefractionAngleDegrees)
				{
					modifiedBeamDirection = GetRefractedBeamDirection(
						originPoint.Position,
						surfacePoint.Position,
						surfacePoint.Direction,
						opticalDensityOfIncidentBeam,
						opticalDensityOfRefractedBeam
					);
					modifiedBeamType = BeamType.Refracted;
				}
				else
				{
					modifiedBeamDirection = GetReflectedBeamDirectionViaRelativePosition(
						originPoint.Position,
						surfacePoint.Position,
						surfacePoint.Direction
					);
					modifiedBeamType = BeamType.Reflected;
				}
			}

			return modifiedBeamDirection;
		}

		static private float GetIncidentBeamAngleDegrees(Vector3 startPosition, Vector3 surfacePosition, Vector3 surfaceNormal)
		{
			Vector3 incidentBeamDirection = surfacePosition - startPosition;
			Vector3 reverseIncidentBeamDirection = incidentBeamDirection * -1;
			Vector3 rotationNormalDirection = Vector3.Cross(reverseIncidentBeamDirection, surfaceNormal).normalized;
			return Vector3.SignedAngle(reverseIncidentBeamDirection, surfaceNormal, rotationNormalDirection);
		}

		/// <summary>
		/// Rotates surface normal vector on incidence angle in plane of incidence.
		/// </summary>
		static private Vector3 GetReflectedBeamDirectionViaRotation(Vector3 startPosition, Vector3 surfacePosition, Vector3 surfaceNormal)
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
		static private Vector3 GetReflectedBeamDirectionViaRelativePosition(Vector3 startPosition, Vector3 surfacePosition, Vector3 surfaceNormal)
		{
			float startPositionToSurfaceMagnitude = Math.Abs(Vector3.Dot(surfaceNormal, startPosition) - Vector3.Dot(surfaceNormal, surfacePosition));

			Vector3 relativeStartPositionViaSurface = startPosition + surfaceNormal.normalized * startPositionToSurfaceMagnitude * -2;
			return (surfacePosition - relativeStartPositionViaSurface).normalized;
		}

		static private Vector3 GetRefractedBeamDirection(
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

		static private double ConvertDegreesToRadians(double degrees) => Math.PI / 180 * degrees;
		static private double ConvertRadiansToDegrees(double radians) => radians * 180 / Math.PI;
	}
}
