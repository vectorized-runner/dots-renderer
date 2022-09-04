using Unity.Entities;
using Unity.Mathematics;

namespace DotsLibrary.Rendering
{
	[GenerateAuthoringComponent]
	public struct CullingSphere : IComponentData
	{
		public float3 Center;
		public float Radius;
	}
}