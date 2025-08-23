using UnityEditor;

namespace Mtl.Toolbox
{
    public static class AssemblyUtility
    {
        [MenuItem("Tools/Reload Assembly %#y")]
        private static void ReloadAssemblies()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
    }
}
