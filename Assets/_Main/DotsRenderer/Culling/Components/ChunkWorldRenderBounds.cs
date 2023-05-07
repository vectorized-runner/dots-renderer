using Unity.Entities;

namespace DotsRenderer
{
	public struct ChunkWorldRenderBounds : IComponentData
	{
		public AABB AABB;
	}
}