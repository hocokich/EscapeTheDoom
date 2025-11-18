using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // для Button
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//using static UnityEditor.Experimental.GraphView.GraphView;      

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Прогресс/уровни
    public int level;
    public int mazeSize;
    public int mazeStep;

    // UI панели
    private GameObject pausePanel;
    private GameObject winPanel;
    private GameObject losePanel;
    private GameObject optionsPanel;

	// Эффекты
	private GameObject OldMonitorPanel;
    private GameObject GlobalVolume;
    private GameObject CameraPixelize;
	private ScriptableRendererFeature aoFeature;

	public bool isPaused = false;

    public GameObject player;
    public Player PreviusPlayer;
    public GameObject MainCamera;
    public Sounds Sound => GetComponent<Sounds>();

	[System.Obsolete]
	void Awake()
    {
		if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            instance.level = 1;
			instance.mazeSize = 5;
            instance.mazeStep = 2;
		}
        else
        {
            //Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) Pause();
			else if (optionsPanel.active) optionsPanel.SetActive(false);
			else Resume();
        }
    }

    // --- Когда сцена загружена ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
		Sound.StopSound();

		Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = (scene.name == "GameScene") ? CursorLockMode.Locked : CursorLockMode.None;

        // запускаем поиск панелей и кнопок через один кадр
        StartCoroutine(FindUINextFrame());

		Sound.PlaySound(Sound.sounds[0]);
	}

    private IEnumerator FindUINextFrame()
    {
        yield return null; // ждём 1 кадр

        //игрок
        player = FindPlayer("Player");
        //
        MainCamera = FindObjectByName("Main Camera");

        // панели
        pausePanel = FindObjectByName("PausePanel");
        winPanel = FindObjectByName("WinPanel");
        losePanel = FindObjectByName("LosePanel");
        optionsPanel = FindObjectByName("OptionsPanel");
        //постобработка
		OldMonitorPanel = FindObjectByName("OldMonitorScreenPanel");
        GlobalVolume = FindObjectByName("Global Volume");
        CameraPixelize = FindObjectByName("Camera MonitorPixelize");

		HideAllPanels();

        // кнопки паузы
        BindButton("Pause_ResumeButton", Resume);
        BindButton("Pause_RestartButton", RestartGame);
		BindButton("Pause_Options", OptionsGame);
        BindButton("Pause_MenuButton", GoToMenu);
		BindButton("Pause_QuitButton", QuitGame);

        // кнопки победы
        BindButton("Win_RestartButton", RestartGame);
        BindButton("Win_NextButton", NextLevel);
        BindButton("Win_MenuButton", GoToMenu);
        BindButton("Win_QuitButton", QuitGame);

        // кнопки поражения
        BindButton("Lose_RestartButton", RestartGame);
        BindButton("Lose_MenuButton", GoToMenu);
        BindButton("Lose_QuitButton", QuitGame);

		// кнопки настроек
		BindButton("Option_BackButton", BackToPause);
		BindButton("Option_OldMonitorButton", OldMonitorMode);
		BindButton("Option_PostProcessButton", PostProcess);
		BindButton("Option_AO", AmbientOcclusion);
        FindAmbientOcclusionFeature(); // ищем эмбиент оклюжен
		OldMonitorMode();//Сразу выключаем монитор
		Debug.Log("UI и кнопки обновлены");
    }

	// --- Поиск объекта по имени среди любых RectTransform ---
	[System.Obsolete]
	private GameObject FindPlayer(string name)
    {
		Player[] players = FindObjectsOfType<Player>();
		foreach (var p in players)
		{
			if (p.name == name)
				return p.gameObject;
			Debug.Log("Игрок найден");

		}
		Debug.Log("Игрок не найден");
		return null;
	}
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
        if (pausePanel) pausePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    // ---------- Пауза ----------
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        if (pausePanel) pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        if (pausePanel) pausePanel.SetActive(false);
    }

    // ---------- Состояния игры ----------
    public void Win()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        if (winPanel) winPanel.SetActive(true);
    }

    public void Lose()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        if (losePanel) losePanel.SetActive(true);
    }
    // ---------- Кнопки ----------
    public void RestartGame()
    {
        instance.level = 1;
		Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void NextLevel()
    {
		instance.PreviusPlayer = player.GetComponent<Player>();
		instance.level++;
		instance.mazeSize += mazeStep;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OptionsGame()
    {
		if (optionsPanel) optionsPanel.SetActive(true);
	}

	public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	// ---------- Кнопки настроек ----------

	public void BackToPause()
	{
		if (optionsPanel) optionsPanel.SetActive(false);
	}

	[System.Obsolete]
	public void OldMonitorMode()
	{
        if (OldMonitorPanel.active)
        {
			CameraPixelize.SetActive(false);
			OldMonitorPanel.SetActive(false);

		}
        else
        {
			CameraPixelize.SetActive(true);
			OldMonitorPanel.SetActive(true);
		}
	}
	public void PostProcess()
	{
		if (GlobalVolume.active)
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