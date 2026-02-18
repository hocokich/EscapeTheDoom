using System.Collections;
using UnityEngine;

public class portraitUI : MonoBehaviour
{
	public GameObject[] pics;
	private Coroutine activeCoroutine;

	public void Norm()
	{
		ShowPic(0);
	}

	public void Damaged()
	{
		ShowPic(1, 1f);
	}

	public void Muhehe()
	{
		ShowPic(2, 1f);
	}

	private void ShowPic(int index, float duration = 0f)
	{
		if (activeCoroutine != null)
			StopCoroutine(activeCoroutine);

		activeCoroutine = StartCoroutine(ShowPicCoroutine(index, duration));
	}

	private IEnumerator ShowPicCoroutine(int index, float duration)
	{
		// Выключаем все
		foreach (GameObject pic in pics)
			pic.SetActive(false);

		// Включаем нужную
		pics[index].SetActive(true);

		// Если задана длительность, ждем и показываем нормальное состояние
		if (duration > 0)
		{
			yield return new WaitForSeconds(duration);
			Norm();
		}

		activeCoroutine = null;
	}
}