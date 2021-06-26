using System;
using UnityEngine;

namespace Optics
{
	public class LightRenderer : MonoBehaviour
	{
		public Wall Wall;
		public LightSource LightSource;
		public GameObject LightPointPrefab;
		public GameObject HelpPointPrefab;
		public float LightPointMinInterval = 0.125F;

		private void Update()
		{
			Vector3 wallPosition = Wall.Transform.position;
			Vector3 lightSourcePosition = LightSource.Transform.position;

			Vector3 lightDirection = wallPosition - lightSourcePosition;

			DrawLightLine(lightSourcePosition, wallPosition);
			
			Vector3 wallSurfaceNormal = Wall.Transform.right;
			DrawHelpLine(Vector3.zero, wallSurfaceNormal * 3);
			Vector3 reverseLightDirection = lightDirection * -1;
			Vector3 rotationNormal = Vector3.Cross(reverseLightDirection, wallSurfaceNormal).normalized;
			DrawHelpLine(Vector3.zero, rotationNormal * 3);
			float entryAngleDegrees = Vector3.SignedAngle(reverseLightDirection, wallSurfaceNormal,  rotationNormal); // equal to reflection
			Debug.Log(entryAngleDegrees);
			
			Quaternion rotation = Quaternion.AngleAxis(entryAngleDegrees, rotationNormal);
			Vector3 reflectedLightNormalizedDirection = (rotation * wallSurfaceNormal).normalized;
			Vector3 reflectedLightDirection = reflectedLightNormalizedDirection * lightDirection.magnitude;
			DrawLightLine(wallPosition, wallPosition + reflectedLightDirection);
		}

		
		private void DrawLightLine(Vector3 start, Vector3 end)  => DrawLine(start, end, LightPointPrefab);
		private void DrawHelpLine(Vector3 start, Vector3 end)  => DrawLine(start, end, HelpPointPrefab);
		private void DrawLine(Vector3 start, Vector3 end, GameObject point)
		{
			Vector3 lightDirection = end - start;
			Vector3 lightDelta = lightDirection.normalized * LightPointMinInterval;
			Vector3 lastLightPointPosition = start;
			for (;;)
			{
				if ((lastLightPointPosition - end).magnitude < LightPointMinInterval)
					break;

				lastLightPointPosition += lightDelta;
				InitializePoint(lastLightPointPosition, point);
			}
		}
		private void InitializePoint(Vector3 position, GameObject point)
		{
			GameObject initializedLightPoint = Instantiate(point, position, Quaternion.identity);
			Destroy(initializedLightPoint, 0.0625F);
		}

		private double DegreeToRadian(float angle) => (Math.PI * angle) / 180;
	}
}