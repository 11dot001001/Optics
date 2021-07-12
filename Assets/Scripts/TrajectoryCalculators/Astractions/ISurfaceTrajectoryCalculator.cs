using Assets.Scripts.TrajectoryCalculators.Models;

namespace Assets.Scripts.TrajectoryCalculators.Astractions
{
	public interface ISurfaceTrajectoryCalculator : ITrajectoryCalculator
	{
		float? GetMinimumDistanceToSurface(TrajectoryPoint originPoint);
	}
}
