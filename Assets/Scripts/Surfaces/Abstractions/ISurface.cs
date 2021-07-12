using UnityEngine;

namespace Assets.Scripts.Surfaces.Abstractions
{
	public interface ISurface
	{
		float OpticalDensity { get; }
		Collider Collider { get; }
	}
}
