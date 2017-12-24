using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Tools.Scenes {

[CreateAssetMenu(menuName="ScenesPackage/Package", order=0)]
public class ScenesPackage : ScriptableObject {
	
	public int TotalCount {
		get {
			int scenesCount = Count;
			
			if(_dependencies != null) {
				for(int i =0; i < _dependencies.Length; ++i) {
					if(_dependencies[i] == this) {
						continue;
					}
					
					scenesCount += _dependencies[i] != null ? _dependencies[i].TotalCount : 0;
				}
			}
			
			return scenesCount;
		}
	}
	
	public int Count {
		get {
			return _scenes != null ? _scenes.Length : 0;
		}
	}
	
	// -- //
	
	public bool _master;
	
	[SerializeField] ScenesPackage[] _dependencies;
	[SerializeField] SceneField[] _scenes;
	
	// -- //
	
	public SceneField[] GetScenes(bool includeDependencies) {
		SceneField[] scenes = new SceneField[includeDependencies ? TotalCount : Count];
		int currentScene    = 0;
		
		if(includeDependencies && _dependencies != null) {
			// Load dependencies first
			for(int i = 0; i < _dependencies.Length; ++i) {
				if(_dependencies[i] == this) {
					continue;
				}
				
				if(_dependencies[i] != null) {
					SceneField[] subDependencies = _dependencies[i].GetScenes(true);
					
					for(int j = 0; j < subDependencies.Length; ++j) {
						scenes[currentScene] = subDependencies[j];
						currentScene++;
					}
				}
			}
		}
		
		// Then the scenes of the package
		for(int i = 0; i < _scenes.Length; ++i) {
			scenes[currentScene] = _scenes[i];
			currentScene++;
		}
		
		return scenes;
	}
	
}

}