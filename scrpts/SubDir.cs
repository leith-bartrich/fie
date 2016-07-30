using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fie{

	/// <summary>
	/// Marks a GameObject as being a representation of a SubDirectory when in the scene view.  Useful for many things.  But ultimately, this is meta-data.
	/// </summary>
	public class SubDir : MonoBehaviour {

		/// <summary>
		/// The name of the director.  As in:  "c:\my\dir" would be named "dir".
		/// </summary>
		public string dirName;

	}
	
}