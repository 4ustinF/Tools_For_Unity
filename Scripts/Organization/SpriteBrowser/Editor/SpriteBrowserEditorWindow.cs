using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
/// Tested on Unity Version: 2022.3.4f1
/// Lets you browse all the sprites in your project (whats in the asset folder). 
/// 
/// Important Note: This was done by following the Unity tutorial "Create a custom Editor window".
/// I made this tool to learn how to create a custom editor tool using the UI toolkit package. I also expanded on the tutorial but if you would like to see the original you can find the link below
/// https://docs.unity3d.com/Manual/UIE-HowTo-CreateEditorWindow.html
/// </summary>

public class SpriteBrowserEditorWindow : EditorWindow
{
    private int m_SelectedIndex = -1;
    private VisualElement m_RightPane = null;

    [MenuItem("External/Sprite Browser")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<SpriteBrowserEditorWindow>();
        wnd.titleContent = new GUIContent("Sprite Browser");

        // Limit size of the window
        wnd.minSize = new Vector2(450.0f, 200.0f);
        wnd.maxSize = new Vector2(1920.0f, 720.0f);
    }

    private void CreateGUI()
    {
        // Get a list of all sprites in the project
        string[] allObjectGuids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" }); // t stands for type

        List<Sprite> allObjects = new List<Sprite>();
        foreach (var guid in allObjectGuids)
        {
            allObjects.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        // Create a two-pane view with the left pane being fixed with
        TwoPaneSplitView splitView = new TwoPaneSplitView(0, 250.0f, TwoPaneSplitViewOrientation.Horizontal);

        // Add the panel to the visual tree by adding it as a child to the root element
        rootVisualElement.Add(splitView);

        // A TwoPaneSplitView always needs exactly two child elements
        ListView leftPane = new ListView();
        splitView.Add(leftPane);
        m_RightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(m_RightPane);

        // Initialize the list view with all sprites' names
        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
        leftPane.itemsSource = allObjects;

        // React to the user's selection
        leftPane.selectionChanged += OnSpriteSelectionChange;

        // Restore the selection index from before the hot reload
        leftPane.selectedIndex = m_SelectedIndex;

        // Store the selection index when the selection changes
        leftPane.selectionChanged += (items) => { m_SelectedIndex = leftPane.selectedIndex; };
    }

    private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
    {
        // Clear all previous content from the pane
        m_RightPane.Clear();

        // Get the selected sprite
        Sprite selectedSprite = selectedItems.First() as Sprite;
        if (selectedSprite == null)
        {
            return;
        }

        // Add a new Image control and display the sprite
        Image spriteImage = new Image();
        spriteImage.scaleMode = ScaleMode.ScaleToFit;
        spriteImage.sprite = selectedSprite;

        // Add the Image control to the right-hand pane
        m_RightPane.Add(spriteImage);
    }
}