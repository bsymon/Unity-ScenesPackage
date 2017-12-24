using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tools.Scenes {

[CreateAssetMenu(menuName="ScenesPackage/Explorer Filter", order=20)]
public class ScenesPackageExplorerFilter : ScriptableObject {
	
	[System.Serializable]
	public struct Filter {
		public string prefix;
		public string name;
	}
	
	// -- //
	
	public Filter[] filters;
	
}

}