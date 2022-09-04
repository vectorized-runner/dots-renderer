using Unity.Entities;

namespace DotsLibrary.Rendering
{
	[GenerateAuthoringComponent]
	public struct CreateSpheresData : IComponentData
	{
		public int Count;
		public float SpawnRadius;
	}
}