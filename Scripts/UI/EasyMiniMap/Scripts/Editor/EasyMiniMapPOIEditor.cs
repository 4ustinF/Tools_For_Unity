using UnityEditor;
using UnityEngine;

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
/// Editor script that attaches the SpriteRenderer component to the SpriteRenderer property.
/// </summary>

[CustomEditor(typeof(EasyMiniMapPOI))] // Replace 'MyComponent' with the name of your script
public class EasyMiniMapPOIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EasyMiniMapPOI easyMiniMapPOI = (EasyMiniMapPOI)target;

        // Automatically assign the Image component reference if it's missing
        if (easyMiniMapPOI.SpriteRenderer == null)
        {
            SpriteRenderer spriteComponent = easyMiniMapPOI.GetComponent<SpriteRenderer>();
            if (spriteComponent != null)
            {
                easyMiniMapPOI.SpriteRenderer = spriteComponent;
                EditorUtility.SetDirty(easyMiniMapPOI);
            }
        }

        DrawDefaultInspector();
    }
}