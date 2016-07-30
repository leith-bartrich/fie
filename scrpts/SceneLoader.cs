using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace fie{

	/// <summary>
	/// Scene loaders are spawned and synced accross the network to handle loading of scenes.  They are loaded by calling Load methods on SceneType objects.  They are spawned from prfabs.
	/// </summary>
	public class SceneLoader : NetworkBehaviour {

		/// <summary>
		/// Name of the scene to laod.
		/// </summary>
		[SyncVar]
		public string sceneName;

		void Awake(){
			GameObject.DontDestroyOnLoad (this.gameObject);
		}

		public override void OnStartClient ()
		{
			base.OnStartClient ();
			//if we're a host, then this is handled in OnStartServer for us already.
			if (!isServer) {
				StartCoroutine (Load ());
			}
		}

		public override void OnStartServer ()
		{
			base.OnStartServer ();
			StartCoroutine (Load ());
		}

		public override void OnNetworkDestroy ()
		{
			base.OnNetworkDestroy ();
			Unload ();
		}

		void OnDestroy(){
			Unload ();
		}

		private IEnumerator Load (){
			var scene = SceneManager.GetSceneByName (sceneName);
			if (!scene.isLoaded){
				var a = SceneManager.LoadSceneAsync (sceneName,LoadSceneMode.Additive);
				while (!a.isDone) {
					yield return null;
				}
				scene = SceneManager.GetSceneByName (sceneName);
			}
			var configurator = (from o in scene.GetRootGameObjects ()
			 where o.GetComponent<SceneConfigurator> () != null
			 select o.GetComponent<SceneConfigurator> ()).FirstOrDefault ();
			if (configurator != null) {
				configurator.loader = this;
				yield return StartCoroutine( configurator.Configure ());
			} else {
				Debug.LogWarning("Scene Loaded.  But no SceneConfigurator found at the root level: " + sceneName);
			}
		}

		private void Unload (){
			var scene = SceneManager.GetSceneByName (sceneName);
			if (scene.isLoaded) {
				var configurator = (from o in scene.GetRootGameObjects ()
				                    where o.GetComponent<SceneConfigurator> () != null
				                    select o.GetComponent<SceneConfigurator> ()).FirstOrDefault ();
				if (configurator != null) {
					configurator.Unload ();
				}
				SceneManager.UnloadScene (sceneName);
			}
		}

	}

}