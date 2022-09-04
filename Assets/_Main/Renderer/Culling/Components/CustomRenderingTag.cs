using Unity.Entities;

namespace DotsLibrary.Rendering
{
	/// <summary>
	/// Objects with this tag will be Rendered with the Custom Renderer.
	/// </summary>
	[GenerateAuthoringComponent]
	public class CustomRenderingTag : IComponentData
	{
	}
}