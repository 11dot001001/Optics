using System.Collections.Generic;
using UnityEngine;

namespace Optics
{
	public class BeamRenderer : MonoBehaviour
	{
		public Painter Painter;

		public void RenderTrajectory(List<Vector3> trajectoryPoints)
		{
			if(trajectoryPoints == null || trajectoryPoints.Count < 2)
				return;

			Vector3 previousPoint = trajectoryPoints[0];
			for (int i = 1; i != trajectoryPoints.Count; i++)
			{
				Vector3 currentPoint = trajectoryPoints[i];
				//Debug.DrawLine(previousPoint, currentPoint, Color.yellow);
				Painter.DrawLightLine(previousPoint, currentPoint);
				previousPoint = currentPoint;
			}
		}
	}
}