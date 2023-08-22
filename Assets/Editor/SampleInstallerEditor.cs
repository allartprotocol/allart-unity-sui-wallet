using System.IO;
using UnityEditor;
using UnityEngine;

public class HardcodedPackageManagerImporter : EditorWindow
{
    // Hardcoded source path of the package you want to copy from
    private readonly string sourcePath = "com.example.package";
    private string destinationPath = "Assets/"; // Default destination path

    [MenuItem("Tools/Copy from Hardcoded Package to Assets")]
    public static void ShowWindow()
    {
        GetWindow<HardcodedPackageManagerImporter>("Copy from Hardcoded Package");
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy content from Hardcoded Package to Assets", EditorStyles.boldLabel);
        GUILayout.Label($"Copying from: {sourcePath}");

        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Copy"))
        {
            CopyContents();
        }
    }

    void CopyContents()
    {
        // Construct full source path
        string fullSourcePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Packages", sourcePath);

        // Check if source exists
        if (!Directory.Exists(fullSourcePath))
        {
            Debug.LogError("Source path does not exist!");
            return;
        }

        string fullDestinationPath = Path.Combine(Application.dataPath, destinationPath);

        // Copying directory
        DirectoryCopy(fullSourcePath, fullDestinationPath, true);
        AssetDatabase.Refresh();
    }

    // Function to copy directories
    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }
}
