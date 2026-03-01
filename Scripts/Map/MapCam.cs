using UnityEngine;

public class MapCam : MonoBehaviour
{
	[SerializeField] private Transform target;

	private void Start()
	{
		float mazeSize = GameObject.Find("GameManager").GetComponent<GameManager>().mazeSize;

		// Ждем пока все объекты загрузятся
		Invoke("SetupCamera", 0.2f);
	}

	private void SetupCamera()
	{
		float mazeSize = GameObject.Find("GameManager").GetComponent<GameManager>().mazeSize;

		// Находим реальный центр всех дочерних объектов
		Bounds bounds = GetCombinedBounds(target);

		// Ставим камеру точно над центром
		Vector3 center = bounds.center;
		transform.position = new Vector3(center.x, 20f, center.z);

		// Смотрим вниз
		transform.rotation = Quaternion.Euler(90f, 0f, 0f);

		// Размер камеры
		GetComponent<Camera>().orthographicSize = 7 + mazeSize;
	}

	private Bounds GetCombinedBounds(Transform parent)
	{
		Bounds bounds = new Bounds(parent.position, Vector3.zero);

		Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderers)
		{
			bounds.Encapsulate(renderer.bounds);
		}

		return bounds;
	}
}