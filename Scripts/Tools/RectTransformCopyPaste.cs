using UnityEditor;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RectTransformCopyPaste : EditorWindow
{
    
    private static List<RectTransform> copiedRectTransforms = new List<RectTransform>();
    private static List<TextMeshProUGUI> copiedTextMeshes = new List<TextMeshProUGUI>();

    [MenuItem("Tools/Copy RectTransforms with All Descendants %#z")] // Shortcut: Shift + Ctrl + C (Shift + Cmd + C on Mac)
    public static void CopyRectTransforms()
    {
        if (Selection.activeTransform == null)
        {
            Debug.LogWarning("No GameObject selected to copy from.");
            return;
        }

        Transform rootTransform = Selection.activeTransform;
        copiedRectTransforms.Clear();
        copiedTextMeshes.Clear();

        // Copy RectTransform properties recursively
        CopyRectTransformRecursive(rootTransform);
    }

    private static void CopyRectTransformRecursive(Transform transform)
    {
        if (transform is RectTransform rectTransform)
        {
            copiedRectTransforms.Add(rectTransform);
            copiedTextMeshes.Add(rectTransform.GetComponent<TextMeshProUGUI>());
            Debug.Log("Copied RectTransform from: " + rectTransform.name);
        }
        else
        {
            Debug.LogWarning($"No UI RectTransform found on: {transform.name}. It will be skipped.");
            return;
        }

        // Recursively copy properties from all children
        foreach (Transform child in transform)
        {
            CopyRectTransformRecursive(child);
        }
    }

    [MenuItem("Tools/Paste RectTransforms to Corresponding Descendants %#x")] // Shortcut: Shift + Ctrl + Z (Shift + Cmd + Z on Mac)
    public static void PasteRectTransforms()
    {
        if (copiedRectTransforms == null || copiedRectTransforms.Count == 0)
        {
            Debug.LogWarning("No RectTransform data to paste. Make sure you've copied the RectTransforms first.");
            return;
        }

        if (Selection.activeTransform == null)
        {
            Debug.LogWarning("No GameObject selected to paste to.");
            return;
        }

        Transform targetRootTransform = Selection.activeTransform;

        // Get all descendants of the target root
        List<RectTransform> targetRectTransforms = new List<RectTransform>();
        GetAllDescendantsRecursive(targetRootTransform, targetRectTransforms);

        // Check if the number of descendants matches the number of copied objects
        if (targetRectTransforms.Count != copiedRectTransforms.Count)
        {
            Debug.LogError($"Mismatch in the number of copied and target descendants. Copied: {copiedRectTransforms.Count}, Target: {targetRectTransforms.Count}. Please ensure they match.");
            return;
        }

        for (int i = 0; i < copiedRectTransforms.Count; i++)
        {
            RectTransform sourceRectTransform = copiedRectTransforms[i];
            RectTransform targetRectTransform = targetRectTransforms[i];

            if (sourceRectTransform == null)
            {
                Debug.LogWarning($"Source RectTransform is null for index {i}. Skipping...");
                continue;
            }

            if (targetRectTransform != null)
            {
                // Record the target transform to support undo
                Undo.RecordObject(targetRectTransform, "Paste RectTransform");

                // Copy anchors, position, size delta, and local scale
                targetRectTransform.anchorMin = sourceRectTransform.anchorMin;
                targetRectTransform.anchorMax = sourceRectTransform.anchorMax;
                targetRectTransform.anchoredPosition = sourceRectTransform.anchoredPosition;
                targetRectTransform.sizeDelta = sourceRectTransform.sizeDelta;
                targetRectTransform.pivot = sourceRectTransform.pivot;
                targetRectTransform.localRotation = sourceRectTransform.localRotation;
                targetRectTransform.localScale = sourceRectTransform.localScale;

                Debug.Log("RectTransform pasted to: " + targetRectTransform.name);

                // Check if both source and target have TextMeshProUGUI component
                TextMeshProUGUI sourceTextMeshPro = copiedTextMeshes[i];
                if (sourceTextMeshPro != null)
                {
                    TextMeshProUGUI targetTextMeshPro = targetRectTransform.GetComponent<TextMeshProUGUI>();
                    if (targetTextMeshPro != null)
                    {
                        PasteTextMeshProProperties(sourceTextMeshPro, targetTextMeshPro);
                    }
                    else
                    {
                        Debug.LogWarning($"Target {targetRectTransform.name} does not have a TextMeshProUGUI component to paste values to.");
                    }
                }
            }
        }
    }

    private static void GetAllDescendantsRecursive(Transform transform, List<RectTransform> descendants)
    {
        if (transform is RectTransform rectTransform)
        {
            descendants.Add(rectTransform);
        }

        // Recursively get all children
        foreach (Transform child in transform)
        {
            GetAllDescendantsRecursive(child, descendants);
        }
    }

    private static void PasteTextMeshProProperties(TextMeshProUGUI sourceTextMeshPro, TextMeshProUGUI targetTextMeshPro)
    {
        // Record the target text component to support undo
        Undo.RecordObject(targetTextMeshPro, "Paste TextMeshProUGUI Properties");

        // Copy relevant TextMeshProUGUI properties
        targetTextMeshPro.enableAutoSizing = sourceTextMeshPro.enableAutoSizing;
        targetTextMeshPro.fontSizeMin = sourceTextMeshPro.fontSizeMin;
        targetTextMeshPro.fontSizeMax = sourceTextMeshPro.fontSizeMax;
        targetTextMeshPro.alignment = sourceTextMeshPro.alignment;
        targetTextMeshPro.enableWordWrapping = sourceTextMeshPro.enableWordWrapping;
        targetTextMeshPro.margin = sourceTextMeshPro.margin;

        Debug.Log("TextMeshProUGUI properties pasted to: " + targetTextMeshPro.name);
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    private static RectTransform copiedRectTransform;
    private static TextMeshProUGUI copiedTextMeshPro;

    [MenuItem("Tools/Copy RectTransform %#q")] // Shortcut: Shift + Ctrl + C (Shift + Cmd + C on Mac)
    public static void CopyRectTransform()
    {
        if (Selection.activeTransform != null && Selection.activeTransform is RectTransform)
        {
            copiedRectTransform = Selection.activeTransform as RectTransform;
            copiedTextMeshPro = copiedRectTransform.GetComponent<TextMeshProUGUI>();

            Debug.Log("RectTransform copied from: " + copiedRectTransform.name);
        }
        else
        {
            Debug.LogWarning("No UI RectTransform selected to copy from.");
        }
    }

    [MenuItem("Tools/Paste RectTransform %#w")] // Shortcut: Shift + Ctrl + Z (Shift + Cmd + Z on Mac)
    public static void PasteRectTransform()
    {
        if (copiedRectTransform != null && Selection.activeTransform != null && Selection.activeTransform is RectTransform)
        {
            RectTransform targetRectTransform = Selection.activeTransform as RectTransform;

            // Record the target transform to support undo
            Undo.RecordObject(targetRectTransform, "Paste RectTransform");

            // Copy anchors, position, size delta, and local scale
            targetRectTransform.anchorMin = copiedRectTransform.anchorMin;
            targetRectTransform.anchorMax = copiedRectTransform.anchorMax;
            targetRectTransform.anchoredPosition = copiedRectTransform.anchoredPosition;
            targetRectTransform.sizeDelta = copiedRectTransform.sizeDelta;
            targetRectTransform.pivot = copiedRectTransform.pivot;
            targetRectTransform.localRotation = copiedRectTransform.localRotation;
            targetRectTransform.localScale = copiedRectTransform.localScale;

            Debug.Log("RectTransform pasted to: " + targetRectTransform.name);

            // Check if both copied and target have TextMeshProUGUI component
            if (copiedTextMeshPro != null)
            {
                TextMeshProUGUI targetTextMeshPro = targetRectTransform.GetComponent<TextMeshProUGUI>();
                if (targetTextMeshPro != null)
                {
                    PasteTextMeshProProperties(targetTextMeshPro);
                }
                else
                {
                    Debug.LogWarning("Target does not have a TextMeshProUGUI component to paste values to.");
                }
            }
        }
        else
        {
            Debug.LogWarning("No RectTransform data to paste, or no UI RectTransform selected.");
        }
    }

    private static void PasteTextMeshProProperties(TextMeshProUGUI targetTextMeshPro)
    {
        // Record the target text component to support undo
        Undo.RecordObject(targetTextMeshPro, "Paste TextMeshProUGUI Properties");

        // Copy relevant TextMeshProUGUI properties
        targetTextMeshPro.enableAutoSizing = copiedTextMeshPro.enableAutoSizing;
        targetTextMeshPro.fontSizeMin = copiedTextMeshPro.fontSizeMin;
        targetTextMeshPro.fontSizeMax = copiedTextMeshPro.fontSizeMax;
        targetTextMeshPro.alignment = copiedTextMeshPro.alignment;
        targetTextMeshPro.enableWordWrapping = copiedTextMeshPro.enableWordWrapping;
        targetTextMeshPro.margin = copiedTextMeshPro.margin;

        Debug.Log("TextMeshProUGUI properties pasted to: " + targetTextMeshPro.name);
    }
    
}
