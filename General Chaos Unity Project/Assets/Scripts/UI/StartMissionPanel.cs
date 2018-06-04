using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class StartMissionPanel : MonoBehaviour {

	public enum LevelDifficulty {
		LEVEL_DIFFICULTY_NONE = 0,
		LEVEL_DIFFICULTY_EASY,
		LEVEL_DIFFICULTY_MEDIUM,
		LEVEL_DIFFICULTY_HARD
	};
	public GameObject selectedUnitButtonPrefab;

	private List<GameObject> selectedUnitButtonList;
	private List<GameObject> levelScreenshotsList;
	private LevelDifficulty currentDisplayedDifficulty = LevelDifficulty.LEVEL_DIFFICULTY_NONE;
	private int currentDisplayedTeam = 0;
	private int currentDisplayedLevel = 0;
	private GameObject easyCheckboxOn;
    private GameObject mediumCheckboxOn;
    private GameObject hardCheckboxOn;
    private GameObject easyCheckboxOff;
    private GameObject mediumCheckboxOff;
    private GameObject hardCheckboxOff;

	private TacticalCombat tcScript;

	private bool isInitialized = false;
	void InitializeIfNeeded () {
		if (this.tcScript == null) {
			Camera mainCamera = Camera.main;
			this.tcScript = mainCamera.GetComponent<TacticalCombat> ();
		}

		if (!isInitialized) {
			isInitialized = true;

			// Create 5 buttons
			this.selectedUnitButtonList = new List<GameObject> ();
			for (int i = 0; i < 5; i++) {
				GameObject selectedUnitButton = (GameObject)Instantiate (this.selectedUnitButtonPrefab, Vector3.zero, Quaternion.identity);
				selectedUnitButton.transform.parent = this.transform;
				RectTransform rectTransform = selectedUnitButton.GetComponent<RectTransform> ();
				rectTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, 528 + 50 * i, 387.0f);
				rectTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, 1181/*337*/, 454.0f);
                //selectedUnitButton.transform.position = new Vector3 (-738 + 360 * i, -143, 0);
                float scaleMultiplier = 0.09f;
                selectedUnitButton.transform.localScale = new Vector3 (scaleMultiplier, scaleMultiplier, scaleMultiplier);

				Button selectedUnitButtonScript = selectedUnitButton.GetComponent<Button> ();
				selectedUnitButtonScript.onClick.AddListener (delegate {
					SelectedUnitButton_Tapped (selectedUnitButton);
				});

				this.selectedUnitButtonList.Add (selectedUnitButton);
			}
				

			SetupLevelScreenshotsListIfNeeded ();
		}

		if (this.easyCheckboxOn == null) {
			easyCheckboxOn = transform.Find ("EasyCheckboxOn").gameObject;
			mediumCheckboxOn = transform.Find ("MediumCheckboxOn").gameObject;
            hardCheckboxOn = transform.Find ("HardCheckboxOn").gameObject;
            easyCheckboxOff = transform.Find("EasyCheckboxOff").gameObject;
            mediumCheckboxOff = transform.Find("MediumCheckboxOff").gameObject;
            hardCheckboxOff = transform.Find("HardCheckboxOff").gameObject;
		}
	}

	// Use this for initialization
	void Start () {
		InitializeIfNeeded ();
	}

	void SelectedUnitButton_Tapped (GameObject selectedUnitButton) {
		tcScript.ShowTeamPanel ();
	}

	void SetupLevelScreenshotsListIfNeeded () {
		if (this.levelScreenshotsList == null) {
			// Populate screenshots list
			this.levelScreenshotsList = new List<GameObject> ();
			for (int i = 0; i < 12; i++) {
				GameObject levelScreenshotGameObject = transform.Find ("screenshot-level-" + (i + 1)).gameObject;
				levelScreenshotsList.Add (levelScreenshotGameObject);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		// Change team display if it has been updated
		if (tcScript.selectedTeam != this.currentDisplayedTeam) {
			SwitchToTeam (tcScript.selectedTeam);
		}
		// Change difficulty display if it has been updated
		if (tcScript.selectedDifficulty != this.currentDisplayedDifficulty) {
			SwitchToDifficulty (tcScript.selectedDifficulty);
		}
		// Change level screenshot display if it has been updated
		if (tcScript.currentLevel != this.currentDisplayedLevel) {
			SwitchToLevel (tcScript.currentLevel);
		}
	}

	void SwitchToDifficulty (LevelDifficulty difficulty) {
		InitializeIfNeeded ();

		tcScript.selectedDifficulty = difficulty;
		this.currentDisplayedDifficulty = difficulty;

        // Hide all checkmarks
        easyCheckboxOn.SetActive(false);
        mediumCheckboxOn.SetActive(false);
        hardCheckboxOn.SetActive(false);
        easyCheckboxOff.SetActive(true);
        mediumCheckboxOff.SetActive(true);
        hardCheckboxOff.SetActive(true);

		// Show appropriate checkmark
		switch (difficulty) {
		    case LevelDifficulty.LEVEL_DIFFICULTY_EASY:
                easyCheckboxOn.SetActive(true);
                easyCheckboxOff.SetActive(false);
			    break;
            case LevelDifficulty.LEVEL_DIFFICULTY_MEDIUM:
                mediumCheckboxOn.SetActive(true);
                mediumCheckboxOff.SetActive(false);
			    break;
            case LevelDifficulty.LEVEL_DIFFICULTY_HARD:
                hardCheckboxOn.SetActive(true);
                hardCheckboxOff.SetActive(false);
			    break;
		}
	}

	void SwitchToTeam (int team) {
		InitializeIfNeeded ();

		this.currentDisplayedTeam = team;

		// Set buttons with units in selected team
		int currentButtonIndex = 0;
		foreach (UnitDataObject unitDataObject in tcScript.unitDataObjectsList) {
			if (unitDataObject.assignedTeam == team && currentButtonIndex < selectedUnitButtonList.Count) {
				GameObject buttonGO = selectedUnitButtonList [currentButtonIndex];
				SelectedUnitButton button = buttonGO.GetComponent<SelectedUnitButton> ();
				button.SetUnitDataObject (unitDataObject);
				button.HideCloseButton ();
				currentButtonIndex++;
			}
		}

		// Clear buttons without a team
		for (int i = currentButtonIndex; i < selectedUnitButtonList.Count; i++) {
			GameObject buttonGO = selectedUnitButtonList [currentButtonIndex];
			SelectedUnitButton button = buttonGO.GetComponent<SelectedUnitButton> ();
			button.ClearButton ();
			currentButtonIndex++;
		}
	}

	public void SwitchToLevel(int level) {
		InitializeIfNeeded ();

		this.tcScript.currentLevel = level;
		//SetupLevelScreenshotsListIfNeeded ();
		// TODO: Something here causes a crash - should not need above methods
		this.currentDisplayedLevel = level;

		foreach (GameObject levelScreenshot in this.levelScreenshotsList) {
			levelScreenshot.SetActive (false);
		}
		GameObject selectedLevelScreenshot = this.levelScreenshotsList[level - 1];
		selectedLevelScreenshot.SetActive (true);
	}

	public void EasyButton_Tapped () {
		this.tcScript.PlayUIAudioClip ();
		SwitchToDifficulty (LevelDifficulty.LEVEL_DIFFICULTY_EASY);
	}

	public void MediumButton_Tapped () {
		this.tcScript.PlayUIAudioClip ();
		SwitchToDifficulty (LevelDifficulty.LEVEL_DIFFICULTY_MEDIUM);
	}

	public void HardButton_Tapped () {
		this.tcScript.PlayUIAudioClip ();
		SwitchToDifficulty (LevelDifficulty.LEVEL_DIFFICULTY_HARD);
	}

	public void StartMissionButton_Tapped () {
        //this.tcScript.PlayUIAudioClip ();
        //this.tcScript.ResetTacticalCombat ();
        //this.tcScript.HideAllPanelsExcept (TacticalCombat.UIPanelType.UI_PANEL_TYPE_NONE);
        this.tcScript.PlayUIAudioClip();
        this.tcScript.HideAllPanelsExcept(TacticalCombat.UIPanelType.UI_PANEL_TYPE_MISSIONOBJECTIVES);
        this.tcScript.ResetTacticalCombat();
        this.tcScript.PauseTacticalCombat();
        this.tcScript.ShowMissionObjectivesPanel();
	}

	public void CloseButton_Tapped () {
		this.tcScript.PlayUIAudioClip ();
		HidePanel ();
	}

	public void ShowPanel () {
		InitializeIfNeeded ();

		// TODO: Something here causes a crash - should not need above method
		this.gameObject.SetActive (true);
		this.tcScript.isMenuOpen = true;
	}

    public void HidePanel () {
        InitializeIfNeeded();

		this.gameObject.SetActive (false);
		this.tcScript.isStartMissionPanelQueued = false;
	}
}
