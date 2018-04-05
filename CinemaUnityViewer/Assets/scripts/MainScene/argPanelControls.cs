using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/**
 * Attached to the Canvas object
 * Represents the controls for a single argument in a database
 */
public class argPanelControls : MonoBehaviour {

	//Sprites for play/pause button
	public Sprite playSprite, pauseSprite;

	//The speed at which the argument plays when 'play' is pressed in seconds between changes
	public float playSpeed;

	//The CinemaArgument being controlled
	private CinemaArgument arg;
	//The CinemaDatabase this argument belongs to
	private CinemaDatabase database;

	//Components
	private Slider slider;
	private Text valueText, labelText;
	private bool isPlaying;
	private Button playButton;

	//Used to track time passed for playing
	private float timer;

	//Locates and initializes components
	public void Init(CinemaDatabase db) {
		database = db;
		slider = GetComponentInChildren<Slider>();
		Text[] texts = GetComponentsInChildren<Text>();
		foreach (Text text in texts) {
			if(text.CompareTag("valueText")) {
				valueText = text;
			}
			else if(text.CompareTag("labelText")) {
				labelText = text;
			}
		}
		Button[] buttons = GetComponentsInChildren<Button>();
		foreach (Button button in buttons) {
			if (button.CompareTag("nextButton")) {
				button.onClick.AddListener(Next);
			}
			else if (button.CompareTag("prevButton")) {
				button.onClick.AddListener(Prev);
			}
			else if (button.CompareTag("firstButton")) {
				button.onClick.AddListener(First);
			}
			else if (button.CompareTag("lastButton")) {
				button.onClick.AddListener(Last);
			}
			else if (button.CompareTag("playButton")) {
				button.onClick.AddListener(Play);
				playButton = button;
				playButton.image.sprite = playSprite;
			}
		}
	}

	// Update is called once per frame
	// Advances variable if enough time has passed while playing
	// Updates arg and database if the slider has moved
	void Update () {
		if (isPlaying) {
			timer += Time.deltaTime;
			if (timer > playSpeed) {
				if (slider.value == slider.maxValue) {
					First();
				}
				else {
					Next();
				}
				timer = 0;
			}
		}
		if (arg != null) {
			if (slider.value != arg.GetSelectedIndex()) {
				arg.SetSelectedIndex((int)slider.value);
				valueText.text = arg.GetSelectedValue();
				database.UpdateTexture();
			}
		}
	}

	public void SetCinemaVariable(CinemaArgument newVar) {
		arg = newVar;
		slider.maxValue = arg.GetValues().Length-1;
		labelText.text = arg.GetLabel();
		valueText.text = arg.GetSelectedValue();
	}

	//Functions for each button. Called when their respective button is pressed

	public void Next() {
		if (slider.value != slider.maxValue) {
			slider.value = slider.value + 1;
		}
	}

	public void Prev() {
		if (slider.value != slider.minValue) {
			slider.value = slider.value - 1;
		}
	}

	public void First() {
		slider.value = slider.minValue;
	}

	public void Last() {
		slider.value = slider.maxValue;
	}

	public void Play() {
		if (isPlaying) {
			isPlaying = false;
			playButton.image.sprite = playSprite;
		}
		else {
			isPlaying = true;
			playButton.image.sprite = pauseSprite;
		}
	}
}
