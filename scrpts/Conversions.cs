using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace fie{

	/// <summary>
	/// Convenience object for common mathematical conversions.
	/// </summary>
	public static class Conversions{
		public const float cm_to_inches = 0.393701f;
		public const float inches_to_cm = 1.0f / cm_to_inches;

		public static void Maters_to_FeetAndInches(float meters, out float feet, out float inches){
			var cm = (meters * 100.0f);
			var totalInches = cm * cm_to_inches;
			feet = Mathf.Floor (totalInches / 12.0f);
			inches = totalInches % 12.0f;
			return;
		}
	}
	
}