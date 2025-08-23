// FILE: GraphToolGenerator.cs

using System;
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

namespace GraphToolKitDB.Editor
{
    public class GraphToolGenerator : EditorWindow
    {
        // EditorPrefs Keys
        private const string PrefsGraphName = "GraphToolGenerator_GraphName";
        private const string PrefsTargetScriptGuid = "GraphToolGenerator_TargetScriptGUID";
        private const string PrefsRuntimeNs = "GraphToolGenerator_RuntimeNS";
        private const string PrefsEditorNs = "GraphToolGenerator_EditorNS";
        private const string PrefsEcsNs = "GraphToolGenerator_ECSNS";
        private const string PrefsRuntimePath = "GraphToolGenerator_RuntimePath";
        private const string PrefsEditorPath = "GraphToolGenerator_EditorPath";
        private const string PrefsEcsPath = "GraphToolGenerator_ECSPath";
        private const string PrefsAddLinkStruct = "GraphToolGenerator_AddLinkStruct";

        private const string PrefsGenEntityType = "GraphToolGenerator_Gen_EntityType";
        private const string PrefsGenRuntimeAsset = "GraphToolGenerator_Gen_RuntimeAsset";
        private const string PrefsGenEditorGraph = "GraphToolGenerator_Gen_EditorGraph";
        private const string PrefsGenImporter = "GraphToolGenerator_Gen_Importer";
        private const string PrefsGenBlob = "GraphToolGenerator_Gen_Blob";
        private const string PrefsGenBaker = "GraphToolGenerator_Gen_Baker";

        private string _graphName;
        private MonoScript _targetMonoBehaviourScript;
        private string _runtimeNamespace;
        private string _editorNamespace;
        private string _ecsNamespace;
        private string _runtimePath;
        private string _editorPath;
        private string _ecsPath;
        private bool _addLinkStruct;

        private bool _genEntityEnum;
        private bool _genRuntimeAsset;
        private bool _genEditorGraph;
        private bool _genImporter;
        private bool _genBlobAsset;
        private bool _genBaker;

        private List<Type> _discoveredTypes = new List<Type>();
        private Vector2 _scrollPosition;
        private Vector2 _mainScrollPosition; // For the main window scroll view
        private Texture2D _folderIcon;

        // Preview
        private class GeneratedFile
        {
            public string FileName;
            public string Path;
            public string Content;
            public bool Enabled;
        }

        private readonly List<GeneratedFile> _previewFiles = new List<GeneratedFile>();
        private int _selectedPreviewIndex;
        private Vector2 _previewScroll;

        [MenuItem("Tools/Graph Code Generator")]
        public static void ShowWindow()
        {
            GetWindow<GraphToolGenerator>("Graph Tool Generator");
        }

        private void OnEnable()
        {
            // Load settings from EditorPrefs
            _graphName = EditorPrefs.GetString(PrefsGraphName, "GameDatabase");
            _runtimeNamespace = EditorPrefs.GetString(PrefsRuntimeNs, "GraphToolKitDB.Runtime");
            _editorNamespace = EditorPrefs.GetString(PrefsEditorNs, "GraphToolKitDB.Editor");
            _ecsNamespace = EditorPrefs.GetString(PrefsEcsNs, _runtimeNamespace + ".ECS");

            _runtimePath = EditorPrefs.GetString(PrefsRuntimePath, "Assets/Scripts/GraphToolKitDB/Runtime");
            _editorPath = EditorPrefs.GetString(PrefsEditorPath, "Assets/Scripts/GraphToolKitDB/Editor");
            _ecsPath = EditorPrefs.GetString(PrefsEcsPath, "Assets/Scripts/GraphToolKitDB/Runtime/ECS");

            _addLinkStruct = EditorPrefs.GetBool(PrefsAddLinkStruct, true);

            _genEntityEnum = EditorPrefs.GetBool(PrefsGenEntityType, true);
            _genRuntimeAsset = EditorPrefs.GetBool(PrefsGenRuntimeAsset, true);
            _genEditorGraph = EditorPrefs.GetBool(PrefsGenEditorGraph, true);
            _genImporter = EditorPrefs.GetBool(PrefsGenImporter, true);
            _genBlobAsset = EditorPrefs.GetBool(PrefsGenBlob, true);
            _genBaker = EditorPrefs.GetBool(PrefsGenBaker, true);

            string scriptGuid = EditorPrefs.GetString(PrefsTargetScriptGuid);
            if (!string.IsNullOrEmpty(scriptGuid))
            {
                string path = AssetDatabase.GUIDToAssetPath(scriptGuid);
                if (!string.IsNullOrEmpty(path))
                {
                    _targetMonoBehaviourScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                }
            }

            _folderIcon = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D;
            FindDiscoverableTypes();
            RebuildPreviews();
        }

        private void OnGUI()
        {
            _mainScrollPosition = EditorGUILayout.BeginScrollView(_mainScrollPosition);
            {
                DrawPreviewAndExport();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        DrawConfiguration();
                        DrawGenerationOptions();
                        EditorGUILayout.EndVertical();
                    }
                    DrawDiscoveredTypes();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawConfiguration()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            _graphName = EditorGUILayout.TextField(
                new GUIContent("Graph Name", "The base name for generated classes and the asset extension."),
                _graphName);
            EditorGUILayout.LabelField("File Extension", _graphName.ToLowerInvariant().Replace(" ", ""));

            _targetMonoBehaviourScript = (MonoScript)EditorGUILayout.ObjectField(
                new GUIContent("Target Script", "The script containing your data lists (e.g., GameDatabase.cs)."),
                _targetMonoBehaviourScript, typeof(MonoScript), false);

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Namespaces", EditorStyles.boldLabel);
            _runtimeNamespace = EditorGUILayout.TextField(
                new GUIContent("Runtime Namespace", "Namespace for the runtime asset script."),
                _runtimeNamespace);
            _editorNamespace = EditorGUILayout.TextField(
                new GUIContent("Editor Namespace", "Namespace for the editor-only scripts."),
                _editorNamespace);
            _ecsNamespace = EditorGUILayout.TextField(
                new GUIContent("ECS/DOTS Namespace", "Namespace for the GraphDatabaseBlobAsset and related types."),
                _ecsNamespace);

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Export Paths", EditorStyles.boldLabel);
            _runtimePath = DrawPathSelector("Runtime Path", "Folder for runtime scripts (Asset, EntityType).",
                _runtimePath);
            _editorPath = DrawPathSelector("Editor Path", "Folder for editor scripts (Graph, Nodes, Importer).",
                _editorPath);
            _ecsPath = DrawPathSelector("ECS/DOTS Path", "Folder for ECS scripts (Blob, Builder, Extensions, Baker).",
                _ecsPath);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(PrefsGraphName, _graphName);
                EditorPrefs.SetString(PrefsRuntimeNs, _runtimeNamespace);
                EditorPrefs.SetString(PrefsEditorNs, _editorNamespace);
                EditorPrefs.SetString(PrefsEcsNs, _ecsNamespace);
                EditorPrefs.SetString(PrefsRuntimePath, _runtimePath);
                EditorPrefs.SetString(PrefsEditorPath, _editorPath);
                EditorPrefs.SetString(PrefsEcsPath, _ecsPath);

                if (_targetMonoBehaviourScript != null)
                {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_targetMonoBehaviourScript, out string guid,
                        out long _);
                    EditorPrefs.SetString(PrefsTargetScriptGuid, guid);
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
            if (GUILayout.Button(new GUIContent(_folderIcon, $"Browse for {label}"), GUILayout.Width(30),
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
            using var scrollView =
                new EditorGUILayout.ScrollViewScope(_scrollPosition, EditorStyles.helpBox, GUILayout.MaxHeight(450));
            EditorGUILayout.LabelField("Discovered Entity Types", EditorStyles.boldLabel);
            _scrollPosition = scrollView.scrollPosition;

            if (_discoveredTypes.Any())
            {
                foreach (var type in _discoveredTypes)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(type.FullName);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No entity types found. Assign a target script.",
                    EditorStyles.centeredGreyMiniLabel);
            }
        }

        private void DrawGenerationOptions()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("What to generate", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            bool linkTypeMissing = _discoveredTypes.All(t => t.Name != "Link");
            if (linkTypeMissing)
            {
                _addLinkStruct = EditorGUILayout.ToggleLeft(
                    new GUIContent("Generate Link struct (if missing)",
                        "Generates a Link struct and adds a 'Links' list to the runtime asset."),
                    _addLinkStruct);
                EditorPrefs.SetBool(PrefsAddLinkStruct, _addLinkStruct);
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Script Groups", EditorStyles.miniBoldLabel);
            _genEntityEnum = TogglePersist("Entity Type enum", _genEntityEnum, PrefsGenEntityType);
            _genRuntimeAsset =
                TogglePersist($"{_graphName}Asset (ScriptableObject)", _genRuntimeAsset, PrefsGenRuntimeAsset);
            _genEditorGraph = TogglePersist($"{_graphName}Graph (Editor)", _genEditorGraph, PrefsGenEditorGraph);
            _genImporter = TogglePersist($"{_graphName}Importer (ScriptedImporter)", _genImporter, PrefsGenImporter);

            EditorGUILayout.Space(2);
            _genBlobAsset = TogglePersist($"{_graphName}BlobAsset + Builder + Extensions (ECS/DOTS)", _genBlobAsset,
                PrefsGenBlob);
            _genBaker = TogglePersist($"{_graphName}Authoring + Baker (ECS/DOTS)", _genBaker, PrefsGenBaker);

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
            EditorGUILayout.BeginVertical("box");

            // Validate paths
            var errors = new List<string>();
            if (_genEntityEnum || _genRuntimeAsset) ValidatePath(_runtimePath, "Runtime Path", errors);
            if (_genEditorGraph || _genImporter) ValidatePath(_editorPath, "Editor Path", errors);
            if (_genBlobAsset || _genBaker) ValidatePath(_ecsPath, "ECS/DOTS Path", errors);

            if (errors.Count > 0)
            {
                foreach (var e in errors) EditorGUILayout.HelpBox(e, MessageType.Warning);
            }

            // Tabs
            var enabledFiles = _previewFiles.Where(f => f.Enabled).ToList();
            if (enabledFiles.Count == 0)
            {
                EditorGUILayout.HelpBox("Nothing selected to generate. Toggle scripts above to preview.",
                    MessageType.Info);
            }
            else
            {
                string[] tabNames = enabledFiles.Select(f => f.FileName).ToArray();
                _selectedPreviewIndex = Mathf.Clamp(_selectedPreviewIndex, 0, enabledFiles.Count - 1);
                _selectedPreviewIndex = GUILayout.Toolbar(_selectedPreviewIndex, tabNames, GUILayout.MinHeight(24));

                var current = enabledFiles[_selectedPreviewIndex];
                TextArea(current);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.SelectableLabel($"{current.Path}/{current.FileName}", GUILayout.Height(18));
                if (GUILayout.Button("Copy to clipboard", GUILayout.Width(140)))
                {
                    EditorGUIUtility.systemCopyBuffer = current.Content;
                }

                if (GUILayout.Button("Save This File", GUILayout.Width(120)))
                {
                    SaveSingleFile(current);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.BeginDisabledGroup(enabledFiles.Count == 0);
            if (GUILayout.Button("Generate Selected Scripts", GUILayout.Height(36)))
            {
                GenerateAndExportSelectedFiles();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }

        private void TextArea(GeneratedFile current)
        {
            var style = new GUIStyle(EditorStyles.textArea)
            {
                fontSize = 11, wordWrap = false,
                font = EditorStyles.miniFont
            };
            using var sv = new EditorGUILayout.ScrollViewScope(_previewScroll);
            _previewScroll = sv.scrollPosition;
            EditorGUILayout.TextArea(current.Content, style, GUILayout.ExpandHeight(true));
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
            _discoveredTypes.Clear();
            if (_targetMonoBehaviourScript == null) return;

            var targetType = _targetMonoBehaviourScript.GetClass();
            if (targetType == null) return;

            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type entityType = field.FieldType.GetGenericArguments()[0];
                    _discoveredTypes.Add(entityType);
                }
            }

            _discoveredTypes = _discoveredTypes.Distinct().OrderBy(t => t.Name).ToList();
        }

        private void RebuildPreviews()
        {
            _previewFiles.Clear();
            if (string.IsNullOrEmpty(_graphName)) return;

            var allUsings = GetRequiredUsings();

            if (_genEntityEnum)
                _previewFiles.Add(new GeneratedFile
                {
                    FileName = "EntityType.cs",
                    Path = _runtimePath,
                    Content = GenerateEntityTypeEnum(),
                    Enabled = true
                });

            if (_genRuntimeAsset)
                _previewFiles.Add(new GeneratedFile
                {
                    FileName = $"{_graphName}Asset.cs",
                    Path = _runtimePath,
                    Content = GenerateRuntimeCode(allUsings),
                    Enabled = true
                });

            if (_genEditorGraph)
                _previewFiles.Add(new GeneratedFile
                {
                    FileName = $"{_graphName}Editor.cs",
                    Path = _editorPath,
                    Content = GenerateEditorCode(allUsings),
                    Enabled = true
                });

            if (_genImporter)
                _previewFiles.Add(new GeneratedFile
                {
                    FileName = $"{_graphName}Importer.cs",
                    Path = _editorPath,
                    Content = GenerateImporterCode(allUsings),
                    Enabled = true
                });

            if (_genBlobAsset)
            {
                _previewFiles.Add(new GeneratedFile
                {
                    FileName = $"{_graphName}BlobAsset.cs",
                    Path = _ecsPath,
                    Content = GenerateBlobAssetCode(allUsings),
                    Enabled = true
                });
                _previewFiles.Add(new GeneratedFile
                {
                    FileName = $"{_graphName}BlobQueryExtensions.cs",
                    Path = _ecsPath,
                    Content = GenerateBlobQueryExtensionsCode(allUsings),
                    Enabled = true
                });
            }

            if (_genBaker)
                _previewFiles.Add(new GeneratedFile
                {
                    FileName = $"{_graphName}AuthoringAndBaker.cs",
                    Path = _ecsPath,
                    Content = GenerateBakerCode(),
                    Enabled = true
                });

            _selectedPreviewIndex = 0;
        }

        private void GenerateAndExportSelectedFiles()
        {
            if ((_genEntityEnum || _genRuntimeAsset) && string.IsNullOrEmpty(_runtimePath) ||
                (_genEditorGraph || _genImporter) && string.IsNullOrEmpty(_editorPath) ||
                (_genBlobAsset || _genBaker) && string.IsNullOrEmpty(_ecsPath))
            {
                EditorUtility.DisplayDialog("Error", "Please set valid export paths for all selected script groups.",
                    "OK");
                return;
            }

            try
            {
                foreach (var file in _previewFiles.Where(f => f.Enabled))
                {
                    Directory.CreateDirectory(file.Path);
                    File.WriteAllText(Path.Combine(file.Path, file.FileName), file.Content);
                }

                AssetDatabase.Refresh();

                var savedList = string.Join("\n- ",
                    _previewFiles.Where(f => f.Enabled).Select(f => $"{f.Path}/{f.FileName}"));
                EditorUtility.DisplayDialog("Success", $"Saved files:\n\n- {savedList}", "OK");
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
            builder.AppendLine($"namespace {_runtimeNamespace}");
            builder.AppendLine("{");
            builder.AppendLine("    public enum EntityType : ushort");
            builder.AppendLine("    {");
            builder.AppendLine("        None = 0,");
            ushort idCounter = 1;
            foreach (var type in _discoveredTypes.Where(t => t.Name != "Link"))
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
            string assetClassName = $"{_graphName}Asset";

            builder.AppendLine($"// ----- AUTO-GENERATED RUNTIME FILE BY {nameof(GraphToolGenerator)}.cs -----");
            builder.AppendLine();
            foreach (var u in usings.OrderBy(x => x)) builder.AppendLine($"using {u};");
            builder.AppendLine();
            builder.AppendLine($"namespace {_runtimeNamespace}");
            builder.AppendLine("{");
            builder.AppendLine($"    public class {assetClassName} : ScriptableObject");
            builder.AppendLine("    {");
            foreach (var type in _discoveredTypes)
            {
                string listName = type.Name == "Link" ? "Links" : SimplePluralize(type.Name);
                builder.AppendLine($"        public List<{type.Name}> {listName} = new List<{type.Name}>();");
            }

            // If link type is missing, but we’re generating it, also add the list to the asset
            bool linkTypeMissing = _discoveredTypes.All(t => t.Name != "Link");
            if (linkTypeMissing && _addLinkStruct)
            {
                builder.AppendLine("        public List<Link> Links = new List<Link>();");
            }

            builder.AppendLine("    }");

            // Generate Link struct if missing and requested
            if (linkTypeMissing && _addLinkStruct)
            {
                builder.AppendLine();
                builder.AppendLine("    [Serializable]");
                builder.AppendLine("    public struct Link");
                builder.AppendLine("    {");
                builder.AppendLine("        public ushort Id;");
                builder.AppendLine("        public ushort ID { get => Id; set => Id = value; }");
                builder.AppendLine("        public EntityType SourceType;");
                builder.AppendLine("        public ushort SourceID;");
                builder.AppendLine("        public EntityType TargetType;");
                builder.AppendLine("        public ushort TargetID;");
                builder.AppendLine("    }");
            }

            builder.AppendLine("}");
            return builder.ToString();
        }

        private string GenerateEditorCode(HashSet<string> usings)
        {
            var builder = new StringBuilder();
            string graphClassName = $"{_graphName}Graph";
            string assetExtension = _graphName.ToLowerInvariant().Replace(" ", "");

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
            builder.AppendLine($"using {_runtimeNamespace};");
            builder.AppendLine();
            builder.AppendLine($"namespace {_editorNamespace}");
            builder.AppendLine("{");
            builder.AppendLine(GetSerializableDictionaryCode());
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
            builder.AppendLine($"        [MenuItem(\"Assets/Create/{_graphName} Graph\")]");
            builder.AppendLine(
                $"        private static void CreateAssetFile() => GraphDatabase.PromptInProjectBrowserToCreateNewAsset<{graphClassName}>(\"New {_graphName}\");");
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
            builder.AppendLine("                    node.NodeID = (ushort)currentId;");
            builder.AppendLine("                    nodeIdCounters[nodeType] = currentId;");
            builder.AppendLine("                }");
            builder.AppendLine("            }");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public static T GetPortValue<T>(IPort port) {");
            builder.AppendLine("            if (port.isConnected) {");
            builder.AppendLine("                var sourceNode = port.firstConnectedPort?.GetNode();");
            builder.AppendLine(
                "                if (sourceNode is IConstantNode c) { c.TryGetValue(out T v); return v; }");
            builder.AppendLine(
                "                if (sourceNode is IVariableNode vn) { vn.variable.TryGetDefaultValue(out T v); return v; }");
            builder.AppendLine("            } else { port.TryGetValue(out T v); return v; }");
            builder.AppendLine("            return default;");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("    #endregion");
            builder.AppendLine();
            builder.AppendLine("    #region Node Definitions");
            builder.AppendLine();
            builder.AppendLine(
                "    public interface IDataNode : INode\n    {\n        ushort NodeID { get; set; }\n        EntityType EntityType { get; }\n    }");
            builder.AppendLine();
            builder.AppendLine("    [UseWithContext(typeof(AchievementNode), typeof(AchievementStateNode))]// all nodes need to be added manually, as it the only supported way// YOU GEMINI AI ADD ALL\n    [Serializable]\n    public class EntryIDBlock : BlockNode\n    {\n        public const string OptionLink = \"Schema\";\n\n        protected override void OnDefineOptions(IOptionDefinitionContext context)\n        {\n            base.OnDefineOptions(context);\n            context.AddOption<EntryIDSchema>(OptionLink).Build();\n        }\n    }");
            builder.AppendLine();
            builder.AppendLine(
                "    [Serializable]\n    public abstract class GraphDBNode : ContextNode, IDataNode\n    {\n        public const string PortInputLink = \"InputLink\";\n        public const string PortOutputLink = \"OutputLink\";\n        \n        [SerializeField] private ushort nodeId;\n        public ushort NodeID\n        {\n            get => nodeId;\n            set => nodeId = value;\n        }\n\n        public abstract EntityType EntityType { get; }\n\n        protected override void OnDefinePorts(IPortDefinitionContext context)\n        {\n            context.AddInputPort<ushort>(PortInputLink).WithDisplayName(\"In\").Build();\n            context.AddOutputPort<ushort>(PortOutputLink).WithDisplayName(\"Out\").Build();\n        }\n    }");
            builder.AppendLine();
            builder.AppendLine(
                "   public class DataNode : IDataNode\n    {\n        public ushort NodeID { get; set; }\n        public EntityType EntityType { get; set; }\n    }");
            foreach (var type in _discoveredTypes.Where(t => t.Name != "Link"))
            {
                GenerateNodeClassForType(builder, type);
                builder.AppendLine();
            }

            builder.AppendLine("    #endregion");
            builder.AppendLine("}");
            return builder.ToString();
        }

        private string GenerateImporterCode(HashSet<string> usings)
        {
            var builder = new StringBuilder();
            string graphClassName = $"{_graphName}Graph";
            string assetClassName = $"{_graphName}Asset";
            string importerClassName = $"{_graphName}Importer";
            string assetExtension = _graphName.ToLowerInvariant().Replace(" ", "");
            bool hasLinkSupport = _discoveredTypes.Any(t => t.Name == "Link") || _addLinkStruct;

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
            builder.AppendLine($"using {_runtimeNamespace};");
            builder.AppendLine();
            builder.AppendLine($"namespace {_editorNamespace}");
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
            builder.AppendLine("            var nodeToIdMap = new Dictionary<INode, GraphDBNode>();");
            builder.AppendLine();
            builder.AppendLine(
                "            // --- PASS 1: Create all data entries and map nodes to their index-based ID ---");
            foreach (var type in _discoveredTypes.Where(t => t.Name != "Link"))
            {
                string nodeClassName = $"{type.Name}Node";
                string pluralName = SimplePluralize(type.Name);
                builder.AppendLine(
                    $"            var {type.Name.ToLower()}Nodes = graph.GetNodes().OfType<{nodeClassName}>().ToList();");
                builder.AppendLine($"            for(ushort i = 0; i < {type.Name.ToLower()}Nodes.Count; i++)");
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

                builder.AppendLine($"                runtimeAsset.{pluralName}.Add(data);");
                builder.AppendLine(
                    "                node.NodeID = i;\n                nodeToIdMap[node] = node;");
                builder.AppendLine("            }");
            }

            if (hasLinkSupport) builder.AppendLine("            PopulateLink(graph, nodeToIdMap, runtimeAsset);");
            builder.AppendLine($"            ctx.AddObjectToAsset(\"{_graphName}\", runtimeAsset);");
            builder.AppendLine("            ctx.SetMainObject(runtimeAsset);");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        private static void PopulateLink(GDBGraph graph, Dictionary<INode, GraphDBNode> nodeToIdMap, GDBAsset runtimeAsset)\n        {\n            ushort linkId = 0;\n            foreach (var sourceNode in graph.GetNodes().OfType<GraphDBNode>())\n            {\n                var outputPort = sourceNode.GetOutputPorts().FirstOrDefault(p => p.name == GraphDBNode.PortOutputLink);\n                foreach (var linkBlock in sourceNode.blockNodes)\n                {\n                    var nodeOption = linkBlock.GetNodeOptionByName(EntryIDBlock.OptionLink);\n                    if (!nodeOption.TryGetValue(out EntryIDSchema entryIDSchema)) continue;\n                    if (entryIDSchema == null) continue;\n                    entryIDSchema.Id = sourceNode.NodeID;\n                    entryIDSchema.Type = sourceNode.EntityType;\n                }\n\n                if (outputPort == null || !outputPort.isConnected) continue;\n\n\n                var connectedPorts = new List<IPort>();\n                outputPort.GetConnectedPorts(connectedPorts);\n                foreach (var connectedPort in connectedPorts.Where(p => p.name == GraphDBNode.PortInputLink))\n                {\n                    var targetNode = connectedPort.GetNode();\n                    if (!nodeToIdMap.TryGetValue(targetNode, out var targetData)) continue;\n                    var link = new Link\n                    {\n                        ID = linkId++,\n                        SourceType = sourceNode.EntityType,\n                        SourceID = sourceNode.NodeID,\n                        TargetType = targetData.EntityType,\n                        TargetID = targetData.NodeID,\n                    };\n                    runtimeAsset.Links.Add(link);\n                }\n            }\n        }");
            
            builder.AppendLine("    }");
            builder.AppendLine("    #endregion");
            builder.AppendLine("}");
            return builder.ToString();
        }

        private string GenerateBlobAssetCode(HashSet<string> usings)
        {
            var blobTypes = _discoveredTypes.ToList();
            bool hasLink = _discoveredTypes.Any(t => t.Name == "Link") || _addLinkStruct;
            var builder = new StringBuilder();

            string assetClassName = $"{_graphName}Asset";
            string blobClassName = $"{_graphName}BlobAsset";

            builder.AppendLine($"// ----- AUTO-GENERATED ECS/DOTS BLOB FILE BY {nameof(GraphToolGenerator)}.cs -----");
            builder.AppendLine();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Runtime.CompilerServices;");
            builder.AppendLine("using Unity.Collections;");
            builder.AppendLine("using Unity.Entities;");
            builder.AppendLine("using Unity.Mathematics;");
            foreach (var u in usings.OrderBy(x => x)) builder.AppendLine($"using {u};");
            builder.AppendLine($"using {_runtimeNamespace};");
            builder.AppendLine();
            builder.AppendLine($"namespace {_ecsNamespace}");
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
                builder.AppendLine("        public BlobArray<ushort> OutgoingIndices;");
                builder.AppendLine("        public BlobArray<LinkEdge> OutgoingEdges;");
                builder.AppendLine("        public BlobArray<ushort> IncomingIndices;");
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
                builder.AppendLine("        public ushort TargetId;");
                builder.AppendLine("        public EntityType TargetType;");
                builder.AppendLine("    }");
                builder.AppendLine();
            }

            builder.AppendLine($"    public struct {_graphName}Component : IComponentData");
            builder.AppendLine("    {");
            builder.AppendLine($"        public BlobAssetReference<{blobClassName}> Blob;");
            builder.AppendLine("    }");
            builder.AppendLine();
            
            builder.AppendLine("#if UNITY_EDITOR");
            builder.AppendLine($"    public static class {_graphName}BlobBuilder");
            builder.AppendLine("    {");
            builder.AppendLine($"        public static BlobAssetReference<{blobClassName}> CreateBlobAsset(this " +
                               assetClassName + " sourceAsset)");
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
            builder.AppendLine(
                $"            return builder.CreateBlobAssetReference<{blobClassName}>(Allocator.Persistent);");
            builder.AppendLine("        }");
            builder.AppendLine();

            builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            builder.AppendLine(
                "        private static void CopyArray<T>(BlobBuilder builder, ref BlobArray<T> target, System.Collections.Generic.List<T> source) where T : struct");
            builder.AppendLine("        {");
            builder.AppendLine("            var array = builder.Allocate(ref target, source?.Count ?? 0);");
            builder.AppendLine("            if (source == null || source.Count == 0) return;");
            builder.AppendLine("            for (int i = 0; i < source.Count; i++) array[i] = source[i];");
            builder.AppendLine("        }");
            builder.AppendLine();

            if (hasLink)
            {
                builder.AppendLine("        private static void BuildCsrLinkSystem(BlobBuilder builder, ref " +
                                   blobClassName + " root, System.Collections.Generic.List<Link> links)");
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
                builder.AppendLine(
                    "            var outgoingGroups = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<LinkEdge>>();");
                builder.AppendLine("            foreach (var link in links)");
                builder.AppendLine("            {");
                builder.AppendLine("                if (!outgoingGroups.TryGetValue(link.SourceID, out var list))");
                builder.AppendLine(
                    "                    outgoingGroups[link.SourceID] = list = new System.Collections.Generic.List<LinkEdge>();");
                builder.AppendLine(
                    "                list.Add(new LinkEdge { TargetId = link.TargetID, TargetType = link.TargetType });");
                builder.AppendLine("            }");
                builder.AppendLine();
                builder.AppendLine(
                    "            var outIndices = builder.Allocate(ref root.OutgoingIndices, maxId + 2);");
                builder.AppendLine("            var outEdgesList = new System.Collections.Generic.List<LinkEdge>();");
                builder.AppendLine("            outIndices[0] = 0;");
                builder.AppendLine("            for (int i = 0; i <= maxId; i++)");
                builder.AppendLine("            {");
                builder.AppendLine(
                    "                if (outgoingGroups.TryGetValue(i, out var edges)) outEdgesList.AddRange(edges);");
                builder.AppendLine("                outIndices[i + 1] = (ushort)outEdgesList.Count;");
                builder.AppendLine("            }");
                builder.AppendLine(
                    "            var outEdges = builder.Allocate(ref root.OutgoingEdges, outEdgesList.Count);");
                builder.AppendLine(
                    "            for (int i = 0; i < outEdgesList.Count; i++) outEdges[i] = outEdgesList[i];");
                builder.AppendLine();
                builder.AppendLine(
                    "            var incomingGroups = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<LinkEdge>>();");
                builder.AppendLine("            foreach (var link in links)");
                builder.AppendLine("            {");
                builder.AppendLine("                if (!incomingGroups.TryGetValue(link.TargetID, out var list))");
                builder.AppendLine(
                    "                    incomingGroups[link.TargetID] = list = new System.Collections.Generic.List<LinkEdge>();");
                builder.AppendLine(
                    "                list.Add(new LinkEdge { TargetId = link.SourceID, TargetType = link.SourceType });");
                builder.AppendLine("            }");
                builder.AppendLine(
                    "            var inIndices = builder.Allocate(ref root.IncomingIndices, maxId + 2);");
                builder.AppendLine("            var inEdgesList = new System.Collections.Generic.List<LinkEdge>();");
                builder.AppendLine("            inIndices[0] = 0;");
                builder.AppendLine("            for (int i = 0; i <= maxId; i++)");
                builder.AppendLine("            {");
                builder.AppendLine(
                    "                if (incomingGroups.TryGetValue(i, out var edges)) inEdgesList.AddRange(edges);");
                builder.AppendLine("                inIndices[i + 1] = (ushort)inEdgesList.Count;");
                builder.AppendLine("            }");
                builder.AppendLine(
                    "            var inEdges = builder.Allocate(ref root.IncomingEdges, inEdgesList.Count);");
                builder.AppendLine(
                    "            for (int i = 0; i < inEdgesList.Count; i++) inEdges[i] = inEdgesList[i];");
                builder.AppendLine("        }");
                builder.AppendLine();
            }

            builder.AppendLine("        private static int CalculateMaxEntityId(" + assetClassName + " asset)");
            builder.AppendLine("        {");
            builder.AppendLine("            int max = 0;");
            foreach (var t in blobTypes.Where(t => t.Name != "Link"))
            {
                string plural = SimplePluralize(t.Name);
                builder.AppendLine(
                    $"            if (asset.{plural} is {{ Count: > 0 }}) max = math.max(max, asset.{plural}.Count - 1);");
            }

            builder.AppendLine("            return max;");
            builder.AppendLine("        }");
            if (hasLink)
            {
                builder.AppendLine();
                builder.AppendLine(
                    "        private static int CalculateMaxEntityIdFromLinks(System.Collections.Generic.List<Link> links)\n        {\n            int max = 0;\n            foreach (var l in links)\n            {\n                if (l.SourceID > max) max = l.SourceID;\n                if (l.TargetID > max) max = l.TargetID;\n            }\n            return max;\n        }");
            }

            builder.AppendLine("    }");
            builder.AppendLine("#endif");
            builder.AppendLine("}");
            return builder.ToString();
        }

        private string GenerateBlobQueryExtensionsCode(HashSet<string> usings)
        {
            var blobTypes = _discoveredTypes.ToList();
            bool hasLink = _discoveredTypes.Any(t => t.Name == "Link") || _addLinkStruct;
            var builder = new StringBuilder();
            string blobClassName = $"{_graphName}BlobAsset";

            builder.AppendLine(
                $"// ----- AUTO-GENERATED ECS/DOTS QUERY EXTENSIONS FILE BY {nameof(GraphToolGenerator)}.cs -----");
            builder.AppendLine();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Runtime.CompilerServices;");
            builder.AppendLine("using Unity.Collections;");
            builder.AppendLine("using Unity.Collections.LowLevel.Unsafe;");
            builder.AppendLine("using Unity.Entities;");
            foreach (var u in usings.OrderBy(x => x)) builder.AppendLine($"using {u};");
            builder.AppendLine($"using {_runtimeNamespace};");
            builder.AppendLine();
            builder.AppendLine($"namespace {_ecsNamespace}");
            builder.AppendLine("{");
            builder.AppendLine($"    public static unsafe class {_graphName}BlobExtensions");
            builder.AppendLine("    {");
            // GetEntity<T>
            builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            builder.AppendLine(
                $"        public static ref readonly T GetEntity<T>(this ref {blobClassName} db, ushort id) where T : struct");
            builder.AppendLine("        {");
            foreach (var t in blobTypes.Where(t => t.Name != "Link"))
            {
                string plural = SimplePluralize(t.Name);
                builder.AppendLine(
                    $"            if (typeof(T) == typeof({t.Name})) {{ ref var r = ref db.{plural}[id]; return ref Unsafe.As<{t.Name}, T>(ref r); }}");
            }

            builder.AppendLine("            throw new ArgumentException($\"Unsupported entity type: {typeof(T)}\");");
            builder.AppendLine("        }");
            builder.AppendLine();

            if (hasLink)
            {
                // Outgoing/Incoming
                builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
                builder.AppendLine(
                    $"        public static ReadOnlySpan<LinkEdge> GetOutgoingEdges(this ref {blobClassName} db, ushort entityId)");
                builder.AppendLine("        {");
                builder.AppendLine(
                    "            if (entityId >= db.OutgoingIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;");
                builder.AppendLine("            int start = db.OutgoingIndices[entityId];");
                builder.AppendLine("            int end = db.OutgoingIndices[entityId + 1];");
                builder.AppendLine("            int count = end - start;");
                builder.AppendLine("            if (count <= 0) return ReadOnlySpan<LinkEdge>.Empty;");
                builder.AppendLine(
                    "            return new ReadOnlySpan<LinkEdge>((LinkEdge*)db.OutgoingEdges.GetUnsafePtr() + start, count);");
                builder.AppendLine("        }");
                builder.AppendLine();
                builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
                builder.AppendLine(
                    $"        public static ReadOnlySpan<LinkEdge> GetIncomingEdges(this ref {blobClassName} db, ushort entityId)");
                builder.AppendLine("        {");
                builder.AppendLine(
                    "            if (entityId >= db.IncomingIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;");
                builder.AppendLine("            int start = db.IncomingIndices[entityId];");
                builder.AppendLine("            int end = db.IncomingIndices[entityId + 1];");
                builder.AppendLine("            int count = end - start;");
                builder.AppendLine("            if (count <= 0) return ReadOnlySpan<LinkEdge>.Empty;");
                builder.AppendLine(
                    "            return new ReadOnlySpan<LinkEdge>((LinkEdge*)db.IncomingEdges.GetUnsafePtr() + start, count);");
                builder.AppendLine("        }");
                builder.AppendLine();
                builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
                builder.AppendLine(
                    $"        public static void GetLinkedEntities<T>(this ref {blobClassName} db, ushort sourceId, EntityType targetType, ref NativeList<T> results) where T : unmanaged");
                builder.AppendLine("        {");
                builder.AppendLine("            results.Clear();");
                builder.AppendLine("            var edges = db.GetOutgoingEdges(sourceId);");
                builder.AppendLine("            for (int i = 0; i < edges.Length; i++)");
                builder.AppendLine(
                    "                if (edges[i].TargetType == targetType) results.Add(db.GetEntity<T>(edges[i].TargetId));");
                builder.AppendLine("        }");
                builder.AppendLine();
                builder.AppendLine(
                    $"        public static bool HasPath(this ref {blobClassName} db, ushort sourceId, ushort targetId, ushort maxDepth = 6)");
                builder.AppendLine("        {");
                builder.AppendLine("            if (sourceId == targetId) return true;");
                builder.AppendLine(
                    "            if (maxDepth <= 0 || sourceId > db.MaxEntityId || targetId > db.MaxEntityId) return false;");
                builder.AppendLine("            const int MAX_STACK_SIZE = 512;");
                builder.AppendLine("            if (db.MaxEntityId < MAX_STACK_SIZE)");
                builder.AppendLine("            {");
                builder.AppendLine("                var visited = stackalloc bool[db.MaxEntityId + 1];");
                builder.AppendLine(
                    "                return HasPathRecursive(ref db, sourceId, targetId, maxDepth, visited);");
                builder.AppendLine("            }");
                builder.AppendLine("            else");
                builder.AppendLine("            {");
                builder.AppendLine(
                    "                var visited = new NativeArray<bool>(db.MaxEntityId + 1, Allocator.Temp);");
                builder.AppendLine(
                    "                bool result = HasPathRecursiveHeap(ref db, sourceId, targetId, maxDepth, visited);");
                builder.AppendLine("                visited.Dispose();");
                builder.AppendLine("                return result;");
                builder.AppendLine("            }");
                builder.AppendLine("        }");
                builder.AppendLine();
                builder.AppendLine(
                    $"        private static bool HasPathRecursive(ref {blobClassName} db, ushort current, ushort target, ushort depth, bool* visited)");
                builder.AppendLine("        {");
                builder.AppendLine("            if (current == target) return true;");
                builder.AppendLine("            if (depth <= 0 || visited[current]) return false;");
                builder.AppendLine("            visited[current] = true;");
                builder.AppendLine("            var edges = db.GetOutgoingEdges(current);");
                builder.AppendLine("            for (int i = 0; i < edges.Length; i++)");
                builder.AppendLine(
                    "                if (HasPathRecursive(ref db, edges[i].TargetId, target, (ushort)(depth - 1), visited)) return true;");
                builder.AppendLine("            return false;");
                builder.AppendLine("        }");
                builder.AppendLine();
                builder.AppendLine(
                    $"        private static bool HasPathRecursiveHeap(ref {blobClassName} db, ushort current, ushort target, ushort depth, NativeArray<bool> visited)");
                builder.AppendLine("        {");
                builder.AppendLine("            if (current == target) return true;");
                builder.AppendLine("            if (depth <= 0 || visited[current]) return false;");
                builder.AppendLine("            visited[current] = true;");
                builder.AppendLine("            var edges = db.GetOutgoingEdges(current);");
                builder.AppendLine("            for (int i = 0; i < edges.Length; i++)");
                builder.AppendLine(
                    "                if (HasPathRecursiveHeap(ref db, edges[i].TargetId, target, (ushort)(depth - 1), visited)) return true;");
                builder.AppendLine("            return false;");
                builder.AppendLine("        }");
                builder.AppendLine();
                builder.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
                builder.AppendLine(
                    $"        public static bool IsConnectedTo(this ref {blobClassName} db, ushort sourceId, ushort targetId, EntityType targetType = EntityType.None)");
                builder.AppendLine("        {");
                builder.AppendLine("            var edges = db.GetOutgoingEdges(sourceId);");
                builder.AppendLine("            for (int i = 0; i < edges.Length; i++)");
                builder.AppendLine(
                    "                if (edges[i].TargetId == targetId && (targetType == EntityType.None || edges[i].TargetType == targetType)) return true;");
                builder.AppendLine("            return false;");
                builder.AppendLine("        }");
            }

            builder.AppendLine("    }");
            builder.AppendLine("}");
            return builder.ToString();
        }

        private string GenerateBakerCode()
        {
            string assetClassName = $"{_graphName}Asset";
            var builder = new StringBuilder();

            builder.AppendLine(
                $"// ----- AUTO-GENERATED ECS AUTHORING + BAKER BY {nameof(GraphToolGenerator)}.cs -----");
            builder.AppendLine("UNITY_EDITOR");
            builder.AppendLine();
            builder.AppendLine("using Unity.Entities;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine($"using {_ecsNamespace};");
            builder.AppendLine();
            builder.AppendLine($"namespace {_ecsNamespace}");
            builder.AppendLine("{");
            builder.AppendLine($"    public sealed class {_graphName}Authoring : MonoBehaviour");
            builder.AppendLine("    {");
            builder.AppendLine($"        public {assetClassName} SourceAsset;");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine($"    public sealed class {_graphName}Baker : Baker<" + _graphName + "Authoring>");
            builder.AppendLine("    {");
            builder.AppendLine($"        public override void Bake({_graphName}Authoring authoring)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (authoring.SourceAsset == null) return;");
            builder.AppendLine("            var blob = authoring.SourceAsset.CreateBlobAsset();");
            builder.AppendLine("            var entity = GetEntity(TransformUsageFlags.None);");
            builder.AppendLine($"            AddComponent(entity, new {_graphName}Component {{ Blob = blob }});");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            builder.AppendLine("#endif");
            return builder.ToString();
        }

        private void GenerateNodeClassForType(StringBuilder builder, Type dataType)
        {
            string nodeClassName = $"{dataType.Name}Node";
            var fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            builder.AppendLine($"    [Serializable]");
            builder.AppendLine($"    public class {nodeClassName} : GraphDBNode, IDataNode");
            builder.AppendLine("    {");
            builder.AppendLine($"        public override EntityType EntityType => EntityType.{dataType.Name};");
            builder.AppendLine();
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
            builder.AppendLine("            base.OnDefinePorts(context);");
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
            if (_targetMonoBehaviourScript != null)
            {
                var cls = _targetMonoBehaviourScript.GetClass();
                if (cls != null && !string.IsNullOrEmpty(cls.Namespace))
                    usings.Add(cls.Namespace);
            }

            foreach (var type in _discoveredTypes)
            {
                if (!string.IsNullOrEmpty(type.Namespace)) usings.Add(type.Namespace);
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (field.FieldType.Namespace != null) usings.Add(field.FieldType.Namespace);
                }
            }

            return usings;
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
            return typeMap.TryGetValue(type, out var name) ? name : type.FullName?.Replace('+', '.');
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
    }
}