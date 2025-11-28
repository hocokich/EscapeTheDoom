using UnityEngine;
using UnityEngine.Rendering;

public class VolumeCntrl : MonoBehaviour
{
	private static GameObject volumeInstance;
	void Awake()
	{
		if (volumeInstance == null)
		{
			volumeInstance = gameObject;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			// ≈сли уже существует - уничтожаем дубликат
			Destroy(gameObject);
		}
	}
}
