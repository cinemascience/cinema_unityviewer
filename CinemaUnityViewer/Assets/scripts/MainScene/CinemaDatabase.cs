using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System;
using System.Security.Policy;
using System.Net;

enum DatabaseSpec {
	specA,
	specB
};

/**
 * This script is to be attached to a Sprite object.
 * It represents a single Cinema database, the image for which is displayed on the sprite
 */
public class CinemaDatabase : MonoBehaviour {

	//Sprites to display in case of errors
	public Sprite errorReadingFileSprite, errorReadingDatabaseSprite;

	//Background Alpha shader to use if the database has alpha
	public Shader alphaShader;

	//Prefab for varPanel, as one will be instantiated for each argument
	public GameObject argPanelPrefab;

	//Whether or not the database succesfully loaded
	private bool loaded;

	//CinemaArgument objects representing the arguments defined in info.json
	//There are separate references for the phi and theta arguments if they exist
	//(though they are also included in the array)
	private CinemaArgument[] arguments;
	private CinemaArgument phiVar, thetaVar;

	//String representing the name_pattern defined in info.json
	private string name_pattern;

	//Arrays containing the values of the phi and theta arguments, if they exist
	private float[] phiValues, thetaValues;

	//This database's cache for loading and storing images
	private Cache cache;

	//Set of parameters for this database (as defined in json)
	private DatabaseParams databaseParams;

	//Components of the Database's control panel
	private Canvas controlPanel;
	private Text panelTitle, panelMessage, cacheText;
	private Image panelFooter;
	private Toggle orbitToggle;

	//Initializes components and calls loadDatabase() on the first frame that this script is active
	void Start() {
		controlPanel = GetComponentInChildren<Canvas>();
		Text[] texts = controlPanel.GetComponentsInChildren<Text>();
		foreach (Text text in texts) {
			if(text.CompareTag("panelTitle"))
				panelTitle = text;
		}
		Image[] images = controlPanel.GetComponentsInChildren<Image>();
		foreach (Image image in images) {
			if(image.CompareTag("panelFooter")) {
				panelFooter = image;
				Text[] footerTexts = panelFooter.GetComponentsInChildren<Text>();
				foreach (Text text in footerTexts) {
					if(text.CompareTag("messageText"))
						panelMessage = text;
					if(text.CompareTag("cacheText"))
						cacheText = text;
				}
				orbitToggle = panelFooter.GetComponentInChildren<Toggle>();
				orbitToggle.onValueChanged.AddListener(orbitToggled);
			}
		}
		//Initialize cache
		cache = new Cache();

		try {
			LoadDatabase();
			loaded = true;
		} catch (Exception e) {
			GetComponent<SpriteRenderer>().sprite = errorReadingDatabaseSprite;
			panelMessage.text = e.GetType().ToString() + "\n" + e.Message;
		} 
	}

	/**
	 * Loads the database
	 */
	private void LoadDatabase() {
		panelTitle.text = databaseParams.getDirectory().Substring(databaseParams.getDirectory().LastIndexOf(Path.DirectorySeparatorChar) + 1);

		//Reads file into JSONObject
		Stream file;
		if(!databaseParams.getOnline()) {
			file = File.OpenRead(databaseParams.getDirectory() + Path.DirectorySeparatorChar + "info.json");
		}
		else {
			WebClient client = new WebClient();
			file = client.OpenRead(databaseParams.getDirectory() + "/" + "info.json");
		}
		StreamReader reader = new StreamReader(file);
		JSONObject json_data = new JSONObject(reader.ReadToEnd());

		//Load arguments into CinemaVariable objects
		if (json_data.HasField("arguments"))
			arguments = new CinemaArgument[json_data["arguments"].Count];
		else
			throw new Exception("Error reading JSON: Couldn't find field 'arguments'");
		//Read name_pattern
		if(json_data.HasField("name_pattern"))
			name_pattern = CinemaArgument.TrimQuotes(json_data["name_pattern"].ToString());
		else
			throw new Exception("Error reading JSON: Couldn't find field 'name_pattern'");

		//Iterates through arguments, initializes phi and theta if found,
		//Creates a varPanel for the others
		int skipped = 0;
		for(int i = 0; i < arguments.Length; i++) {
			arguments[i] = new CinemaArgument(json_data["arguments"][i]);
			if(arguments[i].GetLabel().Equals("phi")) { //Argument is phi
				phiVar = arguments[i];
				phiValues = new float[phiVar.GetValues().Length];
				for(int j = 0; j < phiValues.Length; j++)
					phiValues[j] = float.Parse(phiVar.GetValues()[j]);
				skipped++;
			}
			else if(arguments[i].GetLabel().Equals("theta")) { //Argument is theta
				thetaVar = arguments[i];
				thetaValues = new float[thetaVar.GetValues().Length];
				for(int j = 0; j < thetaValues.Length; j++)
					thetaValues[j] = float.Parse(thetaVar.GetValues()[j]);
				skipped++;
			}
			else { //Argument is neither phi nor theta
				if(arguments[i].GetValues().Length > 1) {
					//Instantiates a varPanel for this argument
					GameObject varPanel = Instantiate<GameObject>(argPanelPrefab);
					varPanel.transform.SetParent(controlPanel.transform, false);
					varPanel.transform.Translate(0, -0.5f - (0.7f * (i - skipped)), 0);
					panelFooter.transform.Translate(0,-0.7f,0);
					argPanelControls controls = varPanel.GetComponent<argPanelControls>();
					controls.Init(this);
					controls.SetCinemaVariable(arguments[i]);
				} else { //Argument is skipped if there is only one value for it
					skipped++;
				}
			}
		}

		//Set up background alpha material if this database has alpha
		if (databaseParams.getHasAlpha()) {
			Material mat = new Material(alphaShader);
			mat.SetColor("_AlphaColor", databaseParams.getAlphaColor());
			mat.SetFloat("_AlphaThreshold", databaseParams.getAlphaThreshold());
			mat.SetFloat("_AlphaSoftness", databaseParams.getAlphaSoftness());
			GetComponent<SpriteRenderer>().material = mat;
		}

	} //end LoadDatabase()

	/*
	 * Called once per frame.
	 * Rotates sprite to face camera
	 * Calculates phi and theta values and calls UpdateTexture (if the database is loaded)
	 */
	void Update () {
		transform.LookAt (PlayerControls.GetGameObject().transform);
		transform.Rotate (new Vector3(0, 180, 0));
	
		//Calculate phi and set shouldUpdate to true if it has changed
		if (phiVar != null) {
			float phi = transform.localRotation.eulerAngles.y;

			//Adjust phi according to alignment, invert and rescaling values
			if(databaseParams.getInvertPhi()) {
				phi = 360 - phi;
			}
			phi = (phi + databaseParams.getPhiAlignment()) % 360;

			if (databaseParams.getPhiRescaled()) {
				float min = phiValues[0];
				float max = phiValues[phiValues.Length - 1];
				float ratio = (phi - databaseParams.getMinPhi()) / (databaseParams.getMaxPhi() - databaseParams.getMinPhi());
				phi = ratio * (max - min) + min;
			}

			phi = FindNearest(phi, phiValues);
			if (!phi.ToString().Equals(phiVar.GetSelectedValue())) {
				phiVar.SetSelectedValue(phi.ToString());
			}
		}

		//Calculate theta and set shouldUpdate to true if it has changed
		if (thetaVar != null) {
			float theta = transform.localRotation.eulerAngles.x;
			theta = theta <= 90 ? theta + 90 : theta - 270; //Corrects rotation

			//Adjust theta according to alignment, invert and rescaling values
			if (databaseParams.getInvertTheta()) {
				theta = 180 - theta;
			}
			theta = (theta + databaseParams.getThetaAlignment()) % 360;

			if (databaseParams.getThetaRescaled()) {
				float min = thetaValues[0];
				float max = thetaValues[thetaValues.Length - 1];
				float ratio = (theta - databaseParams.getMinTheta()) / (databaseParams.getMaxTheta() - databaseParams.getMinTheta());
				theta = ratio * (max - min) + min;
			}
				
			theta = FindNearest(theta, thetaValues);
			if (!theta.ToString().Equals(thetaVar.GetSelectedValue())) {
				thetaVar.SetSelectedValue(theta.ToString());
			}
		}

		if (loaded)
			UpdateTexture();

		float megabytes = cache.getMemorySize() / (1000000.0f);
		cacheText.text = "Cache size: " + megabytes.ToString("F2") + " MB\n(" + 
			cache.getSize() + "/" + Cache.maxItems + ") Images stored";
	}

	/*
	 * Updates the texture of the sprite according the selected variables
	 */
	public void UpdateTexture() {
		try {
			//Iterate through arguments and put values into name_pattern
			string imagePath = String.Copy(name_pattern);
			foreach(CinemaArgument arg in arguments) {
				imagePath = imagePath.Replace("{" + arg.GetLabel() + "}", arg.GetSelectedValue());
			}

			//Fetches the sprite from the cache, or loads it if it isn't in there
			Sprite newSprite = cache.fetchSprite(imagePath);
			if (newSprite == null) {
				Texture2D tex = loadTexture(databaseParams.getDirectory() + Path.DirectorySeparatorChar + imagePath);
				cache.addItem(spriteFromTexture(tex),imagePath,tex.GetRawTextureData().Length);
				newSprite = cache.fetchSprite(imagePath);
			}
			GetComponent<SpriteRenderer>().sprite = newSprite;
			panelMessage.text = "Currently showing:\n" + imagePath;

		} catch (Exception e) {
			panelMessage.text = e.GetType().ToString() + "\n" + e.Message;
			GetComponent<SpriteRenderer>().sprite = errorReadingFileSprite;
		}
	}

	//Called whenever the orbitToggle's value changes.
	//Sets isOrbiting and the orbitingPoint on PlayerControls
	public void orbitToggled(bool toggle) {
		if (toggle) {
			PlayerControls.isOrbiting = true;
			PlayerControls.orbitingPoint = transform.position;
		}
		else {
			PlayerControls.isOrbiting = false;
		}
	}

	public DatabaseParams getParams() {
		return databaseParams;
	}

	public void setParams(DatabaseParams newParams) {
		databaseParams = newParams;
	}

	//Create and return a new Sprite from the given texture
	private Sprite spriteFromTexture(Texture2D tex) {
		return Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(0.5f,0.5f),databaseParams.getPixelScale());
	}

	//Load a texture from the given path and return it
	private Texture2D loadTexture(string path) {
		byte[] fileData;
		if (!databaseParams.getOnline()) {
			fileData = File.ReadAllBytes(path);
		}
		else {
			WebClient client = new WebClient();
			fileData = client.DownloadData(path);
		}
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(fileData);
		return tex;
	}

	//Return the value in arr that test is closest to (assumes arr is sorted numerically (ascending))
	private float FindNearest(float test, float[] arr) {
		//Return first value if less than or equal to it
		if (test <= arr[0]) {
			return arr[0];
		}
		//Return last value if less than or equal to it
		else if (test >= arr[arr.Length-1]) {
			return arr[arr.Length - 1];
		}
		//Iterate through values to find two that test lies between
		else {
			for (int i = 0; i < arr.Length-1; i++) {
				if (test >= arr[i] && test <= arr[i+1]) {
					//Return whichever value test is closet to
					if (Mathf.Abs(test-arr[i]) <= Mathf.Abs(test-arr[i+1])) {
						return arr[i];
					}
					else {
						return arr[i+1];
					}
				}
			}
			return -1.0f; //This may be returned if the list is not sorted right
		}
	}
}
