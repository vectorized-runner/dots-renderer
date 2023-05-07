using System;
using UnityEngine;

namespace DotsRenderer
{
	public readonly struct RenderMesh : IEquatable<RenderMesh>
	{
		public readonly Mesh Mesh;
		public readonly Material Material;
		public readonly int SubMeshIndex;

		public RenderMesh(Mesh mesh, Material material, int subMeshIndex)
		{
			Mesh = mesh;
			Material = material;
			SubMeshIndex = subMeshIndex;
		}

		public bool Equals(RenderMesh other)
		{
			return Equals(Mesh, other.Mesh) && Equals(Material, other.Material) && SubMeshIndex == other.SubMeshIndex;
		}

		public override bool Equals(object obj)
		{
			return obj is RenderMesh other && Equals(other);
		}

		public override int GetHashCode()
		{
			// Fast Hash implementation
			unchecked
			{
				var hash = 17;

				hash = hash * 31 + Mesh.GetHashCode();
				hash = hash * 31 + Material.GetHashCode();
				hash = hash * 31 + SubMeshIndex;

				return hash;
			}
		}

		public static bool operator ==(RenderMesh left, RenderMesh right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RenderMesh left, RenderMesh right)
		{
			return !left.Equals(right);
		}
	}
}