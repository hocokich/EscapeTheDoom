using UnityEngine;
using UnityEngine.UI;

public class AlphaDiagnostics : MonoBehaviour
{
	public RawImage targetImage;
	public RenderTexture renderTexture;

	void Start()
	{
		// ТЕСТ 1: Проверяем, есть ли альфа в Render Texture
		Debug.Log("=== ТЕСТ 1: Render Texture ===");
		Debug.Log("Формат Render Texture: " + renderTexture.format);
		Debug.Log("Поддерживает ли альфа? " + (renderTexture.format.ToString().Contains("A") ? "ДА" : "НЕТ"));

		// ТЕСТ 2: Создаём временную текстуру и читаем пиксели
		Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
		RenderTexture.active = renderTexture;
		tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
		tex.Apply();
		RenderTexture.active = null;

		// Берём центральный пиксель
		Color centerPixel = tex.GetPixel(renderTexture.width / 2, renderTexture.height / 2);
		Debug.Log($"Центральный пиксель - R:{centerPixel.r:F2} G:{centerPixel.g:F2} B:{centerPixel.b:F2} A:{centerPixel.a:F2}");

		// ТЕСТ 3: Проверяем материал Raw Image
		Debug.Log("=== ТЕСТ 3: Raw Image ===");
		Material mat = targetImage.material;
		if (mat != null)
		{
			Debug.Log("Материал: " + mat.name);
			Debug.Log("Шейдер: " + mat.shader.name);
			Debug.Log("Render Queue: " + mat.renderQueue);
		}
		else
		{
			Debug.Log("Материал: НЕТ (используется стандартный)");
		}

		// ТЕСТ 4: Создаём тестовый спрайт с принудительной прозрачностью
		Debug.Log("=== ТЕСТ 4: Принудительная прозрачность ===");
		Texture2D testTex = new Texture2D(100, 100, TextureFormat.RGBA32, false);
		for (int x = 0; x < 100; x++)
		{
			for (int y = 0; y < 100; y++)
			{
				// Создаём градиент прозрачности: слева прозрачно, справа непрозрачно
				float alpha = (float)x / 100f;
				testTex.SetPixel(x, y, new Color(1, 0, 0, alpha));
			}
		}
		testTex.Apply();

		// Создаём временный спрайт и материал
		Sprite testSprite = Sprite.Create(testTex, new Rect(0, 0, 100, 100), Vector2.zero);
		Material testMat = new Material(Shader.Find("UI/Default"));

		// Меняем у Raw Image текстуру и материал
		Texture oldTex = targetImage.texture;
		Material oldMat = targetImage.material;

		targetImage.texture = testTex;
		targetImage.material = testMat;

		Debug.Log("Создан тестовый градиент. Должен быть красный цвет, слева прозрачный, справа непрозрачный.");
		Debug.Log("Если вы видите сплошной красный без градиента - UI игнорирует альфа.");
		Debug.Log("Если видите градиент - альфа работает, проблема в Render Texture.");

		// Через 3 секунды вернём обратно
		Invoke("RestoreOriginal", 3.0f);
	}

	void RestoreOriginal()
	{
		// Здесь нужно будет восстановить оригинальные текстуру и материал
		Debug.Log("Тест завершён");
	}
}