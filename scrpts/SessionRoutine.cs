using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace fie {

	public abstract class SessionRoutine : NetworkBehaviour{

		void Awake(){
			isComplete = false;
			//GameObject.DontDestroyOnLoad (this.gameObject);
		}

		public override void OnStartClient ()
		{
			StartCoroutine (ClientRoutine ());
		}

		public override void OnStartServer ()
		{
			StartCoroutine (ServerRoutine ());
		}

		private bool isComplete;

		[Server]
		public bool IsComplete(){
			return isComplete;
		}

		[Server]
		protected void Complete(){
			isComplete = true;
		}

		public abstract IEnumerator ServerRoutine ();
		public abstract IEnumerator ClientRoutine ();
	}

}