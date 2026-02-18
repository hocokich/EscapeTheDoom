using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

public class TracerScript : MonoBehaviour
{
	public float length = 55;    //длина трассера
	public float speed = 100;    //скрость перемещения трассера
	public float lifefime = .3f;    //время отображения трассера

	Vector3 position;    //текущая позиция трассера
	Vector3 direction;    //направление смещения трассера

	LineRenderer lineRenderer;
	public IObjectPool<GameObject> pool;    //ссылка на хранилище эффектов, визуализирующих трассер

	private void Awake() => lineRenderer = GetComponent<LineRenderer>();

	public void setPoints(Vector3 position, Vector3 direction)  //установка позиции и направления смещения трассера
	{
		this.position = position;
		this.direction = direction;
	}

	void OnEnable() => StartCoroutine(Fade());    //запуск процесса выключения эффекта трассера

	IEnumerator Fade()
	{
		yield return new WaitForSeconds(lifefime);
		//pool.Release(gameObject);    //возвращение эффекта в хранилище
		Destroy(gameObject);

	}

	private void Update()    //смещение точек трассера
	{
		lineRenderer.SetPosition(0, position);
		lineRenderer.SetPosition(1, position + direction * length);
		position += direction * speed * Time.deltaTime;
	}
}