// FILE: GraphToolGenerator.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using GraphToolKitDB.Runtime;
using UnityEditor;
using UnityEngine;

public class GraphToolGenerator : EditorWindow
{
    // EditorPrefs Keys
    private const string PREFS_GRAPH_NAME = "GraphToolGenerator_GraphName";
    private const string PREFS_TARGET_SCRIPT_GUID = "GraphToolGenerator_TargetScriptGUID";
    private const string PREFS_RUNTIME_NS = "GraphToolGenerator_RuntimeNS";
    private const string PREFS_EDITOR_NS = "GraphToolGenerator_EditorNS";
    private const string PREFS_RUNTIME_PATH = "GraphToolGenerator_RuntimePath";
    private const string PREFS_EDITOR_PATH = "GraphToolGenerator_EditorPath";

    private string graphName;
    private MonoScript targetMonoBehaviourScript;
    private string runtimeNamespace;
    private string editorNamespace;
    private string runtimePath;
    private string editorPath;

    private List<Type> discoveredTypes = new List<Type>();
    private Vector2 scrollPosition;
    private Texture2D folderIcon;

    [MenuItem("Tools/Graph Tool Generator (Advanced)")]
    public static void ShowWindow()
    {
        GetWindow<GraphToolGenerator>("Graph Tool Generator");
    }

    private void OnEnable()
    {
        // Load settings from EditorPrefs
        graphName = EditorPrefs.GetString(PREFS_GRAPH_NAME, "GameDatabase");
        runtimeNamespace = EditorPrefs.GetString(PREFS_RUNTIME_NS, "GraphToolKitDB.Runtime");
        editorNamespace = EditorPrefs.GetString(PREFS_EDITOR_NS, "GraphToolKitDB.Editor");
        runtimePath = EditorPrefs.GetString(PREFS_RUNTIME_PATH, "Assets/Scripts/GraphToolKitDB/Runtime");
        editorPath = EditorPrefs.GetString(PREFS_EDITOR_PATH, "Assets/Scripts/GraphToolKitDB/Editor");

        string scriptGuid = EditorPrefs.GetString(PREFS_TARGET_SCRIPT_GUID);
        if (!string.IsNullOrEmpty(scriptGuid))
        {
            string path = AssetDatabase.GUIDToAssetPath(scriptGuid);
            if (!string.IsNullOrEmpty(path))
            {
                targetMonoBehaviourScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            }
        }

        folderIcon = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D;
        FindDiscoverableTypes();
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawConfiguration();
        DrawDiscoveredTypes();
        DrawActions();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Graph Tool Generator", new GUIStyle(EditorStyles.largeLabel) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        EditorGUILayout.HelpBox("This tool generates a complete, link-aware Unity GraphToolKit tool from a MonoBehaviour data container.", MessageType.Info);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    private void DrawConfiguration()
    {
        EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUI.BeginChangeCheck();
        graphName = EditorGUILayout.TextField(new GUIContent("Graph Name", "The base name for generated classes (e.g., 'GameDatabase')."), graphName);
        targetMonoBehaviourScript = (MonoScript)EditorGUILayout.ObjectField(new GUIContent("Target Script", "The MonoBehaviour script (like GameDatabase.cs) to analyze."), targetMonoBehaviourScript, typeof(MonoScript), false);
        
        EditorGUILayout.LabelField("Namespaces", EditorStyles.boldLabel);
        runtimeNamespace = EditorGUILayout.TextField(new GUIContent("Runtime Namespace", "Namespace for the runtime asset script."), runtimeNamespace);
        editorNamespace = EditorGUILayout.TextField(new GUIContent("Editor Namespace", "Namespace for the editor-only scripts."), editorNamespace);

        EditorGUILayout.LabelField("Export Paths", EditorStyles.boldLabel);
        runtimePath = DrawPathSelector("Runtime Path", "Folder for the runtime asset script.", runtimePath);
        editorPath = DrawPathSelector("Editor Path", "Folder for the editor scripts (Graph, Nodes, Importer).", editorPath);

        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(PREFS_GRAPH_NAME, graphName);
            EditorPrefs.SetString(PREFS_RUNTIME_NS, runtimeNamespace);
            EditorPrefs.SetString(PREFS_EDITOR_NS, editorNamespace);
            EditorPrefs.SetString(PREFS_RUNTIME_PATH, runtimePath);
            EditorPrefs.SetString(PREFS_EDITOR_PATH, editorPath);
            if (targetMonoBehaviourScript != null)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(targetMonoBehaviourScript, out string guid, out long _);
                EditorPrefs.SetString(PREFS_TARGET_SCRIPT_GUID, guid);
            }
            FindDiscoverableTypes();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
    
    private string DrawPathSelector(string label, string tooltip, string currentPath)
    {
        EditorGUILayout.BeginHorizontal();
        string newPath = EditorGUILayout.TextField(new GUIContent(label, tooltip), currentPath);
        if (GUILayout.Button(new GUIContent(folderIcon, $"Browse for {label}"), GUILayout.Width(30), GUILayout.Height(20)))
        {
            string absolutePath = EditorUtility.OpenFolderPanel($"Select {label}", "Assets", "");
            if (!string.IsNullOrEmpty(absolutePath) && absolutePath.StartsWith(Application.dataPath))
            {
                newPath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();
        return newPath;
    }

    private void DrawDiscoveredTypes()
    {
        EditorGUILayout.LabelField("Discovered Entity Types", EditorStyles.boldLabel);
        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, EditorStyles.helpBox, GUILayout.ExpandHeight(true)))
        {
            scrollPosition = scrollView.scrollPosition;
            if (discoveredTypes.Any())
            {
                foreach (var type in discoveredTypes)
                {
                    EditorGUILayout.LabelField("- " + type.Name);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No entity types found. Assign a target script.", EditorStyles.centeredGreyMiniLabel);
            }
        }
        EditorGUILayout.Space();
    }

    private void DrawActions()
    {
        EditorGUI.BeginDisabledGroup(!discoveredTypes.Any() || targetMonoBehaviourScript == null);
        if (GUILayout.Button(new GUIContent("Generate and Export Scripts", "Generates all necessary scripts and saves them to the specified paths."), GUILayout.Height(40)))
        {
            GenerateAndExportFiles();
        }
        EditorGUI.EndDisabledGroup();
    }

    private void FindDiscoverableTypes()
    {
        discoveredTypes.Clear();
        if (targetMonoBehaviourScript == null) return;

        var targetType = targetMonoBehaviourScript.GetClass();
        if (targetType == null) return;

        var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type entityType = field.FieldType.GetGenericArguments()[0];
                if (typeof(IPrimaryKey).IsAssignableFrom(entityType))
                {
                    discoveredTypes.Add(entityType);
                }
            }
        }
        discoveredTypes = discoveredTypes.Distinct().OrderBy(t => t.Name).ToList();
    }

    private void GenerateAndExportFiles()
    {
        if (string.IsNullOrEmpty(runtimePath) || string.IsNullOrEmpty(editorPath))
        {
            EditorUtility.DisplayDialog("Error", "Runtime and Editor paths must be set.", "OK");
            return;
        }

        string runtimeCode = GenerateRuntimeCode();
        string editorCode = GenerateEditorCode();

        try
        {
            Directory.CreateDirectory(runtimePath);
            string runtimeFilePath = Path.Combine(runtimePath, $"{graphName}Asset.cs");
            File.WriteAllText(runtimeFilePath, runtimeCode);

            Directory.CreateDirectory(editorPath);
            string editorFilePath = Path.Combine(editorPath, $"{graphName}Editor.cs");
            File.WriteAllText(editorFilePath, editorCode);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", $"Successfully generated and saved scripts:\n\n- {runtimeFilePath}\n- {editorFilePath}", "OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write generated files: {e}");
            EditorUtility.DisplayDialog("Error", "Failed to write generated files. Check the console for details.", "OK");
        }
    }

    private string GenerateRuntimeCode()
    {
        var builder = new StringBuilder();
        string assetClassName = $"{graphName}Asset";

        builder.AppendLine($"// ----- AUTO-GENERATED RUNTIME FILE BY {nameof(GraphToolGenerator)}.cs -----");
        builder.AppendLine();
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine();
        builder.AppendLine($"namespace {runtimeNamespace}");
        builder.AppendLine("{");
        builder.AppendLine($"    public class {assetClassName} : ScriptableObject");
        builder.AppendLine("    {");
        foreach (var type in discoveredTypes)
        {
            string listName = type.Name == "Link" ? "Links" : SimplePluralize(type.Name);
            builder.AppendLine($"        public List<{type.Name}> {listName} = new List<{type.Name}>();");
        }
        builder.AppendLine("    }");
        builder.AppendLine("}");

        return builder.ToString();
    }

    private string GenerateEditorCode()
    {
        var builder = new StringBuilder();
        string graphClassName = $"{graphName}Graph";
        string assetClassName = $"{graphName}Asset";
        string importerClassName = $"{graphName}Importer";
        string assetExtension = graphName.ToLowerInvariant().Replace(" ", "");

        // --- Header ---
        builder.AppendLine($"// ----- AUTO-GENERATED EDITOR FILE BY {nameof(GraphToolGenerator)}.cs -----");
        builder.AppendLine();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine("using Unity.GraphToolkit.Editor;");
        builder.AppendLine("using UnityEditor;");
        builder.AppendLine("using UnityEditor.AssetImporters;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine($"using {runtimeNamespace}; // Import runtime namespace");
        builder.AppendLine();
        builder.AppendLine($"namespace {editorNamespace}");
        builder.AppendLine("{");

        // --- Dummy Link class ---
        builder.AppendLine("    public class Linkable {}");
        builder.AppendLine();

        // --- Graph Class ---
        builder.AppendLine("    #region Graph Definition");
        builder.AppendLine();
        builder.AppendLine($"    [Graph(\"{assetExtension}\")]");
        builder.AppendLine($"    [Serializable]");
        builder.AppendLine($"    public class {graphClassName} : Graph");
        builder.AppendLine("    {");
        builder.AppendLine($"        [MenuItem(\"Assets/Create/{graphName} Graph\")]");
        builder.AppendLine("        private static void CreateAssetFile() => GraphDatabase.PromptInProjectBrowserToCreateNewAsset<" + graphClassName + ">(\"New " + graphName + "\");");
        builder.AppendLine();
        builder.AppendLine("        public static T GetPortValue<T>(IPort port) {");
        builder.AppendLine("            if (port.isConnected) {");
        builder.AppendLine("                var sourceNode = port.firstConnectedPort?.GetNode();");
        builder.AppendLine("                if (sourceNode is IConstantNode c) { c.TryGetValue(out T v); return v; }");
        builder.AppendLine("                if (sourceNode is IVariableNode vn) { vn.variable.TryGetDefaultValue(out T v); return v; }");
        builder.AppendLine("            } else { port.TryGetValue(out T v); return v; }");
        builder.AppendLine("            return default;");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    #endregion");
        builder.AppendLine();

        // --- Node Definitions ---
        builder.AppendLine("    #region Node Definitions");
        builder.AppendLine();
        foreach (var type in discoveredTypes.Where(t => t.Name != "Link"))
        {
            GenerateNodeClassForType(builder, type);
            builder.AppendLine();
        }
        builder.AppendLine("    #endregion");
        builder.AppendLine();

        // --- Scripted Importer ---
        GenerateImporterCode(builder, importerClassName, graphClassName, assetClassName);

        builder.AppendLine("}"); // End namespace
        return builder.ToString();
    }

    private void GenerateNodeClassForType(StringBuilder builder, Type dataType)
    {
        string nodeClassName = $"{dataType.Name}DefinitionNode";
        var fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        builder.AppendLine($"    [Serializable]");
        builder.AppendLine($"    public class {nodeClassName} : Node");
        builder.AppendLine("    {");
        builder.AppendLine("        public const string PortInputLink = \"InputLink\";");
        builder.AppendLine("        public const string PortOutputLink = \"OutputLink\";");
        foreach (var field in fields.Where(f => !f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase)))
        {
            builder.AppendLine($"        public const string Port{ToPascalCase(field.Name)} = \"{ToPascalCase(field.Name)}\";");
        }
        builder.AppendLine();
        builder.AppendLine("        protected override void OnDefinePorts(IPortDefinitionContext context)");
        builder.AppendLine("        {");
        builder.AppendLine("            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(\" \").Build();");
        builder.AppendLine("            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(\" \").Build();");
        builder.AppendLine();
        foreach (var field in fields.Where(f => !f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase)))
        {
            builder.AppendLine($"            context.AddInputPort<{GetCSharpTypeName(field.FieldType)}>(Port{ToPascalCase(field.Name)}).WithDisplayName(\"{SplitPascalCase(field.Name)}\").Build();");
        }
        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private void GenerateImporterCode(StringBuilder builder, string importerName, string graphName, string assetName)
    {
        builder.AppendLine("    #region Scripted Importer");
        builder.AppendLine();
        builder.AppendLine($"    [ScriptedImporter(1, \"{graphName.ToLowerInvariant().Replace("graph", "")}\")]");
        builder.AppendLine($"    public class {importerName} : ScriptedImporter");
        builder.AppendLine("    {");
        builder.AppendLine("        public override void OnImportAsset(AssetImportContext ctx)");
        builder.AppendLine("        {");
        builder.AppendLine($"            var graph = GraphDatabase.LoadGraphForImporter<{graphName}>(ctx.assetPath);");
        builder.AppendLine("            if (graph == null) return;");
        builder.AppendLine();
        builder.AppendLine($"            var runtimeAsset = ScriptableObject.CreateInstance<{assetName}>();");
        builder.AppendLine("            var nodeToIdMap = new Dictionary<INode, int>();");
        builder.AppendLine("            var typeToIdMap = new Dictionary<Type, ushort>();");
        builder.AppendLine();
        builder.AppendLine("            // Pre-populate type IDs for linking");
        ushort typeIdCounter = 1;
        foreach (var type in discoveredTypes)
        {
            builder.AppendLine($"            typeToIdMap[typeof({type.Name})] = {typeIdCounter++};");
        }
        builder.AppendLine();
        builder.AppendLine("            // --- PASS 1: Create all data entries and map nodes to their index-based ID ---");

        foreach (var type in discoveredTypes.Where(t => t.Name != "Link"))
        {
            string nodeClassName = $"{type.Name}DefinitionNode";
            string pluralName = SimplePluralize(type.Name);
            
            builder.AppendLine($"            var {type.Name.ToLower()}Nodes = graph.GetNodes().OfType<{nodeClassName}>().ToList();");
            builder.AppendLine($"            for(int i = 0; i < {type.Name.ToLower()}Nodes.Count; i++)");
            builder.AppendLine("            {");
            builder.AppendLine($"                var node = {type.Name.ToLower()}Nodes[i];");
            builder.AppendLine($"                var data = new {type.Name}();");
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => !f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase)))
            {
                string portConst = $"{nodeClassName}.Port{ToPascalCase(field.Name)}";
                builder.AppendLine($"                data.{field.Name} = {graphName}.GetPortValue<{GetCSharpTypeName(field.FieldType)}>(node.GetInputPortByName({portConst}));");
            }
            builder.AppendLine("                data.ID = i;");
            builder.AppendLine($"                runtimeAsset.{pluralName}.Add(data);");
            builder.AppendLine("                nodeToIdMap[node] = i;");
            builder.AppendLine("            }");
        }
        builder.AppendLine();
        builder.AppendLine("            // --- PASS 2: Create links by traversing graph connections ---");
        builder.AppendLine("            foreach (var sourceNode in graph.GetNodes())");
        builder.AppendLine("            {");
        builder.AppendLine("                var outputPort = sourceNode.GetOutputPorts().FirstOrDefault(p => p.name == \"OutputLink\");");
        builder.AppendLine("                if (outputPort == null || !outputPort.isConnected) continue;");
        builder.AppendLine("                if (!nodeToIdMap.TryGetValue(sourceNode, out int sourceId)) continue;");
        builder.AppendLine();
        builder.AppendLine("                var sourceDataType = GetNodeType(sourceNode);");
        builder.AppendLine("                ushort sourceTypeId = typeToIdMap.GetValueOrDefault(sourceDataType, (ushort)0);");
        builder.AppendLine();
        builder.AppendLine("                var connectedPorts = new List<IPort>();");
        builder.AppendLine("                outputPort.GetConnectedPorts(connectedPorts);");
        builder.AppendLine("                foreach (var connectedPort in connectedPorts.Where(p => p.name == \"InputLink\"))");
        builder.AppendLine("                {");
        builder.AppendLine("                    var targetNode = connectedPort.GetNode();");
        builder.AppendLine("                    if (!nodeToIdMap.TryGetValue(targetNode, out int targetId)) continue;");
        builder.AppendLine();
        builder.AppendLine("                    var targetDataType = GetNodeType(targetNode);");
        builder.AppendLine("                    ushort targetTypeId = typeToIdMap.GetValueOrDefault(targetDataType, (ushort)0);");
        builder.AppendLine();
        builder.AppendLine("                    var link = new Link { SourceType = sourceTypeId, SourceID = sourceId, TargetType = targetTypeId, TargetID = targetId, LinkTypeID = 0 };");
        builder.AppendLine("                    runtimeAsset.Links.Add(link);");
        builder.AppendLine("                }");
        builder.AppendLine("            }");
        builder.AppendLine();
        builder.AppendLine($"            ctx.AddObjectToAsset(\"{graphName}\", runtimeAsset);");
        builder.AppendLine("            ctx.SetMainObject(runtimeAsset);");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine("        private Type GetNodeType(INode node)");
        builder.AppendLine("        {");
        foreach (var type in discoveredTypes.Where(t => t.Name != "Link"))
        {
            builder.AppendLine($"            if (node is {type.Name}DefinitionNode) return typeof({type.Name});");
        }
        builder.AppendLine("            return null;");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    #endregion");
    }

    private static string GetCSharpTypeName(Type type)
    {
        var typeMap = new Dictionary<Type, string>
        {
            { typeof(int), "int" }, { typeof(string), "string" }, { typeof(float), "float" },
            { typeof(bool), "bool" }, { typeof(double), "double" }, { typeof(long), "long" },
            { typeof(short), "short" }, { typeof(byte), "byte" }, { typeof(uint), "uint" },
            { typeof(ulong), "ulong" }, { typeof(ushort), "ushort" }, { typeof(sbyte), "sbyte" }
        };
        return typeMap.TryGetValue(type, out var name) ? name : type.FullName.Replace('+', '.');
    }

    private static string ToPascalCase(string input) => string.IsNullOrEmpty(input) ? input : char.ToUpperInvariant(input[0]) + input.Substring(1);
    private static string SplitPascalCase(string input) => string.IsNullOrEmpty(input) ? input : ToPascalCase(Regex.Replace(input, "(?<!^)([A-Z])", " $1"));
    private static string SimplePluralize(string name) => name.EndsWith("s") ? name + "es" : name + "s";
}