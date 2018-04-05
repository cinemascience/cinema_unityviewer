using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.IO;

/**
 * Script controlling the camera. This is attached to the camera object that is a child
 * of the player object. The camera rotates vertically, while horizontal rotation and movement
 * is handled by the player object
 */
public class CameraControls : MonoBehaviour {
	public static Quaternion currentAngle;
	public static Quaternion fixedAngle;
	public static bool needsFix;
	// Update is called once per frame
	void Update () {
		if(PlayerControls.animationFrame <= PlayerControls.floatFrames-1) 
		{
			animateFrame();
		}
		else
		{
			if (PlayerControls.GetMovementEnabled()) {
				if(needsFix) {
					fixAngleNonStatic();
				}
				if(!PlayerControls.isOrbiting) {
					//Respond normally to mouse movement when not orbiting a point
					float mouseY = Input.GetAxis("Mouse Y");
					transform.Rotate(new Vector3(-mouseY, 0, 0) * PlayerControls.rotationSpeed, Space.Self);
					if(transform.localRotation.eulerAngles.y == 180)
						transform.Rotate(new Vector3(mouseY, 0, 0) * PlayerControls.rotationSpeed, Space.Self);
				} else {
					//When orbiting, rotate to face point

					Vector3 diff = transform.position - PlayerControls.orbitingPoint;
					float HDistance = Mathf.Sqrt(Mathf.Pow(diff.x, 2) + Mathf.Pow(diff.z, 2));
					float theta = Mathf.Atan2(diff.y, HDistance) * Mathf.Rad2Deg;
					transform.localRotation = Quaternion.Euler(new Vector3(theta, 0, 0));

					PlayerControls.FOV += Input.GetAxis("Mouse ScrollWheel") * PlayerControls.zoomSpeed;
					PlayerControls.FOV = Mathf.Clamp(PlayerControls.FOV, PlayerControls.minFOV, PlayerControls.maxFOV);
					Camera.main.fieldOfView = PlayerControls.FOV;

				}
			}
		}
		currentAngle = transform.rotation;
		fixedAngle = currentAngle;
	}

	public static void fixAngle() { //tells this monobehavior to sync up angle after loading a position on next update 
									//(the camera's angle seems to update one frame after the player's angle when animating)
		fixedAngle = PlayerControls.loadCameraRotation;
		needsFix = true;
	}
	void fixAngleNonStatic() { //actually syncs up angle after loading a position
		transform.rotation = fixedAngle;
		currentAngle = transform.rotation;
		needsFix = false;
	}
	void animateFrame()
	{
		if(PlayerControls.isOrbiting){ //turns the camera to face the orbiting point if currently orbiting
			Vector3 diff = transform.position - PlayerControls.orbitingPoint;
			float HDistance = Mathf.Sqrt(Mathf.Pow(diff.x, 2) + Mathf.Pow(diff.z, 2));
			float theta = Mathf.Atan2(diff.y, HDistance) * Mathf.Rad2Deg;
			transform.localRotation = Quaternion.Euler(new Vector3(theta, 0, 0));
			return;
		}
		if(PlayerControls.animationFrame == PlayerControls.floatFrames - 1) {//otherwise increments the camera's angle as calculated in 
			transform.rotation = PlayerControls.lastFrame.angleCam;
			PlayerControls.animationFrame++;

		} else {
			transform.Rotate(new Vector3(PlayerControls.dPhi, 0.0f, 0.0f));
		}
	}
}
