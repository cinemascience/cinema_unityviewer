using UnityEngine;
using System.Collections;

// a class to store the position, FOV, and angle of the camera inside of the viewer
public class cameraPosition
{
	public Quaternion angle { get; set; }
	public Quaternion angleCam { get; set; }

	public Vector3 position{ get; set; }

	public float FOV{ get; set; }


	public cameraPosition ()
	{
		angleCam = new Quaternion();
		angle = new Quaternion ();
		position = new Vector3 ();

		FOV = 60.0f;




	}

	// writes serializablePosition P to the given cameraPosition
	public void fromSerializable (serializablePosition P)
	{
		FOV = P.FOV;

		angleCam = P.toQuatCam();
		angle = P.toQuat ();
		position = P.toV3 ();


	}

};

// a class that transfers data back and forth from camera positions to serializable data types
[System.Serializable]
public class serializablePosition
{
	public float FOV;

	private float posX;
	private float posY;
	private float posZ;

	private float angleW;
	private float angleX;
	private float angleY;
	private float angleZ;

	private float angleCamW;
	private float angleCamX;
	private float angleCamY;
	private float angleCamZ;


	public serializablePosition()
	{
		FOV = 60.0f;

		posX = 0.0f;
		posY = 0.0f;
		posZ = 0.0f;

		angleW = 0.0f;
		angleX = 0.0f;
		angleY = 0.0f;
		angleZ = 0.0f;

		angleCamW = 0.0f;
		angleCamX = 0.0f;
		angleCamY = 0.0f;
		angleCamZ = 0.0f;
	}

	// constructor that makes a serializablePosition from a specific cameraPosition
	public serializablePosition(cameraPosition P)
	{
		FOV = P.FOV;

		posX = P.position.x;
		posY = P.position.y;
		posZ = P.position.z;

		angleW = P.angle.w;
		angleX = P.angle.x;
		angleY = P.angle.y;
		angleZ = P.angle.z;

		angleCamW = P.angleCam.w;
		angleCamX = P.angleCam.x;
		angleCamY = P.angleCam.y;
		angleCamZ = P.angleCam.z;


	}

	// writes cameraPosition P to the given serializablePosition
	public void toSerializable(cameraPosition P)
	{
		FOV = P.FOV;

		posX = P.position.x;
		posY = P.position.y;
		posZ = P.position.z;

		angleW = P.angle.w;
		angleX = P.angle.x;
		angleY = P.angle.y;
		angleZ = P.angle.z;

		angleCamW = P.angleCam.w;
		angleCamX = P.angleCam.x;
		angleCamY = P.angleCam.y;
		angleCamZ = P.angleCam.z;

	}

	// 3 functions that are used to translate back from serializable data types to more useful Unity structs
	public Quaternion toQuatCam()

	{
		Quaternion Q = new Quaternion();
		Q.Set(angleCamX, angleCamY, angleCamZ, angleCamW);
		return Q;
	}
	public Quaternion toQuat()
	{
		Quaternion Q = new Quaternion ();
		Q.Set(angleX, angleY, angleZ, angleW);
		return Q;
	}
	public Vector3 toV3()
	{

		Vector3 V = new Vector3 ();
		V.Set(posX, posY, posZ);
		return V;
	}

};
