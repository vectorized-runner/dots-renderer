using Unity.Entities;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(InitializationSystemGroup))]
	public class RendererInitializationGroup : ComponentSystemGroup
	{
	}
}