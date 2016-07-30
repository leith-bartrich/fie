using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fie{


	/// <summary>
	/// A localization lives in a scene and allows one to lookup localized values via various methods.  Localization libraries contain a list of available Cultures and a default Culture.
	/// </summary>
	[AddComponentMenu("fie/Localization/Localization")]
	public class Localization : MonoBehaviour {

		/// <summary>
		/// List of available cultures.
		/// </summary>
		public List<Culture> Cultures;

		/// <summary>
		/// The default culture.
		/// </summary>
		public TextAsset DefaultCulture;

		/// <summary>
		/// currently chosen data.
		/// </summary>
		private JSONCultureData data;

		void Awake(){
			LoadCulture ();
		}

		void Start(){
			LoadCulture ();
		}

		/// <summary>
		/// Attempts to get a localized string.
		/// </summary>
		/// <returns><c>true</c>, if string was found and gotten, <c>false</c> otherwise.</returns>
		/// <param name="name">The name or key of the string to find.</param>
		/// <param name="found">The found string, or an mangled version of the key, if not found.</param>
		public bool getString (string name, out string found){
			if (data != null) {
				found = (from s in data.Strings
				         where s.key == name
				         select s.value).FirstOrDefault ();
				return !string.IsNullOrEmpty (found);
			} else {
				found = "[" + name + "]";
				return false;
			}
		}

		private void LoadCulture(){
			TextAsset c = (from cul in Cultures
			               where cul.name == Localization.Culture
			               select cul.data).FirstOrDefault ();
			if (c == null){
				c = DefaultCulture;
			}
			if (c != null) {
				data = JsonUtility.FromJson<JSONCultureData> (c.text);
			}
		}

		/// <summary>
		/// The current Culture name to use to localize.  Change this before using any Localization objects, to change the localization.
		/// </summary>
		public static string Culture = "USA";

		/// <summary>
		/// Lookup and localize the specified key in all currently loaded Localizations.  Returns a mangled verison of the key if it cannot be found.
		/// </summary>
		/// <param name="key">Key.</param>
		public static string lookup(string key){
			var localizations = GameObject.FindObjectsOfType<Localization> ();
			foreach (var l in localizations) {
				string ret;
				if (l.getString(key,out ret)){
					return ret;
				}
			}
			return "[" + key + "]";
		}

	}


	
}