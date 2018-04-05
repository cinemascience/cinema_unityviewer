using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

/**
 * Script for the message that appears below the input field on the main menu
 * If something is wrong with path in the field, this will apepar and explain the problem
 * Also disables the launch button if there is an error
 */
public class CameraBrowserMessage : MonoBehaviour {

	public Text message;
	public Button launchButton;
	public Slider sliderFrames;

	//This is called every time the text in the input field changes (ca browser)
	public void CheckPath (string path) {
		if(path.Length >= 4) {
			if(path.Substring(path.Length - 3).Equals(".ca")) {
				message.text = "";
				launchButton.interactable = true;
				SaveLoad.path = path.ToString();
				sliderFrames.interactable = true;

			} else {
				message.text = "File must be a '.ca' file.";
				launchButton.interactable = false;
				sliderFrames.interactable = false;
				SaveLoad.path = "";

			}
		} else if(!path.Equals("")) {
			message.text = "File must be a '.ca' file.";
			launchButton.interactable = false;
			sliderFrames.interactable = false;
			SaveLoad.path = "";
		}
		else
		{
			message.text = "";
			launchButton.interactable = true;
			SaveLoad.path = "";
			sliderFrames.interactable = false;
		}

	}
}
