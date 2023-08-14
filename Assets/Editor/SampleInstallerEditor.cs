// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SampleInstallerEditor : using UnityEngine;
// using UnityEditor;

// public class SampleInstallerEditor : EditorWindow {

//     [MenuItem("allart-sui-wallet/SampleInstaller")]
//     private static void ShowWindow() {
//         var window = GetWindow<SampleInstallerEditor>();
//         window.titleContent = new GUIContent("SampleInstaller");
//         window.Show();
//     }

//     private void OnGUI() {
//         GUILayout.Label("Click the button below to install the package.", EditorStyles.wordWrappedLabel);

//         if (GUILayout.Button("Install"))
//         {
//             string packagePath = "Assets/MyNpmPackage/MyPackage.unitypackage"; // Update this with your actual package path
//             AssetDatabase.ImportPackage(packagePath, true);
//         }
//     }
// }
