using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour
{
    public RawImage mapImage;
    public Camera mapCamera;
    public int width = 550;
    public int height = 550;
    
    private RenderTexture rt;
    private Texture2D mapTexture;
    
    void Start()
    {
        rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        rt.Create();
        
        mapCamera.targetTexture = rt;
        mapCamera.clearFlags = CameraClearFlags.SolidColor;
        mapCamera.backgroundColor = new Color(0, 0, 0, 0);
        
        mapTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        mapImage.texture = mapTexture;
        
        if (mapImage.material == null)
        {
            mapImage.material = new Material(Shader.Find("UI/Default"));
        }
    }
    
    void Update()
    {
        // Рендерим камеру каждый кадр
        mapCamera.Render();
        
        // Копируем в текстуру
        RenderTexture.active = rt;
        mapTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        mapTexture.Apply();
        RenderTexture.active = null;
        
        // Делаем чёрный фон прозрачным
        Color[] pixels = mapTexture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r < 0.1f && pixels[i].g < 0.1f && pixels[i].b < 0.1f)
            {
                pixels[i].a = 0f;
            }
            else
            {
                pixels[i].a = 1f;
            }
        }
        mapTexture.SetPixels(pixels);
        mapTexture.Apply();
    }
    
    void OnDestroy()
    {
        if (rt != null) rt.Release();
        if (mapTexture != null) Destroy(mapTexture);
    }
}