using Unity.Entities;

namespace DotsRenderer
{
	/// <summary>
	/// Objects with this tag will be Rendered with the Custom Renderer.
	/// </summary>
	[GenerateAuthoringComponent]
	public struct CustomRenderingTag : IComponentData
	{
	}
}