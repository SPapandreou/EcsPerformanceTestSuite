using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Core.Editor.Build
{
    public class AppConfigManager : ScriptableObject, IPostprocessBuildWithReport
    {
        public TextAsset appConfig;
        
        public int callbackOrder => 0;
        
        public void OnPostprocessBuild(BuildReport report)
        {
            var manager = CreateInstance<AppConfigManager>();
            var sourcePath = AssetDatabase.GetAssetPath(manager.appConfig);
            var targetPath = Path.Combine(Path.GetDirectoryName(report.summary.outputPath), Path.GetFileName(sourcePath));
            
            File.Copy(sourcePath, targetPath, overwrite: true);
        }
    }
}