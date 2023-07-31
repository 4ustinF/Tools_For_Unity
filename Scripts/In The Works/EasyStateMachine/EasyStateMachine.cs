using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EasyStateMachine : MonoBehaviour
{
    private EasyStateMachineEditorWindow _window = null;

    public void ShowEditorWindow()
    {
        // Prevent multiple windows being open at a time
        if (_window != null)
        {
            CloseWindow();
        }

        //EasyStateMachineEditorWindow wnd = new EasyStateMachineEditorWindow();
        _window = ScriptableObject.CreateInstance<EasyStateMachineEditorWindow>();
        _window.ShowEasyStateMachineEditorWindow();

    }

    private void CloseWindow()
    {
        // TODO: Save data
        _window.Close();
    }
}

// ----------------------------------------------------------------------------------------------------

public class EasyStateMachineEditorWindow : EditorWindow
{
    private class ESMNode
    {
        public string name = "node";
        public Rect rect = new Rect();
    }

    private class Transition
    {
        public string name = "transition";
        public ESMNode fromNode = null; 
        public ESMNode toNode = null; 
    }

    // Scroll data
    private const float ZoomSpeed = 0.1f; // Adjust this value to control zoom speed
    private float zoomLevel = 1.0f;
    private Vector2 scrollPosition = Vector2.zero;

    // Node data
    List<ESMNode> _nodes = new List<ESMNode>();
    private Vector2 _buttonSize = new Vector2(224.0f, 59.0f);
    private ESMNode _defaultNode = null;
    private ESMNode _selectedNode = null;
    private float _nodeOutlineSize = 2.0f;

    // Transition data
    List<Transition> _transitions = new List<Transition>();

    // Dragging logic
    private ESMNode _nodeToDrag = null;
    private bool _isDragging = false;
    private Vector2 _dragOffSet = Vector2.zero;

    // Other
    private int _gridSpacing = 15; // Spacing between grid lines
    private Vector2 _viewOffSet = Vector2.zero;
    private float _arrowHeadLength = 10.0f;


    public EasyStateMachineEditorWindow()
    {
        // TODO load data
    }

    public void ShowEasyStateMachineEditorWindow()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<EasyStateMachineEditorWindow>();
        wnd.titleContent = new GUIContent("ESM Graph Editor");

        // Limit size of the window
        wnd.minSize = new Vector2(450.0f, 200.0f);
        wnd.maxSize = new Vector2(1920.0f, 720.0f);
    }

    private void OnGUI()
    {
        Event currentEvent = Event.current;

        DrawCalls(currentEvent);
        HandleContextMenu(currentEvent);

        DragNode(currentEvent);
    }

    #region ---DrawCalls---

    private void DrawCalls(Event currentEvent)
    {
        DrawGrid();
        DrawNodes(currentEvent);
    }

    private void DrawGrid()
    {
        Handles.BeginGUI();

        // Get the size of the current window
        var windowSize = position.size;

        // Draw vertical lines
        Handles.color = Color.black;
        for (float x = 0.0f; x < windowSize.x; x += _gridSpacing)
        {
            Handles.DrawLine(new Vector3(x, 0.0f), new Vector3(x, windowSize.y));
        }

        // Draw horizontal lines
        for (float y = 0.0f; y < windowSize.y; y += _gridSpacing)
        {
            Handles.DrawLine(new Vector3(0.0f, y), new Vector3(windowSize.x, y));
        }

        Handles.EndGUI();
    }

    private void DrawNodes(Event currentEvent)
    {
        foreach (var node in _nodes)
        {
            // Get font color for nodes
            Color fontColor = GetNodeFontColor(node);

            // Get rect for the node
            Rect rect = new Rect(node.rect.position + _viewOffSet, node.rect.size);

            // Highlight node
            if(node == _selectedNode)
            {
                Rect outlineRect = new Rect(rect.x - _nodeOutlineSize, rect.y - _nodeOutlineSize, rect.width + _nodeOutlineSize * 2.0f, rect.height + _nodeOutlineSize * 2.0f);
                EditorGUI.DrawRect(outlineRect, Color.black);
            }

            // Create and visualize node
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.normal.textColor = fontColor;
            buttonStyle.hover.textColor = fontColor;

            // Draw button node
            GUI.Box(rect, node.name, buttonStyle);

            // TODO: Seperate events from the draw function
            if (node.rect.Contains(currentEvent.mousePosition))
            {
                switch(Event.current.type)
                {
                    case EventType.MouseDown:
                        _nodeToDrag = node;
                        _isDragging = true;
                        _dragOffSet = currentEvent.mousePosition - node.rect.position;
                        SelectNode(node);
                        break;
                    case EventType.MouseUp:
                        _nodeToDrag = null;
                        _isDragging = false;
                        break;
                }
            }
        }
    }

    #endregion

    private void HandleZoom()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Add your custom GUI elements here
        GUILayout.Label("Zoom Level: " + zoomLevel.ToString("F2"));

        EditorGUILayout.EndScrollView();

        Event currentEvent = Event.current;
        Rect windowRect = position;

        Vector2 mousePosition = currentEvent.mousePosition - windowRect.position;
        Vector2 pivotPoint = mousePosition / zoomLevel;

        switch (currentEvent.type)
        {
            case EventType.ScrollWheel:
                float zoomDelta = -currentEvent.delta.y * ZoomSpeed;
                float newZoom = zoomLevel + zoomDelta;

                if (newZoom > 0.1f) // Adjust the minimum zoom level as desired
                {
                    zoomLevel = newZoom;
                    Repaint();
                }

                break;
        }

        GUIUtility.ScaleAroundPivot(new Vector2(zoomLevel, zoomLevel), pivotPoint);
    }

    private void HandleContextMenu(Event currentEvent)
    {
        if (currentEvent.type != EventType.ContextClick)
        {
            return;
        }

        GenericMenu menu = new GenericMenu();
        foreach(var node in _nodes)
        {
            if(node.rect.Contains(currentEvent.mousePosition))
            {
                menu.AddItem(new GUIContent("Make Transition"), false, MakeTransitionCallback);
                menu.AddItem(new GUIContent("Set as Default"), false, () => SetAsDefaultCallback(node));
                menu.AddItem(new GUIContent("Delete"), false, () => DeleteNodeCallback(node, node == _defaultNode));

                menu.ShowAsContext();
                return;
            }
        }

        // Default generic menu
        menu.AddItem(new GUIContent("Create State"), false, () => CreateStateCallback(currentEvent));
        menu.AddItem(new GUIContent("Create Any State"), false, CreateAnyStateCallback);
        menu.ShowAsContext();
    }

    #region ---Callbacks---
    private void CreateStateCallback(Event currentEvent)
    {
        // Populate data
        ESMNode newNode = new ESMNode();
        newNode.name = "New Node";

        //// calculate new position for new rect and snap to grid
        //float newX = Mathf.Round(currentEvent.mousePosition.x / _gridSpacing) * _gridSpacing;
        //float newY = Mathf.Round(currentEvent.mousePosition.y / _gridSpacing) * _gridSpacing;
        //newNode.rect = new Rect(new Vector2(newX, newY), _buttonSize);
        newNode.rect = new Rect(currentEvent.mousePosition, _buttonSize);

        // Add node to list
        _nodes.Add(newNode);

        // Make it the selected node as the user just made ti they most likely will want to edit it
        SelectNode(newNode);

        // Lastly set the node to default if its the only one
        if (_nodes.Count == 1)
        {
            _defaultNode = newNode;
        }
    }

    private void CreateAnyStateCallback()
    {
        Debug.Log("CreateAnyStateCallback selected");
    }

    private void MakeTransitionCallback()
    {

    }

    private void SetAsDefaultCallback(ESMNode node)
    {
        _defaultNode = node;
    }

    private void DeleteNodeCallback(ESMNode node, bool isDefault)
    {
        // Remove node
        _nodes.Remove(node);

        // Set a new default if the deleted node was the default
        if(isDefault)
        {
            _defaultNode = _nodes.Count > 0 ? _nodes[0] : null;
        }
    }

    #endregion

    private void DragNode(Event currentEvent)
    {
        if(_isDragging == false || _nodeToDrag == null)
        {
            return;
        }

        // Calculate new position for new rect
        float newX = currentEvent.mousePosition.x - _dragOffSet.x;
        float newY = currentEvent.mousePosition.y - _dragOffSet.y;

        //// Snap to grid
        //newX = Mathf.Round(newX / _gridSpacing) * _gridSpacing;
        //newY = Mathf.Round(newY / _gridSpacing) * _gridSpacing;

        // Update Rect
        _nodeToDrag.rect = new Rect(new Vector2(newX, newY), _nodeToDrag.rect.size);
    }

    private Color GetNodeFontColor(ESMNode node)
    {
        if (node == _defaultNode)
        {
            return Color.green;
        }

        return Color.white;
    }

    private void SelectNode(ESMNode node)
    {
        _selectedNode = node;

        // TODO: Show node date in inspector
    }

    private void DrawArrow(Vector2 fromPos, Vector2 toPos)
    {
        // Draw the arrow body
        Handles.DrawLine(fromPos, toPos);

        // Calculate directional vector
        Vector2 dir = (toPos - fromPos).normalized;

        // Calculate perpendicular vector
        float px = -dir.y;
        float py = dir.x;

        // Get arrow points
        Vector2 middlePoint = Vector2.Lerp(fromPos, toPos, 0.5f);
        Vector2 arrowheadPoint1 = new Vector2(middlePoint.x + _arrowHeadLength * px, middlePoint.y + _arrowHeadLength * py);
        Vector2 arrowheadPoint2 = new Vector2(middlePoint.x - _arrowHeadLength * px, middlePoint.y - _arrowHeadLength * py);

        // Draw the arrow in the middle of the line
        Handles.DrawAAConvexPolygon(arrowheadPoint1, arrowheadPoint2, middlePoint + dir * _arrowHeadLength);
    }
}

