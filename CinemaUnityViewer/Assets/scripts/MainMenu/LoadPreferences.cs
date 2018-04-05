using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadPreferences : MonoBehaviour {

	public Slider animationSlider;
	public InputField pathInput, pathInputMng;
	public Toggle singleDatabaseToggle;

	// Use this for initialization
	void Start () {
		animationSlider.value = PlayerPrefs.GetInt("sliderValue", 30);
		pathInput.text = PlayerPrefs.GetString("path", "");
		pathInputMng.text = PlayerPrefs.GetString("path2", "");
		singleDatabaseToggle.isOn = (PlayerPrefs.GetInt("singleDatabase", 0) == 1);
	}
}
