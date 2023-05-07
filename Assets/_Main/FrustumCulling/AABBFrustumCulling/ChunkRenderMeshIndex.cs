using System;
using Unity.Entities;

namespace DotsLibrary.Rendering
{
	public readonly struct ChunkRenderMeshIndex : IComponentData, IEquatable<ChunkRenderMeshIndex>
	{
		public readonly int Value;

		public ChunkRenderMeshIndex(int value)
		{
			Value = value;
		}

		public bool Equals(ChunkRenderMeshIndex other)
		{
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			return obj is ChunkRenderMeshIndex other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Value;
		}

		public static bool operator ==(ChunkRenderMeshIndex left, ChunkRenderMeshIndex right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ChunkRenderMeshIndex left, ChunkRenderMeshIndex right)
		{
			return !left.Equals(right);
		}
	}
}