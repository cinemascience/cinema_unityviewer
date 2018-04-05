using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class displayFrames : MonoBehaviour {
	
	// displays the value of the frames slider
	public  Text t;
	public Image fill;
	public Image bg;

	// Update is called once per frame
	void Update()
	{
		if(!sliderValue.interactable) {
			t.fontStyle = FontStyle.BoldAndItalic;
			t.text = "n/a";
			t.color = Color.gray;
			fill.color = Color.gray;
			bg.color = Color.gray;
			return;
		}
		t.fontStyle = FontStyle.Bold;
		t.color = Color.white;
		fill.color = Color.white;
		bg.color = Color.white;

		if(sliderValue.animated)
			t.text = sliderValue.frames + " Frames";
		else
			t.text = "Not animated";	
		

			


		

	}
}
