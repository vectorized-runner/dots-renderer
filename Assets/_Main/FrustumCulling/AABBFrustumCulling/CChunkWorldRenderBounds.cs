using Unity.Entities;

namespace DotsLibrary.Rendering
{
	public struct CChunkWorldRenderBounds : IComponentData
	{
		public AABB Value;
	}
}