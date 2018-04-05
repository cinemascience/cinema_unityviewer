using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * Script for "Camera Options" Button on the main menu.
 * Shows/Hides the panel for camera options
 */
public class CameraOptionButton : MonoBehaviour {

	public RectTransform mainPanel, cameraOptionPanel;

	private bool isShowing;

	//Called when button is pressed
	public void OnPressed() {
		if (isShowing) {
			isShowing = false;
			cameraOptionPanel.gameObject.SetActive(false);
			mainPanel.sizeDelta = new Vector2(400,330); 
		}
		else {
			isShowing = true;
			cameraOptionPanel.gameObject.SetActive(true);
			mainPanel.sizeDelta = new Vector2(400,480); 
		}
	}
}
