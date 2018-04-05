using UnityEngine;
using System;

/**
 * Represents a single varialble or "argument" in a Cinema database
 */
public class CinemaArgument {

	private string label;
	private string[] values;
	private string defaultValue;
	private int selectedIndex = 0; //Index of the currently selected value in values

	/**
	 * Create a new CinemaVariable from a JSONObject
	 */
	public CinemaArgument(JSONObject arg) {
		if(arg.HasField("label"))
			label = Parse(arg["label"]);
		else
			throw new Exception("Error reading JSON: Couldn't find field 'label' in argument");
		if(arg.HasField("default"))
			defaultValue = Parse(arg["default"]);
		else
			throw new Exception("Error reading JSON: Couldn't find field 'default' in argument");
		
		//Read in values and set selectedIndex if a value is found with the default
		if(arg.HasField("values"))
			values = new string[arg["values"].Count];
		else
			throw new Exception("Error reading JSON: Couldn't find field 'values' in argument");
		
		for (int i = 0; i < values.Length; i++) {
			values[i] = Parse(arg["values"][i]);
			if (values[i].Equals(defaultValue)) {
				selectedIndex = i;
			}
		}
	}

	public string GetLabel() {
		return label;
	}

	public string GetSelectedValue() {
		return values[selectedIndex];
	}

	//Set the selected value to the value given
	//Nothing will happen if the given value is not among values
	public void SetSelectedValue(string value) {
		for (int i = 0; i < values.Length; i++) {
			if (values[i].Equals(value)) {
				selectedIndex = i;
			}
		}
	}

	public int GetSelectedIndex() {
		return selectedIndex;
	}

	public void SetSelectedIndex(int index) {
		selectedIndex = index;
	}

	public string[] GetValues() {
		return values;
	}
		
	//JSONObject returns strings with the quotes still around them. This function trims them off
	public static string TrimQuotes(string input) {
		return input.Substring(1, input.Length - 2);
	}

	//Return a string of the value of a JSONObject, trimming off quotes if necesary.
	private string Parse(JSONObject input) {
		if (input.IsString) {
			return TrimQuotes(input.ToString());
		}
		else {
			return input.ToString();
		}
	}
}


