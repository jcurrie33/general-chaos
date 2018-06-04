using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectionPanel : MonoBehaviour {
	
	public List<GameObject> levelScreenshotsList;


	private List<GameObject> levelButtonsList;
	private List<GameObject> levelLocksList;

	private GameObject explosion1;
	private GameObject explosion2;

	private TacticalCombat tcScript;

	private bool isInitialized = false;
	void InitializeIfNeeded () {
		if (this.tcScript == null) {
			Camera mainCamera = Camera.main;
			this.tcScript = mainCamera.GetComponent<TacticalCombat> ();
		}

		if (explosion1 == null) {
			explosion1 = transform.Find ("ui-map-explosion-1").gameObject;
			explosion2 = transform.Find ("ui-map-explosion-2").gameObject;
		}

		if (!isInitialized) {
			isInitialized = true;

			int levelCount = 1;
			levelButtonsList = new List<GameObject> ();
			levelLocksList = new List<GameObject> ();
			foreach (GameObject level in this.tcScript.levelsList) {
				GameObject levelButtonGameObject = transform.Find ("Level" + levelCount + "Button").gameObject;
				levelButtonsList.Add (levelButtonGameObject);
				GameObject levelLockGameObject = transform.Find ("Level" + levelCount + "Button/Lock").gameObject;
				levelLocksList.Add (levelLockGameObject);
				levelCount++;
			}
		}
	}

	// Use this for initialization
	void Start () {
		InitializeIfNeeded ();
	}
		
	// The Awake function is called on all objects in the scene before any object's Start function is called.
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void ShowPanel () {
		InitializeIfNeeded ();

		// Enable/Disable buttons according to unlock level
		int levelCount = 1;
		foreach (GameObject levelButton in levelButtonsList) {
			if (levelCount > this.tcScript.maxUnlockLevel) {
				Button button = levelButton.GetComponent<Button> ();
				if (button) {
					button.interactable = false;
				}
			} else {
				Button button = levelButton.GetComponent<Button> ();
				if (button) {
					button.interactable = true;
				}
			}

			if (levelCount == this.tcScript.maxUnlockLevel) {
				explosion1.transform.position = new Vector3 (
					levelButton.transform.position.x - 8, 
					levelButton.transform.position.y - 21, 
					levelButton.transform.position.z);
				explosion2.transform.position = new Vector3 (
					levelButton.transform.position.x + 14, 
					levelButton.transform.position.y - 15, 
					levelButton.transform.position.z);
			}

			levelCount++;
		}

		// Show/Hide locks according to unlock level
		levelCount = 1;
		Debug.Log ("maxUnlockLevel = " + this.tcScript.maxUnlockLevel);
		foreach (GameObject levelLock in levelLocksList) {
			if (levelCount > this.tcScript.maxUnlockLevel) {
				levelLock.SetActive (true);
			} else {
				levelLock.SetActive (false);
			}
			levelCount++;
		}

		// Hide the level screenshots
		HideAllLevelScreenshots ();

		this.gameObject.SetActive (true);
		this.tcScript.isMenuOpen = true;
	}


	// Methods called when touch held on level button, to show level details (screenshot)
	public void LevelSelectionPanel_Level1Button_PointerDown ()		{		ShowDetailsForLevel (1);	}
	public void LevelSelectionPanel_Level2Button_PointerDown ()		{		ShowDetailsForLevel (2);	}
	public void LevelSelectionPanel_Level3Button_PointerDown ()		{		ShowDetailsForLevel (3);	}
	public void LevelSelectionPanel_Level4Button_PointerDown ()		{		ShowDetailsForLevel (4);	}
	public void LevelSelectionPanel_Level5Button_PointerDown ()		{		ShowDetailsForLevel (5);	}
	public void LevelSelectionPanel_Level6Button_PointerDown ()		{		ShowDetailsForLevel (6);	}
	public void LevelSelectionPanel_Level7Button_PointerDown ()		{		ShowDetailsForLevel (7);	}
	public void LevelSelectionPanel_Level8Button_PointerDown ()		{		ShowDetailsForLevel (8);	}
	public void LevelSelectionPanel_Level9Button_PointerDown ()		{		ShowDetailsForLevel (9);	}
	public void LevelSelectionPanel_Level10Button_PointerDown ()	{		ShowDetailsForLevel (10);	}
	public void LevelSelectionPanel_Level11Button_PointerDown ()	{		ShowDetailsForLevel (11);	}
	public void LevelSelectionPanel_Level12Button_PointerDown ()	{		ShowDetailsForLevel (12);	}


	// Methods called when level buttons are tapped, to load the appropriate level
	public void LevelSelectionPanel_Level1Button_Tapped () 	{		StartLevel (1);		}
	public void LevelSelectionPanel_Level2Button_Tapped () 	{		StartLevel (2);		}
	public void LevelSelectionPanel_Level3Button_Tapped () 	{		StartLevel (3);		}
	public void LevelSelectionPanel_Level4Button_Tapped () 	{		StartLevel (4);		}
	public void LevelSelectionPanel_Level5Button_Tapped () 	{		StartLevel (5);		}
	public void LevelSelectionPanel_Level6Button_Tapped () 	{		StartLevel (6);		}
	public void LevelSelectionPanel_Level7Button_Tapped () 	{		StartLevel (7);		}
	public void LevelSelectionPanel_Level8Button_Tapped () 	{		StartLevel (8);		}
	public void LevelSelectionPanel_Level9Button_Tapped () 	{		StartLevel (9);		}
	public void LevelSelectionPanel_Level10Button_Tapped () {		StartLevel (10);	}
	public void LevelSelectionPanel_Level11Button_Tapped ()	{		StartLevel (11);	}
	public void LevelSelectionPanel_Level12Button_Tapped () {		StartLevel (12);	}

	public void StartLevel (int level) {
		InitializeIfNeeded ();

		this.tcScript.PlayUIAudioClip ();

//		this.tcScript.isMenuOpen = false;

		this.tcScript.currentLevel = level;

		//this.tcScript.ResetTacticalCombat ();
		StartMissionPanel startMissionPanelScript = this.tcScript.startMissionPanel.GetComponent<StartMissionPanel> ();
		startMissionPanelScript.SwitchToLevel (this.tcScript.currentLevel);
		this.tcScript.ShowStartMissionPanel ();
	}

	public void HidePanel () {
		InitializeIfNeeded ();

		HideAllLevelScreenshots ();

		this.gameObject.SetActive (false);

		this.tcScript.isLevelSelectionPanelQueued = false;
	}

	public void ShowDetailsForLevel (int level) {
		return;
		//HideAllLevelScreenshots ();
		//Utilities.Show (levelScreenshotsList [level-1]);
	}

	public void HideAllLevelScreenshots () {
		foreach (GameObject levelScreenshot in levelScreenshotsList) {
			Utilities.Hide (levelScreenshot);
		}
	}
}
