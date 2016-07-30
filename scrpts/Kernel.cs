using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace fie {




	public partial class Kernel : MonoBehaviour {

		public static Kernel GetKernel(){
			if (instance == null) {
				Debug.LogError ("No kernel loaded.  You seem to have called GetKernel in a context where EnsureLoaded isn't guaronteed to have run.");
				Debug.Break ();
				throw new System.Exception ("Kernel Not Loaded.");
			}
			return instance;
		}

		private static Kernel instance;

		const string SCENE_NAME = "kernel";

		public static bool IsLoaded(){
			return GameObject.FindObjectOfType<Kernel> () != null;
		}

		private KernelStartRoutine kernelStartRoutine;

		public bool GetKernelStartRoutine<T>(out T found) where T: KernelStartRoutine {
			found = kernelStartRoutine as T;
			return found != null;
		}

		public static IEnumerator EnsureLoaded(){
			if (IsLoaded ()) {
				yield break;
			} else {
				SceneManager.LoadScene (SCENE_NAME, LoadSceneMode.Additive);
				yield return null;

                instance.LoadSpawnableTypes();

				instance.kernelStartRoutine = GameObject.FindObjectOfType<KernelStartRoutine> ();
				if (instance.kernelStartRoutine == null) {
					Debug.LogError ("No Kernel Start Routine exists.  Nothing to do.  Fatal error.");
					Debug.Break ();
					yield break;
				}
				//block while initing.
				yield return instance.StartCoroutine (instance.kernelStartRoutine.InitKernelStartRoutine ());
				//run the routine in parallel.
				instance.kernelStartRoutine.StartCoroutine (instance.kernelStartRoutine.OnKernelStartRoutine());
				yield break;
			}
		}

        public void LoadSpawnableTypes()
        {
            //find SceneTypes and register them.
            foreach (var st in GameObject.FindObjectsOfType<SpawnableType>())
            {
                if (!networkManager.spawnPrefabs.Contains(st.GetPrefab()))
                {
                    networkManager.spawnPrefabs.Add(st.GetPrefab());
                }
            }
        }


		public NetworkManager networkManager{
			get{
				return this.GetComponentInChildren<NetworkManager> ();
			}
		}

		void Awake(){
			DontDestroyOnLoad (this.gameObject);
			instance = this;
		}

		private SessionRoutine sessionRoutine;

		public T GetSession<T>() where T : SessionRoutine {
			return sessionRoutine as T;
		}

		/// <summary>
		/// Starts a host-only session.
		/// Technically it's still a network game.
		/// But the server is local and no-one from the outside can log in.
		/// The network manager does not manage scene loading in this mode.
		/// </summary>
		/// <param name="playerPrefab">Player prefab to spawn.</param>
		public void StartLocalSession(NetworkIdentity playerPrefab, SessionType routineType ){
			//theoretically, this is now a secure session.
			NetworkServer.dontListen = true;
			networkManager.offlineScene = string.Empty;
			networkManager.onlineScene = string.Empty;
			networkManager.playerPrefab = playerPrefab.gameObject;
			networkManager.StartHost ();
			sessionRoutine = routineType.SpawnSession();
		}

		public enum EnumSessionType{
			CLIENT,
			HOST,
			SERVER,
		}

		/// <summary>
		/// Starts a network based session meant for multiple clients.
		/// 
		/// </summary>
		/// <param name="offlineScene">Offline scene.</param>
		/// <param name="onlineScene">Online scene.</param>
		/// <param name="playerPrefab">Player prefab.</param>
		public void StartNetworkClientSession(EnumSessionType sessionType, string offlineScene, string onlineScene, NetworkIdentity playerPrefab, SessionType routineType){
			//theoretically, this is now a secure session.
			NetworkServer.dontListen = false;
			networkManager.offlineScene = offlineScene;
			networkManager.onlineScene = onlineScene;
			networkManager.playerPrefab = playerPrefab.gameObject;
			switch (sessionType) {
				case EnumSessionType.CLIENT:
					networkManager.StartClient ();
					break;
				case EnumSessionType.SERVER:
					networkManager.StartServer ();
					break;
				case EnumSessionType.HOST:
					networkManager.StartHost ();
					break;
				default:
					Debug.LogError ("Asked to start a session of unsupported type.");
				Debug.Break();
					break;
			}
			sessionRoutine = routineType.SpawnSession ();


		}

		public void EndSession(){
			Destroy (sessionRoutine.gameObject);

			if (NetworkServer.active && NetworkClient.active) {
				networkManager.StopHost ();
			}
			if (NetworkServer.active && !NetworkClient.active) {
				networkManager.StopServer ();
			}
			if (!NetworkServer.active && NetworkClient.active) {
				networkManager.StopClient ();
			}
			NetworkServer.dontListen = false;
		}

	}

}