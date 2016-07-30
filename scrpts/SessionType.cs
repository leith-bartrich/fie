using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace fie {

	public abstract class SessionType : SpawnableType<SessionRoutine>{
		public abstract SessionRoutine SpawnSession ();
	}

	public abstract class SessionType<T> : SessionType where T: SessionRoutine {

		private void Configure (SessionRoutine instance){
			Configure (instance as T);
		}

		protected abstract void Configure (T instance);

		public override SessionRoutine SpawnSession(){
			return Spawn (Configure);
		}
	}

}