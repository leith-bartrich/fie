using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/// <summary>
/// Sizes a convas in world units.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Canvas))]
[AddComponentMenu("UI/Canvas Sizer (World)")]
public class CanvasSizerWorld : MonoBehaviour {

	/// <summary>
	/// MatchType.  Which proprotion to constrain.
	/// </summary>
	public enum MatchTypeEnum{
		Horizontal,
		Vertical,
		Both
	}

	/// <summary>
	/// Canvas to constrain.  If left null, will get from this object or its parents.
	/// </summary>
	public Canvas canvas;


	private RectTransform rtrans;

	/// <summary>
	/// Match type to use.
	/// </summary>
	public MatchTypeEnum MatchType;

	//Size to constrain to, in world-units.  The X and Y values are used or ignored based on the MatchType.
	public Vector2 Size;

	// Update is called once per frame
	void Update () {
		canvas = this.GetComponent<Canvas> () ?? this.GetComponentInParent<Canvas> ();
		if (canvas != null) {
			rtrans = canvas.GetComponent<RectTransform> ();
			canvas.renderMode = RenderMode.WorldSpace;
			var rect = rtrans.rect;
			Vector3 scale;
			switch (MatchType) {
			case MatchTypeEnum.Both:
				scale = new Vector3 ((1.0f / rect.width) * Size.x, ((1.0f) / rect.height) * Size.y, 1.0f);
				break;
			case MatchTypeEnum.Horizontal:
				scale = new Vector3 ((1.0f / rect.width) * Size.x, ((1.0f) / rect.height) * (rect.height / rect.width) * Size.x, 1.0f);
				break;
			case MatchTypeEnum.Vertical:
				scale = new Vector3 ((1.0f / rect.width) * (rect.width/rect.height) * Size.y, ((1.0f) / rect.height) * Size.y, 1.0f);
				break;
			default:
				throw new System.NotImplementedException ();
			}
			rtrans.localScale = scale;
			var boxCollider = this.GetComponent<BoxCollider> ();
			if (boxCollider != null) {
				var size = boxCollider.size;
				size.x = rect.width;
				size.y = rect.height;
				boxCollider.size = size;
				boxCollider.center = new Vector3 (0.0f, 0.0f, -0.5f * size.z);
			}
		}
	}


}
