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
/// This script will parent all game objects selected in the hierarchy to a new empty game object named "New Parent".
/// You can find it in the editor under "Edit/Create Parent for Selected"
/// </summary>

#if UNITY_EDITOR
public class CreateParent : MonoBehaviour
{
    private string _parentName = "New Parent";

    [MenuItem("External/Create Parent for Selected")]
    static private void CreateParentForSelected()
    {
        // Early return if no objects are selected
        if (Selection.gameObjects.Length <= 0)
        {
            return;
        }

        // Create empty parent GameObject
        GameObject parentGameObject = new GameObject();

        // Make sure its transform is reset
        parentGameObject.transform.position = Vector3.zero;
        parentGameObject.transform.eulerAngles = Vector3.zero;
        parentGameObject.transform.localScale = Vector3.one;
        parentGameObject.name = _parentName;

        // Record the creation of the parent object for undo
        Undo.RegisterCreatedObjectUndo(parentGameObject, "Create Parent");

        // Set the new parent for each selected object
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.SetTransformParent(obj.transform, parentGameObject.transform, "Set new parent");
        }
    }
}
#endif
