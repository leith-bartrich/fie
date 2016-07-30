using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace fie{

	public abstract class SpawnableType : MonoBehaviour {

		public abstract GameObject GetPrefab();

		IEnumerator Start(){
			yield return this.StartCoroutine(fie.Kernel.EnsureLoaded ());
			var spawnable = NetworkManager.singleton.spawnPrefabs;
			if (!spawnable.Contains(GetPrefab())){
				spawnable.Add (GetPrefab ());
			}
			yield break;
		}

		public GameObject Spawn(){
			var go = GameObject.Instantiate (GetPrefab ());
			NetworkServer.Spawn (go);
			return go;
		}

	}

	public abstract class SpawnableType<T> : SpawnableType where T : NetworkBehaviour {

		public override GameObject GetPrefab ()
		{
			return prefab.gameObject;
		}

		public T prefab;

		public T Spawn(System.Action<T> configureAction){
			var go = GameObject.Instantiate (this.prefab.gameObject);
			var instance = go.GetComponent<T> ();
			configureAction (instance);
			NetworkServer.Spawn (go);
			return instance;
		}

	}

}