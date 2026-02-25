using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentRotatingLight : MonoBehaviour
{
	public float rotationSpeed = 10f;
	public Vector3 rotationAxis = Vector3.up;

	private static PersistentRotatingLight instance;

	private void Awake()
	{
		// Паттерн Singleton для гарантии единственного экземпляра
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			LoadRotation();
		}
		else
		{
			Destroy(gameObject); // Удаляем дубликат при возвращении в сцену
		}
	}

	private void Update()
	{
		transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
	}

	void LoadRotation()
	{
		if (PlayerPrefs.HasKey("LightRotationY"))
		{
			float x = PlayerPrefs.GetFloat("LightRotationX");
			float y = PlayerPrefs.GetFloat("LightRotationY");
			float z = PlayerPrefs.GetFloat("LightRotationZ");

			transform.eulerAngles = new Vector3(x, y, z);

			Debug.Log($"Свет загружен: {transform.eulerAngles}");
		}
	}
}