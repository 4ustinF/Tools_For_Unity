using UnityEngine;
using UnityEngine.UI;

// Copyright <2023> <William Dean Clark>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

/// <summary>
/// Tested on Unity Version: 2020.3.18f1
/// </summary>

enum CameraProjection
{
    Orthographic,
    Perspective
}

enum ScreenLocation
{
    TopLeft,
    TopMiddle,
    TopRight,
    MiddleLeft,
    MiddleMiddle,
    MiddleRight,
    BottomLeft,
    BottomMiddle,
    BottomRight,
}

public class EasyMiniMapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _followTransform = null;
    [SerializeField] private Camera _camera = null;
    [SerializeField] private RectTransform _maskTransform = null;
    [SerializeField] private RectTransform _borderTransform = null;
    [SerializeField] private RenderTexture _miniMapRT = null;

    [Header("Map Settings")]
    [SerializeField] private ScreenLocation _screenLocation = ScreenLocation.TopLeft;
    private ScreenLocation _previousScreenLocation = ScreenLocation.TopLeft;
    [SerializeField] private Vector2 _mapSize = new Vector2(256.0f, 256.0f);
    [SerializeField] private Vector2 _mapOffset = new Vector2(150.0f, -150.0f);
    [SerializeField] private int _resolution = 512;
    [SerializeField] private bool _isGame2D = true;

    [Header("Camera Settings")]
    [SerializeField] private CameraProjection _cameraProjection = CameraProjection.Orthographic;
    [SerializeField] private LayerMask _cullingMask = new LayerMask();
    // Orthographic Settings
    [SerializeField] private float _cameraSize = 25.0f;
    // Perspective Settings
    [SerializeField] private float _cameraHeight = 10.0f;

    [Header("Mask Settings")]
    [SerializeField] private Sprite _maskSprite = null;

    [Header("Border Settings")]
    [SerializeField] private Sprite _borderSprite = null;
    [SerializeField] private Color _borderColor = Color.white;
    [SerializeField] private Vector2 _borderScale = new Vector2(1.02f, 1.02f);

    private void FixedUpdate()
    {
        FollowTransform(_followTransform.position);
    }

    /// <summary>
    /// Updates the camera position so it is always centered around the _followTransform
    /// </summary>
    private void FollowTransform(Vector3 followPosition)
    {
        _camera.transform.position = _isGame2D ? new Vector3(followPosition.x, followPosition.y, -_cameraHeight)
            : new Vector3(followPosition.x, _cameraHeight, followPosition.z);
    }


    #region ---Validation---

#if UNITY_EDITOR
    private void OnValidate() // OnValidate is called when a variable is changed in the editor
    {
        ValidateCamera();
        ValidateMap();
        ValidateBorder();
        ValidateOther();
    }
#endif

    private void ValidateCamera()
    {
        _camera.cullingMask = _cullingMask;
        _camera.transform.eulerAngles = new Vector3(_isGame2D ? 0.0f : 90.0f, 0.0f, 0.0f);

        switch (_cameraProjection)
        {
            case CameraProjection.Orthographic:
                _camera.orthographic = true;
                _camera.orthographicSize = _cameraSize;
                break;
            case CameraProjection.Perspective:
                _camera.orthographic = false;
                _camera.fieldOfView = _cameraSize;
                break;
        }

        if (_followTransform != null)
        {
            float camPosX = _followTransform.position.x;
            float camPosY = _isGame2D ? _followTransform.position.y : _cameraHeight;
            float camPosZ = _isGame2D ? _cameraHeight : _followTransform.position.z;
            FollowTransform(new Vector3(camPosX, camPosY, camPosZ));
        }
        else
        {
            float camPosX = 0.0f;
            float camPosY = _isGame2D ? 0.0f : _cameraHeight;
            float camPosZ = _isGame2D ? _cameraHeight : 0.0f;
            FollowTransform(new Vector3(camPosX, camPosY, camPosZ));
        }

    }

    private void ValidateMap()
    {
        if (_previousScreenLocation != _screenLocation)
        {
            _previousScreenLocation = _screenLocation;

            switch (_screenLocation)
            {
                case ScreenLocation.TopLeft:
                    UpdateAnchors(new Vector2(150.0f, -150.0f), new Vector2(0.0f, 1.0f));
                    break;
                case ScreenLocation.TopMiddle:
                    UpdateAnchors(new Vector2(0.0f, -150.0f), new Vector2(0.5f, 1.0f));
                    break;
                case ScreenLocation.TopRight:
                    UpdateAnchors(new Vector2(-150.0f, -150.0f), new Vector2(1.0f, 1.0f));
                    break;
                case ScreenLocation.MiddleLeft:
                    UpdateAnchors(new Vector2(150.0f, 0.0f), new Vector2(0.0f, 0.5f));
                    break;
                case ScreenLocation.MiddleMiddle:
                    UpdateAnchors(new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.5f));
                    break;
                case ScreenLocation.MiddleRight:
                    UpdateAnchors(new Vector2(-150.0f, 0.0f), new Vector2(1.0f, 0.5f));
                    break;
                case ScreenLocation.BottomLeft:
                    UpdateAnchors(new Vector2(150.0f, 150.0f), new Vector2(0.0f, 0.0f));
                    break;
                case ScreenLocation.BottomMiddle:
                    UpdateAnchors(new Vector2(0.0f, 150.0f), new Vector2(0.5f, 0.0f));
                    break;
                case ScreenLocation.BottomRight:
                    UpdateAnchors(new Vector2(-150.0f, 150.0f), new Vector2(1.0f, 0.0f));
                    break;
            }
        }

        void UpdateAnchors(Vector2 offSet, Vector2 anchorPos)
        {
            _mapOffset = offSet;
            _maskTransform.anchorMin = anchorPos;
            _maskTransform.anchorMax = anchorPos;
        }

        _maskTransform.anchoredPosition = _mapOffset; // Map Position
        _maskTransform.sizeDelta = new Vector2(_mapSize.x, _mapSize.y); // Size of map

        _miniMapRT.Release();
        _miniMapRT.width = _resolution;
        _miniMapRT.height = _resolution;
    }

    private void ValidateBorder()
    {
        _borderTransform.localScale = _borderScale;

        // Update Border Image
        var borderImage = _borderTransform.gameObject.GetComponent<Image>();

        borderImage.enabled = _borderSprite != null;
        borderImage.sprite = _borderSprite;
        borderImage.color = _borderColor;
    }

    private void ValidateOther()
    {
        // Update Mask Image
        var maskImage = _maskTransform.gameObject.GetComponent<Image>();
        maskImage.sprite = _maskSprite;
    }

    #endregion

}
