using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Notification : MonoBehaviour {

	private Text textComponent;
	private CanvasRenderer canvas;
	private float timeSinceNotification;

	public float stayingTime, fadingTime;

	// Use this for initialization
	void Start () {
		textComponent = GetComponent<Text>();
		canvas = GetComponent<CanvasRenderer>();
		canvas.SetAlpha(0.0f);
	}

	// Update is called once per frame
	void Update () {
		if(canvas.GetAlpha() > 0.0f) {
			timeSinceNotification += Time.deltaTime;
			if (timeSinceNotification >= stayingTime)
				canvas.SetAlpha(1.0f - ((timeSinceNotification - stayingTime) / fadingTime));
		}
	}

	public void notify(string message) {
		timeSinceNotification = 0;
		textComponent.text = message;
		canvas.SetAlpha(1.0f);
	}
}
