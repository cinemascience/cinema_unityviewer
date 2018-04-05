using UnityEngine;
using System.Collections;
//using System.IO;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters;
//using System.Runtime.Serialization.Formatters.Binary;


// a container class for cameraPositions that simplifies saving and loading from disk
public class CameraManager
{

	public cameraPosition position1;
	public cameraPosition position2;
	public cameraPosition position3;
	public cameraPosition position4;
	public cameraPosition position5;
	public cameraPosition position6;
	public cameraPosition position7;
	public cameraPosition position8;
	public cameraPosition position9;
	public cameraPosition position0;

	public CameraManager()
	{
		
		position1 = new cameraPosition ();
		position2 = new cameraPosition ();
		position3 = new cameraPosition ();
		position4 = new cameraPosition ();
		position5 = new cameraPosition ();
		position6 = new cameraPosition ();
		position7 = new cameraPosition ();
		position8 = new cameraPosition ();
		position9 = new cameraPosition ();
		position0 = new cameraPosition ();

	}



	public void fromSerializable(serializableManager M)
	{
		position1.fromSerializable (M.position1);
		position2.fromSerializable (M.position2);
		position3.fromSerializable (M.position3);
		position4.fromSerializable (M.position4);
		position5.fromSerializable (M.position5);
		position6.fromSerializable (M.position6);
		position7.fromSerializable (M.position7);
		position8.fromSerializable (M.position8);
		position9.fromSerializable (M.position9);
		position0.fromSerializable (M.position0);
	}

};

// a container class for serializablePositions
[System.Serializable]
public class serializableManager
{
	public serializablePosition position1;
	public serializablePosition position2;
	public serializablePosition position3;
	public serializablePosition position4;
	public serializablePosition position5;
	public serializablePosition position6;
	public serializablePosition position7;
	public serializablePosition position8;
	public serializablePosition position9;
	public serializablePosition position0;

	public serializableManager ()
	{
		position1 = new serializablePosition();
		position2 = new serializablePosition();
		position3 = new serializablePosition();
		position4 = new serializablePosition();
		position5 = new serializablePosition();
		position6 = new serializablePosition();
		position7 = new serializablePosition();
		position8 = new serializablePosition();
		position9 = new serializablePosition();
		position0 = new serializablePosition();

	}

	public void toSerializable(CameraManager M)
	{
		position1.toSerializable (M.position1);
		position2.toSerializable (M.position2);
		position3.toSerializable (M.position3);
		position4.toSerializable (M.position4);
		position5.toSerializable (M.position5);
		position6.toSerializable (M.position6);
		position7.toSerializable (M.position7);
		position8.toSerializable (M.position8);
		position9.toSerializable (M.position9);
		position0.toSerializable (M.position0);
	}
}