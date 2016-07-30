using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace fie{


	public abstract class SceneType : SpawnableType {
		

		/// <summary>
		/// Load the given scene.  If know exactly what type of scene you are loading and want to configure it beyond what's in the prefab, use the generic form of this method instead if possible.
		/// </summary>
		/// <param name="sceneName">Name of the scene to load.</param>
		public SceneLoader Load(string sceneName){
			//race condition for double loading handled here.
			//check if some other scene loader has loaded this scene
			foreach (var l in (from l in GameObject.FindObjectsOfType<SceneLoader> () where l.sceneName == sceneName select l)){
				//destroy the others first
				NetworkServer.Destroy (l.gameObject);
			}
			var go = GameObject.Instantiate (GetPrefab());
			var loader = go.GetComponent<SceneLoader> ();
			loader.sceneName = sceneName;
			NetworkServer.Spawn (go);
			return loader;
		}
	}


	/// <summary>
	/// Abstract Base for Loadable Scene Types.
	/// Create one of these for each SceneLoader type.  Attach it to the kernel.
	/// </summary>
	public abstract class SceneType<T> : SceneType where T : SceneLoader {

		public override GameObject GetPrefab ()
		{
			return loaderPrefab.gameObject;
		}

		/// <summary>
		/// The prefab from which to instance the SceneLoader.  It is neccesary to use a prefab to load scenes for Networking purposes.
		/// </summary>
		public T loaderPrefab;


		/// <summary>
		/// Load the given scene.  Configure it using the given action, which usualy will be an in-line delegate or anonymous function.  Don't worry about setting the sceneName in the Action.  That's handled for you.
		/// </summary>
		/// <param name="sceneName">Name of the scene to load.</param>
		/// <param name="configureAction">Delegate to run on the instanced SceneLoader to configure it.  Usually this is used to setup the scene paramters.</param>
		protected T Load(string sceneName, System.Action<T> configureAction){
			//race condition for double loading handled here.
			//check if some other scene loader has loaded this scene
			foreach (var l in (from l in GameObject.FindObjectsOfType<SceneLoader> () where l.sceneName == sceneName select l)){
				//destroy the others first
				NetworkServer.Destroy (l.gameObject);
			}
			var go = GameObject.Instantiate (this.loaderPrefab.gameObject);
			var loader = go.GetComponent<T> ();
			loader.sceneName = sceneName;
			configureAction (loader);
			NetworkServer.Spawn (go);
			return loader;
		}

	}
	
}