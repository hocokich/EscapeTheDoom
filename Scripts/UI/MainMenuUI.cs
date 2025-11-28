using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI; // для Button
using System.Collections;
using Button = UnityEngine.UI.Button;

public class MainMenuUI : MonoBehaviour
{
	// UI панели
	private GameObject optionsPanel;

	// Эффекты
	private GameObject GlobalVolume;
	private GameObject CameraPixelize;
	private ScriptableRendererFeature aoFeature;

	public Sounds Sound => GetComponent<Sounds>();

	[System.Obsolete]
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Sound.StopSound();

		// запускаем поиск панелей и кнопок через один кадр
		//StartCoroutine(FindUINextFrame());

		//Sound.PlaySound(Sound.sounds[0]);
	}
	private IEnumerator FindUINextFrame()
	{
		yield return null; // ждём 1 кадр

		// панели
		optionsPanel = FindObjectByName("OptionsPanel");

		//постобработка
		GlobalVolume = FindObjectByName("Global Volume");

		HideAllPanels();

		// кнопки паузы
		BindButton("Main_PlayButton", PlayGame);
		BindButton("Main_QuitButton", QuitGame);
		BindButton("Main_OptionsButton", OptionsGame);

		// кнопки настроек
		BindButton("Option_BackButton", BackToMenu);
		BindButton("Option_PostProcessButton", PostProcess);
		BindButton("Option_AO", AmbientOcclusion);
		FindAmbientOcclusionFeature(); // ищем эмбиент оклюжен
		Debug.Log("UI и кнопки обновлены");
	}

	// ---------- Кнопки ----------
	public void PlayGame()
    {
        if (GameManager.instance != null)
        {
			GameManager.instance.player = null;
			GameManager.instance._PreviusPlayer = null;
			GameManager.instance.level = 1;
            GameManager.instance.mazeSize = 5;   // старт
            GameManager.instance.mazeStep = 2;   // шаг роста
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }
    public void QuitGame()
    {
        if (GameManager.instance != null) GameManager.instance.QuitGame();
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
	public void OptionsGame()
	{
		if (optionsPanel) optionsPanel.SetActive(true);
	}
	// ---------- Методы определения кнопок и панелей ----------
	private GameObject FindObjectByName(string name)
	{
		switch (name)
		{
			case "Main Camera":
				GameObject mainCam = GameObject.Find(name);
				return mainCam;

			case "Camera MonitorPixelize":
				GameObject camPixelz = GameObject.Find(name);
				return camPixelz;

			case "Global Volume":
				GameObject volume = GameObject.Find(name);
				return volume;

			default:
				RectTransform[] rects = GameObject.FindObjectsOfType<RectTransform>(true); // true = ищем и среди неактивных
				foreach (var r in rects)
				{
					if (r.name == name)
						return r.gameObject;
				}
				return null;
		}

	}
	private void BindButton(string buttonName, UnityEngine.Events.UnityAction action)
	{
		Button[] buttons = GameObject.FindObjectsOfType<Button>(true); // true = ищем и среди выключенных
		foreach (var btn in buttons)
		{
			if (btn.name == buttonName)
			{
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(action);
				Debug.Log($"Кнопка {buttonName} подключена к {action.Method.Name}");
				return;
			}
		}

		Debug.LogWarning($"Кнопка {buttonName} не найдена");
	}
	private void HideAllPanels()
	{
		if (optionsPanel) optionsPanel.SetActive(false);
	}
	// ---------- Кнопки настроек ----------
	public void BackToMenu()
	{
		if (optionsPanel) optionsPanel.SetActive(false);
	}
	public void PostProcess()
	{
		if (GlobalVolume.activeSelf)
		{
			GlobalVolume.SetActive(false);

		}
		else
		{
			GlobalVolume.SetActive(true);
		}
	}
	public void AmbientOcclusion()
	{
		if (aoFeature != null)
		{
			aoFeature.SetActive(!aoFeature.isActive);
			Debug.Log("Ambient Occlusion: " + (aoFeature.isActive ? "Включен" : "Выключен"));
		}
	}
	void FindAmbientOcclusionFeature()
	{
		// Получаем URP pipeline asset
		UniversalRenderPipelineAsset pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

		if (pipelineAsset == null)
		{
			Debug.LogError("URP Pipeline Asset не найден!");
			return;
		}

		// Получаем renderer data через рефлексию (так как поле скрыто)
		var field = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList",
			System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

		if (field == null)
		{
			Debug.LogError("Не удалось найти поле m_RendererDataList");
			return;
		}

		ScriptableRendererData[] rendererDataList = field.GetValue(pipelineAsset) as ScriptableRendererData[];

		if (rendererDataList == null || rendererDataList.Length == 0)
		{
			Debug.LogError("Renderer Data List пуст!");
			return;
		}

		// Ищем Ambient Occlusion в первом renderer data (обычно UniversalRenderer)
		foreach (var rendererData in rendererDataList)
		{
			if (rendererData == null) continue;

			foreach (var feature in rendererData.rendererFeatures)
			{
				if (feature != null && (feature.name.ToLower().Contains("ambient") ||
					feature.name.ToLower().Contains("occlusion") ||
					feature.GetType().Name.ToLower().Contains("ambient")))
				{
					aoFeature = feature;
					Debug.Log("Найден Ambient Occlusion: " + feature.name);
					return;
				}
			}
		}

		Debug.LogWarning("Ambient Occlusion feature не найден!");
	}
}