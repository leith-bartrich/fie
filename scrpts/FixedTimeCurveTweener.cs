using UnityEngine;
using System.Collections;

namespace fie {

	public abstract class FixedTimeCurveTweener<T> : FixedTimeTweener<T> {
		public AnimationCurve curve;
		public UnityEngine.Events.UnityEvent completedInterpolation;

		public override void SetValue (T value)
		{
			StartInterpolating (value);
			setValue (value);
		}

		protected abstract void setValue(T value);
		protected abstract void StartInterpolating (T endValue);
		protected abstract T Interpolate (float i, T startValue, T endValue);

		protected override IEnumerator InterpolatingRoutine (float startTime, T startValue, T endValue){
			StartInterpolating (endValue);

			while (Time.fixedTime < startTime + duration) {
				setValue (Interpolate (curve.Evaluate (Mathf.Min (1.0f, (Time.fixedTime - startTime) / duration)), startValue, endValue));
				yield return null;
			}
			setValue (endValue);
			completedInterpolation.Invoke ();
		}
	}

}