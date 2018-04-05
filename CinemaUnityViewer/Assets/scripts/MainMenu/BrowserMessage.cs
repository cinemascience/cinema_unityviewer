using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

/**
 * Script for the message that appears below the input field on the main menu
 * If something is wrong with path in the field, this will apepar and explain the problem
 * Also disables the launch button if there is an error
 */
public class BrowserMessage : MonoBehaviour {

	public Text message;
	public Button launchButton;

	//This is called every time the text in the input field changes (metadatabase browser)
	public void CheckPath (string path) {
		if (path.Length >= 6) {
			if (File.Exists (path)) {
				if (path.Substring (path.Length - 5).Equals (".json")) {
					message.text = "";
					launchButton.interactable = true;
				} else {
					message.text = "File must be a '.json' file.";
					launchButton.interactable = false;
				}
			} else {
				message.text = "File does not exist!";
				launchButton.interactable = false;
			}
		} else {
			message.text = "";
			launchButton.interactable = false;
		}
	}
}
