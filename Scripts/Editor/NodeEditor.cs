﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary> Base class to derive custom Node editors from. Use this to create your own custom inspectors and editors for your nodes. </summary>
public class NodeEditor {
    /// <summary> Fires every whenever a node was modified through the editor </summary>
    public static Action<Node> onUpdateNode;
    public Node target;
    public SerializedObject serializedObject;
    public static Dictionary<NodePort, Vector2> portPositions;

    /// <summary> Draws the node GUI.</summary>
    /// <param name="portPositions">Port handle positions need to be returned to the NodeEditorWindow </param>
    public void OnNodeGUI() {
        OnHeaderGUI();
        OnBodyGUI();
    }

    protected void OnHeaderGUI() {
        GUI.color = Color.white;
        string title = NodeEditorUtilities.PrettifyCamelCase(target.name);
        GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
    }

    /// <summary> Draws standard field editors for all public fields </summary>
    protected virtual void OnBodyGUI() {
        string[] excludes = { "m_Script", "graph", "position", "ports" };
        portPositions = new Dictionary<NodePort, Vector2>();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        EditorGUIUtility.labelWidth = 84;
        while (iterator.NextVisible(enterChildren)) {
            enterChildren = false;
            if (excludes.Contains(iterator.name)) continue;
            NodeEditorGUILayout.PropertyField(iterator, true);
        }
    }

    public virtual int GetWidth() {
        return 200;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class CustomNodeEditorAttribute : Attribute {
    public Type inspectedType { get { return _inspectedType; } }
    private Type _inspectedType;
    public string contextMenuName { get { return _contextMenuName; } }
    private string _contextMenuName;
    /// <summary> Tells a NodeEditor which Node type it is an editor for </summary>
    /// <param name="inspectedType">Type that this editor can edit</param>
    /// <param name="contextMenuName">Path to the node</param>
    public CustomNodeEditorAttribute(Type inspectedType, string contextMenuName) {
        _inspectedType = inspectedType;
        _contextMenuName = contextMenuName;
    }
}