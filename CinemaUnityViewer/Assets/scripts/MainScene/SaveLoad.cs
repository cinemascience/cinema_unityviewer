using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.IO;
using System;

// a class used for saving and loading serializableManagers 
public class SaveLoad {
	
	public static List<serializableManager> serializableManList = new List<serializableManager> ();
	public static string path = "";

	// tries to save serializableManager M to the given path, returns false if it is unsuccessful (e.g. the given path is protected)
	public static bool SaveManager(serializableManager M)
	{
		serializableManList.Add(M);
		BinaryFormatter bf = new BinaryFormatter();
		try{		
			FileStream file = File.Create(path);
			bf.Serialize(file, SaveLoad.serializableManList[serializableManList.Count - 1]);
			file.Close(); 
			return true;
		} catch(Exception){
		}
		return false;
	}

	// tries to save serializableManager M to the given path, returns false if it is unsuccessful (e.g. the given path does not exist)
	public static bool LoadManager(ref serializableManager M)
	{
		if(File.Exists(path) && !path.Equals("")) {
			try {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(path, FileMode.Open);
				M = (serializableManager)bf.Deserialize(file);
				file.Close();
				return true;
			} catch(Exception) {
			}
		}
		return false;
	}
}