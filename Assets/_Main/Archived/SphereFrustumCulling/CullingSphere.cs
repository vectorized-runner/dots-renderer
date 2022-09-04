using Unity.Entities;
using Unity.Mathematics;

namespace DotsRenderer.Archived
{
	[GenerateAuthoringComponent]
	public struct CullingSphere : IComponentData
	{
		public float3 Center;
		public float Radius;
	}
}