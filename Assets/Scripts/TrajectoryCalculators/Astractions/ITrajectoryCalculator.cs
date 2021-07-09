using Assets.Scripts.TrajectoryCalculators.Models;
using System.Collections.Generic;

namespace Assets.Scripts.TrajectoryCalculators.Astractions
{
	public interface ITrajectoryCalculator
	{
		IEnumerable<TrajectoryPoint> CalculateTrajectory(TrajectoryPoint startPoint);
	}
}
