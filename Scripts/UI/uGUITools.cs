﻿using UnityEditor;
using UnityEngine;

// Copyright <2023> <William Clark>
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
/// AnchorsToCorners() is a method that adjusts the anchors of a selected RectTransform based on its offset and the size of its parent RectTransform. 
/// This operation is commonly used in Unity UI to maintain consistent positioning and scaling of UI elements when the parent container is resized.
/// </summary>

/// <summary>
/// CornersToAnchors() is a method that resets the offset values of a selected RectTransform, effectively aligning it with the parent container or resetting any 
/// positional adjustments made to the transform. This can be useful when you want to ensure consistent positioning or remove any manual adjustments applied to the transform.
/// </summary>

#if UNITY_EDITOR
public class uGUITools : MonoBehaviour {
	[MenuItem("External/uGUI/Anchors to Corners %[")]
	static void AnchorsToCorners(){
		RectTransform t = Selection.activeTransform as RectTransform;
		RectTransform pt = Selection.activeTransform.parent as RectTransform;

		if (t == null || pt == null)
		{
			return;
		}
		
		Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
		                                    t.anchorMin.y + t.offsetMin.y / pt.rect.height);
		Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
		                                    t.anchorMax.y + t.offsetMax.y / pt.rect.height);

		t.anchorMin = newAnchorsMin;
		t.anchorMax = newAnchorsMax;
		t.offsetMin = t.offsetMax = new Vector2(0.0f, 0.0f);
	}

	[MenuItem("External/uGUI/Corners to Anchors %]")]
	static void CornersToAnchors(){
		RectTransform t = Selection.activeTransform as RectTransform;

		if (t == null)
		{
			return;
		}

		t.offsetMin = t.offsetMax = new Vector2(0.0f, 0.0f);
	}
}
#endif
