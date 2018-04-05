using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * Script for the "Launch" button on the main menu.
 * Sets the Sprite manager's path to the path in the input field
 * and loads the main scene.
 */
public class SaveButton : MonoBehaviour {

	public InputField field;
	//Called when button is pressed
	public void OnPressed() {
		PlayerPrefs.SetString("path", field.text);
	}
}
