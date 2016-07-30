using UnityEngine;
using System.Collections;


/// <summary>
/// Top level color fader.  Operates outside of cameras.  Effects the whole screen.  Just use it from code.  Get the static Instance and go.  No need to set it up in the scene.
/// </summary>
[AddComponentMenu("fie/Color Fader")]
public class ColorFader : MonoBehaviour
{

	void Awake(){
		DontDestroyOnLoad (this.gameObject);
	}

    private static ColorFader instance;
    private float fadeValue = 0;
    private Texture2D _texture;

    private Texture2D colorTexture
    {
        get
        {
            if (_texture == null)
            {
                setColor(Color.black);
            }
            return _texture;
        }
    }

    private void setColor(Color c)
    {
        _texture = new Texture2D(1, 1);
        _texture.SetPixel(1, 1, c);
        _texture.Apply();
    }

    public static ColorFader Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ColorFader>();
            if (instance == null)
                instance = new GameObject("_ColorFader").AddComponent<ColorFader>();
            return instance;
        }
    }

	public void SetFader(float value, Color color){
		setColor (color);
		fadeValue = value;
	}

    public void FadeIn(float time, Color color) { routine = StartCoroutine(CoroutineFadeIn(time,color)); }
    public void FadeIn(float time) { routine = StartCoroutine(CoroutineFadeIn(time)); }
    public void FadeOut(float time, Color color) { routine = StartCoroutine(CoroutineFadeOut(time,color)); }

	private Coroutine routine;

    private IEnumerator CoroutineFadeIn(float time)
    {
		if (routine != null) {
			StopCoroutine (routine);
		}
        fadeValue = 1;
        while (fadeValue > 0) { yield return null; fadeValue -= (1 / time) * Time.deltaTime; }
    }

    private IEnumerator CoroutineFadeIn(float time, Color color)
    {
		if (routine != null) {
			StopCoroutine (routine);
		}
        setColor(color);
        fadeValue = 1;
        while (fadeValue > 0) { yield return null; fadeValue -= (1 / time) * Time.deltaTime; }
    }

    private IEnumerator CoroutineFadeOut(float time, Color color)
    {
		if (routine != null) {
			StopCoroutine (routine);
		}
        setColor(color);
        fadeValue = 0;
        while (fadeValue < 1) { yield return null; fadeValue += (1 / time) * Time.deltaTime; }
    }

    void OnGUI()
    {

        if (fadeValue <= 0)
            return;
        Color cached = GUI.color;
        GUI.color = new Color(1, 1, 1, fadeValue);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), colorTexture);
        GUI.color = cached;
    }
}
