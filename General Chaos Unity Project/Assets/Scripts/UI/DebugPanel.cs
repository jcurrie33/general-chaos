using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class DebugPanel : MonoBehaviour {


	private TacticalCombat tcScript;

	private GameObject debugHealthText;
	private GameObject debugAttackText;
	private GameObject debugFirerateText;
	private GameObject debugProjectileSpeedText;
	private GameObject debugRangeText;
	private GameObject debugSpeedText;
	private GameObject debugMeleeText;

	private GameObject debugHealthValue;
	private GameObject debugAttackValue;
	private GameObject debugFirerateValue;
	private GameObject debugProjectileSpeedValue;
	private GameObject debugRangeValue;
	private GameObject debugSpeedValue;
	private GameObject debugMeleeValue;

//	private GameObject debugHealthSlider;
//	private GameObject debugAttackSlider;
//	private GameObject debugFirerateSlider;
//	private GameObject debugProjectileSpeedSlider;
//	private GameObject debugRangeSlider;
//	private GameObject debugSpeedSlider;
//	private GameObject debugMeleeSlider;

	private GameObject debugEnableAIToggle;
	private GameObject debugResetButton;


	// The Awake function is called on all objects in the scene before any object's Start function is called.
	void Awake () {

		// Store UI elements for later access

		Camera mainCamera = Camera.main;
		this.tcScript = mainCamera.GetComponent<TacticalCombat> ();

		debugHealthText = GameObject.Find ("HealthText");
		debugAttackText = GameObject.Find ("AttackText");
		debugFirerateText = GameObject.Find ("FirerateText");
		debugProjectileSpeedText = GameObject.Find ("ProjectileSpeedText");
		debugRangeText = GameObject.Find ("RangeText");
		debugSpeedText = GameObject.Find ("SpeedText");
		debugMeleeText = GameObject.Find ("MeleeText");

		debugHealthValue = GameObject.Find ("HealthValue");
		debugAttackValue = GameObject.Find ("AttackValue");
		debugFirerateValue = GameObject.Find ("FirerateValue");
		debugProjectileSpeedValue = GameObject.Find ("ProjectileSpeedValue");
		debugRangeValue = GameObject.Find ("RangeValue");
		debugSpeedValue = GameObject.Find ("SpeedValue");
		debugMeleeValue = GameObject.Find ("MeleeValue");

		//		debugHealthSlider = GameObject.Find ("HealthSlider");
		//		debugAttackSlider = GameObject.Find ("AttackSlider");
		//		debugFirerateSlider = GameObject.Find ("FirerateSlider");
		//		debugProjectileSpeedSlider = GameObject.Find ("ProjectileSpeedSlider");
		//		debugRangeSlider = GameObject.Find ("RangeSlider");
		//		debugSpeedSlider = GameObject.Find ("SpeedSlider");
		//		debugMeleeSlider = GameObject.Find ("MeleeSlider");

		debugEnableAIToggle = GameObject.Find ("EnableAIToggle");
		debugResetButton = GameObject.Find ("ResetButton");

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DebugPanel_MapButton_Tapped () {
//		debugPanel.SetActive (false);
//
//		PauseTacticalCombat ();
//
//		foreach (Unit unit in unitsList) {
//			unit.StopMovingOrAttacking ();
//		}
//
//		StartCoroutine (ShowLevelSelectionPanelAfterDelay (0));
	}

	public void UpdateDebugUIForUnit (Unit unit) {

		Text healthValue = debugHealthValue.GetComponent<Text> ();
		healthValue.text = "" + unit.BaseHealth ();
//		Slider healthSlider = debugHealthSlider.GetComponent<Slider> ();
//		healthSlider.value = unit.Health ();

		Text attackValue = debugAttackValue.GetComponent<Text> ();
		attackValue.text = "" + unit.BaseAttack ();
//		Slider attackSlider = debugAttackSlider.GetComponent<Slider> ();
//		attackSlider.value = unit.Attack ();

		Text firerateValue = debugFirerateValue.GetComponent<Text> ();
		firerateValue.text = "" + unit.BaseFirerate ();
//		Slider firerateSlider = debugFirerateSlider.GetComponent<Slider> ();
//		firerateSlider.value = unit.Firerate ();

		Text projectileSpeedValue = debugProjectileSpeedValue.GetComponent<Text> ();
		projectileSpeedValue.text = "" + unit.BaseProjectileSpeed ();
//		Slider projectileSpeedSlider = debugProjectileSpeedSlider.GetComponent<Slider> ();
//		projectileSpeedSlider.value = unit.ProjectileSpeed ();

		Text rangeValue = debugRangeValue.GetComponent<Text> ();
		rangeValue.text = "" + unit.BaseRange ();
//		Slider rangeSlider = debugRangeSlider.GetComponent<Slider> ();
//		rangeSlider.value = unit.Range ();

		Text speedValue = debugSpeedValue.GetComponent<Text> ();
		speedValue.text = "" + unit.BaseSpeed ();
//		Slider speedSlider = debugSpeedSlider.GetComponent<Slider> ();
//		speedSlider.value = unit.Speed ();

		Text meleeValue = debugMeleeValue.GetComponent<Text> ();
		meleeValue.text = "" + unit.BaseMelee ();
//		Slider meleeSlider = debugMeleeSlider.GetComponent<Slider> ();
//		meleeSlider.value = unit.Melee ();
	}

	public void EnableAIToggleValueChanged (Boolean selected) {
		Utilities.DebugLog ("EnableAIToggleValueChanged ()");
		Toggle enableAIToggle = GameObject.Find ("EnableAIToggle").GetComponent<Toggle> ();
		tcScript.isAIEnabled = enableAIToggle.isOn;
	}

	public void EnableDebugUIToggleValueChanged (Boolean selected) {
		Utilities.DebugLog ("EnableDebugUIToggleValueChanged ()");
		Toggle enableDebugToggle = GameObject.Find ("EnableDebugToggle").GetComponent<Toggle> ();
		selected = enableDebugToggle.isOn;

		this.gameObject.SetActive (selected);
	}
		


	public void UpdateUnitHealthToValue (float newValue, Boolean isRelative) {
		Text healthValue = debugHealthValue.GetComponent<Text> ();
		foreach (Unit unit in tcScript.unitsList) {
			if (unit.isSelected) {
				if (isRelative) {
					newValue = unit.BaseHealth () + newValue;
				}
				unit.Health (newValue);
				healthValue.text = "" + unit.BaseHealth ();
			}
		}
	}

	public void UpdateUnitAttackToValue (float newValue, Boolean isRelative) {
		Text attackValue = debugAttackValue.GetComponent<Text> ();
		foreach (Unit unit in tcScript.unitsList) {
			if (unit.isSelected) {
				if (isRelative) {
					newValue = unit.BaseAttack () + newValue;
				}
				unit.Attack (newValue);
				attackValue.text = "" + unit.BaseAttack ();
			}
		}
	}

	public void UpdateUnitFirerateToValue (float newValue, Boolean isRelative) {
		Text firerateValue = debugFirerateValue.GetComponent<Text> ();
		foreach (Unit unit in tcScript.unitsList) {
			if (unit.isSelected) {
				if (isRelative) {
					newValue = unit.BaseFirerate () + newValue;
				}
				unit.Firerate (newValue);
				firerateValue.text = "" + unit.BaseFirerate ();
			}
		}
	}

	public void UpdateUnitProjectileSpeedToValue (float newValue, Boolean isRelative) {
		Text projectileSpeedValue = debugProjectileSpeedValue.GetComponent<Text> ();
		foreach (Unit unit in tcScript.unitsList) {
			if (unit.isSelected) {
				if (isRelative) {
					newValue = unit.BaseProjectileSpeed () + newValue;
				}
				unit.ProjectileSpeed (newValue);
				projectileSpeedValue.text = "" + unit.BaseProjectileSpeed ();
			}
		}
	}

	public void UpdateUnitRangeToValue (float newValue, Boolean isRelative) {
		Text rangeValue = debugRangeValue.GetComponent<Text> ();
		foreach (Unit unit in tcScript.unitsList) {
			if (unit.isSelected) {
				if (isRelative) {
					newValue = unit.BaseRange () + newValue;
				}
				unit.Range (newValue);
				rangeValue.text = "" + unit.BaseRange ();
			}
		}
	}

	public void UpdateUnitSpeedToValue (float newValue, Boolean isRelative) {
		Text speedValue = debugSpeedValue.GetComponent<Text> ();
		foreach (Unit unit in tcScript.unitsList) {
			if (unit.isSelected) {
				if (isRelative) {
					newValue = unit.BaseSpeed () + newValue;
				}
				unit.Speed (newValue);
				speedValue.text = "" + unit.BaseSpeed ();
			}
		}
	}

	public void UpdateUnitMeleeToValue (float newValue, Boolean isRelative) {
		Text meleeValue = debugMeleeValue.GetComponent<Text> ();
		foreach (Unit unit in tcScript.unitsList) {
			if (unit.isSelected) {
				if (isRelative) {
					newValue = unit.BaseMelee () + newValue;
				}
				unit.Melee (newValue);
				meleeValue.text = "" + unit.BaseMelee ();
			}
		}
	}



	public void HealthButton_Plus_OnClick () {
		UpdateUnitHealthToValue (1, true);
	}
	public void HealthButton_Minus_OnClick () {
		UpdateUnitHealthToValue (-1, true);
	}
	public void AttackButton_Plus_OnClick () {
		UpdateUnitAttackToValue (1, true);
	}
	public void AttackButton_Minus_OnClick () {
		UpdateUnitAttackToValue (-1, true);
	}
	public void FirerateButton_Plus_OnClick () {
		UpdateUnitFirerateToValue (1, true);
	}
	public void FirerateButton_Minus_OnClick () {
		UpdateUnitFirerateToValue (-1, true);
	}
	public void ProjectileSpeedButton_Plus_OnClick () {
		UpdateUnitProjectileSpeedToValue (1, true);
	}
	public void ProjectileSpeedButton_Minus_OnClick () {
		UpdateUnitProjectileSpeedToValue (-1, true);
	}
	public void RangeButton_Plus_OnClick () {
		UpdateUnitRangeToValue (1, true);
	}
	public void RangeButton_Minus_OnClick () {
		UpdateUnitRangeToValue (-1, true);
	}
	public void SpeedButton_Plus_OnClick () {
		UpdateUnitSpeedToValue (0.1f, true);
	}
	public void SpeedButton_Minus_OnClick () {
		UpdateUnitSpeedToValue (-0.1f, true);
	}
	public void MeleeButton_Plus_OnClick () {
		UpdateUnitMeleeToValue (1, true);
	}
	public void MeleeButton_Minus_OnClick () {
		UpdateUnitMeleeToValue (-1, true);
	}



//	public void OnHealthSliderValueChanged () {
//		Utilities.DebugLog ("OnHealthSliderValueChanged ()");
//		Slider healthSlider = debugHealthSlider.GetComponent<Slider> ();
//		UpdateUnitHealthToValue (healthSlider.value);
//	}
//
//	public void OnAttackSliderValueChanged () {
//		Utilities.DebugLog ("OnAttackSliderValueChanged ()");
//		Slider attackSlider = debugAttackSlider.GetComponent<Slider> ();
//		UpdateUnitAttackToValue (attackSlider.value);
//	}
//
//	public void OnFirerateSliderValueChanged () {
//		Utilities.DebugLog ("OnFirerateSliderValueChanged ()");
//		Slider firerateSlider = debugFirerateSlider.GetComponent<Slider> ();
//		UpdateUnitFirerateToValue (firerateSlider.value);
//	}
//
//	public void OnProjectileSpeedSliderValueChanged () {
//		Utilities.DebugLog ("OnProjectileSpeedSliderValueChanged ()");
//		Slider projectileSpeedSlider = debugProjectileSpeedSlider.GetComponent<Slider> ();
//		UpdateUnitProjectileSpeedToValue (projectileSpeedSlider.value);
//	}
//
//	public void OnRangeSliderValueChanged () {
//		Utilities.DebugLog ("OnRangeSliderValueChanged ()");
//		Slider rangeSlider = debugRangeSlider.GetComponent<Slider> ();
//		UpdateUnitRangeToValue (rangeSlider.value);
//	}
//
//	public void OnSpeedSliderValueChanged () {
//		Utilities.DebugLog ("OnSpeedSliderValueChanged ()");
//		Slider speedSlider = debugSpeedSlider.GetComponent<Slider> ();
//		UpdateUnitSpeedToValue (speedSlider.value);
//	}
//
//	public void OnMeleeSliderValueChanged () {
//		Utilities.DebugLog ("OnMeleeSliderValueChanged ()");
//		Slider meleeSlider = debugMeleeSlider.GetComponent<Slider> ();
//		UpdateUnitMeleeToValue (meleeSlider.value);
//	}
}
