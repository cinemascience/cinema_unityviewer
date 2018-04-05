using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * Script for the "Launch" button on the main menu.
 * Sets the Sprite manager's path to the path in the input field
 * and loads the main scene.
 */
public class LaunchButton : MonoBehaviour {

	public InputField field;
	public InputField fieldAlpha;
	public Toggle singleDatabaseToggle;
	public static bool fixPath = false;

	//Called when button is pressed
	public void OnPressed() {
		PlayerPrefs.SetString("path", field.text);
		PlayerPrefs.SetString("path2", SaveLoad.path);
		PlayerPrefs.SetInt("singleDatabase", singleDatabaseToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("sliderValue", sliderValue.frames);

		DatabaseManager.SetPath(field.text);
		DatabaseManager.SetSingleDatabase(singleDatabaseToggle.isOn);
		SceneManager.LoadScene(1);
	}
}
