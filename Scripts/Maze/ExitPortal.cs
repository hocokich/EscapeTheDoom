using TMPro;
using UnityEngine;
using System.Collections;

public class ExitPortal : MonoBehaviour
{
	GameObject MessageUI;						//Надпись
	private Coroutine fadeCoroutine;			//Плавное исчезновение надписи

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			if (FindInactiveObject("Maze").GetComponent<MazeGenerator>().Keys == GameManager.instance.player.GetComponent<Player>().Keys)
				GameManager.instance.Win();
			else
			{
				Message();
			}
	}

	void Message()
	{
		MessageUI = FindInactiveObject("FindKey_Message");

		if (MessageUI != null)
		{
			// Получаем TextMeshPro компонент
			TextMeshProUGUI textMesh = MessageUI.GetComponent<TextMeshProUGUI>();
			if (textMesh != null)
			{
				// Делаем видимым и запускаем исчезновение
				textMesh.alpha = 1f;
				MessageUI.SetActive(true);

				// Останавливаем предыдущую корутину если есть
				if (fadeCoroutine != null)
					StopCoroutine(fadeCoroutine);

				fadeCoroutine = StartCoroutine(FadeOutText(textMesh));
			}
		}
	}

	IEnumerator FadeOutText(TextMeshProUGUI textMesh)
	{
		float fadeTime = 3f;
		float elapsed = 0f;

		while (elapsed < fadeTime)
		{
			elapsed += Time.deltaTime;
			textMesh.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
			yield return null;
		}

		textMesh.alpha = 0f;
		MessageUI.SetActive(false);
	}

	GameObject FindInactiveObject(string name)
	{
		GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
		foreach (GameObject obj in allObjects)
		{
			if (obj.name == name)
				return obj;
		}
		return null;
	}
}