using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class MissionObjectivesPanel : MonoBehaviour {


	private TacticalCombat tcScript;

    public GameObject[] emptyMarkers;
    public GameObject[] successMarkers;
    public GameObject[] failMarkers;
    public GameObject[] objectiveBGs;
    public Text[] objectiveTexts;

	private bool isInitialized = false;
	void InitializeIfNeeded () {
		if (this.tcScript == null) {
			Camera mainCamera = Camera.main;
			this.tcScript = mainCamera.GetComponent<TacticalCombat> ();
		}

		if (!isInitialized) {
			isInitialized = true;

            UpdatePanelForLevel(this.tcScript.currentLevel);
		}
	}
	
	// Use this for initialization
	void Start () {
		InitializeIfNeeded ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdatePanelForLevel (int level) {
        // Hide all objectives initially
        for (int i = 0; i < 3; i++) {
            emptyMarkers[i].SetActive(false);
            successMarkers[i].SetActive(false);
            failMarkers[i].SetActive(false);
            objectiveBGs[i].SetActive(false);
            objectiveTexts[i].gameObject.SetActive(false);
        }

        // Update objective marker visibility
        TacticalCombat.LevelObjective[] objectives = this.tcScript.levelObjectives[level - 1];
        for (int i = 0; i < objectives.Length; i++) {
            if (this.tcScript.isObjectiveComplete[level - 1][i] == -1) 
            {
                failMarkers[i].SetActive(true);
            } else if (this.tcScript.isObjectiveComplete[level - 1][i] == 0)
            {
                emptyMarkers[i].SetActive(true);
            } else if (this.tcScript.isObjectiveComplete[level - 1][i] == 1)
            {
                successMarkers[i].SetActive(true);
            }
            objectiveBGs[i].SetActive(true);
            objectiveTexts[i].gameObject.SetActive(true);
            objectiveTexts[i].text = this.tcScript.GetLevelObjectiveDescription(objectives[i]);
        }
    }

    public void StartButton_Tapped()
    {
        this.tcScript.PlayUIAudioClip();
        //this.tcScript.ResetTacticalCombat();
        this.tcScript.StartCountdown();
        this.tcScript.HideAllPanelsExcept(TacticalCombat.UIPanelType.UI_PANEL_TYPE_NONE);
        this.tcScript.isMenuOpen = false;
    }

    public void BackButton_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        this.tcScript.isMenuOpen = false;

        this.tcScript.ShowLevelSelectionPanel();

        this.HidePanel();

        this.tcScript.ShowNavBar();
    }

    public void ShowPanel()
    {
        InitializeIfNeeded();

        UpdatePanelForLevel(this.tcScript.currentLevel);

        // TODO: Something here causes a crash - should not need above method
        this.gameObject.SetActive(true);
        this.tcScript.isMenuOpen = true;
    }

    public void HidePanel()
    {
        this.gameObject.SetActive(false);
        //this.tcScript.isMissionObjectivesPanelQueued = false;
    }
}
