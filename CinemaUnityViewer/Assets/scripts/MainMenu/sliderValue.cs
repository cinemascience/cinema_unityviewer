using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// a script to manage the value of the slider and to disable animation if the slider is set to the lowest value
public class sliderValue : MonoBehaviour {
	public static int frames;
	public static bool interactable;
	public static bool animated;
	// Use this for initialization
	void Start () 
	{
		frames = (int) GetComponent<Slider>().value;
		if(frames == 14)
			animated = false;
		else 
			animated = true;
		interactable = true;
	}
	void Update()
	{
		if(!(bool)GetComponent<Slider>().IsInteractable())
			interactable = false;
		else
			interactable = true;
	}
	public void onChanged()
	{
		if((bool)GetComponent<Slider>().IsInteractable()) {
			frames = (int)GetComponent<Slider>().value;
			interactable = true;
			if(frames == 14)
				animated = false;
			else
				animated = true;
		} else {
			interactable = false;
		}

	}
}
