using UnityEngine;
using UnityEditor;

namespace Game.Tools.Scenes {

public class ScenesPackageGUIUtils {
	
	static public GUIContent noLabel = new GUIContent("");
	
	// -- //
	
	static bool init;
	static GUIStyle arrayTitleLabel;
	
	// -- //
	
	static void InitGUIStyle() {
		if(init) {
			return;
		}
		
		arrayTitleLabel = new GUIStyle();
		arrayTitleLabel.fontStyle = FontStyle.Bold;
		arrayTitleLabel.fontSize  = 15;
		arrayTitleLabel.alignment = TextAnchor.MiddleCenter;
		
		init = true;
	}
	
	public static void ArrayGUI(SerializedProperty arrayProp, string label, string helpMessage) {
		InitGUIStyle();
		
		bool addElement    = false;
		int moveBefore     = -1;
		int moveAfter      = -1;
		int addAfter       = -1;
		int toDelete       = -1;
		
		EditorGUILayout.BeginVertical("Box");
		
			EditorGUILayout.LabelField(label, arrayTitleLabel);
			EditorGUILayout.Separator();
			EditorGUILayout.HelpBox(helpMessage, MessageType.None);
			EditorGUILayout.Separator();
			
			addElement = GUILayout.Button("Ajouter un élément");
			
			EditorGUILayout.Separator();
			
			EditorGUI.indentLevel++;
				if(arrayProp.arraySize == 0) {
					EditorGUILayout.LabelField("Aucun élément", EditorStyles.boldLabel);
				}
				
				for(int i = 0; i < arrayProp.arraySize; ++i) {
					EditorGUILayout.BeginHorizontal();
					
						moveBefore = GUILayout.Button("▲") ? i : moveBefore;
						moveAfter  = GUILayout.Button("▼") ? i : moveAfter;
						
						EditorGUILayout.PropertyField(arrayProp.GetArrayElementAtIndex(i), noLabel);
						
						addAfter = GUILayout.Button("+") ? i : addAfter;
						toDelete = GUILayout.Button("X") ? i : toDelete;
					
					EditorGUILayout.EndHorizontal();
				}
			EditorGUI.indentLevel--;
		
		EditorGUILayout.Separator();
		EditorGUILayout.EndVertical();
		
		// Add element
		if(addElement) {
			arrayProp.arraySize++;
		}
		
		// Move Before
		if(moveBefore > -1 && moveBefore - 1 > -1) {
			arrayProp.MoveArrayElement(moveBefore, moveBefore - 1);
		}
		
		// Move After
		if(moveAfter > -1 && moveAfter + 1 < arrayProp.arraySize) {
			arrayProp.MoveArrayElement(moveAfter, moveAfter + 1);
		}
		
		// Add after an element
		if(addAfter > -1) {
			arrayProp.InsertArrayElementAtIndex(addAfter);
		}
		
		// Delete an element
		if(toDelete > -1) {
			arrayProp.DeleteArrayElementAtIndex(toDelete);
		}
	}
	
}

}