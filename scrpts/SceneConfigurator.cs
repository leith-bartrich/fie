using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace fie{

	/// <summary>
	/// Abstarct base for Scene Configuration behaviors.
	/// The scene configurator is given a reference to the SceneLoader and called upon Scene Load.  This gives the opportunity to set up the scene based on synced settings in the loader.
	/// </summary>
	public abstract class SceneConfigurator : MonoBehaviour {

		/// <summary>
		/// The scene loader that is loading the scene.  Shoudl be treated as "settings" for this scene. 
		/// </summary>
		public SceneLoader loader;

		/// <summary>
		/// Conveniently cast the loader to a more specific type of loader.
		/// </summary>
		/// <returns>The loader.</returns>
		/// <typeparam name="L">Type of loader to cast to.</typeparam>
		public L GetLoader<L>() where L : SceneLoader{
			return loader as L;
		}

		/// <summary>
		/// Called by the loader to configure the scene.
		/// </summary>
		public IEnumerator Configure(){
			
			var kernel = GameObject.FindObjectOfType<Kernel> ();
			kernel.LoadSpawnableTypes ();

			yield return StartCoroutine( ConfigureRoutine ());
		}

		/// <summary>
		/// Implement this routine to do your configuration.  Access the SceneLoader settings by the loader property or the GetLoader[T] generic method.
		/// </summary>
		/// <returns>The routine.</returns>
		public abstract IEnumerator ConfigureRoutine ();

		/// <summary>
		/// Implement this routine to do any unloading.  Access the SceneLoader settings by the loader property or the GetLoader[T] generic method.
		/// </summary>
		public abstract void Unload ();

	}

	
}