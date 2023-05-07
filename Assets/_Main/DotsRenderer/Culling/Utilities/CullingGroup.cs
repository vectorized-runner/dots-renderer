using Unity.Entities;

namespace DotsRenderer
{
	[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
	public class CullingGroup : ComponentSystemGroup
	{
	}
}