using UnityEngine;

namespace Game.Tools.Scenes {

[CreateAssetMenu(menuName="ScenesPackage/Creator Template", order=10)]
public class ScenesPackageCreatorTemplate : ScriptableObject {
	
	public string folder;
	public string packageName;
	public string[] scenesToCreate;
	
}

}