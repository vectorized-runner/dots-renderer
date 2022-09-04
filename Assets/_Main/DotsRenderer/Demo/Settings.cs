using UnityEngine;

namespace DotsRenderer.Demo
{
	public class Settings : MonoBehaviour
	{
		void Start()
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 600;
			
			Destroy(gameObject);
		}
	}
}