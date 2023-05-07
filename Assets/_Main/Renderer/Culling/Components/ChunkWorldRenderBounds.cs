using Unity.Entities;

namespace DotsLibrary.Rendering
{
	public struct ChunkWorldRenderBounds : IComponentData
	{
		public AABB AABB;
	}
}