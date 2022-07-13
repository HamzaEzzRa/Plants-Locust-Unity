using UnityEngine;

[RequireComponent(typeof(Camera))]
public class IconGenerator : MonoBehaviour
{
    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [SerializeField] private bool transparent = true;

    private Camera cam;
    [SerializeField] private string savePath = "Assets/Textures/CM_Icons";
    
    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void Screenshot()
    {
        if (cam == null)
        {
            if (!TryGetComponent(out cam))
                return;
        }

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        cam.targetTexture = renderTexture;
        CameraClearFlags clearFlags = cam.clearFlags;

        if (transparent)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.clear;
        }

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGBA32, false);
        cam.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        cam.targetTexture = null;
        RenderTexture.active = null;
        cam.clearFlags = clearFlags;

        if (Application.isEditor)
            DestroyImmediate(renderTexture);
        else
            Destroy(renderTexture);

        byte[] data = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath + "/Icon_" + System.DateTime.Now.ToString("dd-HH-mm-ss-ff") + ".png", data);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
