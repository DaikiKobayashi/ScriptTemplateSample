using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

internal class ScriptTemplates
{
    private static readonly string _templatPpath = "Assets/ScriptTemplate/Scripts/Editor/Templates";

    [MenuItem("Assets/Create/Script/new ScriptableObject", priority = 20)]
    public static void CreateActionScript() =>
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<DoCreateScriptAsset>(),
            "New ScriptableObject.cs",
            (Texture2D)EditorGUIUtility.IconContent("cs Script Icon").image,
            $"{_templatPpath}/ScriptableObjectTemplate.txt");



    private class DoCreateScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string text = File.ReadAllText(resourceFile);

            string fileName = Path.GetFileName(pathName);
            {
                string newName = fileName.Replace(" ", "");
                pathName = pathName.Replace(fileName, newName);
                fileName = newName;
            }

            string fileNameWithoutExtension = fileName.Substring(0, fileName.Length - 3);
            text = text.Replace("#SCRIPTNAME#", fileNameWithoutExtension);

            string runtimeName = fileNameWithoutExtension;

            for (int i = runtimeName.Length - 1; i > 0; i--)
                if (char.IsUpper(runtimeName[i]) && char.IsLower(runtimeName[i - 1]))
                    runtimeName = runtimeName.Insert(i, " ");

            text = text.Replace("#RUNTIMENAME#", runtimeName);
            text = text.Replace("#RUNTIMENAME_WITH_SPACES#", runtimeName);

            string fullPath = Path.GetFullPath(pathName);
            var encoding = new UTF8Encoding(true);
            File.WriteAllText(fullPath, text, encoding);
            AssetDatabase.ImportAsset(pathName);
            ProjectWindowUtil.ShowCreatedAsset(AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object)));
        }
    }
}