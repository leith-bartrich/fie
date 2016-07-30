using UnityEngine;
using System.Collections;

namespace fie {

	public abstract class FixedTimeTweener<T> : MonoBehaviour {

		private Coroutine routine;
		public float duration;
		public T target;

		public void SetDuration(float duration){
			this.duration = duration;
		}

		public void SetTarget(T target){
			this.target = target;
		}

		public abstract void SetValue (T value);

		protected abstract T currentValue ();

		public IEnumerator TweenRoutine(){
			if (routine != null) {
				StopCoroutine (routine);
			}
			yield return StartCoroutine (InterpolatingRoutine (Time.fixedTime, currentValue(), target));
		}

		[ContextMenu("Tween")]
		public void Tween(){
			if (routine != null) {
				StopCoroutine (routine);
			}
			routine = StartCoroutine (InterpolatingRoutine (Time.fixedTime, currentValue(), target));
		}

		protected abstract IEnumerator InterpolatingRoutine (float startTime, T startValue, T endValue);

	}

}