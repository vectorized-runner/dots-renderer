using System;
using Unity.Entities;

namespace DotsRenderer
{
	[GenerateAuthoringComponent]
	public readonly struct RenderMeshIndex : IComponentData, IEquatable<RenderMeshIndex>
	{
		public readonly int Value;

		public RenderMeshIndex(int value)
		{
			Value = value;
		}

		public bool Equals(RenderMeshIndex other)
		{
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			return obj is RenderMeshIndex other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Value;
		}

		public static bool operator ==(RenderMeshIndex left, RenderMeshIndex right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RenderMeshIndex left, RenderMeshIndex right)
		{
			return !left.Equals(right);
		}
	}
}