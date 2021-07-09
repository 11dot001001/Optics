using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Surfaces.Abstractions
{
	public interface ISurface
	{
		float OpticalDensity { get; }
		IEnumerable<Collider> Colliders { get; }
	}
}
