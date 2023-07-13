using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

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
/// Tested on Unity Version: 2019.1.10f1
/// Logs the total number of vertices in a selection or in the scene.
/// </summary>

#if UNITY_EDITOR
public class VertexCount : MonoBehaviour
{
    [MenuItem("External/VertexCount/Log Selected Vertex Count")]
    static private void LogSelectedVertexCount()
    {
        // Early return if no objects are selected
        if (Selection.gameObjects.Length <= 0)
        {
            Debug.Log("No Objects Selected");
            return;
        }

        // Vertex count
        int count = 0;

        foreach (GameObject obj in Selection.gameObjects)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                count += meshFilter.sharedMesh.vertexCount;
            }

            ProBuilderMesh proBuilderMesh = obj.GetComponent<ProBuilderMesh>();
            if(proBuilderMesh != null)
            {
                count += proBuilderMesh.vertexCount;
            }
        }

        Debug.Log($"Vertex Count: {count}");
    }

    [MenuItem("External/VertexCount/Log Total Vertex Count")]
    static private void LogTotalVertexCount()
    {
        int count = 0;

        var meshFilters = GameObject.FindObjectsOfType<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            count += meshFilter.sharedMesh.vertexCount;
        }

        var proBuilderMeshs = GameObject.FindObjectsOfType<ProBuilderMesh>();
        foreach (var mesh in proBuilderMeshs)
        {
            count += mesh.vertexCount;
        }

        Debug.Log($"Vertex Count: {count}");
    }
}
#endif