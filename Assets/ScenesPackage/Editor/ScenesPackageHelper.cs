using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Game.Tools.Scenes {

public class ScenesPackageHelper {
	
	[MenuItem("Scenes/Close all scenes")]
	static public void CloseAllScenes() {
		EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
	}
	
	static public void CloseAllScenesButActive() {
		Scene activeScene = EditorSceneManager.GetActiveScene();
		int currentScene  = EditorSceneManager.loadedSceneCount - 1;
		
		while(EditorSceneManager.loadedSceneCount > 1) {
			Scene scene = EditorSceneManager.GetSceneAt(currentScene);
			
			if(scene != activeScene) {
				EditorSceneManager.CloseScene(scene, true);
			}
			
			currentScene--;
		}
	}
	
	static public void SaveAllScenes(ScenesPackage package) {
		SceneField[] scenes = package.GetScenes(includeDependencies: true);
		
		for(int i = 0; i < scenes.Length; ++i) {
			Scene scene = EditorSceneManager.GetSceneByPath(scenes[i].ScenePath);
			EditorSceneManager.SaveScene(scene);
		}
	}
	
	static public void LoadScenesPackage(ScenesPackage package) {
		SceneField[] scenes = package.GetScenes(includeDependencies: true);
		bool activeSet      = false;
		
		for(int i = 0; i < scenes.Length; ++i) {
			if(scenes[i].Empty) {
				continue;
			}
			
			Scene opened = EditorSceneManager.OpenScene(scenes[i].ScenePath, OpenSceneMode.Additive);
			
			if(!activeSet) {
				EditorSceneManager.SetActiveScene(opened);
				CloseAllScenesButActive();
				
				activeSet = true;
			}
		}
	}
	
	static public void AddScenesPackageInBuild(ScenesPackage package) {
		List<string> scenesPath  = new List<string>(GetScenesBuildPath());
		SceneField[] scenesToAdd = package.GetScenes(includeDependencies: false);
		
		for(int i = 0; i < scenesToAdd.Length; ++i) {
			if(scenesToAdd[i].Empty || scenesPath.Contains(scenesToAdd[i].ScenePath)) {
				continue;
			}
			
			if(package._master && i == 0) {
				scenesPath.Insert(0, scenesToAdd[i].ScenePath);
			} else {
				scenesPath.Add(scenesToAdd[i].ScenePath);
			}
		}
		
		SetScenesBuildPath(scenesPath.ToArray());
		
		Debug.Log(string.Format("{0} : Les scenes ont été ajoutées à la build", package.name), package);
	}
	
	static public void RemoveScenesPackageFromBuild(ScenesPackage package) {
		List<string> scenesPath     = new List<string>(GetScenesBuildPath());
		SceneField[] scenesToRemove = package.GetScenes(includeDependencies: false);
		
		for(int i = 0; i < scenesToRemove.Length; ++i) {
			if(scenesToRemove[i].Empty || !scenesPath.Contains(scenesToRemove[i].ScenePath)) {
				continue;
			}
			
			scenesPath.Remove(scenesToRemove[i].ScenePath);
		}
		
		SetScenesBuildPath(scenesPath.ToArray());
		
		Debug.Log(string.Format("{0} : Les scenes ont été retirées à la build", package.name), package);
	}
	
	static public string[] GetScenesBuildPath() {
		string[] scenesPath = new string[EditorBuildSettings.scenes.Length];
		
		for(int i = 0; i < scenesPath.Length; ++i) {
			scenesPath[i] = EditorBuildSettings.scenes[i].path;
		}
		
		return scenesPath;
	}
	
	static public void SetScenesBuildPath(string[] path) {
		List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();
		
		for(int i = 0; i < path.Length; ++i) {
			buildScenes.Add(new EditorBuildSettingsScene(path[i], true));
		}
		
		EditorBuildSettings.scenes = buildScenes.ToArray();
	}
	
	static public ScenesPackage CreatePackage(string folder, string packageName, string[] scenesToCreate) {
		if(scenesToCreate == null || scenesToCreate.Length == 0) {
			Debug.LogWarning("ScenesPackage Creator : Indiquez au moins une scène à créer");
			return null;
		} else if(!AssetDatabase.IsValidFolder(folder)) {
			Debug.LogWarning("ScenesPackage Creator : Le dossier spécifié n'est pas valide");
			return null;
		}
		
		ScenesPackage package = ScriptableObject.CreateInstance<ScenesPackage>();
		string assetFolder    = AssetDatabase.GenerateUniqueAssetPath(folder + "/" + packageName);
		string assetPath;
		
		// Create the base folder and the scene package
		AssetDatabase.CreateFolder(folder, packageName);
		assetPath = AssetDatabase.GenerateUniqueAssetPath(assetFolder + "/" + packageName);
		AssetDatabase.CreateAsset(package, assetPath + ".asset");
		
		// Create the scenes
		SerializedObject packageObject = new SerializedObject(package);
		SerializedProperty scenesProp  = packageObject.FindProperty("_scenes");
		int scenesCreated = 0;
		
		packageObject.Update();
		
		for(int i = 0; i < scenesToCreate.Length; ++i) {
			if(scenesToCreate[i] == "") {
				continue;
			}
			
			// New scene
			Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			EditorSceneManager.SaveScene(newScene, assetPath + "_" + scenesToCreate[i] + ".unity");
			
			// Get the asset of the scene and add it on the package
			SceneAsset newSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(newScene.path);
			
			scenesProp.arraySize++;
			scenesProp.GetArrayElementAtIndex(i).FindPropertyRelative("sceneAsset").objectReferenceValue = newSceneAsset;
			
			scenesCreated++;
		}
		
		packageObject.ApplyModifiedProperties();
		
		ScenesPackageHelper.LoadScenesPackage(packageObject.targetObject as ScenesPackage);
		
		Debug.Log(string.Format("ScenesPackage Creator : \"{0}\" créé, incluant {1} scènes", packageName, scenesCreated));
		
		return packageObject.targetObject as ScenesPackage;
	}
	
	static public void DeletePackage(ScenesPackage package) {
		SceneField[] scenes = package.GetScenes(includeDependencies: false);
		
		for(int i = 0; i < scenes.Length; ++i) {
			AssetDatabase.MoveAssetToTrash(scenes[i].ScenePath);
		}
		
		AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(package));
	}
	
}

}