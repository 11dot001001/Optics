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

		public LightRenderer LightRenderer;
		
		public void Initialize(Vector3 position, Vector3 direction)
		{
			_initialPosition = position;
			_initialDirection = direction;
		}

		private void Start()
		{
			Wall[] walls = FindObjectsOfType<Wall>();
			GeneratePathPosition(_initialPosition, _initialDirection, walls);
			LightRenderer.RenderLightPath(_controlPoints);
		}

		private void GeneratePathPosition(Vector3 startPosition, Vector3 direction, Wall[] walls)
		{
			_controlPoints.Add(startPosition);
			Ray ray = new Ray(startPosition, direction);
			RaycastHit hit = default;
			Wall wall = walls.FirstOrDefault(x => x.BoxCollider.Raycast(ray, out hit, float.MaxValue));
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

			Vector3 reflectedLightNormalizedDirection = GetReflectedLightDirectionViaRelativePosition(
				startPosition, 
				wallPoint.Position,
				wallPoint.Normal
			);
			
			GeneratePathPosition(wallPoint.Position, reflectedLightNormalizedDirection,walls);
		}
		
		/// <summary>
		/// Rotates surface normal vector on incidence angle in plane of incidence.
		/// </summary>
		private Vector3 GetReflectedLightDirectionViaRotation(Vector3 startPosition, Vector3 surfacePosition, Vector3 surfaceNormal)
		{
			Vector3 lightDirection = surfacePosition - startPosition;
			Vector3 reverseLightDirection = lightDirection * -1;
			Vector3 rotationNormal = Vector3.Cross(reverseLightDirection, surfaceNormal).normalized;
			
			float entryAngleDegrees = Vector3.SignedAngle(reverseLightDirection, surfaceNormal, rotationNormal); // equal to reflection
			Quaternion rotation = Quaternion.AngleAxis(entryAngleDegrees, rotationNormal);
			return (rotation * surfaceNormal).normalized;
		}
		/// <summary>
		/// Returns reflected direction via relative start position regarding surface.
		/// </summary>
		private Vector3 GetReflectedLightDirectionViaRelativePosition(Vector3 startPosition, Vector3 surfacePosition, Vector3 surfaceNormal)
		{
			float startPositionToSurfaceMagnitude = Math.Abs(Vector3.Dot(surfaceNormal, startPosition) - Vector3.Dot(surfaceNormal, surfacePosition));
			
			Vector3 relativeStartPositionViaSurface = startPosition + surfaceNormal.normalized * startPositionToSurfaceMagnitude * -2;
			return (surfacePosition - relativeStartPositionViaSurface).normalized;
		}
	}
}