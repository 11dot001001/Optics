using Optics;
using System.Collections.Generic;
using UnityEngine;

public class LensMeshBuilder : MonoBehaviour
{
	public Painter Painter;
	public float SphereRadius = 8f;
	public int MeridiansCount = 64;
	public int ParallelsCount = 32;
	public int DividingMeridian = 0;

	public void UpdateMesh()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = BuildSphereMesh();

		DestroyImmediate(GetComponent<MeshCollider>());
		gameObject.AddComponent<MeshCollider>();
	}

	private void DrawTrapeze(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4)
	{
		DrawTriangle(vertex1, vertex2, vertex3);
		DrawTriangle(vertex1, vertex3, vertex4);
	}
	private void DrawTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
	{
		Painter.DrawHelpLine(vertex1, vertex2, false);
		Painter.DrawHelpLine(vertex2, vertex3, false);
		Painter.DrawHelpLine(vertex1, vertex3, false);
	}

	private int[] GetTrianglesFromTrapeze(int vertex1, int vertex2, int vertex3, int vertex4)
	{
		return new int[6]
		{
			vertex1, vertex2, vertex3,
			vertex1, vertex3, vertex4
		};
	}
	private List<int> GetTriangles()
	{
		List<int> triangles = new List<int>();

		for (int parallelIndex = 0; parallelIndex != ParallelsCount - 1; parallelIndex++)
		{
			triangles.AddRange(new int[]
			{
				0,
				ParallelsCount + parallelIndex,
				ParallelsCount + parallelIndex + 1
			});
		}
		triangles.AddRange(new int[]
		{
			0,
			ParallelsCount + ParallelsCount - 1,
			ParallelsCount
		});

		for (int meridianIndex = 1; meridianIndex != MeridiansCount - 1; meridianIndex++)
		{
			if (DividingMeridian != 0 && meridianIndex == DividingMeridian)
			{
				int divisionCenterVertexIndex = (DividingMeridian + 1) * ParallelsCount;
				for (int parallelIndex = 0; parallelIndex != ParallelsCount - 1; parallelIndex++)
				{
					triangles.AddRange(new int[]
					{
						divisionCenterVertexIndex,
						DividingMeridian * ParallelsCount + parallelIndex + 1,
						DividingMeridian * ParallelsCount + parallelIndex
					});
				}
				triangles.AddRange(new int[]
				{
					divisionCenterVertexIndex,
					DividingMeridian * ParallelsCount,
					DividingMeridian * ParallelsCount + ParallelsCount - 1
				});
				break;
			}
			for (int parallelIndex = 0; parallelIndex != ParallelsCount - 1; parallelIndex++)
			{
				triangles.AddRange(GetTrianglesFromTrapeze(
					meridianIndex * ParallelsCount + parallelIndex,
					(meridianIndex + 1) * ParallelsCount + parallelIndex,
					(meridianIndex + 1) * ParallelsCount + parallelIndex + 1,
					meridianIndex * ParallelsCount + parallelIndex + 1
				));
			}
			triangles.AddRange(GetTrianglesFromTrapeze(
				(meridianIndex + 1) * ParallelsCount - 1,
				(meridianIndex + 1) * ParallelsCount + ParallelsCount - 1,
				(meridianIndex + 1) * ParallelsCount,
				meridianIndex * ParallelsCount
			));
		}

		return triangles;
	}
	private Mesh BuildSphereMesh()
	{
		Mesh mesh = new Mesh();
		mesh.name = "lens";

		List<Vector3> vertices = new List<Vector3>();
		for (int meridianIndex = 0; meridianIndex != MeridiansCount; meridianIndex++)
		{
			if (DividingMeridian != 0 && meridianIndex - 1 == DividingMeridian)
			{
				float previousMeridianPart = (float)(meridianIndex - 1) / (MeridiansCount - 1);
				Vector3 divisionCenterVertex = new Vector3(
					0,
					SphereRadius * Mathf.Sin(Mathf.PI * previousMeridianPart) * Mathf.Sin(2 * Mathf.PI * 0),
					SphereRadius * Mathf.Cos(Mathf.PI * previousMeridianPart)
				);
				vertices.Add(divisionCenterVertex);
				break;
			}
			float meridianPart = (float)meridianIndex / (MeridiansCount - 1);
			for (int parallelIndex = 0; parallelIndex != ParallelsCount; parallelIndex++)
			{
				float parallelPart = (float)parallelIndex / ParallelsCount;
				Vector3 vertex = new Vector3(
					SphereRadius * Mathf.Sin(Mathf.PI * meridianPart) * Mathf.Cos(2 * Mathf.PI * parallelPart),
					SphereRadius * Mathf.Sin(Mathf.PI * meridianPart) * Mathf.Sin(2 * Mathf.PI * parallelPart),
					SphereRadius * Mathf.Cos(Mathf.PI * meridianPart)
				);
				vertices.Add(vertex);
			}
		}
		List<int> triangles = GetTriangles();

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		return mesh;
	}
}
