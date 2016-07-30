using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fie{

	/// <summary>
	/// JSON Syntax for a localized string in a culture.
	/// </summary>
	[System.Serializable]
	public class JSONCultureStringEntry{
		public string key;
		public string value;
	}
	
}