/*
 * Prefab Icon Exporter
 *
 * Purpose:
 * Batch renders selected prefabs to transparent PNG icons for UI use.
 *
 * Attribution:
 * This editor tool was created with AI assistance (Pretty much wrote it all and adapted
 * for this project. It uses standard Unity editor scripting techniques
 * such as EditorWindow, PrefabUtility, Camera rendering, and RenderTexture capture.
 *
 * Project: Garbage Cleanup (Mini Capstone)
 * Author: Ciaran McIlvaney
 * 
 */

using System.IO;
using UnityEditor;
using UnityEngine;

public class PrefabIconExporter : EditorWindow
{
    [Header("Export Settings")]
    private int iconSize = 256;
    private string outputFolder = "Assets/ExportedIcons";

    [Header("Camera Settings")]
    private Vector3 cameraDirection = new Vector3(1.8f, 1.2f, -1.8f);
    private float framingPadding = 1.35f;

    [Header("Background")]
    private Color backgroundColor = new Color(0f, 0f, 0f, 0f); // transparent

    [MenuItem("Tools/Prefab Icon Exporter")]
    public static void ShowWindow()
    {
        GetWindow<PrefabIconExporter>("Prefab Icon Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Export Transparent Prefab Icons", EditorStyles.boldLabel);

        GUILayout.Space(10);

        iconSize = EditorGUILayout.IntField("Icon Size", iconSize);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        GUILayout.Space(10);

        cameraDirection = EditorGUILayout.Vector3Field("Camera Direction", cameraDirection);
        framingPadding = EditorGUILayout.FloatField("Framing Padding", framingPadding);

        GUILayout.Space(10);

        backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);

        GUILayout.Space(15);

        EditorGUILayout.HelpBox(
            "Select one or more prefab assets in the Project window, then click the button below to export transparent PNG icons.",
            MessageType.Info
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Export Selected Prefabs", GUILayout.Height(35)))
        {
            ExportSelectedPrefabs();
        }
    }

    private void ExportSelectedPrefabs()
    {
        Object[] selectedObjects = Selection.objects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogWarning("[PrefabIconExporter] No prefabs selected.");
            return;
        }

        CreateFolderIfNeeded(outputFolder);

        int exportedCount = 0;

        foreach (Object obj in selectedObjects)
        {
            GameObject prefab = obj as GameObject;

            if (prefab == null)
                continue;

            if (PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.NotAPrefab)
                continue;

            bool success = ExportPrefabIcon(prefab);

            if (success)
                exportedCount++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"[PrefabIconExporter] Finished exporting {exportedCount} icon(s).");
    }

    private bool ExportPrefabIcon(GameObject prefab)
    {
        // Create a temporary instance of the prefab
        GameObject tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (tempInstance == null)
        {
            Debug.LogWarning($"[PrefabIconExporter] Could not instantiate prefab: {prefab.name}");
            return false;
        }

        // Reset transform so bounds calculations are predictable
        tempInstance.transform.position = Vector3.zero;
        tempInstance.transform.rotation = Quaternion.identity;
        tempInstance.transform.localScale = Vector3.one;

        // Collect all renderers from the prefab
        Renderer[] renderers = tempInstance.GetComponentsInChildren<Renderer>();

        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning($"[PrefabIconExporter] No renderers found on prefab: {prefab.name}");
            DestroyImmediate(tempInstance);
            return false;
        }

        // Calculate combined bounds so we know how large the object is
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        Vector3 center = bounds.center;

        // Largest object dimension used for framing
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        // Create temporary camera
        GameObject cameraObject = new GameObject("TempIconCamera");
        Camera cam = cameraObject.AddComponent<Camera>();

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = backgroundColor;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 100f;

        // Orthographic camera gives cleaner UI icons with no perspective distortion
        cam.orthographic = true;
        cam.orthographicSize = maxSize * 0.6f * framingPadding;

        // Normalize direction and position camera
        Vector3 camDir = cameraDirection.normalized;
        float distance = maxSize * 3f;

        cam.transform.position = center + camDir * distance;
        cam.transform.LookAt(center);

        // Create temporary directional light
        GameObject lightObject = new GameObject("TempIconLight");
        Light tempLight = lightObject.AddComponent<Light>();
        tempLight.type = LightType.Directional;
        tempLight.intensity = 1.2f;
        tempLight.transform.rotation = Quaternion.Euler(40f, -30f, 0f);

        // Create render texture
        RenderTexture rt = new RenderTexture(iconSize, iconSize, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        RenderTexture.active = rt;

        // Render the object
        cam.Render();

        // Read pixels into a Texture2D
        Texture2D outputTexture = new Texture2D(iconSize, iconSize, TextureFormat.RGBA32, false);
        outputTexture.ReadPixels(new Rect(0, 0, iconSize, iconSize), 0, 0);
        outputTexture.Apply();

        // Convert to PNG
        byte[] pngBytes = outputTexture.EncodeToPNG();

        string filePath = Path.Combine(outputFolder, prefab.name + ".png");
        File.WriteAllBytes(filePath, pngBytes);

        // Cleanup temp objects
        RenderTexture.active = null;
        cam.targetTexture = null;

        DestroyImmediate(rt);
        DestroyImmediate(outputTexture);
        DestroyImmediate(cameraObject);
        DestroyImmediate(lightObject);
        DestroyImmediate(tempInstance);

        Debug.Log($"[PrefabIconExporter] Exported: {filePath}");
        return true;
    }

    private void CreateFolderIfNeeded(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] splitPath = folderPath.Split('/');
        string currentPath = splitPath[0];

        for (int i = 1; i < splitPath.Length; i++)
        {
            string nextPath = currentPath + "/" + splitPath[i];

            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, splitPath[i]);
            }

            currentPath = nextPath;
        }
    }
}