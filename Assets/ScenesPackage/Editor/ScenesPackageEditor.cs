using UnityEngine;
using UnityEditor;

namespace Game.Tools.Scenes {

[CustomEditor(typeof(ScenesPackage))]
public class ScenesPackageEditor : Editor {
	
	public static ScenesPackage lastPackage;
	
	// -- //
	
	SerializedProperty scenesProp;
	SerializedProperty dependenciesProp;
	SerializedProperty masterProp;
	
	bool masterFoldout;
	bool dangerZoneFoldout;
	bool saveScenes;
	
	// -- //
	
	void OnEnable() {
		if(target != null) {
			scenesProp       = serializedObject.FindProperty("_scenes");
			dependenciesProp = serializedObject.FindProperty("_dependencies");
			masterProp       = serializedObject.FindProperty("_master");
			lastPackage      = target as ScenesPackage;
		}
	}
	
	override public void OnInspectorGUI() {
		serializedObject.Update();
		
		ScenesPackageGUIUtils.ArrayGUI(dependenciesProp, "Dépendances", "Package de scènes qui sera chargé avant celles de celui-ci.");
		ScenesPackageGUIUtils.ArrayGUI(scenesProp, "Scènes", "Scènes à charger, dans l'ordre.");
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button("Charger le package", GUILayout.Height(30))) {
			ScenesPackageHelper.LoadScenesPackage(serializedObject.targetObject as ScenesPackage);
		}
		
		if(GUILayout.Button("Sauvegarder toutes les scènes")) {
			ScenesPackageHelper.SaveAllScenes(serializedObject.targetObject as ScenesPackage);
		}
			
		EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Sauvegarder et fermer")) {
				if(EditorUtility.DisplayDialog("Fermer", "Fermer le package ? Seules les scènes du package seront sauvegardées.", "Oui", "Non")) {
					saveScenes = true;
					EditorApplication.delayCall += CloseDelayed;
				}
			}
			
			if(GUILayout.Button("Fermer")) {
				if(EditorUtility.DisplayDialog("Fermer", "Fermer le package ? Toutes les modifications non sauvegardées seront perdues.", "Oui", "Non")) {
					EditorApplication.delayCall += CloseDelayed;
				}
			}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button("Ajouter les scenes dans la build")) {
			ScenesPackageHelper.AddScenesPackageInBuild(serializedObject.targetObject as ScenesPackage);
		}
		
		if(GUILayout.Button("Retirer les scenes de la build")) {
			ScenesPackageHelper.RemoveScenesPackageFromBuild(serializedObject.targetObject as ScenesPackage);
		}
		
		if(GUILayout.Button("Ouvrir les Build Settings")) {
			EditorApplication.ExecuteMenuItem("File/Build Settings...");
		}
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginVertical("Box");
			EditorGUI.indentLevel ++;
			masterFoldout = EditorGUILayout.Foldout(masterFoldout, "Master package");
			EditorGUI.indentLevel --;
			
			if(masterFoldout) {
				EditorGUILayout.HelpBox("Master Package : La 1ère scène de ce package sera la 1ère scène chargée en jeu.", MessageType.Info);
				
				if(GUILayout.Button(masterProp.boolValue ? "Désactiver master" : "Activer master")) {
					saveScenes = false;
					masterProp.boolValue = !masterProp.boolValue;
				}
			}
		EditorGUILayout.EndVertical();
		
		serializedObject.ApplyModifiedProperties();
		
		GUI.backgroundColor = Color.red;
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.BeginVertical("Box");
			EditorGUI.indentLevel++;
			
			dangerZoneFoldout = EditorGUILayout.Foldout(dangerZoneFoldout, "Danger zone");
			
			if(dangerZoneFoldout) {
				EditorGUILayout.HelpBox("Faites attentions à ce que vous faites !", MessageType.Warning);
				
				if(GUILayout.Button("Supprimer le package")) {
					if(EditorUtility.DisplayDialog("Supprimer", "Êtes-vous sûr de vouloir supprimer le package. Toutes les scènes seront supprimées.", "Oui", "Non")) {
						ScenesPackageHelper.DeletePackage(serializedObject.targetObject as ScenesPackage);
						EditorApplication.delayCall += CloseDelayed;
					}
				}
			}
			
			EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical();
		GUI.backgroundColor = Color.white;
	}
	
	void CloseDelayed() {
		if(saveScenes) {
			ScenesPackageHelper.SaveAllScenes(serializedObject.targetObject as ScenesPackage);
		}
		
		ScenesPackageHelper.CloseAllScenes();
		Selection.activeObject = target;
	}
	
}

}