using Unity.Entities;

namespace DotsRenderer.PerfTesting
{
	[GenerateAuthoringComponent]
	public struct SpawnData : IComponentData
	{
		public Entity Entity;
		public int CountX;
		public int CountZ;
		public float Spacing;
	}
}