using UnityEditor;

namespace Game.Tools.Scenes {

public class ScenesPackageModificationProcessor : UnityEditor.AssetModificationProcessor {
	
	static public AssetDeleteResult OnWillDeleteAsset(string asset, RemoveAssetOptions options) {
		ScenesPackage scenePackage = AssetDatabase.LoadAssetAtPath<ScenesPackage>(asset);
		
		if(scenePackage != null) {
			// Delete the scene from the build settings
			ScenesPackageHelper.RemoveScenesPackageFromBuild(scenePackage);
		}
		
		return AssetDeleteResult.DidNotDelete;
	}
	
}

}