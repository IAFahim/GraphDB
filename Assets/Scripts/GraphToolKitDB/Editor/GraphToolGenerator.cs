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
using Unity.Collections;
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
    private const string PREFS_ADD_LINK_STRUCT = "GraphToolGenerator_AddLinkStruct";

    private string graphName;
    private MonoScript targetMonoBehaviourScript;
    private string runtimeNamespace;
    private string editorNamespace;
    private string runtimePath;
    private string editorPath;
    private bool addLinkStruct;

    private List<Type> discoveredTypes = new List<Type>();
    private Vector2 scrollPosition;
    private Texture2D folderIcon, warningIcon, okIcon;

    [MenuItem("Tools/Graph Tool Generator (Definitive)")]
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
        addLinkStruct = EditorPrefs.GetBool(PREFS_ADD_LINK_STRUCT, true);

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
        warningIcon = EditorGUIUtility.IconContent("console.warnicon.sml").image as Texture2D;
        okIcon = EditorGUIUtility.IconContent("TestPassed").image as Texture2D;
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
        EditorGUILayout.LabelField("Graph Tool Generator",
            new GUIStyle(EditorStyles.largeLabel) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        EditorGUILayout.HelpBox(
            "This tool generates a complete, link-aware Unity GraphToolKit tool from a MonoBehaviour data container.",
            MessageType.Info);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    private void DrawConfiguration()
    {
        EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUI.BeginChangeCheck();
        graphName = EditorGUILayout.TextField(
            new GUIContent("Graph Name", "The base name for generated classes and the asset extension."), graphName);
        EditorGUILayout.LabelField("File Extension", graphName.ToLowerInvariant().Replace(" ", ""));

        targetMonoBehaviourScript = (MonoScript)EditorGUILayout.ObjectField(
            new GUIContent("Target Script", "The script containing your data lists (e.g., GameDatabase.cs)."),
            targetMonoBehaviourScript, typeof(MonoScript), false);

        EditorGUILayout.LabelField("Namespaces", EditorStyles.boldLabel);
        runtimeNamespace =
            EditorGUILayout.TextField(new GUIContent("Runtime Namespace", "Namespace for the runtime asset script."),
                runtimeNamespace);
        editorNamespace =
            EditorGUILayout.TextField(new GUIContent("Editor Namespace", "Namespace for the editor-only scripts."),
                editorNamespace);

        EditorGUILayout.LabelField("Export Paths", EditorStyles.boldLabel);
        runtimePath = DrawPathSelector("Runtime Path", "Folder for runtime scripts (Asset, EntityType).", runtimePath);
        editorPath = DrawPathSelector("Editor Path", "Folder for editor scripts (Graph, Nodes, Importer).", editorPath);

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
        if (GUILayout.Button(new GUIContent(folderIcon, $"Browse for {label}"), GUILayout.Width(30),
                GUILayout.Height(20)))
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
        using (var scrollView =
               new EditorGUILayout.ScrollViewScope(scrollPosition, EditorStyles.helpBox, GUILayout.ExpandHeight(true)))
        {
            scrollPosition = scrollView.scrollPosition;
            if (discoveredTypes.Any())
            {
                foreach (var type in discoveredTypes)
                {
                    bool hasEntity = typeof(IEntity).IsAssignableFrom(type);
                    EditorGUILayout.BeginHorizontal();

                    var icon = hasEntity ? okIcon : warningIcon;
                    var tooltip = hasEntity ? "Ready" : "Missing IEntity interface.";
                    GUILayout.Label(new GUIContent(icon, tooltip), GUILayout.Width(20), GUILayout.Height(20));

                    EditorGUILayout.LabelField(type.Name);
                    if (!hasEntity)
                    {
                        GUI.color = new Color(1f, 0.8f, 0.4f); // Orange-yellow
                        if (GUILayout.Button(
                                new GUIContent("Implement IEntity",
                                    "Click to automatically add the IEntity interface and ID property to the source file."),
                                GUILayout.ExpandWidth(false)))
                        {
                            AddEntityToSource(type);
                        }

                        GUI.color = Color.white;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No entity types found. Assign a target script.",
                    EditorStyles.centeredGreyMiniLabel);
            }
        }

        EditorGUILayout.Space();
    }

    private void DrawActions()
    {
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        bool linkTypeMissing = !discoveredTypes.Any(t => t.Name == "Link" || t.Name == "PrimaryKey");
        if (linkTypeMissing)
        {
            EditorGUI.BeginChangeCheck();
            addLinkStruct =
                EditorGUILayout.Toggle(
                    new GUIContent("Generate Link Struct",
                        "Generates a 'Link' struct if one is not found in your target script."), addLinkStruct);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(PREFS_ADD_LINK_STRUCT, addLinkStruct);
            }
        }

        EditorGUI.BeginDisabledGroup(!discoveredTypes.Any() || targetMonoBehaviourScript == null);
        if (GUILayout.Button(
                new GUIContent("Generate All Scripts",
                    "Generates all necessary scripts and saves them to the specified paths."), GUILayout.Height(40)))
        {
            GenerateAndExportFiles();
        }

        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();
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
                discoveredTypes.Add(entityType);
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

        var allUsings = GetRequiredUsings();
        string entityTypeCode = GenerateEntityTypeEnum();
        string runtimeCode = GenerateRuntimeCode(allUsings);
        string editorCode = GenerateEditorCode(allUsings);
        string importerCode = GenerateImporterCode(allUsings);

        try
        {
            Directory.CreateDirectory(runtimePath);
            string entityTypePath = Path.Combine(runtimePath, "EntityType.cs");
            File.WriteAllText(entityTypePath, entityTypeCode);
            string runtimeFilePath = Path.Combine(runtimePath, $"{graphName}Asset.cs");
            File.WriteAllText(runtimeFilePath, runtimeCode);

            Directory.CreateDirectory(editorPath);
            string editorFilePath = Path.Combine(editorPath, $"{graphName}Editor.cs");
            File.WriteAllText(editorFilePath, editorCode);
            string importerFilePath = Path.Combine(editorPath, $"{graphName}Importer.cs");
            File.WriteAllText(importerFilePath, importerCode);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success",
                $"Successfully generated and saved scripts:\n\n- {entityTypePath}\n- {runtimeFilePath}\n- {editorFilePath}\n- {importerFilePath}",
                "OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write generated files: {e}");
            EditorUtility.DisplayDialog("Error", "Failed to write generated files. Check the console for details.",
                "OK");
        }
    }

    private string GenerateEntityTypeEnum()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"// ----- AUTO-GENERATED ENTITY TYPE FILE BY {nameof(GraphToolGenerator)}.cs -----");
        builder.AppendLine();
        builder.AppendLine($"namespace {runtimeNamespace}");
        builder.AppendLine("{");
        builder.AppendLine("    public enum EntityType : ushort");
        builder.AppendLine("    {");
        builder.AppendLine("        None = 0,");
        ushort idCounter = 1;
        foreach (var type in discoveredTypes)
        {
            builder.AppendLine($"        {type.Name} = {idCounter++},");
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private string GenerateRuntimeCode(HashSet<string> usings)
    {
        var builder = new StringBuilder();
        string assetClassName = $"{graphName}Asset";

        builder.AppendLine($"// ----- AUTO-GENERATED RUNTIME FILE BY {nameof(GraphToolGenerator)}.cs -----");
        builder.AppendLine();
        foreach (var u in usings.OrderBy(x => x)) builder.AppendLine($"using {u};");
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

        bool linkTypeMissing = !discoveredTypes.Any(t => t.Name == "Link" || t.Name == "PrimaryKey");
        if (linkTypeMissing && addLinkStruct)
        {
            builder.AppendLine();
            builder.AppendLine("    [Serializable]");
            builder.AppendLine("    public struct Link : IEntity");
            builder.AppendLine("    {");
            builder.AppendLine("        public int Id;");
            builder.AppendLine("        public int ID { get => Id; set => Id = value; }");
            builder.AppendLine("        public EntityType SourceType;");
            builder.AppendLine("        public int SourceID;");
            builder.AppendLine("        public EntityType TargetType;");
            builder.AppendLine("        public int TargetID;");
            builder.AppendLine("        public ushort LinkTypeID;");
            builder.AppendLine("    }");
        }

        builder.AppendLine("}");

        return builder.ToString();
    }

    private string GenerateEditorCode(HashSet<string> usings)
    {
        var builder = new StringBuilder();
        string graphClassName = $"{graphName}Graph";
        string assetExtension = graphName.ToLowerInvariant().Replace(" ", "");

        builder.AppendLine($"// ----- AUTO-GENERATED EDITOR FILE BY {nameof(GraphToolGenerator)}.cs -----");
        builder.AppendLine();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine("using System.Text.RegularExpressions;");
        builder.AppendLine("using Unity.GraphToolkit.Editor;");
        builder.AppendLine("using UnityEditor;");
        builder.AppendLine("using UnityEngine;");
        foreach (var u in usings.OrderBy(x => x)) builder.AppendLine($"using {u};");
        builder.AppendLine($"using {runtimeNamespace};");
        builder.AppendLine();
        builder.AppendLine($"namespace {editorNamespace}");
        builder.AppendLine("{");
        builder.AppendLine(GetSerializableDictionaryCode());
        builder.AppendLine("    public class Linkable {}");
        builder.AppendLine();
        builder.AppendLine("    #region Graph Definition");
        builder.AppendLine($"    [Graph(\"{assetExtension}\")]");
        builder.AppendLine($"    [Serializable]");
        builder.AppendLine($"    public class {graphClassName} : Graph");
        builder.AppendLine("    {");
        builder.AppendLine("        [SerializeField]");
        builder.AppendLine(
            "        private SerializableDictionary<Type, int> nodeIdCounters = new SerializableDictionary<Type, int>();");
        builder.AppendLine();
        builder.AppendLine($"        [MenuItem(\"Assets/Create/{graphName} Graph\")]");
        builder.AppendLine(
            "        private static void CreateAssetFile() => GraphDatabase.PromptInProjectBrowserToCreateNewAsset<" +
            graphClassName + ">(\"New " + graphName + "\");");
        builder.AppendLine();
        builder.AppendLine("        public override void OnGraphChanged(GraphLogger infos)");
        builder.AppendLine("        {");
        builder.AppendLine("            base.OnGraphChanged(infos);");
        builder.AppendLine("            foreach (var node in GetNodes().OfType<IDataNode>())");
        builder.AppendLine("            {");
        builder.AppendLine("                if (node.NodeID == 0)");
        builder.AppendLine("                {");
        builder.AppendLine("                    var nodeType = node.GetType();");
        builder.AppendLine("                    nodeIdCounters.TryGetValue(nodeType, out int currentId);");
        builder.AppendLine("                    currentId++;");
        builder.AppendLine("                    node.NodeID = currentId;");
        builder.AppendLine("                    nodeIdCounters[nodeType] = currentId;");
        builder.AppendLine("                }");
        builder.AppendLine("            }");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine("        public static T GetPortValue<T>(IPort port) {");
        builder.AppendLine("            if (port.isConnected) {");
        builder.AppendLine("                var sourceNode = port.firstConnectedPort?.GetNode();");
        builder.AppendLine("                if (sourceNode is IConstantNode c) { c.TryGetValue(out T v); return v; }");
        builder.AppendLine(
            "                if (sourceNode is IVariableNode vn) { vn.variable.TryGetDefaultValue(out T v); return v; }");
        builder.AppendLine("            } else { port.TryGetValue(out T v); return v; }");
        builder.AppendLine("            return default;");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("    #endregion");
        builder.AppendLine();
        builder.AppendLine("    #region Node Definitions");
        builder.AppendLine("    public interface IDataNode : INode { int NodeID { get; set; } }");
        builder.AppendLine();
        foreach (var type in discoveredTypes.Where(t => t.Name != "Link" && t.Name != "PrimaryKey"))
        {
            GenerateNodeClassForType(builder, type);
            builder.AppendLine();
        }

        builder.AppendLine("    #endregion");
        builder.AppendLine();
        builder.AppendLine(GetEditorHelperCode());
        builder.AppendLine("}");
        return builder.ToString();
    }

    private string GenerateImporterCode(HashSet<string> usings)
    {
        var builder = new StringBuilder();
        string graphClassName = $"{graphName}Graph";
        string assetClassName = $"{graphName}Asset";
        string importerClassName = $"{graphName}Importer";
        string assetExtension = graphName.ToLowerInvariant().Replace(" ", "");

        builder.AppendLine($"// ----- AUTO-GENERATED IMPORTER FILE BY {nameof(GraphToolGenerator)}.cs -----");
        builder.AppendLine();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine("using System.Reflection;");
        builder.AppendLine("using Unity.GraphToolkit.Editor;");
        builder.AppendLine("using UnityEditor.AssetImporters;");
        builder.AppendLine("using UnityEngine;");
        foreach (var u in usings.OrderBy(x => x)) builder.AppendLine($"using {u};");
        builder.AppendLine($"using {runtimeNamespace};");
        builder.AppendLine();
        builder.AppendLine($"namespace {editorNamespace}");
        builder.AppendLine("{");
        builder.AppendLine("    #region Scripted Importer");
        builder.AppendLine($"    [ScriptedImporter(1, \"{assetExtension}\")]");
        builder.AppendLine($"    public class {importerClassName} : ScriptedImporter");
        builder.AppendLine("    {");
        builder.AppendLine("        public override void OnImportAsset(AssetImportContext ctx)");
        builder.AppendLine("        {");
        builder.AppendLine(
            $"            var graph = GraphDatabase.LoadGraphForImporter<{graphClassName}>(ctx.assetPath);");
        builder.AppendLine("            if (graph == null) return;");
        builder.AppendLine();
        builder.AppendLine($"            var runtimeAsset = ScriptableObject.CreateInstance<{assetClassName}>();");
        builder.AppendLine("            var nodeToIdMap = new Dictionary<INode, int>();");
        builder.AppendLine();
        builder.AppendLine(
            "            // --- PASS 1: Create all data entries and map nodes to their index-based ID ---");

        foreach (var type in discoveredTypes.Where(t => t.Name != "Link" && t.Name != "PrimaryKey"))
        {
            string nodeClassName = $"{type.Name}DefinitionNode";
            string pluralName = SimplePluralize(type.Name);

            builder.AppendLine(
                $"            var {type.Name.ToLower()}Nodes = graph.GetNodes().OfType<{nodeClassName}>().ToList();");
            builder.AppendLine($"            for(int i = 0; i < {type.Name.ToLower()}Nodes.Count; i++)");
            builder.AppendLine("            {");
            builder.AppendLine($"                var node = {type.Name.ToLower()}Nodes[i];");
            builder.AppendLine($"                var data = new {type.Name}();");
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f =>
                         !f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) &&
                         !f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)))
            {
                string portConst = $"{nodeClassName}.Port{ToPascalCase(field.Name)}";
                builder.AppendLine(
                    $"                data.{field.Name} = {graphClassName}.GetPortValue<{GetCSharpTypeName(field.FieldType)}>(node.GetInputPortByName({portConst}));");
            }

            builder.AppendLine("                data.ID = i;");
            builder.AppendLine($"                runtimeAsset.{pluralName}.Add(data);");
            builder.AppendLine("                nodeToIdMap[node] = i;");
            builder.AppendLine("            }");
        }

        builder.AppendLine();
        builder.AppendLine("            // --- PASS 2: Create links by traversing graph connections ---");
        builder.AppendLine("            int linkId = 0;");
        builder.AppendLine("            foreach (var sourceNode in graph.GetNodes())");
        builder.AppendLine("            {");
        builder.AppendLine(
            "                var outputPort = sourceNode.GetOutputPorts().FirstOrDefault(p => p.name == \"OutputLink\");");
        builder.AppendLine("                if (outputPort == null || !outputPort.isConnected) continue;");
        builder.AppendLine("                if (!nodeToIdMap.TryGetValue(sourceNode, out int sourceId)) continue;");
        builder.AppendLine();
        builder.AppendLine("                var sourceDataType = GetNodeType(sourceNode);");
        builder.AppendLine("                if (sourceDataType == null) continue;");
        builder.AppendLine();
        builder.AppendLine("                var connectedPorts = new List<IPort>();");
        builder.AppendLine("                outputPort.GetConnectedPorts(connectedPorts);");
        builder.AppendLine(
            "                foreach (var connectedPort in connectedPorts.Where(p => p.name == \"InputLink\"))");
        builder.AppendLine("                {");
        builder.AppendLine("                    var targetNode = connectedPort.GetNode();");
        builder.AppendLine("                    if (!nodeToIdMap.TryGetValue(targetNode, out int targetId)) continue;");
        builder.AppendLine();
        builder.AppendLine("                    var targetDataType = GetNodeType(targetNode);");
        builder.AppendLine("                    if (targetDataType == null) continue;");
        builder.AppendLine();
        builder.AppendLine(
            "                    var link = new Link { ID = linkId++, SourceType = (EntityType)Enum.Parse(typeof(EntityType), sourceDataType.Name), SourceID = sourceId, TargetType = (EntityType)Enum.Parse(typeof(EntityType), targetDataType.Name), TargetID = targetId };");
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
        foreach (var type in discoveredTypes.Where(t => t.Name != "Link" && t.Name != "PrimaryKey"))
        {
            builder.AppendLine($"            if (node is {type.Name}DefinitionNode) return typeof({type.Name});");
        }

        builder.AppendLine("            return null;");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("    #endregion");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private void GenerateNodeClassForType(StringBuilder builder, Type dataType)
    {
        string nodeClassName = $"{dataType.Name}DefinitionNode";
        var fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        string displayTitle = SplitPascalCase(dataType.Name);

        builder.AppendLine($"    [Serializable]");
        builder.AppendLine($"    public class {nodeClassName} : Node, IDataNode");
        builder.AppendLine("    {");
        builder.AppendLine("        [SerializeField]");
        builder.AppendLine("        private int m_NodeID;");
        builder.AppendLine("        public int NodeID { get => m_NodeID; set => m_NodeID = value; }");
        builder.AppendLine();
        builder.AppendLine($"        public new string Title => $\"{{m_NodeID}} - {displayTitle}\";");
        builder.AppendLine();
        builder.AppendLine("        public const string PortInputLink = \"InputLink\";");
        builder.AppendLine("        public const string PortOutputLink = \"OutputLink\";");
        foreach (var field in fields.Where(f =>
                     !f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) &&
                     !f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)))
        {
            builder.AppendLine(
                $"        public const string Port{ToPascalCase(field.Name)} = \"{ToPascalCase(field.Name)}\";");
        }

        builder.AppendLine();
        builder.AppendLine("        protected override void OnDefinePorts(IPortDefinitionContext context)");
        builder.AppendLine("        {");
        builder.AppendLine(
            "            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(\"In\").Build();");
        builder.AppendLine(
            "            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(\"Out\").Build();");
        builder.AppendLine();
        foreach (var field in fields.Where(f =>
                     !f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) &&
                     !f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)))
        {
            builder.AppendLine(
                $"            context.AddInputPort<{GetCSharpTypeName(field.FieldType)}>(Port{ToPascalCase(field.Name)}).WithDisplayName(\"{SplitPascalCase(field.Name)}\").Build();");
        }

        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private HashSet<string> GetRequiredUsings()
    {
        var usings = new HashSet<string> { "System", "System.Collections.Generic", "UnityEngine" };
        if (targetMonoBehaviourScript != null)
        {
            usings.Add(targetMonoBehaviourScript.GetClass().Namespace);
        }

        foreach (var type in discoveredTypes)
        {
            if (!string.IsNullOrEmpty(type.Namespace)) usings.Add(type.Namespace);
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!string.IsNullOrEmpty(field.FieldType.Namespace)) usings.Add(field.FieldType.Namespace);
            }
        }

        return usings;
    }

    private void AddEntityToSource(Type type)
    {
        string path = AssetDatabase.GetAssetPath(targetMonoBehaviourScript);
        string content = File.ReadAllText(path);

        string typePattern = $"(struct|class)\\s+{type.Name}";
        Match typeMatch = Regex.Match(content, typePattern);
        if (!typeMatch.Success) return;

        string existingInterfaces =
            Regex.Match(content.Substring(typeMatch.Index), @":\s*([^{]+)").Groups[1].Value.Trim();
        string newContent;

        if (string.IsNullOrEmpty(existingInterfaces))
        {
            newContent = Regex.Replace(content, typePattern, $"$1 {type.Name} : IEntity");
        }
        else
        {
            newContent = Regex.Replace(content, $"({typePattern}\\s*:\\s*{existingInterfaces})", $"$1, IEntity");
        }

        string bodyPattern = $"({typePattern}[^{{}}]*{{)";
        string bodyReplacement = "$1\n        public int Id;\n        public int ID { get => Id; set => Id = value; }";
        newContent = Regex.Replace(newContent, bodyPattern, bodyReplacement);

        if (!newContent.Contains("public interface IEntity"))
        {
            string namespacePattern = @"(namespace\s+[\w\.]+)\s*{";
            Match nsMatch = Regex.Match(newContent, namespacePattern);
            if (nsMatch.Success)
            {
                newContent = newContent.Insert(nsMatch.Groups[1].Index + nsMatch.Groups[1].Length + 1,
                    "\n    public interface IEntity { public int ID { get; set; } }\n");
            }
            else
            {
                newContent = "public interface IEntity { public int ID { get; set; } }\n\n" + newContent;
            }
        }

        File.WriteAllText(path, newContent);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success",
            $"Successfully added IEntity to {type.Name} in {Path.GetFileName(path)}.", "OK");
        FindDiscoverableTypes();
    }

    private static string GetCSharpTypeName(Type type)
    {
        var typeMap = new Dictionary<Type, string>
        {
            { typeof(int), "int" }, { typeof(string), "string" }, { typeof(float), "float" },
            { typeof(bool), "bool" }, { typeof(double), "double" }, { typeof(long), "long" },
            { typeof(short), "short" }, { typeof(byte), "byte" }, { typeof(uint), "uint" },
            { typeof(ulong), "ulong" }, { typeof(ushort), "ushort" }, { typeof(sbyte), "sbyte" },
            { typeof(FixedString32Bytes), "string" }, { typeof(FixedString64Bytes), "string" },
            { typeof(FixedString128Bytes), "string" }, { typeof(FixedString512Bytes), "string" },
            { typeof(FixedString4096Bytes), "string" }
        };
        return typeMap.TryGetValue(type, out var name) ? name : type.FullName.Replace('+', '.');
    }

    private static string ToPascalCase(string input) =>
        string.IsNullOrEmpty(input) ? input : char.ToUpperInvariant(input[0]) + input.Substring(1);

    private static string SplitPascalCase(string input) => string.IsNullOrEmpty(input)
        ? input
        : ToPascalCase(Regex.Replace(input, "(?<!^)([A-Z])", " $1"));

    private static string SimplePluralize(string name) => name.EndsWith("s") ? name + "es" : name + "s";

    private static string GetSerializableDictionaryCode()
    {
        return @"
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> m_Keys = new List<TKey>();
        [SerializeField] private List<TValue> m_Values = new List<TValue>();
        
        private Dictionary<TKey, TValue> m_Dictionary;
        private bool m_IsDeserialized;

        private void EnsureDeserialized()
        {
            if (!m_IsDeserialized)
            {
                m_Dictionary = new Dictionary<TKey, TValue>(m_Keys.Count);
                for (int i = 0; i < m_Keys.Count; i++)
                {
                    if (i < m_Values.Count)
                    {
                        m_Dictionary[m_Keys[i]] = m_Values[i];
                    }
                }
                m_IsDeserialized = true;
            }
        }

        public void OnBeforeSerialize()
        {
            if (m_Dictionary == null) return;
            m_Keys.Clear();
            m_Values.Clear();
            foreach (var kvp in m_Dictionary)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            m_IsDeserialized = false;
        }

        public TValue this[TKey key] { get { EnsureDeserialized(); return m_Dictionary[key]; } set { EnsureDeserialized(); m_Dictionary[key] = value; } }
        public ICollection<TKey> Keys { get { EnsureDeserialized(); return m_Dictionary.Keys; } }
        public ICollection<TValue> Values { get { EnsureDeserialized(); return m_Dictionary.Values; } }
        public int Count { get { EnsureDeserialized(); return m_Dictionary.Count; } }
        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).IsReadOnly;
        public void Add(TKey key, TValue value) { EnsureDeserialized(); m_Dictionary.Add(key, value); }
        public void Add(KeyValuePair<TKey, TValue> item) { EnsureDeserialized(); ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).Add(item); }
        public void Clear() { EnsureDeserialized(); m_Dictionary.Clear(); }
        public bool Contains(KeyValuePair<TKey, TValue> item) { EnsureDeserialized(); return m_Dictionary.Contains(item); }
        public bool ContainsKey(TKey key) { EnsureDeserialized(); return m_Dictionary.ContainsKey(key); }
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { EnsureDeserialized(); ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).CopyTo(array, arrayIndex); }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { EnsureDeserialized(); return m_Dictionary.GetEnumerator(); }
        public bool Remove(TKey key) { EnsureDeserialized(); return m_Dictionary.Remove(key); }
        public bool Remove(KeyValuePair<TKey, TValue> item) { EnsureDeserialized(); return ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).Remove(item); }
        public bool TryGetValue(TKey key, out TValue value) { EnsureDeserialized(); return m_Dictionary.TryGetValue(key, out value); }
        IEnumerator IEnumerable.GetEnumerator() { EnsureDeserialized(); return m_Dictionary.GetEnumerator(); }
    }
";
    }

    private static string GetEditorHelperCode()
    {
        return @"
    internal static class EditorGraphHelper
    {
        public static string ToPascalCase(string input) => string.IsNullOrEmpty(input) ? input : char.ToUpperInvariant(input[0]) + input.Substring(1);
        public static string SplitPascalCase(string input) => string.IsNullOrEmpty(input) ? input : ToPascalCase(Regex.Replace(input, ""(?<!^)([A-Z])"", "" $1""));
    }
";
    }
}