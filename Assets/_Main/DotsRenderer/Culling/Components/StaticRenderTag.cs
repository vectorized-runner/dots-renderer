using Unity.Entities;

namespace DotsRenderer
{
	/// <summary>
	/// Renderer assumes this Entity is static and makes optimizations.
	/// </summary>
	[GenerateAuthoringComponent]
	public struct StaticRenderTag : IComponentData
	{
	}
}