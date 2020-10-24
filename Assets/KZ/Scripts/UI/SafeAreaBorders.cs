using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KZ;
using System.ComponentModel;

[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class SafeAreaBorders : MonoBehaviour {


    Rect _safeArea = Rect.zero;
    Resolution _resolution;
    RectTransform _rt = null;
    [SerializeField] Padding _padding = null;


    private void Start() {
        _rt = GetComponent<RectTransform>();
        UpdateSafeArea();
    }

    private void LateUpdate() {
        UpdateSafeArea();
    }


    private void UpdateSafeArea() {
#if UNITY_EDITOR
        if (!Application.IsPlaying(gameObject))
            UpdateSafeArea_EditorMode();
        else
#endif
            UpdateSafeArea_PlayMode();
    }
    private void UpdateSafeArea_PlayMode() {
        if (_safeArea == Screen.safeArea && _resolution.Equals(Screen.currentResolution))
            return;

        Debug.Log($"safe area: {_safeArea}");

        _rt.pivot = Vector2.zero;
        _rt.rotation = Quaternion.identity;
        _rt.localScale = Vector3.one;
        _rt.anchoredPosition = Vector2.zero;
        _rt.sizeDelta = Vector2.zero;

        // Get safe area (screen)
        _safeArea = Screen.safeArea;
        _resolution = Screen.currentResolution;

        // Convert to canvas scale
        Vector2 anchorMin = _safeArea.position;
        Vector2 anchorMax = _safeArea.position + _safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMax.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.y /= Screen.height;

        // Add Padding
        var cv = _rt.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        var targetMaxX = cv.sizeDelta.x;
        var targetMaxY = cv.sizeDelta.y;

        var xMin = _padding.left / targetMaxX;
        var yMin = _padding.bottom / targetMaxY;

        var yMax = (targetMaxY - _padding.top) / targetMaxY;
        var xMax = (targetMaxX - _padding.right) / targetMaxX;

        anchorMin.x = Math.Max(anchorMin.x, xMin);
        anchorMin.y = Math.Max(anchorMin.y, yMin);

        anchorMax.x = Math.Min(anchorMax.x, xMax);
        anchorMax.y = Math.Min(anchorMax.y, yMax);

        // Set values to rect transform
        _rt.anchorMin = anchorMin;
        _rt.anchorMax = anchorMax;
        Canvas.ForceUpdateCanvases();
    }

    [Serializable]
    public class Padding {
        public float left, right, top, bottom;
    }


    [Editor]
    public void UpdateSafeArea_EditorMode() {
        var rt = GetComponent<RectTransform>();

        // force use stretch mode
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = Vector2.zero;
        rt.rotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        // Add padding
        rt.anchoredPosition = new Vector2(_padding.left, _padding.bottom);
        rt.sizeDelta = new Vector2(-_padding.left - _padding.right, -_padding.top - _padding.bottom);
    }
}