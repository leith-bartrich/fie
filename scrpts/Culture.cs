using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fie{

	/// <summary>
	/// A named reference to a culture file in a Localization.
	/// </summary>
	[System.Serializable]
	public class Culture{
		public string name;
		public TextAsset data;
	}
	
}