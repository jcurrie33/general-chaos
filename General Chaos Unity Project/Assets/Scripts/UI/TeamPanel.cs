using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class TeamPanel : MonoBehaviour {

	public GameObject selectedUnitButtonPrefab;
	public List<GameObject> selectedUnitButtonList;

	private int currentDisplayedTeam = 0;
	private GameObject team1Tab;
	private GameObject team2Tab;
	private GameObject team3Tab;

	private TacticalCombat tcScript;

	private bool isInitialized = false;
	void InitializeIfNeeded () {
		if (this.tcScript == null) {
			Camera mainCamera = Camera.main;
			this.tcScript = mainCamera.GetComponent<TacticalCombat> ();
		}

		if (!isInitialized) {
			isInitialized = true;

			this.selectedUnitButtonList = new List<GameObject> ();
			// Create 5 buttons
			for (int i = 0; i < 5; i++) {
				GameObject selectedUnitButton = (GameObject)Instantiate (this.selectedUnitButtonPrefab, Vector3.zero, Quaternion.identity);
				selectedUnitButton.transform.parent = this.transform;
				RectTransform rectTransform = selectedUnitButton.GetComponent<RectTransform> ();
				rectTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, 90 + 400 * i, 387.0f);
				rectTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, 370, 454.0f);
				//selectedUnitButton.transform.position = new Vector3 (-738 + 360 * i, -143, 0);
				selectedUnitButton.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

				Button selectedUnitButtonScript = selectedUnitButton.GetComponent<Button> ();
				selectedUnitButtonScript.onClick.AddListener (delegate {
					SelectedUnitButton_Tapped (selectedUnitButton);
				});

				this.selectedUnitButtonList.Add (selectedUnitButton);
			}
		}

		if (team1Tab == null) {
			team1Tab = transform.Find ("Team1Tab").gameObject;
			team2Tab = transform.Find ("Team2Tab").gameObject;
			team3Tab = transform.Find ("Team3Tab").gameObject;
		}
	}

	// Use this for initialization
	void Start () {
		InitializeIfNeeded ();
	}

	void SelectedUnitButton_Tapped (GameObject selectedUnitButton) {
		StartMissionPanel startMissionPanelScript = this.tcScript.startMissionPanel.GetComponent<StartMissionPanel> ();
		startMissionPanelScript.HidePanel ();
		HidePanel ();
		tcScript.ShowUnitSelectionPanel ();
	}
	
	// Update is called once per frame
	void Update () {
		// Change team display if it has been updated
		if (tcScript.selectedTeam != this.currentDisplayedTeam) {
			SwitchToTeam (tcScript.selectedTeam);
		}
	}

	void SwitchToTeam (int team) {
		InitializeIfNeeded ();

		tcScript.selectedTeam = team;
		this.currentDisplayedTeam = team;

		// Show all team tabs
		team1Tab.SetActive (true);
		team2Tab.SetActive (true);
		team3Tab.SetActive (true);

		// Hide the appropriate team tab
		switch (team) {
		case 0:
			// Show nothing
			break;
		case 1:
			team1Tab.SetActive (false);
			break;
		case 2:
			team2Tab.SetActive (false);
			break;
		case 3:
			team3Tab.SetActive (false);
			break;
		}

		// Set buttons with units in selected team
		int currentButtonIndex = 0;
		foreach (UnitDataObject unitDataObject in tcScript.unitDataObjectsList) {
			if (unitDataObject.assignedTeam == team && currentButtonIndex < selectedUnitButtonList.Count) {
				GameObject buttonGO = selectedUnitButtonList [currentButtonIndex];
				SelectedUnitButton button = buttonGO.GetComponent<SelectedUnitButton> ();
				button.SetUnitDataObject (unitDataObject);
				//button.HideCloseButton ();
				currentButtonIndex++;
			}
		}

		// Clear buttons without a team
		for (int i = currentButtonIndex; i < selectedUnitButtonList.Count; i++) {
			GameObject buttonGO = selectedUnitButtonList [i];
			SelectedUnitButton button = buttonGO.GetComponent<SelectedUnitButton> ();
			button.ClearButton ();
		}
	}
		
	public void Team1Tab_Tapped () {
		InitializeIfNeeded ();

		this.tcScript.PlayUIAudioClip ();
		SwitchToTeam (1);
	}

	public void Team2Tab_Tapped () {
		InitializeIfNeeded ();

		this.tcScript.PlayUIAudioClip ();
		SwitchToTeam (2);
	}

	public void Team3Tab_Tapped () {
		InitializeIfNeeded ();

		this.tcScript.PlayUIAudioClip ();
		SwitchToTeam (3);
	}

	public void CloseButton_Tapped () {
		this.tcScript.PlayUIAudioClip ();
		HidePanel ();
	}

	public void ShowPanel () {
		InitializeIfNeeded ();

		this.gameObject.SetActive (true);
		this.tcScript.isMenuOpen = true;
		SwitchToTeam (tcScript.selectedTeam);
	}

	public void HidePanel () {
		this.gameObject.SetActive (false);
	}
}
