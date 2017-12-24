using UnityEngine;
using UnityEditor;

namespace Game.Tools.Scenes {

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldDrawer : PropertyDrawer {
	
	override public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
		SerializedProperty sceneName  = property.FindPropertyRelative("sceneName");
		SerializedProperty scenePath  = property.FindPropertyRelative("scenePath");
		
		EditorGUI.BeginChangeCheck();
		
		sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, label, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
		
		if(EditorGUI.EndChangeCheck()) {
			if(sceneAsset.objectReferenceValue != null) {
				sceneName.stringValue = ((SceneAsset) sceneAsset.objectReferenceValue).name;
				scenePath.stringValue = AssetDatabase.GetAssetPath(((SceneAsset) sceneAsset.objectReferenceValue));
			} else {
				sceneName.stringValue = "";
				scenePath.stringValue = "";
			}
		}
	}
	
}

}