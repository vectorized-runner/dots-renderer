using Unity.Burst;
using UnityEngine;

namespace DotsRenderer
{
	public static class FastLog
	{
		[BurstDiscard]
		public static void Info(string message, Object context = null)
		{
			Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, context, message);
		}

		[BurstDiscard]
		public static void Warning(string message, Object context = null)
		{
			Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, context, message);
		}

		[BurstDiscard]
		public static void Error(string message, Object context = null)
		{
			Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, context, message);
		}
	}
}