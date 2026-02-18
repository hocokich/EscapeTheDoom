using UnityEngine;
using UnityEngine.Pool;

public class TracerSystem : MonoBehaviour
{
	public GameObject tracerPrefab; //ссылка на шаблон эффекта трассировки полёта снаряда
	public int poolsize;
	public int maxPoolsize;

	IObjectPool<GameObject> tracers; //хранилище эффектов

	void Start() => //создание хранилища
		tracers = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, poolsize, maxPoolsize);

	GameObject CreatePooledItem() //метод, описывающий создание эффекта в хранилище
	{
		GameObject tracer = Instantiate(tracerPrefab);
		tracer.GetComponent<TracerScript>().pool = tracers;
		return tracer;
	}

	void OnReturnedToPool(GameObject tracer) => //метод, описывающий действия при возвращении эффекта в хранилище
		tracer.gameObject.SetActive(false);

	void OnTakeFromPool(GameObject tracer) => //метод, описывающий действия при извлечении эффекта из хранилища
		tracer.gameObject.SetActive(true);

	void OnDestroyPoolObject(GameObject tracer) => //метод, описывающий действия при уничтожении эффекта
		Destroy(tracer);

	public void CreateTracer(Vector3 position, Vector3 direction) //метод, запускающий процесс отображения эффекта в сцене
	{
		GameObject tracer = tracers.Get();
		tracer.GetComponent<TracerScript>().setPoints(position, direction);
	}
}