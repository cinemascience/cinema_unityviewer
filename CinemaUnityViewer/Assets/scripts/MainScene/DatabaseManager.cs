using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;

/**
 * Controls the "meta" of multiple databases.
 * Instantiates CinemaDatabase objects and assembles the play area
 */
public class DatabaseManager : MonoBehaviour {

	public GameObject databasePrefab;

	public Text errorLog;

	//Planes forming the edges of the play area
	public Transform ground, ceiling, north, south, east, west;

	//Path to a .json file containg the metadata for multiple databases
	//Or the info.json for a single database
	private static string path;

	//Default Database params, defined in the metadata
	private static DatabaseParams defaultParams;

	//Whether or not the path leads to a single database info.json
	private static bool singleDatabase;

	// Use this for initialization
	//Path must be set before this function is called!
	void Start () {
		Init();
	}

	private void Init() {
		string directory = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));

		if(!singleDatabase) {
			//Load multiple databases
			//Read file into JSONObject
			JSONObject json_data = new JSONObject(File.ReadAllText(path));

			//Attempt to create boundaries from data in json, throwing an exception if data couldn't be found
			try {
				Vector3 max = VectorFromJson(json_data["metadata"]["maxBounds"]);
				Vector3 min = VectorFromJson(json_data["metadata"]["minBounds"]);
				CreateBoundaries(max, min);
			} catch(Exception e) {
				CreateBoundaries(new Vector3(100, 100, 100), new Vector3(-100, -100, -100));
				addError("Boundaries not defined.\n" +
				"Defaulting to [100,100,100],[-100,-100,-100].\n" +
					"Details:\n" + e.GetType().ToString() + "\n" + e.Message);
			}

			try {
				Cache.maxItems = int.Parse(json_data["metadata"]["maxCacheSize"].ToString());
			}
			catch (Exception e) {
				addError("Max cache size not defined.\nDefaulting to 100 images.\n" +
					"Details:\n" + e.GetType().ToString() + "\n" + e.Message);
				Cache.maxItems = 100;
			}

			float scale;
			try {
				scale = float.Parse(json_data["metadata"]["scale"].ToString());
			} catch (Exception) {
				scale = 1.0f;
			}

			defaultParams = new DatabaseParams();
			try {
				defaultParams.parseParameters(json_data["metadata"]);
			} catch (Exception e) {
				addError("Error parsing default parameters:\n" + e.GetType().ToString() + "\n" + e.Message);
			}

			//Iterate through databases and instantiate a database prefab for each one
			JSONObject[] databases = new JSONObject[0];
			if(json_data.HasField("runs"))
				databases = new JSONObject[json_data["runs"].Count];
			else
				addError("Error reading JSON: Couldn't find field 'runs'");
			
			for(int i = 0; i < databases.Length; i++) {
				try {
					databases[i] = json_data["runs"][i];
					Vector3 spawnPos = VectorFromJson(databases[i]["position"]);
					spawnPos *= scale;
					GameObject newDatabase = (GameObject)Instantiate(databasePrefab, spawnPos, new Quaternion());
					newDatabase.SetActive(false);
					CinemaDatabase db = newDatabase.GetComponent<CinemaDatabase>();

					db.setParams(new DatabaseParams(defaultParams));
					db.getParams().parseParameters(databases[i]);
					if (!db.getParams().getOnline()) {
						db.getParams().setDirectory(directory + db.getParams().getDirectory());
					}
					newDatabase.SetActive(true);

				} catch(Exception e) {
					addError("Error initializing database:\n" + e.GetType().ToString() + "\n" + e.Message); 
				}
			}
		}
		else {
			try {
				//Load single database
				CreateBoundaries(new Vector3(15, 15, 15), new Vector3(-15, -15, -15));
				GameObject newDatabase = (GameObject)Instantiate(databasePrefab);
				CinemaDatabase db = newDatabase.GetComponent<CinemaDatabase>();
				db.setParams(new DatabaseParams());
				db.getParams().setDirectory(directory);
				Cache.maxItems = 100;
			}
			catch (Exception e) {
				addError("Error initializing database:\n" + e.GetType() + "\n" + e.Message);
			}
		}

	}

	//Transform the 6 planes to form the walls, floor and ceiling area according to the boundaries given
	private void CreateBoundaries(Vector3 maxBounds, Vector3 minBounds) {
		PlayerControls.maxPos = maxBounds;
		PlayerControls.minPos = minBounds;

		//Expand bounds outwards a bit to place the floor, ceiling and walls
		minBounds -= new Vector3(0.5f, 0.5f, 0.5f);
		maxBounds += new Vector3(0.5f, 0.5f, 0.5f);
		float sizeX, sizeY, sizeZ;
		sizeX = maxBounds.x - minBounds.x;
		sizeY = maxBounds.y - minBounds.y;
		sizeZ = maxBounds.z - minBounds.z;

		const int gridSize = 10;

		//Move, scale and rotate planes to form boundaries of play area
		ground.position = new Vector3(sizeX/2.0f+minBounds.x,minBounds.y,sizeZ/2.0f+minBounds.z);
		ground.localScale = new Vector3(sizeX/10.0f, 1, sizeZ/10.0f);
		ground.GetComponent<Renderer>().material.mainTextureScale = new Vector2(ground.localScale.x,ground.localScale.z)*gridSize;

		ceiling.position = ground.position + new Vector3(0, sizeY, 0);
		ceiling.localScale = ground.localScale;
		ceiling.Rotate(180,0,0);
		ceiling.GetComponent<Renderer>().material.mainTextureScale = new Vector2(ceiling.localScale.x,ceiling.localScale.z)*gridSize;

		north.position = new Vector3(maxBounds.x, sizeY / 2.0f + minBounds.y, sizeZ / 2.0f + minBounds.z);
		north.localScale = new Vector3(sizeY / 10.0f, 1, sizeZ / 10.0f);
		north.Rotate(0,0,90);
		north.GetComponent<Renderer>().material.mainTextureScale = new Vector2(north.localScale.x,north.localScale.z)*gridSize;

		south.position = north.position + new Vector3(-sizeX, 0, 0);
		south.localScale = north.localScale;
		south.Rotate(0,0,270);
		south.GetComponent<Renderer>().material.mainTextureScale = new Vector2(south.localScale.x,south.localScale.z)*gridSize;

		east.position = new Vector3(sizeX / 2.0f + minBounds.x, sizeY / 2.0f + minBounds.y, maxBounds.z);
		east.localScale = new Vector3(sizeX / 10.0f, 1, sizeY / 10.0f);
		east.Rotate(270,0,0);
		east.GetComponent<Renderer>().material.mainTextureScale = new Vector2(east.localScale.x,east.localScale.z)*gridSize;

		west.position = east.position + new Vector3(0, 0, -sizeZ);
		west.localScale = east.localScale;
		west.Rotate(90,0,0);
		west.GetComponent<Renderer>().material.mainTextureScale = new Vector2(west.localScale.x,west.localScale.z)*gridSize;
	}

	private void addError(string error) {
		errorLog.text = errorLog.text + "\n\n" + error;
	}

	//Creates a vector 3 from a json object containing an array of 3 numbers
	public static Vector3 VectorFromJson(JSONObject json) {
		return new Vector3(float.Parse(json[0].ToString()), float.Parse(json[1].ToString()), float.Parse(json[2].ToString()));
	}

	public static void SetPath(string newPath) {
		path = newPath;
	}

	public static void SetSingleDatabase(bool single) {
		singleDatabase = single;
	}
}
