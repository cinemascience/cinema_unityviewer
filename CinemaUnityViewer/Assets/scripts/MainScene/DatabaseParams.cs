using System;
using UnityEngine;
using UnityEngine.UI;

//A class keeping track of various parameters for a database defined in the meta database file
using System.CodeDom;
using System.IO;


public class DatabaseParams {

	//The pixelScale of the sprite. (In pixels per game unit) 
	private float pixelScale = 100;

	//Used to remove the background (as defined in meta json)
	private Color alphaColor;
	private float alphaThreshold;
	private float alphaSoftness;
	private bool hasAlpha;

	//Realigning and rescaling properties (as defined in meta json)
	private float minPhi, maxPhi, minTheta, maxTheta;
	private bool phiRescaled, thetaRescaled;
	private bool invertPhi, invertTheta;
	private float phiAlignment, thetaAlignment; 

	//Path to directory containing the info.json file as well as images
	private string directory = "NO_PATH_PROVIDED";

	//Whether or not this database is online
	private bool online;

	//Construct a new DatabaseParams as a copy of a given DatabaseParams
	public DatabaseParams(DatabaseParams toCopy) {
		pixelScale = toCopy.getPixelScale();
		alphaColor = toCopy.getAlphaColor();
		alphaThreshold = toCopy.getAlphaThreshold();
		alphaSoftness = toCopy.getAlphaThreshold();
		hasAlpha = toCopy.getHasAlpha();
		minPhi = toCopy.getMinPhi();
		maxPhi = toCopy.getMaxPhi();
		minTheta = toCopy.getMinTheta();
		maxTheta = toCopy.getMaxTheta();
		phiRescaled = toCopy.getPhiRescaled();
		thetaRescaled = toCopy.getThetaRescaled();
		invertPhi = toCopy.getInvertPhi();
		invertTheta = toCopy.getInvertTheta();
		phiAlignment = toCopy.getPhiAlignment();
		thetaAlignment = toCopy.getThetaAlignment();
		directory = toCopy.getDirectory();
		online = toCopy.getOnline();
	}

	//Default Constructor
	public DatabaseParams() {}

	//Set parameters based on their definitions in the given JSONObject
	public void parseParameters(JSONObject json) {

		if(json.HasField("path")) {
			directory = CinemaArgument.TrimQuotes(json["path"].ToString());
		}

		if (json.HasField("online")) {
			online = bool.Parse(json["online"].ToString());
		}

		if(json.HasField("size")) {
			pixelScale = float.Parse(json["size"].ToString());
		}
			
		if(json.HasField("alphaColor")) {
			Vector3 color = DatabaseManager.VectorFromJson(json["alphaColor"]);
			alphaColor = new Color(color.x,color.y,color.z);
			hasAlpha = true;
		}

		if (json.HasField("alphaThreshold")) {
			alphaThreshold = float.Parse(json["alphaThreshold"].ToString());
		}

		if (json.HasField("alphaSoftness")) {
			alphaSoftness = float.Parse(json["alphaSoftness"].ToString());
		}

		if(json.HasField("minPhi")) {
			minPhi = float.Parse(json["minPhi"].ToString());
			phiRescaled = true;
		}

		if(json.HasField("maxPhi")) {
			maxPhi = float.Parse(json["maxPhi"].ToString());
			phiRescaled = true;
		}

		if(json.HasField("minTheta")) {
			minTheta = float.Parse(json["minTheta"].ToString());
		}

		if(json.HasField("maxTheta")) {
			maxTheta = float.Parse(json["maxTheta"].ToString());
			thetaRescaled = true;
		}

		if (json.HasField("invertPhi")) {
			invertPhi = bool.Parse(json["invertPhi"].ToString());
		}

		if (json.HasField("invertTheta")) {
			invertTheta = bool.Parse(json["invertTheta"].ToString());
		}

		if (json.HasField("phiAlignment")) {
			phiAlignment = float.Parse(json["phiAlignment"].ToString());
		}

		if (json.HasField("thetaAlignment")) {
			thetaAlignment = float.Parse(json["thetaAlignment"].ToString());
		}
	}

	public float getPixelScale() {
		return pixelScale;
	}

	public Color getAlphaColor() {
		return alphaColor; 
	}

	public float getAlphaThreshold() {
		return alphaThreshold;
	}

	public float getAlphaSoftness() {
		return alphaSoftness;
	}

	public bool getHasAlpha() {
		return hasAlpha;
	}

	public float getMinPhi() {
		return minPhi;
	}

	public float getMaxPhi() {
		return maxPhi;
	}

	public float getMinTheta() {
		return minTheta;
	}

	public float getMaxTheta() {
		return maxTheta;
	}

	public bool getPhiRescaled() {
		return phiRescaled;
	}

	public bool getThetaRescaled() {
		return thetaRescaled;
	}

	public bool getInvertPhi() {
		return invertPhi;
	}

	public bool getInvertTheta() {
		return invertTheta;
	}

	public float getPhiAlignment() {
		return phiAlignment;
	}

	public float getThetaAlignment() {
		return thetaAlignment;
	}

	public string getDirectory() {
		return directory;
	}

	public bool getOnline() {
		return online;
	}

	public void setDirectory(string newDirectory) {
		directory = newDirectory;
	}
}

