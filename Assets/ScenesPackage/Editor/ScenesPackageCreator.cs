using UnityEngine;
using UnityEditor;

namespace Game.Tools.Scenes {

public class ScenesPackageCreator : EditorWindow {
	
	[SerializeField] ScenesPackageCreatorTemplate template;
	[SerializeField] string folder;
	[SerializeField] string packageName;
	[SerializeField] string[] scenesToCreate;
	
	SerializedObject windowObject;
	SerializedProperty templateProp;
	SerializedProperty folderProp;
	SerializedProperty packageNameProp;
	SerializedProperty scenesToCreateProp;
	
	GUIContent folderLabel;
	GUIContent packageNameLabel;
	
	Vector2 scroll;
	
	// -- //
	
	[MenuItem("Scenes/ScenesPackage Creator")]
	static void InitWindow() {
		ScenesPackageCreator window = EditorWindow.GetWindow(typeof(ScenesPackageCreator)) as ScenesPackageCreator;
		window.titleContent = new GUIContent("ScenesPackage Creator");
		
		window.Show();
	}
	
	// -- //
	
	void OnEnable() {
		InitProperties();
		InitGUIStyle();
	}
	
	void OnGUI() {
		EditorGUILayout.LabelField("ScenesPackage Creator", EditorStyles.largeLabel);
		EditorGUILayout.Separator();
		
		windowObject.Update();
		
		scroll = EditorGUILayout.BeginScrollView(scroll);
		
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(templateProp);
		
		if(EditorGUI.EndChangeCheck()) {
			SetTemplate();
		} 
		
		EditorGUILayout.PropertyField(folderProp, folderLabel);
		EditorGUILayout.PropertyField(packageNameProp, packageNameLabel);
		
		EditorGUILayout.Separator();
		
		ScenesPackageGUIUtils.ArrayGUI(scenesToCreateProp, "Scenes à créer", "Indiquez le nom des scènes à créer. Elles seront préfixées avec le nom du package.");
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button("Créer le package", GUILayout.Height(30))) {
			ScenesPackage newPackage = ScenesPackageHelper.CreatePackage(folder, packageName, scenesToCreate);
			
			if(newPackage != null) {
				Selection.activeObject = newPackage;
			}
		}
		
		EditorGUILayout.EndScrollView();
		
		windowObject.ApplyModifiedProperties();
	}
	
	void SetTemplate() {
		ClearAll();
		
		if(templateProp.objectReferenceValue != null) {
			ScenesPackageCreatorTemplate newTemplate = templateProp.objectReferenceValue as ScenesPackageCreatorTemplate;
			
			folderProp.stringValue      = newTemplate.folder;
			packageNameProp.stringValue = newTemplate.packageName;
			
			scenesToCreateProp.ClearArray();
			scenesToCreateProp.arraySize = newTemplate.scenesToCreate.Length;
			
			for(int i = 0; i < scenesToCreateProp.arraySize; ++i) {
				scenesToCreateProp.GetArrayElementAtIndex(i).stringValue = newTemplate.scenesToCreate[i];
			}
		}
	}
	
	void ClearAll() {
		folderProp.stringValue      = "";
		packageNameProp.stringValue = "";
		
		scenesToCreateProp.ClearArray();
	}
	
	void InitProperties() {
		windowObject = new SerializedObject(this);
		templateProp       = windowObject.FindProperty("template");
		folderProp         = windowObject.FindProperty("folder");
		packageNameProp    = windowObject.FindProperty("packageName");
		scenesToCreateProp = windowObject.FindProperty("scenesToCreate");
	}
	
	void InitGUIStyle() {
		folderLabel      = new GUIContent("Dossier");
		packageNameLabel = new GUIContent("Nom du package");
	}
	
}

}