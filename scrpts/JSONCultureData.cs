using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fie{

	/// <summary>
	/// JSON Syntax for a culture file.
	/// </summary>
	[System.Serializable]
	public class JSONCultureData{
		public JSONCultureStringEntry[] Strings;
	}
	
}