using UnityEngine;
using UnityEngine.SceneManagement;

public class RotatingLight : MonoBehaviour
{
	public float rotationSpeed = 10f; // скорость вращения в градусах в секунду
	public Vector3 rotationAxis = Vector3.up; // ось вращения (по умолчанию Y)

	private void Start()
	{
		// Делаем объект неуничтожаемым при загрузке новых сцен
		DontDestroyOnLoad(gameObject);

		// Загружаем сохраненный угол поворота
		LoadRotation();
	}

	private void Update()
	{
		// Вращаем свет
		transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);

		// Сохраняем текущий угол (можно реже для оптимизации)
		SaveRotation();
	}

	void SaveRotation()
	{
		// Сохраняем угол поворота по оси Y (или по нужной оси)
		PlayerPrefs.SetFloat("LightRotationY", transform.eulerAngles.y);
		PlayerPrefs.Save();
	}

	void LoadRotation()
	{
		// Загружаем сохраненный угол, если он есть
		if (PlayerPrefs.HasKey("LightRotationY"))
		{
			float savedAngle = PlayerPrefs.GetFloat("LightRotationY");
			Vector3 currentRotation = transform.eulerAngles;
			currentRotation.y = savedAngle;
			transform.eulerAngles = currentRotation;
		}
	}

	// Опционально: очистка сохраненных данных
	public void ResetRotation()
	{
		PlayerPrefs.DeleteKey("LightRotationY");
	}

	private void OnDestroy()
	{
		// Сохраняем при уничтожении объекта
		SaveRotation();
	}
}