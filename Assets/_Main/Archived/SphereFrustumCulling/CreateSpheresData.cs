using Unity.Entities;

namespace DotsRenderer.Archived
{
	[GenerateAuthoringComponent]
	public struct CreateSpheresData : IComponentData
	{
		public int Count;
		public float SpawnRadius;
	}
}