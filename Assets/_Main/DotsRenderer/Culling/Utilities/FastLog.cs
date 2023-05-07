using Unity.Burst;
using UnityEngine;

namespace DotsRenderer
{
	public class FastLog
	{
		[BurstDiscard]
		public static void Info(string message, Object context = null)
		{
			Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, context, message);
		}
	}
}