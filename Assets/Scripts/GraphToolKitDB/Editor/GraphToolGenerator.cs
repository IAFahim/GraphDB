// FILE: GraphToolGenerator.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using GraphToolKitDB.Runtime;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An Editor Window that reflects on a target MonoBehaviour (like GameDatabase)
/// and generates a complete, link-aware GraphToolKit implementation.
/// </summary>
public class GraphToolGenerator : EditorWindow
{
    private const string DefaultGraphName = "GameDatabase";
    private string graphName = DefaultGraphName;
    private MonoScript targetMonoBehaviourScript;
    private List<Type> discoveredTypes = new List<Type>();
    private Vector2 scrollPosition;

    [MenuItem("Tools/Graph Tool Generator (Link-Aware)")]
    public static void ShowWindow()
    {
        GetWindow<GraphToolGenerator>("Graph Tool Generator");
    }

    private void OnEnable()
    {
        if (targetMonoBehaviourScript == null)
        {
            var guid = AssetDatabase.FindAssets("t:Script GameDatabase").FirstOrDefault();
            if (guid != null)
            {
                targetMonoBehaviourScript = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid));
                FindDiscoverableTypes();
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Graph Tool Generator", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "This tool generates a complete, link-aware Unity GraphToolKit tool from a MonoBehaviour data container (like your GameDatabase class).",
            MessageType.Info);

        graphName = EditorGUILayout.TextField("Graph Name", graphName);
        targetMonoBehaviourScript = (MonoScript)EditorGUILayout.ObjectField("Target MonoBehaviour", targetMonoBehaviourScript, typeof(MonoScript), false);

        if (GUILayout.Button("Find Entity Types in Target"))
        {
            FindDiscoverableTypes();
        }

        EditorGUILayout.LabelField("Discovered Entity Types:", EditorStyles.boldLabel);
        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, EditorStyles.helpBox))
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
                EditorGUILayout.LabelField("No entity types found. Assign a target script and click 'Find'.");
            }
        }

        EditorGUI.BeginDisabledGroup(!discoveredTypes.Any() || targetMonoBehaviourScript == null);
        if (GUILayout.Button("Generate Full Graph Tool and Copy to Clipboard", GUILayout.Height(40)))
        {
            GenerateFullGraphToolCode();
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

    private void GenerateFullGraphToolCode()
    {
        var builder = new StringBuilder();
        string graphClassName = $"{graphName}Graph";
        string assetClassName = $"{graphName}Asset";
        string importerClassName = $"{graphName}Importer";
        string assetExtension = graphName.ToLowerInvariant().Replace(" ", "");

        // --- Header ---
        builder.AppendLine($"// ----- AUTO-GENERATED FILE BY {nameof(GraphToolGenerator)}.cs -----");
        builder.AppendLine($"// This file contains the complete GraphToolKit implementation for {graphName}.");
        builder.AppendLine($"// Generated on: {DateTime.Now}");
        builder.AppendLine();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine("using Unity.GraphToolkit.Editor;");
        builder.AppendLine("using UnityEditor;");
        builder.AppendLine("using UnityEditor.AssetImporters;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine();

        // --- Dummy Link class for port typing ---
        builder.AppendLine("// A dummy class to represent a linkable port in the graph.");
        builder.AppendLine("public class Linkable {}");
        builder.AppendLine();

        // --- Runtime Asset ---
        builder.AppendLine("#region Runtime Asset");
        builder.AppendLine();
        builder.AppendLine($"public class {assetClassName} : ScriptableObject");
        builder.AppendLine("{");
        foreach (var type in discoveredTypes)
        {
            // *** FIX: Special case for the 'Links' list ***
            string listName = type.Name == "PrimaryKey" ? "Links" : SimplePluralize(type.Name);
            builder.AppendLine($"    public List<{type.Name}> {listName} = new List<{type.Name}>();");
        }
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("#endregion");
        builder.AppendLine();

        // --- Graph Class ---
        builder.AppendLine("#region Graph Definition");
        builder.AppendLine();
        builder.AppendLine($"[Graph(\"{assetExtension}\")]");
        builder.AppendLine($"[Serializable]");
        builder.AppendLine($"public class {graphClassName} : Graph");
        builder.AppendLine("{");
        builder.AppendLine($"    [MenuItem(\"Assets/Create/{graphName} Graph\")]");
        builder.AppendLine("    private static void CreateAssetFile() => GraphDatabase.PromptInProjectBrowserToCreateNewAsset<" + graphClassName + ">(\"New " + graphName + "\");");
        builder.AppendLine();
        builder.AppendLine("    public static T GetPortValue<T>(IPort port) {");
        builder.AppendLine("        if (port.isConnected) {");
        builder.AppendLine("            var sourceNode = port.firstConnectedPort?.GetNode();");
        builder.AppendLine("            if (sourceNode is IConstantNode c) { c.TryGetValue(out T v); return v; }");
        builder.AppendLine("            if (sourceNode is IVariableNode vn) { vn.variable.TryGetDefaultValue(out T v); return v; }");
        builder.AppendLine("        } else { port.TryGetValue(out T v); return v; }");
        builder.AppendLine("        return default;");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("#endregion");
        builder.AppendLine();

        // --- Node Definition Classes ---
        builder.AppendLine("#region Node Definitions");
        builder.AppendLine();
        foreach (var type in discoveredTypes.Where(t => t.Name != "PrimaryKey"))
        {
            GenerateNodeClassForType(builder, type);
            builder.AppendLine();
        }
        builder.AppendLine("#endregion");
        builder.AppendLine();

        // --- Scripted Importer ---
        GenerateImporterCode(builder, importerClassName, graphClassName, assetClassName, assetExtension);

        // --- Finalization ---
        string finalCode = builder.ToString();
        EditorGUIUtility.systemCopyBuffer = finalCode;
        EditorUtility.DisplayDialog("Generation Complete", $"Successfully generated the full Graph Tool for '{graphName}' and copied it to the clipboard. Paste it into a new C# file.", "OK");
    }

    #region Generation Helpers

    private void GenerateNodeClassForType(StringBuilder builder, Type dataType)
    {
        string nodeClassName = $"{dataType.Name}DefinitionNode";
        var fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        builder.AppendLine($"[Serializable]");
        builder.AppendLine($"public class {nodeClassName} : Node");
        builder.AppendLine("{");
        builder.AppendLine("    public const string PortInputLink = \"InputLink\";");
        builder.AppendLine("    public const string PortOutputLink = \"OutputLink\";");
        foreach (var field in fields.Where(f => f.Name != "ID")) // Don't create a port for the ID field
        {
            builder.AppendLine($"    public const string Port{ToPascalCase(field.Name)} = \"{ToPascalCase(field.Name)}\";");
        }
        builder.AppendLine();
        builder.AppendLine("    protected override void OnDefinePorts(IPortDefinitionContext context)");
        builder.AppendLine("    {");
        builder.AppendLine("        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(\" \").Build();");
        builder.AppendLine("        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(\" \").Build();");
        builder.AppendLine();
        foreach (var field in fields.Where(f => f.Name != "ID"))
        {
            builder.AppendLine($"        context.AddInputPort<{GetCSharpTypeName(field.FieldType)}>(Port{ToPascalCase(field.Name)}).WithDisplayName(\"{SplitPascalCase(field.Name)}\").Build();");
        }
        builder.AppendLine("    }");
        builder.AppendLine("}");
    }

    private void GenerateImporterCode(StringBuilder builder, string importerName, string graphName, string assetName, string extension)
    {
        builder.AppendLine("#region Scripted Importer");
        builder.AppendLine();
        builder.AppendLine($"[ScriptedImporter(1, \"{extension}\")]");
        builder.AppendLine($"public class {importerName} : ScriptedImporter");
        builder.AppendLine("{");
        builder.AppendLine("    public override void OnImportAsset(AssetImportContext ctx)");
        builder.AppendLine("    {");
        builder.AppendLine($"        var graph = GraphDatabase.LoadGraphForImporter<{graphName}>(ctx.assetPath);");
        builder.AppendLine("        if (graph == null) return;");
        builder.AppendLine();
        builder.AppendLine($"        var runtimeAsset = ScriptableObject.CreateInstance<{assetName}>();");
        builder.AppendLine("        var nodeToIdMap = new Dictionary<INode, int>();");
        builder.AppendLine("        var typeToIdMap = new Dictionary<Type, ushort>();");
        builder.AppendLine("        int nextId = 1;");
        builder.AppendLine();
        builder.AppendLine("        // Pre-populate type IDs for linking");
        ushort typeIdCounter = 1;
        foreach (var type in discoveredTypes)
        {
            builder.AppendLine($"        typeToIdMap[typeof({type.Name})] = {typeIdCounter++};");
        }
        builder.AppendLine();
        builder.AppendLine("        // --- PASS 1: Create all data entries and map nodes to a new, unique ID ---");

        foreach (var type in discoveredTypes.Where(t => t.Name != "PrimaryKey"))
        {
            string nodeClassName = $"{type.Name}DefinitionNode";
            string pluralName = SimplePluralize(type.Name);
            string listVarName = char.ToLowerInvariant(pluralName[0]) + pluralName.Substring(1);

            builder.AppendLine($"        var {listVarName}Nodes = graph.GetNodes().OfType<{nodeClassName}>();");
            builder.AppendLine($"        foreach (var node in {listVarName}Nodes)");
            builder.AppendLine("        {");
            builder.AppendLine($"            var data = new {type.Name}();");
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => f.Name != "ID"))
            {
                string portConst = $"{nodeClassName}.Port{ToPascalCase(field.Name)}";
                builder.AppendLine($"            data.{field.Name} = {graphName}.GetPortValue<{GetCSharpTypeName(field.FieldType)}>(node.GetInputPortByName({portConst}));");
            }
            builder.AppendLine("            data.ID = nextId;");
            builder.AppendLine($"            runtimeAsset.{pluralName}.Add(data);");
            builder.AppendLine("            nodeToIdMap[node] = nextId;");
            builder.AppendLine("            nextId++;");
            builder.AppendLine("        }");
        }
        builder.AppendLine();
        builder.AppendLine("        // --- PASS 2: Create links by traversing graph connections ---");
        builder.AppendLine("        foreach (var sourceNode in graph.GetNodes())");
        builder.AppendLine("        {");
        builder.AppendLine("            var outputPort = sourceNode.GetOutputPorts().FirstOrDefault(p => p.name == \"OutputLink\");");
        builder.AppendLine("            if (outputPort == null || !outputPort.isConnected) continue;");
        builder.AppendLine();
        builder.AppendLine("            if (!nodeToIdMap.TryGetValue(sourceNode, out int sourceId)) continue;");
        builder.AppendLine();
        builder.AppendLine("            var sourceDataType = GetNodeType(sourceNode);");
        // *** FIX: Explicitly cast default value to ushort ***
        builder.AppendLine("            ushort sourceTypeId = typeToIdMap.GetValueOrDefault(sourceDataType, (ushort)0);");
        builder.AppendLine();
        builder.AppendLine("            var connectedPorts = new List<IPort>();");
        builder.AppendLine("            outputPort.GetConnectedPorts(connectedPorts);");
        builder.AppendLine("            foreach (var connectedPort in connectedPorts.Where(p => p.name == \"InputLink\"))");
        builder.AppendLine("            {");
        builder.AppendLine("                var targetNode = connectedPort.GetNode();");
        builder.AppendLine("                if (!nodeToIdMap.TryGetValue(targetNode, out int targetId)) continue;");
        builder.AppendLine();
        builder.AppendLine("                var targetDataType = GetNodeType(targetNode);");
        // *** FIX: Explicitly cast default value to ushort ***
        builder.AppendLine("                ushort targetTypeId = typeToIdMap.GetValueOrDefault(targetDataType, (ushort)0);");
        builder.AppendLine();
        builder.AppendLine("                var link = new PrimaryKey { SourceType = sourceTypeId, SourceID = sourceId, TargetType = targetTypeId, TargetID = targetId, LinkTypeID = 0 };");
        // *** FIX: Use the correct list name 'Links' ***
        builder.AppendLine("                runtimeAsset.Links.Add(link);");
        builder.AppendLine("            }");
        builder.AppendLine("        }");
        builder.AppendLine();
        builder.AppendLine($"        ctx.AddObjectToAsset(\"{graphName}\", runtimeAsset);");
        builder.AppendLine("        ctx.SetMainObject(runtimeAsset);");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    private Type GetNodeType(INode node)");
        builder.AppendLine("    {");
        foreach (var type in discoveredTypes.Where(t => t.Name != "PrimaryKey"))
        {
            builder.AppendLine($"        if (node is {type.Name}DefinitionNode) return typeof({type.Name});");
        }
        builder.AppendLine("        return null;");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("#endregion");
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

    #endregion
}