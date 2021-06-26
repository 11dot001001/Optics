using System.Collections.Generic;
using UnityEngine;

namespace Optics
{
	public class LightRenderer : MonoBehaviour
	{
		public Painter Painter;

		public void RenderLightPath(List<Vector3> controlPoints)
		{
			if(controlPoints == null || controlPoints.Count < 2)
				return;

			Vector3 previousPoint = controlPoints[0];
			for (int i = 1; i != controlPoints.Count; i++)
			{
				Vector3 currentPoint = controlPoints[i];
				Painter.DrawLightLine(previousPoint, currentPoint);
				previousPoint = currentPoint;
			}
		}
	}
}