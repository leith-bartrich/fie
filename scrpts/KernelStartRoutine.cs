using UnityEngine;
using System.Collections;

namespace fie
{
	/// <summary>
	/// Kernel Start Routines are only executed fully when they are
	/// in the first scene that is laoded upon running the application.
	/// In a sense, they're the main loop.  When they're part of a scene
	/// that's loaded after the Kernel has been loaded, they don't do
	/// anything.
	/// 
	/// Therefore, your first/main scene should have a KernelStartRoutine
	/// in it that is your main application/game routine.
	/// 
	/// However, you can also put them in other scenes that normally
	/// would not be the first scene loaded. During editing or
	/// testing, these routines can set up the scene
	/// to be tested.
	/// </summary>
    public abstract class KernelStartRoutine : MonoBehaviour
    {

		void Awake(){
			GameObject.DontDestroyOnLoad (this.gameObject);
		}

		void Start(){
			StartCoroutine(	StartRoutine ());
		}

		private IEnumerator StartRoutine(){
			//if this results in a kernel loaded, you can expect this call to result in OnKernelStartRoutine being called.
			yield return StartCoroutine (Kernel.EnsureLoaded ());
		}

		/// <summary>
		/// Override this routine in order to init the main loop to be exeucted by the kernel upon start.
		/// 
		/// Any behavior that ensures the kernel is laoded before executing will be guaronteed that at least
		/// this init has finished.
		/// </summary>
		/// <returns>The kernel start routine.</returns>
		public abstract IEnumerator InitKernelStartRoutine();

		/// <summary>
		/// Override this routine in order to do implement the main loop tobe executed by teh kernel upon start.
		/// This will not be executed if the scene is loaded after the kernal has been loaded.
		/// 
		/// This routine runs on the client and the server.
		/// </summary>
		public abstract IEnumerator OnKernelStartRoutine();


    }

}
