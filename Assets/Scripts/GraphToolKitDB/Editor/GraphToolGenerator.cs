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
    private const string PREFS_ECS_NS = "GraphToolGenerator_ECSNS";
    private const string PREFS_RUNTIME_PATH = "GraphToolGenerator_RuntimePath";
    private const string PREFS_EDITOR_PATH = "GraphToolGenerator_EditorPath";
    private const string PREFS_ECS_PATH = "GraphToolGenerator_ECSPath";
    private const string PREFS_ADD_LINK_STRUCT = "GraphToolGenerator_AddLinkStruct";

    private const string PREFS_GEN_ENTITYTYPE = "GraphToolGenerator_Gen_EntityType";
    private const string PREFS_GEN_RUNTIME_ASSET = "GraphToolGenerator_Gen_RuntimeAsset";
    private const string PREFS_GEN_EDITOR_GRAPH = "GraphToolGenerator_Gen_EditorGraph";
    private const string PREFS_GEN_IMPORTER = "GraphToolGenerator_Gen_Importer";
    private const string PREFS_GEN_BLOB = "GraphToolGenerator_Gen_Blob";
    private const string PREFS_GEN_BAKER = "GraphToolGenerator_Gen_Baker";

    private string graphName;
    private MonoScript targetMonoBehaviourScript;
    private string runtimeNamespace;
    private string editorNamespace;
    private string ecsNamespace;
    private string runtimePath;
    private string editorPath;
    private string ecsPath;
    private bool addLinkStruct;

    private bool genEntityEnum;
    private bool genRuntimeAsset;
    private bool genEditorGraph;
    private bool genImporter;
    private bool genBlobAsset;
    private bool genBaker;

    private List<Type> discoveredTypes = new List<Type>();
    private Vector2 scrollPosition;
    private Texture2D folderIcon, warningIcon, okIcon, infoIcon;

    // Preview
    private class GeneratedFile
    {
        public string Key;
        public string FileName;
        public string Path;
        public string Content;
        public bool Enabled;
    }

    private readonly List<GeneratedFile> previewFiles = new List<GeneratedFile>();
    private int selectedPreviewIndex = 0;
    private Vector2 previewScroll;

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
        ecsNamespace = EditorPrefs.GetString(PREFS_ECS_NS, runtimeNamespace + ".ECS");

        runtimePath = EditorPrefs.GetString(PREFS_RUNTIME_PATH, "Assets/Scripts/GraphToolKitDB/Runtime");
        editorPath = EditorPrefs.GetString(PREFS_EDITOR_PATH, "Assets/Scripts/GraphToolKitDB/Editor");
        ecsPath = EditorPrefs.GetString(PREFS_ECS_PATH, "Assets/Scripts/GraphToolKitDB/Runtime/ECS");

        addLinkStruct = EditorPrefs.GetBool(PREFS_ADD_LINK_STRUCT, true);

        genEntityEnum = EditorPrefs.GetBool(PREFS_GEN_ENTITYTYPE, true);
        genRuntimeAsset = EditorPrefs.GetBool(PREFS_GEN_RUNTIME_ASSET, true);
        genEditorGraph = EditorPrefs.GetBool(PREFS_GEN_EDITOR_GRAPH, true);
        genImporter = EditorPrefs.GetBool(PREFS_GEN_IMPORTER, true);
        genBlobAsset = EditorPrefs.GetBool(PREFS_GEN_BLOB, true);
        genBaker = EditorPrefs.GetBool(PREFS_GEN_BAKER, true);

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
        infoIcon = EditorGUIUtility.IconContent("console.infoicon.sml").image as Texture2D;

        FindDiscoverableTypes();
        RebuildPreviews();
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawConfiguration();

        EditorGUILayout.Space(6);
        DrawDiscoveredTypes();

        EditorGUILayout.Space(6);
        DrawGenerationOptions();

        EditorGUILayout.Space(8);
        DrawPreviewAndExport();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Graph Tool Generator",
            new GUIStyle(EditorStyles.largeLabel) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        EditorGUILayout.HelpBox(
            "Generate runtime + editor GraphToolKit code from a data MonoBehaviour. Now with DOTS/ECS Blob assets, selective generation, and tabbed preview.",
            MessageType.Info);
        EditorGUILayout.EndVertical();
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

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Namespaces", EditorStyles.boldLabel);
        runtimeNamespace = EditorGUILayout.TextField(new GUIContent("Runtime Namespace", "Namespace for the runtime asset script."),
            runtimeNamespace);
        editorNamespace = EditorGUILayout.TextField(new GUIContent("Editor Namespace", "Namespace for the editor-only scripts."),
            editorNamespace);
        ecsNamespace = EditorGUILayout.TextField(new GUIContent("ECS/DOTS Namespace", "Namespace for the GraphDatabaseBlobAsset and related types."),
            ecsNamespace);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Export Paths", EditorStyles.boldLabel);
        runtimePath = DrawPathSelector("Runtime Path", "Folder for runtime scripts (Asset, EntityType).", runtimePath);
        editorPath = DrawPathSelector("Editor Path", "Folder for editor scripts (Graph, Nodes, Importer).", editorPath);
        ecsPath = DrawPathSelector("ECS/DOTS Path", "Folder for ECS scripts (Blob, Builder, Extensions, Baker).", ecsPath);

        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(PREFS_GRAPH_NAME, graphName);
            EditorPrefs.SetString(PREFS_RUNTIME_NS, runtimeNamespace);
            EditorPrefs.SetString(PREFS_EDITOR_NS, editorNamespace);
            EditorPrefs.SetString(PREFS_ECS_NS, ecsNamespace);
            EditorPrefs.SetString(PREFS_RUNTIME_PATH, runtimePath);
            EditorPrefs.SetString(PREFS_EDITOR_PATH, editorPath);
            EditorPrefs.SetString(PREFS_ECS_PATH, ecsPath);

            if (targetMonoBehaviourScript != null)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(targetMonoBehaviourScript, out string guid, out long _);
                EditorPrefs.SetString(PREFS_TARGET_SCRIPT_GUID, guid);
            }

            FindDiscoverableTypes();
            RebuildPreviews();
        }

        EditorGUILayout.EndVertical();
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
        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, EditorStyles.helpBox, GUILayout.MinHeight(100)))
        {
            scrollPosition = scrollView.scrollPosition;

            if (discoveredTypes.Any())
            {
                foreach (var type in discoveredTypes)
                {
                    bool hasEntity = typeof(IEntity).IsAssignableFrom(type);
                    bool blittable = IsBlittableCached(type);
                    EditorGUILayout.BeginHorizontal();

                    var icon = hasEntity ? okIcon : warningIcon;
                    var tooltip = hasEntity ? "Implements IEntity" : "Missing IEntity interface.";
                    GUILayout.Label(new GUIContent(icon, tooltip), GUILayout.Width(20), GUILayout.Height(20));
                    GUILayout.Label(new GUIContent(blittable ? okIcon : warningIcon, blittable ? "Blittable (ECS OK)" : "Not blittable (skipped in Blob)"), GUILayout.Width(20), GUILayout.Height(20));

                    EditorGUILayout.LabelField(type.FullName);

                    if (!hasEntity)
                    {
                        GUI.color = new Color(1f, 0.8f, 0.4f);
                        if (GUILayout.Button(new GUIContent("Implement IEntity", "Auto-add IEntity and ID to the source file."), GUILayout.ExpandWidth(false)))
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
                EditorGUILayout.LabelField("No entity types found. Assign a target script.", EditorStyles.centeredGreyMiniLabel);
            }
        }
    }

    private void DrawGenerationOptions()
    {
        EditorGUILayout.LabelField("What to generate", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUI.BeginChangeCheck();

        bool linkTypeMissing = !discoveredTypes.Any(t => t.Name == "Link");
        if (linkTypeMissing)
        {
            addLinkStruct = EditorGUILayout.ToggleLeft(
                new GUIContent("Generate Link struct (if missing)", "Generates a Link struct and adds a 'Links' list to the runtime asset."),
                addLinkStruct);
            EditorPrefs.SetBool(PREFS_ADD_LINK_STRUCT, addLinkStruct);
        }

        EditorGUILayout.Space(2);
        EditorGUILayout.LabelField("Script Groups", EditorStyles.miniBoldLabel);
        genEntityEnum = TogglePersist("Entity Type enum", genEntityEnum, PREFS_GEN_ENTITYTYPE);
        genRuntimeAsset = TogglePersist($"{graphName}Asset (ScriptableObject)", genRuntimeAsset, PREFS_GEN_RUNTIME_ASSET);
        genEditorGraph = TogglePersist($"{graphName}Graph (Editor)", genEditorGraph, PREFS_GEN_EDITOR_GRAPH);
        genImporter = TogglePersist($"{graphName}Importer (ScriptedImporter)", genImporter, PREFS_GEN_IMPORTER);

        EditorGUILayout.Space(2);
        genBlobAsset = TogglePersist($"{graphName}BlobAsset + Builder + Extensions (ECS/DOTS)", genBlobAsset, PREFS_GEN_BLOB);
        genBaker = TogglePersist($"{graphName}Authoring + Baker (ECS/DOTS)", genBaker, PREFS_GEN_BAKER);

        if (EditorGUI.EndChangeCheck())
        {
            RebuildPreviews();
        }

        EditorGUILayout.EndVertical();
    }

    private bool TogglePersist(string label, bool value, string key)
    {
        bool newValue = EditorGUILayout.ToggleLeft(label, value);
        if (newValue != value) EditorPrefs.SetBool(key, newValue);
        return newValue;
    }

    private void DrawPreviewAndExport()
    {
        EditorGUILayout.LabelField("Preview & Export", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        // Validate paths
        var errors = new List<string>();
        if (genEntityEnum || genRuntimeAsset) ValidatePath(runtimePath, "Runtime Path", errors);
        if (genEditorGraph || genImporter) ValidatePath(editorPath, "Editor Path", errors);
        if (genBlobAsset || genBaker) ValidatePath(ecsPath, "ECS/DOTS Path", errors);

        if (errors.Count > 0)
        {
            foreach (var e in errors) EditorGUILayout.HelpBox(e, MessageType.Warning);
        }

        // Tabs
        var enabledFiles = previewFiles.Where(f => f.Enabled).ToList();
        if (enabledFiles.Count == 0)
        {
            EditorGUILayout.HelpBox("Nothing selected to generate. Toggle scripts above to preview.", MessageType.Info);
        }
        else
        {
            string[] tabNames = enabledFiles.Select(f => f.FileName).ToArray();
            selectedPreviewIndex = Mathf.Clamp(selectedPreviewIndex, 0, enabledFiles.Count - 1);
            selectedPreviewIndex = GUILayout.Toolbar(selectedPreviewIndex, tabNames, GUILayout.MinHeight(24));

            var current = enabledFiles[selectedPreviewIndex];
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(infoIcon), GUILayout.Width(18), GUILayout.Height(18));
            EditorGUILayout.SelectableLabel($"{current.Path}/{current.FileName}", EditorStyles.miniLabel, GUILayout.Height(18));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Copy to clipboard", GUILayout.Width(140)))
            {
                EditorGUIUtility.systemCopyBuffer = current.Content;
            }
            if (GUILayout.Button("Save This File", GUILayout.Width(120)))
            {
                SaveSingleFile(current);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(2);
            var style = new GUIStyle(EditorStyles.textArea) { fontSize = 11, wordWrap = false };
            style.font = EditorStyles.miniFont;
            using (var sv = new EditorGUILayout.ScrollViewScope(previewScroll, GUILayout.MinHeight(220)))
            {
                previewScroll = sv.scrollPosition;
                EditorGUILayout.TextArea(current.Content, style, GUILayout.ExpandHeight(true));
            }
        }

        EditorGUILayout.Space(6);
        EditorGUI.BeginDisabledGroup(enabledFiles.Count == 0);
        if (GUILayout.Button("Generate Selected Scripts", GUILayout.Height(36)))
        {
            GenerateAndExportSelectedFiles();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndVertical();
    }

    private void ValidatePath(string path, string label, List<string> errors)
    {
        if (string.IsNullOrEmpty(path)) errors.Add($"{label} is empty.");
        if (!string.IsNullOrEmpty(path) && !path.StartsWith("Assets"))
            errors.Add($"{label} must be inside 'Assets/'.");
    }

    private void SaveSingleFile(GeneratedFile file)
    {
        try
        {
            Directory.CreateDirectory(file.Path);
            File.WriteAllText(Path.Combine(file.Path, file.FileName), file.Content);
            AssetDatabase.Refresh();
            ShowNotification(new GUIContent($"Saved: {file.FileName}"));
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed saving {file.FileName}: {e}");
            EditorUtility.DisplayDialog("Save Error", $"Failed to write {file.FileName}. See console.", "OK");
        }
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

    private void RebuildPreviews()
    {
        previewFiles.Clear();
        if (string.IsNullOrEmpty(graphName)) return;

        var allUsings = GetRequiredUsings();

        if (genEntityEnum)
            previewFiles.Add(new GeneratedFile
            {
                Key = "EntityType",
                FileName = "EntityType.cs",
                Path = runtimePath,
                Content = GenerateEntityTypeEnum(),
                Enabled = true
            });

        if (genRuntimeAsset)
            previewFiles.Add(new GeneratedFile
            {
                Key = "RuntimeAsset",
                FileName = $"{graphName}Asset.cs",
                Path = runtimePath,
                Content = GenerateRuntimeCode(allUsings),
                Enabled = true
            });

        if (genEditorGraph)
            previewFiles.Add(new GeneratedFile
            {
                Key = "EditorGraph",
                FileName = $"{graphName}Editor.cs",
                Path = editorPath,
                Content = GenerateEditorCode(allUsings),
                Enabled = true
            });

        if (genImporter)
            previewFiles.Add(new GeneratedFile
            {
                Key = "Importer",
                FileName = $"{graphName}Importer.cs",
                Path = editorPath,
                Content = GenerateImporterCode(allUsings),
                Enabled = true
            });

        if (genBlobAsset)
            previewFiles.Add(new GeneratedFile
            {
                Key = "Blob",
                FileName = $"{graphName}BlobAsset.cs",
                Path = ecsPath,
                Content = GenerateBlobAssetCode(allUsings),
                Enabled = true
            });

        if (genBaker)
            previewFiles.Add(new GeneratedFile
            {
                Key = "Baker",
                FileName = $"{graphName}AuthoringAndBaker.cs",
                Path = ecsPath,
                Content = GenerateBakerCode(),
                Enabled = true
            });

        selectedPreviewIndex = 0;
    }

    private void GenerateAndExportSelectedFiles()
    {
        if ((genEntityEnum || genRuntimeAsset) && string.IsNullOrEmpty(runtimePath) ||
            (genEditorGraph || genImporter) && string.IsNullOrEmpty(editorPath) ||
            (genBlobAsset || genBaker) && string.IsNullOrEmpty(ecsPath))
        {
            EditorUtility.DisplayDialog("Error", "Please set valid export paths for all selected script groups.", "OK");
            return;
        }

        try
        {
            foreach (var file in previewFiles.Where(f => f.Enabled))
            {
                Directory.CreateDirectory(file.Path);
                File.WriteAllText(Path.Combine(file.Path, file.FileName), file.Content);
            }
            AssetDatabase.Refresh();

            var savedList = string.Join("\n- ", previewFiles.Where(f => f.Enabled).Select(f => $"{f.Path}/{f.FileName}"));
            EditorUtility.DisplayDialog("Success", $"Saved files:\n\n- {savedList}", "OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write generated files: {e}");
            EditorUtility.DisplayDialog("Error", "Failed to write generated files. Check the console for details.", "OK");
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
        foreach (var type in discoveredTypes.Where(t => t.Name != "Link"))
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

        // If link type is missing but we’re generating it, also add the list to the asset
        bool linkTypeMissing = !discoveredTypes.Any(t => t.Name == "Link");
        if (linkTypeMissing && addLinkStruct)
        {
            builder.AppendLine("        public List<Link> Links = new List<Link>();");
        }
        builder.AppendLine("    }");

        // Generate Link struct if missing and requested
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
        builder.AppendLine("        private SerializableDictionary<Type, int> nodeIdCounters = new SerializableDictionary<Type, int>();");
        builder.AppendLine();
        builder.AppendLine($"        [MenuItem(\"Assets/Create/{graphName} Graph\")]");
        builder.AppendLine($"        private static void CreateAssetFile() => GraphDatabase.PromptInProjectBrowserToCreateNewAsset<{graphClassName}>(\"New {graphName}\");");
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
        builder.AppendLine("                if (sourceNode is IVariableNode vn) { vn.variable.TryGetDefaultValue(out T v); return v; }");
        builder.AppendLine("            } else { port.TryGetValue(out T v); return v; }");
        builder.AppendLine("            return default;");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("    #endregion");
        builder.AppendLine();
        builder.AppendLine("    #region Node Definitions");
        builder.AppendLine("    public interface IDataNode : INode { int NodeID { get; set; } }");
        builder.AppendLine();
        foreach (var type in discoveredTypes.Where(t => t.Name != "Link"))
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
        bool hasLinkSupport = discoveredTypes.Any(t => t.Name == "Link") || addLinkStruct;

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
        builder.AppendLine($"            var graph = GraphDatabase.LoadGraphForImporter<{graphClassName}>(ctx.assetPath);");
        builder.AppendLine("            if (graph == null) return;");
        builder.AppendLine();
        builder.AppendLine($"            var runtimeAsset = ScriptableObject.CreateInstance<{assetClassName}>();");
        builder.AppendLine("            var nodeToIdMap = new Dictionary<INode, int>();");
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

        if (hasLinkSupport)
        {
            builder.AppendLine();
            builder.AppendLine("            // --- PASS 2: Create links by traversing graph connections ---");
            builder.AppendLine("            int linkId = 0;");
            builder.AppendLine("            foreach (var sourceNode in graph.GetNodes())");
            builder.AppendLine("            {");
            builder.AppendLine("                var outputPort = sourceNode.GetOutputPorts().FirstOrDefault(p => p.name == \"OutputLink\");");
            builder.AppendLine("                if (outputPort == null || !outputPort.isConnected) continue;");
            builder.AppendLine("                if (!nodeToIdMap.TryGetValue(sourceNode, out int sourceId)) continue;");
            builder.AppendLine();
            builder.AppendLine("                var sourceDataType = GetNodeType(sourceNode);");
            builder.AppendLine("                if (sourceDataType == null) continue;");
            builder.AppendLine();
            builder.AppendLine("                var connectedPorts = new List<IPort>();");
            builder.AppendLine("                outputPort.GetConnectedPorts(connectedPorts);");
            builder.AppendLine("                foreach (var connectedPort in connectedPorts.Where(p => p.name == \"InputLink\"))");
            builder.AppendLine("                {");
            builder.AppendLine("                    var targetNode = connectedPort.GetNode();");
            builder.AppendLine("                    if (!nodeToIdMap.TryGetValue(targetNode, out int targetId)) continue;");
            builder.AppendLine();
            builder.AppendLine("                    var targetDataType = GetNodeType(targetNode);");
            builder.AppendLine("                    if (targetDataType == null) continue;");
            builder.AppendLine();
            builder.AppendLine("                    var link = new Link");
            builder.AppendLine("                    {");
            builder.AppendLine("                        ID = linkId++,");
            builder.AppendLine("                        SourceType = (EntityType)Enum.Parse(typeof(EntityType), sourceDataType.Name),");
            builder.AppendLine("                        SourceID = sourceId,");
            builder.AppendLine("                        TargetType = (EntityType)Enum.Parse(typeof(EntityType), targetDataType.Name),");
            builder.AppendLine("                        TargetID = targetId");
            builder.AppendLine("                    };");
            builder.AppendLine("                    runtimeAsset.Links.Add(link);");
            builder.AppendLine("                }");
            builder.AppendLine("            }");
        }

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
        builder.AppendLine("    #endregion");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private string GenerateBlobAssetCode(HashSet<string> usings)
    {
        var blobTypes = discoveredTypes.Where(IsBlittableCached).ToList();
        bool hasLink = discoveredTypes.Any(t => t.Name == "Link") || addLinkStruct;
        var builder = new StringBuilder();

        string assetClassName = $"{graphName}Asset";
        string blobClassName = $"{graphName}BlobAsset";

        builder.AppendLine($"// ----- AUTO-GENERATED ECS/DOTS BLOB FILE BY {nameof(GraphToolGenerator)}.cs -----");
        builder.AppendLine();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Runtime.CompilerServices;");
        builder.AppendLine("using Unity.Collections;");
        builder.AppendLine("using Unity.Collections.LowLevel.Unsafe;");
        builder.AppendLine("using Unity.Entities;");
        builder.AppendLine("using Unity.Mathematics;");
        foreach (var u in usings.OrderBy(x => x)) builder.AppendLine($"using {u};");
        builder.AppendLine($"using {runtimeNamespace};");
        builder.AppendLine();
        builder.AppendLine($"namespace {ecsNamespace}");
        builder.AppendLine("{");
        builder.AppendLine("    // Blob asset for ultra-fast runtime queries");
        builder.AppendLine($"    public struct {blobClassName}");
        builder.AppendLine("    {");
        foreach (var t in blobTypes.Where(t => t.Name != "Link"))
        {
            builder.AppendLine($"        public BlobArray<{t.Name}> {SimplePluralize(t.Name)};");
        }
        if (hasLink)
        {
            builder.AppendLine("        public BlobArray<Link> Links;");
            builder.AppendLine("        public BlobArray<int> OutgoingIndices;");
            builder.AppendLine("        public BlobArray<LinkEdge> OutgoingEdges;");
            builder.AppendLine("        public BlobArray<int> IncomingIndices;");
            builder.AppendLine("        public BlobArray<LinkEdge> IncomingEdges;");
        }
        builder.AppendLine("        public int MaxEntityId;");
        builder.AppendLine("    }");
        builder.AppendLine();

        if (hasLink)
        {
            builder.AppendLine("    // Edge data for CSR graph traversal");
            builder.AppendLine("    public struct LinkEdge");
            builder.AppendLine("    {");
            builder.AppendLine("        public int TargetId;");
            builder.AppendLine("        public EntityType TargetType;");
            builder.AppendLine("    }");
            builder.AppendLine();
        }

        builder.AppendLine($"    public struct {graphName}Component : IComponentData");
        builder.AppendLine("    {");
        builder.AppendLine($"        public BlobAssetReference<{blobClassName}> Blob;");
        builder.AppendLine("    }");
        builder.AppendLine();

        builder.AppendLine($"    public static class {graphName}BlobBuilder");
        builder.AppendLine("    {");
        builder.AppendLine($"        public static BlobAssetReference<{blobClassName}> CreateBlobAsset(this " + assetClassName + " sourceAsset)");
        builder.AppendLine("        {");
        builder.AppendLine("            using var builder = new BlobBuilder(Allocator.Temp);");
        builder.AppendLine($"            ref var root = ref builder.ConstructRoot<{blobClassName}>();");
        builder.AppendLine();

        // Copy arrays
        foreach (var t in blobTypes.Where(t => t.Name != "Link"))
        {
            string plural = SimplePluralize(t.Name);
            builder.AppendLine($"            CopyArray(builder, ref root.{plural}, sourceAsset.{plural});");
        }
        if (hasLink)
        {
            builder.AppendLine($"            CopyArray(builder, ref root.Links, sourceAsset.Links);");
            builder.AppendLine($"            BuildCsrLinkSystem(builder, ref root, sourceAsset.Links);");
        }
        builder.AppendLine("            root.MaxEntityId = CalculateMaxEntityId(sourceAsset);");
        builder.AppendLine();
        builder.AppendLine($"            return builder.CreateBlobAssetReference<{blobClassName}>(Allocator.Persistent);");
        builder.AppendLine("        }");
        builder.AppendLine();

        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine("        private static void CopyArray<T>(BlobBuilder builder, ref BlobArray<T> target, System.Collections.Generic.List<T> source) where T : struct");
        builder.AppendLine("        {");
        builder.AppendLine("            var array = builder.Allocate(ref target, source != null ? source.Count : 0);");
        builder.AppendLine("            if (source == null || source.Count == 0) return;");
        builder.AppendLine("            for (int i = 0; i < source.Count; i++) array[i] = source[i];");
        builder.AppendLine("        }");
        builder.AppendLine();

        if (hasLink)
        {
            builder.AppendLine("        private static void BuildCsrLinkSystem(BlobBuilder builder, ref " + blobClassName + " root, System.Collections.Generic.List<Link> links)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (links == null || links.Count == 0)");
            builder.AppendLine("            {");
            builder.AppendLine("                builder.Allocate(ref root.OutgoingIndices, 1)[0] = 0;");
            builder.AppendLine("                builder.Allocate(ref root.OutgoingEdges, 0);");
            builder.AppendLine("                builder.Allocate(ref root.IncomingIndices, 1)[0] = 0;");
            builder.AppendLine("                builder.Allocate(ref root.IncomingEdges, 0);");
            builder.AppendLine("                return;");
            builder.AppendLine("            }");
            builder.AppendLine();
            builder.AppendLine("            int maxId = CalculateMaxEntityIdFromLinks(links);");
            builder.AppendLine("            var outgoingGroups = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<LinkEdge>>();");
            builder.AppendLine("            foreach (var link in links)");
            builder.AppendLine("            {");
            builder.AppendLine("                if (!outgoingGroups.TryGetValue(link.SourceID, out var list))");
            builder.AppendLine("                    outgoingGroups[link.SourceID] = list = new System.Collections.Generic.List<LinkEdge>();");
            builder.AppendLine("                list.Add(new LinkEdge { TargetId = link.TargetID, TargetType = link.TargetType });");
            builder.AppendLine("            }");
            builder.AppendLine();
            builder.AppendLine("            var outIndices = builder.Allocate(ref root.OutgoingIndices, maxId + 2);");
            builder.AppendLine("            var outEdgesList = new System.Collections.Generic.List<LinkEdge>();");
            builder.AppendLine("            outIndices[0] = 0;");
            builder.AppendLine("            for (int i = 0; i <= maxId; i++)");
            builder.AppendLine("            {");
            builder.AppendLine("                if (outgoingGroups.TryGetValue(i, out var edges)) outEdgesList.AddRange(edges);");
            builder.AppendLine("                outIndices[i + 1] = outEdgesList.Count;");
            builder.AppendLine("            }");
            builder.AppendLine("            var outEdges = builder.Allocate(ref root.OutgoingEdges, outEdgesList.Count);");
            builder.AppendLine("            for (int i = 0; i < outEdgesList.Count; i++) outEdges[i] = outEdgesList[i];");
            builder.AppendLine();
            builder.AppendLine("            var incomingGroups = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<LinkEdge>>();");
            builder.AppendLine("            foreach (var link in links)");
            builder.AppendLine("            {");
            builder.AppendLine("                if (!incomingGroups.TryGetValue(link.TargetID, out var list))");
            builder.AppendLine("                    incomingGroups[link.TargetID] = list = new System.Collections.Generic.List<LinkEdge>();");
            builder.AppendLine("                list.Add(new LinkEdge { TargetId = link.SourceID, TargetType = link.SourceType });");
            builder.AppendLine("            }");
            builder.AppendLine("            var inIndices = builder.Allocate(ref root.IncomingIndices, maxId + 2);");
            builder.AppendLine("            var inEdgesList = new System.Collections.Generic.List<LinkEdge>();");
            builder.AppendLine("            inIndices[0] = 0;");
            builder.AppendLine("            for (int i = 0; i <= maxId; i++)");
            builder.AppendLine("            {");
            builder.AppendLine("                if (incomingGroups.TryGetValue(i, out var edges)) inEdgesList.AddRange(edges);");
            builder.AppendLine("                inIndices[i + 1] = inEdgesList.Count;");
            builder.AppendLine("            }");
            builder.AppendLine("            var inEdges = builder.Allocate(ref root.IncomingEdges, inEdgesList.Count);");
            builder.AppendLine("            for (int i = 0; i < inEdgesList.Count; i++) inEdges[i] = inEdgesList[i];");
            builder.AppendLine("        }");
            builder.AppendLine();
        }

        builder.AppendLine("        private static int CalculateMaxEntityId(" + assetClassName + " asset)");
        builder.AppendLine("        {");
        builder.AppendLine("            int max = 0;");
        foreach (var t in blobTypes.Where(t => t.Name != "Link"))
        {
            string plural = SimplePluralize(t.Name);
            builder.AppendLine($"            if (asset.{plural} != null && asset.{plural}.Count > 0) max = math.max(max, asset.{plural}.Count - 1);");
        }
        builder.AppendLine("            return max;");
        builder.AppendLine("        }");
        if (hasLink)
        {
            builder.AppendLine();
            builder.AppendLine("        private static int CalculateMaxEntityIdFromLinks(System.Collections.Generic.List<Link> links)");
            builder.AppendLine("        {");
            builder.AppendLine("            int max = 0;");
            builder.AppendLine("            for (int i = 0; i < links.Count; i++)");
            builder.AppendLine("            {");
            builder.AppendLine("                var l = links[i];");
            builder.AppendLine("                if (l.SourceID > max) max = l.SourceID;");
            builder.AppendLine("                if (l.TargetID > max) max = l.TargetID;");
            builder.AppendLine("            }");
            builder.AppendLine("            return max;");
            builder.AppendLine("        }");
        }
        builder.AppendLine("    }");
        builder.AppendLine();

        builder.AppendLine($"    public static unsafe class {graphName}BlobExtensions");
        builder.AppendLine("    {");
        // GetEntity<T>
        builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"        public static ref readonly T GetEntity<T>(this ref {blobClassName} db, int id) where T : struct, IEntity");
        builder.AppendLine("        {");
        foreach (var t in blobTypes.Where(t => t.Name != "Link"))
        {
            string plural = SimplePluralize(t.Name);
            builder.AppendLine($"            if (typeof(T) == typeof({t.Name})) {{ ref var r = ref db.{plural}[id]; return ref Unsafe.As<{t.Name}, T>(ref r); }}");
        }
        builder.AppendLine("            throw new ArgumentException($\"Unsupported entity type: {typeof(T)}\");");
        builder.AppendLine("        }");
        builder.AppendLine();

        if (hasLink)
        {
            // Outgoing/Incoming
            builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            builder.AppendLine($"        public static ReadOnlySpan<LinkEdge> GetOutgoingEdges(this ref {blobClassName} db, int entityId)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (entityId >= db.OutgoingIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;");
            builder.AppendLine("            int start = db.OutgoingIndices[entityId];");
            builder.AppendLine("            int end = db.OutgoingIndices[entityId + 1];");
            builder.AppendLine("            int count = end - start;");
            builder.AppendLine("            if (count <= 0) return ReadOnlySpan<LinkEdge>.Empty;");
            builder.AppendLine("            return new ReadOnlySpan<LinkEdge>((LinkEdge*)db.OutgoingEdges.GetUnsafePtr() + start, count);");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            builder.AppendLine($"        public static ReadOnlySpan<LinkEdge> GetIncomingEdges(this ref {blobClassName} db, int entityId)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (entityId >= db.IncomingIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;");
            builder.AppendLine("            int start = db.IncomingIndices[entityId];");
            builder.AppendLine("            int end = db.IncomingIndices[entityId + 1];");
            builder.AppendLine("            int count = end - start;");
            builder.AppendLine("            if (count <= 0) return ReadOnlySpan<LinkEdge>.Empty;");
            builder.AppendLine("            return new ReadOnlySpan<LinkEdge>((LinkEdge*)db.IncomingEdges.GetUnsafePtr() + start, count);");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            builder.AppendLine($"        public static void GetLinkedEntities<T>(this ref {blobClassName} db, int sourceId, EntityType targetType, ref NativeList<T> results) where T : unmanaged, IEntity");
            builder.AppendLine("        {");
            builder.AppendLine("            results.Clear();");
            builder.AppendLine("            var edges = db.GetOutgoingEdges(sourceId);");
            builder.AppendLine("            for (int i = 0; i < edges.Length; i++)");
            builder.AppendLine("                if (edges[i].TargetType == targetType) results.Add(db.GetEntity<T>(edges[i].TargetId));");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        public static bool HasPath(this ref {blobClassName} db, int sourceId, int targetId, int maxDepth = 6)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (sourceId == targetId) return true;");
            builder.AppendLine("            if (maxDepth <= 0 || sourceId > db.MaxEntityId || targetId > db.MaxEntityId) return false;");
            builder.AppendLine("            const int MAX_STACK_SIZE = 512;");
            builder.AppendLine("            if (db.MaxEntityId < MAX_STACK_SIZE)");
            builder.AppendLine("            {");
            builder.AppendLine("                var visited = stackalloc bool[db.MaxEntityId + 1];");
            builder.AppendLine("                return HasPathRecursive(ref db, sourceId, targetId, maxDepth, visited);");
            builder.AppendLine("            }");
            builder.AppendLine("            else");
            builder.AppendLine("            {");
            builder.AppendLine("                var visited = new NativeArray<bool>(db.MaxEntityId + 1, Allocator.Temp);");
            builder.AppendLine("                bool result = HasPathRecursiveHeap(ref db, sourceId, targetId, maxDepth, visited);");
            builder.AppendLine("                visited.Dispose();");
            builder.AppendLine("                return result;");
            builder.AppendLine("            }");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        private static bool HasPathRecursive(ref {blobClassName} db, int current, int target, int depth, bool* visited)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (current == target) return true;");
            builder.AppendLine("            if (depth <= 0 || visited[current]) return false;");
            builder.AppendLine("            visited[current] = true;");
            builder.AppendLine("            var edges = db.GetOutgoingEdges(current);");
            builder.AppendLine("            for (int i = 0; i < edges.Length; i++)");
            builder.AppendLine("                if (HasPathRecursive(ref db, edges[i].TargetId, target, depth - 1, visited)) return true;");
            builder.AppendLine("            return false;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        private static bool HasPathRecursiveHeap(ref {blobClassName} db, int current, int target, int depth, NativeArray<bool> visited)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (current == target) return true;");
            builder.AppendLine("            if (depth <= 0 || visited[current]) return false;");
            builder.AppendLine("            visited[current] = true;");
            builder.AppendLine("            var edges = db.GetOutgoingEdges(current);");
            builder.AppendLine("            for (int i = 0; i < edges.Length; i++)");
            builder.AppendLine("                if (HasPathRecursiveHeap(ref db, edges[i].TargetId, target, depth - 1, visited)) return true;");
            builder.AppendLine("            return false;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            builder.AppendLine($"        public static bool IsConnectedTo(this ref {blobClassName} db, int sourceId, int targetId, EntityType targetType = EntityType.None)");
            builder.AppendLine("        {");
            builder.AppendLine("            var edges = db.GetOutgoingEdges(sourceId);");
            builder.AppendLine("            for (int i = 0; i < edges.Length; i++)");
            builder.AppendLine("                if (edges[i].TargetId == targetId && (targetType == EntityType.None || edges[i].TargetType == targetType)) return true;");
            builder.AppendLine("            return false;");
            builder.AppendLine("        }");
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private string GenerateBakerCode()
    {
        string assetClassName = $"{graphName}Asset";
        string blobClassName = $"{graphName}BlobAsset";
        var builder = new StringBuilder();

        builder.AppendLine($"// ----- AUTO-GENERATED ECS AUTHORING + BAKER BY {nameof(GraphToolGenerator)}.cs -----");
        builder.AppendLine();
        builder.AppendLine("using Unity.Entities;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine($"using {ecsNamespace};");
        builder.AppendLine();
        builder.AppendLine($"namespace {ecsNamespace}");
        builder.AppendLine("{");
        builder.AppendLine($"    public sealed class {graphName}Authoring : MonoBehaviour");
        builder.AppendLine("    {");
        builder.AppendLine($"        public {assetClassName} SourceAsset;");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine($"    public sealed class {graphName}Baker : Baker<" + graphName + "Authoring>");
        builder.AppendLine("    {");
        builder.AppendLine($"        public override void Bake({graphName}Authoring authoring)");
        builder.AppendLine("        {");
        builder.AppendLine("            if (authoring.SourceAsset == null) return;");
        builder.AppendLine("            var blob = authoring.SourceAsset.CreateBlobAsset();");
        builder.AppendLine("            var entity = GetEntity(TransformUsageFlags.None);");
        builder.AppendLine($"            AddComponent(entity, new {graphName}Component {{ Blob = blob }});");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
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
        foreach (var field in fields.Where(f => !f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) && !f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)))
        {
            builder.AppendLine($"        public const string Port{ToPascalCase(field.Name)} = \"{ToPascalCase(field.Name)}\";");
        }
        builder.AppendLine();
        builder.AppendLine("        protected override void OnDefinePorts(IPortDefinitionContext context)");
        builder.AppendLine("        {");
        builder.AppendLine("            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(\"In\").Build();");
        builder.AppendLine("            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(\"Out\").Build();");
        builder.AppendLine();
        foreach (var field in fields.Where(f => !f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) && !f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)))
        {
            builder.AppendLine($"            context.AddInputPort<{GetCSharpTypeName(field.FieldType)}>(Port{ToPascalCase(field.Name)}).WithDisplayName(\"{SplitPascalCase(field.Name)}\").Build();");
        }
        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private HashSet<string> GetRequiredUsings()
    {
        var usings = new HashSet<string> { "System", "System.Collections.Generic", "UnityEngine" };
        if (targetMonoBehaviourScript != null)
        {
            var cls = targetMonoBehaviourScript.GetClass();
            if (cls != null && !string.IsNullOrEmpty(cls.Namespace))
                usings.Add(cls.Namespace);
        }
        foreach (var type in discoveredTypes)
        {
            if (!string.IsNullOrEmpty(type.Namespace)) usings.Add(type.Namespace);
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.FieldType.Namespace != null) usings.Add(field.FieldType.Namespace);
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

        string existingInterfaces = Regex.Match(content.Substring(typeMatch.Index), @":\s*([^{]+)").Groups[1].Value.Trim();
        string newContent;

        if (string.IsNullOrEmpty(existingInterfaces))
            newContent = Regex.Replace(content, typePattern, $"$1 {type.Name} : IEntity");
        else
            newContent = Regex.Replace(content, $"({typePattern}\\s*:\\s*{existingInterfaces})", $"$1, IEntity");

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
        RebuildPreviews();
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
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { EnsureDeserialized(); ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary)).CopyTo(array, arrayIndex); }
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

    // --------- Blittability Cache/Check for ECS generation ------------
    private readonly Dictionary<Type, bool> _blittableCache = new Dictionary<Type, bool>();

    private bool IsBlittableCached(Type t)
    {
        if (_blittableCache.TryGetValue(t, out var v)) return v;
        v = IsBlittable(t, new HashSet<Type>());
        _blittableCache[t] = v;
        return v;
    }

    private bool IsBlittable(Type t, HashSet<Type> visited)
    {
        if (t == null) return false;
        if (!t.IsValueType) return false;
        if (t.IsPrimitive || t.IsEnum) return true;
        if (t == typeof(decimal)) return false;
        if (t.FullName != null && t.FullName.StartsWith("Unity.Collections.FixedString")) return true;

        if (!visited.Add(t)) return true; // prevent recursion loops

        var fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var f in fields)
        {
            var ft = f.FieldType;
            if (ft == typeof(string) || !IsBlittable(ft, visited)) return false;
        }
        return true;
    }
}