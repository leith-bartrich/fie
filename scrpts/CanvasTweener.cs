using UnityEngine;
using System.Collections;

namespace fie {



	[RequireComponent(typeof(UnityEngine.CanvasGroup))]
	public class CanvasTweener : FixedTimeCurveTweener<float> {
		#region implemented abstract members of FixedTimeCurveTweener

		protected override void setValue (float value)
		{
			var canvasGroup = this.GetComponent<CanvasGroup> ();
			canvasGroup.alpha = value;
		}
		protected override void StartInterpolating (float endValue)
		{
			var canvasGroup = this.GetComponent<CanvasGroup> ();
			canvasGroup.interactable = Mathf.RoundToInt (endValue) != 0;
		}

		protected override float Interpolate (float i, float startValue, float endValue)
		{
			return (startValue * (1.0f - i)) + (endValue * (i));
		}

		protected override float currentValue ()
		{
			return this.GetComponent<CanvasGroup> ().alpha;
		}

		#endregion






	}

}