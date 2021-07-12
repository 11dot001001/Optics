using UnityEngine;

namespace Optics
{
	public class Painter : MonoBehaviour
	{
		public GameObject LightPointPrefab;
		public GameObject HelpPointPrefab;
		public float LinePointInterval = 0.062F;

		public void DrawLightLine(Vector3 startPosition, Vector3 endPosition, bool needDestroy) => DrawLine(startPosition, endPosition, LightPointPrefab, needDestroy);
		public void DrawHelpLine(Vector3 startPosition, Vector3 endPosition, bool needDestroy) => DrawLine(startPosition, endPosition, HelpPointPrefab, needDestroy);
		public void DrawHelpPoint(Vector3 position, bool needDestroy) => InitializePoint(position, HelpPointPrefab, needDestroy);

		private void DrawLine(Vector3 startPosition, Vector3 endPosition, GameObject point, bool needDestroy)
		{
			Vector3 lightDirection = endPosition - startPosition;
			Vector3 lightDelta = lightDirection.normalized * LinePointInterval;
			Vector3 lastLightPointPosition = startPosition;
			for (; ; )
			{
				if ((lastLightPointPosition - endPosition).magnitude < LinePointInterval)
					break;

				lastLightPointPosition += lightDelta;
				InitializePoint(lastLightPointPosition, point, needDestroy);
			}
		}

		private void InitializePoint(Vector3 position, GameObject point, bool needDestroy)
		{
			GameObject initializedLightPoint = Instantiate(point, position, Quaternion.identity);
			if (needDestroy)
				Destroy(initializedLightPoint, 0.0625F);
		}
	}
}