using UnityEngine;
using System.Collections;

/**
 * Script for the ControlPanel on a database sprite. Attached to the Canvas object
 * Opens or closes via an animator when told to by the Player Object
 */
public class ControlPanel : MonoBehaviour {

	private Animator animator; //The animator controlling the open/close animations

	// Use this for initialization
	void Start () {
		PlayerControls.AddControlPanel(this); //Adds itself to Player's list of control panels to control
		animator = GetComponent<Animator>();
		animator.SetBool("open", false);
	}

	public void Open() {
		//Debug.Log("Opened!");
		animator.SetBool("open", true);
	}

	public void Close() {
		//Debug.Log("Closed!");
		animator.SetBool("open", false);
	}
}
