using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Game.Tools.Scenes {

public class ScenesPackageExplorer : EditorWindow {
	
	const string NO_CATEGORY = "@None@";
	
	// -- //
	
	[SerializeField] ScenesPackageExplorerFilter filters;
	
	SerializedObject windowObject;
	SerializedProperty filtersProp;
	
	Dictionary<string, string> categories = new Dictionary<string, string>();
	Dictionary<string, List<ScenesPackage>> packages = new Dictionary<string, List<ScenesPackage>>();
	
	Color backgroundColor1;
	Color backgroundColor2;
	bool[] foldouts;
	Vector2 scroll;
	
	// -- //
	
	[MenuItem("Scenes/ScenesPackage Explorer")]
	static void InitWindow() {
		ScenesPackageExplorer window = EditorWindow.GetWindow(typeof(ScenesPackageExplorer)) as ScenesPackageExplorer;
		window.titleContent = new GUIContent("ScenesPackage Explorer");
		
		window.Show();
	}
	
	// -- //
	
	void OnEnable() {
		InitBackgroundColors();
		InitProperties();
		InitCategories();
		FindAllAssets();
	}
	
	void OnInspectorUpdate() {
		Repaint();
	}
	
	void OnGUI() {
		EditorGUILayout.LabelField("ScenesPackage Explorer", EditorStyles.largeLabel);
		EditorGUILayout.Separator();
		
		if(ScenesPackageEditor.lastPackage != null) {
			if(GUILayout.Button(string.Format("Aller à \"{0}\"", ScenesPackageEditor.lastPackage.name), GUILayout.Height(30))) {
				Selection.activeObject = ScenesPackageEditor.lastPackage;
			}
		}
		
		windowObject.Update();
		scroll = EditorGUILayout.BeginScrollView(scroll);
		
		// Filters
		EditorGUILayout.LabelField("Filtres", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(filtersProp, ScenesPackageGUIUtils.noLabel);
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button("Rafraichir")) {
			Refresh();
		}
		
		EditorGUILayout.Separator();
		
		// Scenes
		if(packages.Count == 0) {
			EditorGUILayout.LabelField("Aucun package", EditorStyles.boldLabel);
		}
		
		int currentFoldout = 0;
		try {
			foreach(string currentCategory in packages.Keys) {
				GUI.backgroundColor = backgroundColor1;
				
				CategoryGUI(currentCategory, currentFoldout);
				
				currentFoldout++;
			}
		} catch(System.InvalidOperationException) {
			Refresh();
			Repaint();
			return;
		}
		
		EditorGUILayout.EndScrollView();
		windowObject.ApplyModifiedProperties();
	}
	
	void CategoryGUI(string category, int fold) {
		int i = 0;
		
		try {
			EditorGUILayout.BeginVertical("Box");
				foldouts[fold] = EditorGUILayout.Foldout(foldouts[fold], categories[category]);
				
				if(foldouts[fold]) {
					EditorGUI.indentLevel++;
					foreach(ScenesPackage currentPackage in packages[category]) {
						AssetGUI(currentPackage, i % 2 == 0 ? backgroundColor1 : backgroundColor2);
						i++;
					}
					EditorGUI.indentLevel--;
				}
			EditorGUILayout.EndVertical();
		} catch(System.ArgumentException) {
			Refresh();
			Repaint();
			return;
		}
	}
	
	void AssetGUI(ScenesPackage asset, Color backgroundColor) {
		GUI.backgroundColor = backgroundColor;
		
		try {
			EditorGUILayout.BeginHorizontal("Box");
				EditorGUILayout.LabelField(asset.name, EditorStyles.boldLabel);
				
				if(GUILayout.Button("Sélectionner")) {
					Selection.activeObject = asset;
				}
			EditorGUILayout.EndHorizontal();
		} catch(MissingReferenceException) {
			Refresh();
			Repaint();
			return;
		}
	}
	
	void InitBackgroundColors() {
		backgroundColor1 = GUI.backgroundColor;
		backgroundColor2 = new Color(0.9f, 0.9f, 0.9f, 1f);
	}
	
	void InitProperties() {
		windowObject = new SerializedObject(this);
		filtersProp  = windowObject.FindProperty("filters");
	}
	
	void InitCategories() {
		categories.Clear();
		
		if(filters != null) {
			for(int i = 0; i < filters.filters.Length; ++i) {
				categories.Add(filters.filters[i].prefix, filters.filters[i].name);
			}
		}
		
		categories.Add(NO_CATEGORY, "Non trié");
		
		foldouts = new bool[4];
	}
	
	void FindAllAssets() {
		ClearAssets();
		
		string[] assetsGUID = AssetDatabase.FindAssets("t:" + typeof(ScenesPackage).Name);
		string[] assetsPath = new string[assetsGUID.Length];
		
		// Sort by name
		for(int i = 0; i < assetsGUID.Length; ++i) {
			assetsPath[i] = AssetDatabase.GUIDToAssetPath(assetsGUID[i]);
		}
		
		System.Array.Sort(assetsPath, (a, b) => string.Compare(a, b));
		
		// Sort in categories
		for(int i = 0; i < assetsPath.Length; ++i) {
			ScenesPackage package = AssetDatabase.LoadAssetAtPath<ScenesPackage>(assetsPath[i]);
			string sceneName      = System.IO.Path.GetFileNameWithoutExtension(assetsPath[i]);
			bool inCategory       = false;
			
			foreach(string category in categories.Keys) {
				if(sceneName.StartsWith(category)) {
					AddInCategory(category, package);
					inCategory = true;
					break;
				}
			}
			
			if(!inCategory) {
				AddInCategory(NO_CATEGORY, package);
			}
		}
	}
	
	void ClearAssets() {
		foreach(List<ScenesPackage> listPackage in packages.Values) {
			listPackage.Clear();
		}
		
		packages.Clear();
	}
	
	void AddInCategory(string category, ScenesPackage package) {
		List<ScenesPackage> packInCat;
		
		if(!packages.TryGetValue(category, out packInCat)) {
			packInCat = new List<ScenesPackage>();
			packages.Add(category, packInCat);
		}
		
		packInCat.Add(package);
	}
	
	void Refresh() {
		InitCategories();
		FindAllAssets();
	}
	
}

}