using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Tools.Scenes {

[System.Serializable]
public class SceneField {
	
	public string SceneName {
		get {
			#if UNITY_EDITOR
			CheckAssetNameChange();
			#endif
			
			return sceneName;
		}
	}
	
	public string ScenePath {
		get {
			#if UNITY_EDITOR
			CheckAssetNameChange();
			#endif
			
			return scenePath;
		}
	}
	
	public bool Empty {
		get { return sceneAsset == null; }
	}
	
	// -- //
	
	[SerializeField]
	private Object sceneAsset;
	
	[SerializeField]
	private string sceneName;
	
	[SerializeField]
	private string scenePath;
	
	// -- //
	
	public SceneField(Object sceneAsset) {
		this.sceneAsset = sceneAsset;
	}
	
	// -- //
	
	static public implicit operator string(SceneField sceneField) {
		return sceneField.SceneName;
	}
	
#if UNITY_EDITOR
	static public implicit operator SceneAsset(SceneField sceneField) {
		return sceneField.sceneAsset as SceneAsset;
	}
	
	void CheckAssetNameChange() {
		if(sceneAsset == null) {
			return;
		}
		
		SceneAsset asset = sceneAsset as SceneAsset;
		string path      = AssetDatabase.GetAssetPath(asset);
		
		if(asset.name != sceneName) {
			sceneName = asset.name;
		}
		
		if(path != scenePath) {
			scenePath = path;
		}
	}
#endif
	
}

}