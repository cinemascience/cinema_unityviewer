using UnityEngine;
using UnityEngine.UI;
using System.IO;

/*
 * Downloaded (and modified) from Unity Wiki:
 * http://wiki.unity3d.com/index.php?title=ImprovedFileBrowser
 * 
 * Script for the browse button on the main menu
 * When pressed, opens the file browser and writes the resulting
 * path into the file path input field
 */

public class BrowseButton : MonoBehaviour {

	public InputField field, fieldMng;
	public GUISkin skin;
	public Texture2D directoryImage, fileImage;
	public Selectable singleDatabaseToggle, launchButton, browseButton, browseButtonMng, cameraOptionButton;
	public Slider sliderFrames;

	private FileBrowser fileBrowser;

	//Called every frame. Calls the file browser's on GUI if it is not null (meaning it's open)
	void OnGUI () {
		if (fileBrowser != null) {
			//Disable controls while browser is open to prevent clicking through browser
			launchButton.interactable = false;
			singleDatabaseToggle.interactable = false;
			GetComponent<Button>().interactable = false;
			field.interactable = false;
			sliderFrames.interactable = false;
			browseButton.interactable = false;
			browseButtonMng.interactable = false;
			fieldMng.interactable = false;
			cameraOptionButton.interactable = false;
			GUI.skin = skin;
			fileBrowser.OnGUI();
		}
	}

	//Called when the file browser finishes.
	//Writes the path into the input field if a file was selected
	protected void FileSelectedCallback(string filename) {
		//Re enable controls
		launchButton.interactable = true;
		singleDatabaseToggle.interactable = true;
		GetComponent<Button>().interactable = true;
		field.interactable = true;
		sliderFrames.interactable = true;
		browseButton.interactable = true;
		browseButtonMng.interactable = true;
		fieldMng.interactable = true;
		cameraOptionButton.interactable = true;
		fileBrowser = null;
		if(filename != null) {
			field.text = filename;
		}
	}

	//Called when the button is pressed
	//Opens the file browser
	public void OnPressed() {
		fileBrowser = new FileBrowser(
			new Rect(Screen.width/2-250, Screen.height/2-200, 500, 400),
			"Choose .json file",
			FileSelectedCallback
		);
		if(File.Exists(field.text)) {
			fileBrowser.CurrentDirectory = field.text.Substring(0,field.text.LastIndexOf(Path.DirectorySeparatorChar)+1);
		}
		fileBrowser.SelectionPattern = "*.json";
		fileBrowser.DirectoryImage = directoryImage;
		fileBrowser.FileImage = fileImage;
	}
}
