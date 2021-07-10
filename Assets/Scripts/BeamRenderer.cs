using System.Collections.Generic;
using UnityEngine;

namespace Optics
{
	public class BeamRenderer : MonoBehaviour
	{
		public LineRenderer LineRendererPrefab;

		public void RenderTrajectory(List<Vector3> trajectoryPoints)
		{
			if (trajectoryPoints == null || trajectoryPoints.Count < 2)
				return;
			
			LineRenderer lineRenderer = Instantiate(LineRendererPrefab);
			lineRenderer.positionCount = trajectoryPoints.Count;
			lineRenderer.SetPositions(trajectoryPoints.ToArray());
			Destroy(lineRenderer.gameObject, 0.0625F);
		}
	}
}