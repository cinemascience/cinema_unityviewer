using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * Script for controlling the player object. Attached to the Player object itself.
 * Moves and rotates horizontally. The child camera object of the player handles
 * vertical rotation.
 * Also handles the state of the camera (movement enabled or disabled), and everything involved with saving and loading
 */
using System;


public class PlayerControls : MonoBehaviour {

	//Non-static variables used so they can be set in the Unity editor
	public float moveSpeedNonStatic, rotationSpeedNonStatic, zoomSpeedNonStatic;
	public Notification saveLoadNotification, fovNotification;

	private static bool movementEnabled, fileExists;
	//The list of control panels to open or close when camera state changes
	private static List<ControlPanel> controlPanels = new List<ControlPanel>();

	//The GameObject this script is attached to.
	private static GameObject playerObject;

	//used for keyboard/mouse navigation
	public static Vector3 maxPos, minPos;
	public static float moveSpeed, rotationSpeed, zoomSpeed;
	public static bool isOrbiting, isLoading;
	public static int angleID;
	public static Vector3 orbitingPoint;
	public static Quaternion loadCameraRotation;

	public static float FOV = 60.0f;
	public static float prevFOV = FOV;
	public static float minFOV = 15f;
	public static float maxFOV = 90f;

	//used for animating camera angles 
	private static float dX, dY, dZ, dF, dTheta;
	public static float dPhi;
	public static int frames;
	public static int animationFrame;
	public static float floatFrames;

	//used for saving and loading camera angles
	public static CameraManager manager = new CameraManager();
	public static serializableManager SM = new serializableManager();
	public static cameraPosition lastFrame = new cameraPosition();

	// Use this for initialization
	void Start () {
		moveSpeed = moveSpeedNonStatic;
		rotationSpeed = rotationSpeedNonStatic;
		zoomSpeed = zoomSpeedNonStatic;
		isOrbiting = false;
		movementEnabled = true;
		Cursor.lockState = CursorLockMode.Locked;
		playerObject = gameObject;
		manager = new CameraManager();
		fileExists = loadFromDisk();
		if(sliderValue.animated) {
			frames = sliderValue.frames + 1;
			floatFrames = frames;
		} 
		else
			frames = 0;
		animationFrame = 0;	

	}

	// Update is called once per frame
	void Update () {
		if(movementEnabled) {
			Cursor.lockState = CursorLockMode.Locked;
			//checks if a cameraPosition was selected the in the last frame to load from;
			//if a cameraPosition was selected, input is disabled for this frame and the cameraPosition is loaded
			if(!isLoading) {
				if(animationFrame <= frames - 1) { //keeps track of frames if a cameraPosition is currently animating. Also disables input
					animateFrame(); 
				} else if(fileExists) {
					if(sliderValue.animated)
						saveLoadInputAnimated(ref manager); //there are 2 different functions to take input for saving and loading 
															// (to account for if the loading is animated or not) 
					else
						saveLoadInput(ref manager);
				}
				if(animationFrame==frames){

					if(!isOrbiting) {
						//Respond normally to input when not orbiting
						float moveFwd = Input.GetAxis("Vertical"); // W/S
						float moveRgt = Input.GetAxis("Horizontal"); // A/D
						float mouseX = 0.0f;
						mouseX = Input.GetAxis("Mouse X");	
						transform.Rotate(new Vector3(0, mouseX, 0) * rotationSpeed, Space.World);

						FOV += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
						FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
						Camera.main.fieldOfView = FOV;
						if (FOV != prevFOV) {
							prevFOV = FOV;
							fovNotification.notify("FOV: " + String.Format("{0:F2}",FOV));
						}

						float moveUp = Input.GetAxis("Depth"); // Q/E
						Vector3 movement = new Vector3(moveRgt, moveUp, moveFwd);
						transform.Translate(movement * moveSpeed * (Time.deltaTime / (1.0f / 60.0f)));
					} else {
						//When orbiting, use spherical coordinates to rotate around point
						Vector3 diff = transform.position - orbitingPoint;
						float radius = diff.magnitude;
						float HDistance = Mathf.Sqrt(Mathf.Pow(diff.x, 2) + Mathf.Pow(diff.z, 2));
						float phi = Mathf.Atan2(-diff.x, -diff.z) * Mathf.Rad2Deg;
						float theta = Mathf.Atan2(diff.y, HDistance) * Mathf.Rad2Deg;

						float moveTheta = Input.GetAxis("Vertical"); // W/S
						float movePhi = Input.GetAxis("Horizontal"); // A/D
						float moveRadius = Input.GetAxis("Depth"); //Q/E

						transform.rotation = Quaternion.Euler(new Vector3(0, phi, 0));

						phi = (270 - (phi - movePhi * (Time.deltaTime / (1.0f / 60.0f)))) % 360;
						theta = Mathf.Clamp(90 - (theta + moveTheta * (Time.deltaTime / (1.0f / 60.0f))), 0.1f, 179.9f);
						radius = radius + (moveRadius * moveSpeed * (Time.deltaTime / (1.0f / 60.0f)));

						float x = (radius * Mathf.Sin(theta * Mathf.Deg2Rad) * Mathf.Cos(phi * Mathf.Deg2Rad)) + orbitingPoint.x;
						float z = (radius * Mathf.Sin(theta * Mathf.Deg2Rad) * Mathf.Sin(phi * Mathf.Deg2Rad)) + orbitingPoint.z;
						float y = (radius * Mathf.Cos(theta * Mathf.Deg2Rad)) + orbitingPoint.y;

						transform.position = new Vector3(x, y, z);
					}

					//Clamp camera positon to boundaries
					float clampedX = Mathf.Clamp(transform.position.x, minPos.x, maxPos.x);
					float clampedY = Mathf.Clamp(transform.position.y, minPos.y, maxPos.y);
					float clampedZ = Mathf.Clamp(transform.position.z, minPos.z, maxPos.z);
					transform.position = new Vector3(clampedX, clampedY, clampedZ);

					//Disable camera if LMB is pressed
					if(Input.GetMouseButtonUp(0)) {
						movementEnabled = false;
						Cursor.lockState = CursorLockMode.None;
						Cursor.visible = true;
						foreach(ControlPanel panel in controlPanels) {
							panel.Open();
						}
					}
				}
			} else {
				load(angleID, ref manager);
			}
		}

		else {
			//Enable camera if RMB is pressed
			if (Input.GetMouseButtonUp(1)) {
				movementEnabled = true;
				Cursor.lockState = CursorLockMode.Locked;
				foreach (ControlPanel panel in controlPanels) {
					panel.Close();
				}
			}
		}
	}

	public static void AddControlPanel(ControlPanel newPanel) {
		controlPanels.Add(newPanel);
	}

	public static bool GetMovementEnabled() {
		return movementEnabled;
	}

	public static GameObject GetGameObject() {
		return playerObject;
	}
		
	void saveLoadInput(ref CameraManager manager)
	{
		//checks for saving and loading input if no frames were selected
		if(Input.GetAxis("position1")<0.0f)
		{
			savePosition(1, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n"); 
			print( "and FOV " + FOV + "\nto position1");*/
			saveLoadNotification.notify("Recorded camera position: 1");
		}

		if(Input.GetAxis("position1")>0.0f)
		{
			if (!isLoading) {
				isLoading = true;
				angleID = 1;
			}
		}

		if(Input.GetAxis("position2")<0.0f)
		{
			savePosition(2, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString ()+ "\n");  
			print("and FOV" + FOV + "\nto position2");*/
			saveLoadNotification.notify("Recorded camera position: 2");
		}

		if(Input.GetAxis("position2")>0.0f)
		{
			if (!isLoading) {
				isLoading = true;
				angleID = 2;
			}
		}

		if(Input.GetAxis("position3")<0.0f)
		{
			savePosition(3, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n"); 
			print("and FOV" + FOV + "\nto position3");*/
			saveLoadNotification.notify("Recorded camera position: 3");
		}

		if(Input.GetAxis("position3")>0.0f)
		{
			if (!isLoading) {
				isLoading =  true;
				angleID = 3;
			}
		}

		if(Input.GetAxis("position4")<0.0f)
		{
			savePosition(4, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position4");*/
			saveLoadNotification.notify("Recorded camera position: 4");
		}

		if(Input.GetAxis("position4")>0.0f)
		{
			if (!isLoading) {
				isLoading =  true;
				angleID = 4;
			}
		}

		if(Input.GetAxis("position5")<0.0f)
		{
			/*savePosition(5, manager);
			print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position5");*/
			saveLoadNotification.notify("Recorded camera position: 5");
		}

		if(Input.GetAxis("position5")>0.0f)
		{
			if (!isLoading) {
				isLoading =  true;
				angleID = 5;
			}
		}

		if(Input.GetAxis("position6")<0.0f)
		{
			savePosition(6, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position6");*/
			saveLoadNotification.notify("Recorded camera position: 6");
		}

		if(Input.GetAxis("position6")>0.0f)
		{
			if (!isLoading) {
				isLoading =  true;
				angleID = 6;
			}
		}

		if(Input.GetAxis("position7")<0.0f)
		{
			savePosition(7, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position7");*/
			saveLoadNotification.notify("Recorded camera position: 7");
		}

		if(Input.GetAxis("position7")>0.0f)
		{
			if (!isLoading) {
				isLoading =  true;
				angleID = 7;
			}
		}

		if(Input.GetAxis("position8")<0.0f)
		{
			savePosition(8, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position8");*/
			saveLoadNotification.notify("Recorded camera position: 8");
		}

		if(Input.GetAxis("position8")>0.0f)
		{
			if (!isLoading) {
				isLoading =  true;
				angleID = 8;
			}
		}

		if(Input.GetAxis("position9")<0.0f)
		{
			savePosition(9, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position9");*/
			saveLoadNotification.notify("Recorded camera position: 9");
		}

		if(Input.GetAxis("position9")>0.0f)
		{
			if (!isLoading) {
				isLoading =  true;
				angleID = 9;
			}
		}
		if(Input.GetAxis("position10")<0.0f)
		{
			savePosition(0, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position10");*/
			saveLoadNotification.notify("Recorded camera position: 10");
		}

		if(Input.GetAxis("position10")>0.0f)
		{
			if (!isLoading) {
				isLoading =  true;
				angleID = 0;
			}
		}

		//	if (Input.GetKeyDown ("return")) {
		if(Input.GetAxis("submit")>0.0f){
			if(fileExists) {
				SM.toSerializable(manager);
				if(SaveLoad.SaveManager(SM))
					//print("written to disk: " + SaveLoad.path + "\n");
					saveLoadNotification.notify("Camera positions saved:\n" + SaveLoad.path);
				else if(SaveLoad.path.Equals(""))
					//print("no write file selected\n");
					saveLoadNotification.notify("Couldn't save camera positions!\nNo save file selected.");
				else
					//print("write file " + SaveLoad.path + " doesn't exist\n");
					saveLoadNotification.notify("Couldn't save camera positions!\nSave file \'" + SaveLoad.path + "\' doesn't exist.");
			}
			else 
				//print ("no write file selected\n");
				saveLoadNotification.notify("Couldn't save camera positions!\nNo save file selected.");
		}
	}

	void saveLoadInputAnimated(ref CameraManager manager) 
	{
		//checks for saving and loading input if frames>=15 was selected
		if(Input.GetKeyDown ("f1"))
		{
			savePosition(1, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n"); 
			print( "and FOV " + FOV + "\nto position1");*/
			saveLoadNotification.notify("Recorded camera position: 1");
		}

		if (Input.GetKeyDown ("1"))
		{
			loadPositionAnimated(manager.position1);
		}

		if (Input.GetKeyDown ("f2"))
		{
			savePosition(2, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString ()+ "\n");  
			print("and FOV" + FOV + "\nto position2");*/
			saveLoadNotification.notify("Recorded camera position: 2");
		}

		if (Input.GetKeyDown ("2"))
		{
			loadPositionAnimated(manager.position2);
		}
		if (Input.GetKeyDown ("f3"))
		{
			savePosition(3, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n"); 
			print("and FOV" + FOV + "\nto position3");*/
			saveLoadNotification.notify("Recorded camera position: 3");
		}

		if (Input.GetKeyDown ("3"))
		{
			loadPositionAnimated(manager.position3);
		}

		if (Input.GetKeyDown ("f4"))
		{
			savePosition(4, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position4");*/
			saveLoadNotification.notify("Recorded camera position: 4");
		}

		if (Input.GetKeyDown ("4"))
		{
			loadPositionAnimated(manager.position4);
		}

		if (Input.GetKeyDown ("f5"))
		{
			savePosition(5, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position5");*/
			saveLoadNotification.notify("Recorded camera position: 5");
		}

		if (Input.GetKeyDown ("5"))
		{
			loadPositionAnimated(manager.position5);
		}

		if (Input.GetKeyDown ("f6"))
		{
			savePosition(6, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position6");*/
			saveLoadNotification.notify("Recorded camera position: 6");
		}

		if (Input.GetKeyDown ("6"))
		{
			loadPositionAnimated(manager.position6);
		}

		if (Input.GetKeyDown ("f7"))
		{
			savePosition(7, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position7");*/
			saveLoadNotification.notify("Recorded camera position: 7");
		}

		if (Input.GetKeyDown ("7"))
		{
			loadPositionAnimated(manager.position7);
		}

		if (Input.GetKeyDown ("f8"))
		{
			savePosition(8, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position8");*/
			saveLoadNotification.notify("Recorded camera position: 8");
		}

		if (Input.GetKeyDown ("8"))
		{
			loadPositionAnimated(manager.position8);
		}

		if (Input.GetKeyDown ("f9"))
		{
			savePosition(9, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position9");*/
			saveLoadNotification.notify("Recorded camera position: 9");
		}

		if (Input.GetKeyDown ("9"))
		{
			loadPositionAnimated(manager.position9);
		}
		if (Input.GetKeyDown ("f10"))
		{
			savePosition(0, manager);
			/*print ("saving angle" + transform.rotation.eulerAngles.ToString () + "\n"
				+ "and position" + transform.position.ToString () + "\n");  
			print("and FOV" + FOV + "\nto position10");*/
			saveLoadNotification.notify("Recorded camera position: 10");
		}

		if (Input.GetKeyDown ("0"))
		{
			loadPositionAnimated(manager.position0);
		}
		if (Input.GetKeyDown ("return")) {
			if(fileExists) {
				SM.toSerializable(manager);
				if(SaveLoad.SaveManager(SM))
					//print("written to disk: " + SaveLoad.path + "\n");
					saveLoadNotification.notify("Camera positions saved:\n" + SaveLoad.path);
				else if(SaveLoad.path.Equals(""))
					//print("no write file selected\n");
					saveLoadNotification.notify("Couldn't save camera positions!\nNo save file selected.");
				else
					//print("write file " + SaveLoad.path + " doesn't exist\n");
					saveLoadNotification.notify("Couldn't save camera positions!\nSave file \'" + SaveLoad.path + "\' doesn't exist.");
			}
			else 
				//print ("no write file selected\n");
				saveLoadNotification.notify("Couldn't save camera positions!\nNo save file selected.");
		}

	}

	//saves the current position, angle, and FOV to the correct cameraPosition contained in cameraManager M
	void savePosition(int angleID, CameraManager M)
	{
		switch (angleID) {
		case 1:
			M.position1.FOV = FOV;
			M.position1.angle = transform.rotation;
			M.position1.position = transform.position;
			M.position1.angleCam = CameraControls.currentAngle;
			break;
		case 2:
			M.position2.FOV = FOV;
			M.position2.angle = transform.rotation;
			M.position2.position = transform.position;
			M.position2.angleCam = CameraControls.currentAngle;
			break;
		case 3:
			M.position3.FOV = FOV;
			M.position3.angle = transform.rotation;
			M.position3.position = transform.position;
			M.position3.angleCam = CameraControls.currentAngle;
			break;
		case 4:
			M.position4.FOV = FOV;
			M.position4.angle = transform.rotation;
			M.position4.position = transform.position;
			M.position4.angleCam = CameraControls.currentAngle;
			break;
		case 5:
			M.position5.FOV = FOV;
			M.position5.angle = transform.rotation;
			M.position5.position = transform.position;
			M.position5.angleCam = CameraControls.currentAngle;
			break;
		case 6:
			M.position6.FOV = FOV;
			M.position6.angle = transform.rotation;
			M.position6.position = transform.position;
			M.position6.angleCam = CameraControls.currentAngle;
			break;
		case 7:
			M.position7.FOV = FOV;
			M.position7.angle = transform.rotation;
			M.position7.position = transform.position;
			M.position7.angleCam = CameraControls.currentAngle;
			break;
		case 8:
			M.position8.FOV = FOV;
			M.position8.angle = transform.rotation;
			M.position8.position = transform.position;
			M.position8.angleCam = CameraControls.currentAngle;
			break;
		case 9:
			M.position9.FOV = FOV;
			M.position9.angle = transform.rotation;
			M.position9.position = transform.position;
			M.position9.angleCam = CameraControls.currentAngle;
			break;
		case 0:
			M.position0.FOV = FOV;
			M.position0.angle = transform.rotation;
			M.position0.position = transform.position;
			M.position0.angleCam = CameraControls.currentAngle;
			break;
		}


	}

	//loads the correct cameraPosition from cameraManager M
	void load(int angleID, ref CameraManager M) {
		switch(angleID) {
		case 1:
			loadPosition(M.position1);
			//print("from position1\n");
			break;
		case 2:
			loadPosition(M.position2);
			//print("from position2\n");
			break;
		case 3:
			loadPosition (manager.position3);
			//print ("from position3\n");
			break;
		case 4:
			loadPosition (manager.position4);
			//print ("from position4\n");
			break;
		case 5:
			loadPosition (manager.position5);
			//print ("from position5\n");
			break;
		case 6:
			loadPosition (manager.position6);
			//print ("from position6\n");
			break;
		case 7:
			loadPosition (manager.position7);
			//print ("from position7\n");
			break;
		case 8:
			loadPosition (manager.position8);
			//print ("from position8\n");
			break;
		case 9:
			loadPosition (manager.position9);
			//print ("from position9\n");
			break;
		case 0:
			loadPosition (manager.position0);
			//print ("from position10\n");
			break;



		}
	}
	void loadPosition(cameraPosition P) //sets current position, player angle(theta), and FOV to selected save and prepares camera to load angle(phi)
	{
		Input.ResetInputAxes (); //resets input from the movement keys

		float clampedX = Mathf.Clamp(P.position.x, minPos.x, maxPos.x); //accounts for if the position was saved while viewing a database that had different bounds
		float clampedY = Mathf.Clamp(P.position.y, minPos.y, maxPos.y);
		float clampedZ = Mathf.Clamp(P.position.z, minPos.z, maxPos.z);
		transform.position = new Vector3(clampedX, clampedY, clampedZ);

		Camera.main.fieldOfView = P.FOV; 	
		FOV = P.FOV;
		transform.rotation = P.angle;

		loadCameraRotation = P.angleCam;
		CameraControls.fixAngle();

		/*print ("loading angle (" + P.angleCam.eulerAngles.x + ", " + P.angle.eulerAngles.y + ", 0.0)\n"
			+ "and position" + P.position.ToString () + "\n"); 
		print("and FOV " + P.FOV + "\n");*/

		isLoading = false;

	}

	void loadPositionAnimated(cameraPosition P) //sets the value of the variables used to keep track of animation
	{
		Input.ResetInputAxes (); //resets input from the movement keys
		dX = (P.position.x - transform.position.x) / (floatFrames-1); // the distance incremented in the +X direction each frame
		dY = (P.position.y - transform.position.y) / (floatFrames-1); // +Y direction increment
		dZ = (P.position.z - transform.position.z) / (floatFrames-1); // +Z direction increment

		/* calculates the minimum increment necessary to animate the player's angle theta (left and right)
		 * either ((theta - current theta) / frames),((theta - current theta - 360) / frames), or ((theta - current theta + 360) / frames)
		 * essentially, the angle is corrected by +- 360 / frames if it means an increment with a smaller absolute value.
		 */
		float dTheta1 = (P.angle.eulerAngles.y - transform.eulerAngles.y) / (floatFrames-1);

		float dTheta2 = (P.angle.eulerAngles.y - transform.eulerAngles.y - 360) / (floatFrames - 1);
		float dTheta3 = (P.angle.eulerAngles.y - transform.eulerAngles.y + 360) / (floatFrames - 1);


		if(Mathf.Min(Mathf.Min(Mathf.Abs(dTheta1), Mathf.Abs(dTheta2)), dTheta3) == Mathf.Abs(dTheta1))
			dTheta = dTheta1;
		else if(Mathf.Min(Mathf.Abs(dTheta2), dTheta3) == Mathf.Abs(dTheta2))
			dTheta = dTheta2;
		else
			dTheta = dTheta3;

		// calculates the minimum increment necessary to animate the camera's angle phi (up and down)
		// either ((phi - current phi) / frames),((phi - current phi - 360) / frames), or ((phi - current phi + 360) / frames)

		float dPhi1 = (P.angleCam.eulerAngles.x - CameraControls.currentAngle.eulerAngles.x) / (floatFrames-1);
		float dPhi2 = ((P.angleCam.eulerAngles.x - CameraControls.currentAngle.eulerAngles.x) - 360) / (floatFrames - 1);
		float dPhi3 = ((P.angleCam.eulerAngles.x - CameraControls.currentAngle.eulerAngles.x) + 360) / (floatFrames - 1);

		if(Mathf.Min((Mathf.Min(Mathf.Abs(dPhi1), Mathf.Abs(dPhi2))), dPhi3) == Mathf.Abs(dPhi1))
			dPhi = dPhi1;
		else if(Mathf.Min((Mathf.Min(Mathf.Abs(dPhi1), Mathf.Abs(dPhi2))), dPhi3) == Mathf.Abs(dPhi2))
			dPhi = dPhi2;
		else 
			dPhi = dPhi3;

		dF = (P.FOV - FOV) / (floatFrames-1); // the FOV increment

		animationFrame = 0;
		if(isOrbiting)
			animationFrame = 1;
		lastFrame = P;

		/*print ("loading angle (" + P.angleCam.eulerAngles.x + ", " + P.angle.eulerAngles.y + ", 0.0)\n"
			+ "and position" + P.position.ToString () + "\n"); 
		print("and FOV " + P.FOV + " and animating\n");*/
	}

	void animateFrame() //iteratively called to translate and rotate the player each frame while animating
	{	
		Camera.main.fieldOfView += dF;
		FOV += dF;
		transform.position += new Vector3(dX, dY, dZ);
		if(!isOrbiting) { 
			transform.Rotate(new Vector3(0.0f, dTheta, 0.0f));
		} 
		else { //turns the camera to face the orbiting point if currently orbiting
			Vector3 diff = transform.position - orbitingPoint;
			float phi = Mathf.Atan2(-diff.x, -diff.z) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(new Vector3(0, phi, 0));
		}
		animationFrame++;
	}

	bool loadFromDisk() 
	/* attempts to load from disk. If the path returns false from SaveLoad.LoadManager, this will return false.
	 * If loading is successful (the file exists and is accessible), then it will return true.
	 * Otherwise, if the path is valid but a save file doesn't exist, then a new cameraManager is initialized and saved to the path.
	 */
	{
		if(SaveLoad.path.Equals("")) return false;
		if(SaveLoad.LoadManager(ref SM)) {
			manager.fromSerializable(SM);

			print("read from disk: " + SaveLoad.path + "\n");
			return true;
		} else if(SaveLoad.path.Equals(""))
			print("no read file selected\n");
		else {
			print("file " + SaveLoad.path + " doesn't exist.\n");
			print("new manager initialized and written to " + SaveLoad.path + "\n");
			CameraManager blank = new CameraManager();
			serializableManager blankSerializable = new serializableManager();
			blankSerializable.toSerializable(blank);
			SaveLoad.SaveManager(blankSerializable);
			return true;

		}
		return false;
	}

}
