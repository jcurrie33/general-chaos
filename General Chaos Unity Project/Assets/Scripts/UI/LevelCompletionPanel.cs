using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelCompletionPanel : MonoBehaviour {

	private TacticalCombat tcScript;
	private GameObject rewardText;
	private GameObject retryButton;

    public GameObject[] emptyMarkers;
    public GameObject[] successMarkers;
    public GameObject[] failMarkers;
    public GameObject[] objectiveBGs;
    public Text[] objectiveTexts;
    public GameObject[] lootboxes1;
    public GameObject[] lootboxes2;
    public GameObject[] lootboxes3;
    public GameObject lootboxBG;
    public GameObject[] medals;
    public GameObject[] difficultyIcons;
    public Text titleSuccessText;
    public Text titleFailText;
    public Text statsLeft;
    public Text statsRight;


	// The Awake function is called on all objects in the scene before any object's Start function is called.
	void Awake () {
		Camera mainCamera = Camera.main;
		this.tcScript = mainCamera.GetComponent<TacticalCombat> ();

		rewardText = transform.Find ("RewardText").gameObject;
		retryButton = transform.Find ("RetryButton").gameObject;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
    }

    public void UpdatePanelForLevel(int level)
    {
        // Hide all objectives initially
        for (int i = 0; i < 3; i++)
        {
            emptyMarkers[i].SetActive(false);
            successMarkers[i].SetActive(false);
            failMarkers[i].SetActive(false);
            objectiveBGs[i].SetActive(false);
            objectiveTexts[i].gameObject.SetActive(false);
        }

        // Update objective marker visibility
        TacticalCombat.LevelObjective[] objectives = this.tcScript.levelObjectives[level - 1];
        for (int i = 0; i < objectives.Length; i++)
        {
            if (this.tcScript.isObjectiveComplete[level - 1][i] == -1)
            {
                failMarkers[i].SetActive(true);
            }
            else if (this.tcScript.isObjectiveComplete[level - 1][i] == 0)
            {
                emptyMarkers[i].SetActive(true);
            }
            else if (this.tcScript.isObjectiveComplete[level - 1][i] == 1)
            {
                successMarkers[i].SetActive(true);
            }
            objectiveBGs[i].SetActive(true);
            objectiveTexts[i].gameObject.SetActive(true);
            objectiveTexts[i].text = this.tcScript.GetLevelObjectiveDescription(objectives[i]);
        }


        // Update stats text
        int numEnemyUnitsDisabled = 0;
        int numPlayerUnitsDisabled = 0;
        int totalEnemyUnits = 0;
        int totalPlayerUnits = 0;
        foreach (Unit unit in this.tcScript.unitsList)
        {
            if (unit.isPlayerControlled)
            {
                totalPlayerUnits++;
                if (unit.currentState == Unit.UnitState.UNIT_STATE_DEAD || unit.currentState == Unit.UnitState.UNIT_STATE_DISABLED)
                {
                    numPlayerUnitsDisabled++;
                }
            }
            else
            {
                totalEnemyUnits++;
                if (unit.currentState == Unit.UnitState.UNIT_STATE_DEAD || unit.currentState == Unit.UnitState.UNIT_STATE_DISABLED)
                {
                    numEnemyUnitsDisabled++;
                }
            }
        }
        float timeElapsed = Mathf.Floor((Time.time - this.tcScript.timeAtLevelStart) * 10.0f) / 10.0f - TacticalCombat.DELAY_BEFORE_SHOWING_LEVEL_COMPLETION_PANEL;
        statsLeft.text = numEnemyUnitsDisabled + "\n" + numPlayerUnitsDisabled + "\n" + timeElapsed + " seconds";
        statsRight.text = this.tcScript.numShotsFired + "\n" + "x" + " percent\n" + "x" + " percent";

        // Update difficulty icon visibility
        StartMissionPanel.LevelDifficulty difficulty = this.tcScript.selectedDifficulty;
        int rewardDoubleDollarAmount = 0;
        int rewardDiamondAmount = 0;
        for (int i = 0; i < difficultyIcons.Length; i++)
        {
            difficultyIcons[i].SetActive(false);
        }
        for (int i = 0; i < medals.Length; i++)
        {
            medals[i].SetActive(false);
        }
        for (int i = 0; i < lootboxes1.Length; i++)
        {
            lootboxes1[i].SetActive(false);
        }
        for (int i = 0; i < lootboxes2.Length; i++)
        {
            lootboxes2[i].SetActive(false);
        }
        for (int i = 0; i < lootboxes3.Length; i++)
        {
            lootboxes3[i].SetActive(false);
        }
        lootboxBG.SetActive(false);

        if (this.tcScript.isLevelComplete)
        {
            lootboxBG.SetActive(true);
        }

        switch (difficulty)
        {
            case StartMissionPanel.LevelDifficulty.LEVEL_DIFFICULTY_NONE:
                // No reward
                break;
            case StartMissionPanel.LevelDifficulty.LEVEL_DIFFICULTY_EASY:
                difficultyIcons[0].SetActive(true);
                if (this.tcScript.isLevelComplete)
                {
                    rewardDoubleDollarAmount = 1000;
                    for (int i = 0; i < lootboxes1.Length; i++)
                    {
                        lootboxes1[i].SetActive(true);
                    }
                }
                break;
            case StartMissionPanel.LevelDifficulty.LEVEL_DIFFICULTY_MEDIUM:
                difficultyIcons[1].SetActive(true);
                if (this.tcScript.isLevelComplete)
                {
                    rewardDoubleDollarAmount = 2000;
                    for (int i = 0; i < lootboxes2.Length; i++)
                    {
                        lootboxes2[i].SetActive(true);
                    }
                }
                break;
            case StartMissionPanel.LevelDifficulty.LEVEL_DIFFICULTY_HARD:
                difficultyIcons[2].SetActive(true);
                if (this.tcScript.isLevelComplete)
                {
                    rewardDoubleDollarAmount = 3000;
                    for (int i = 0; i < lootboxes3.Length; i++)
                    {
                        lootboxes3[i].SetActive(true);
                    }
                }
                break;
        }

        // TODO: Adjust medals based on additional parameters
        if (this.tcScript.isLevelComplete)
        {
            if (numPlayerUnitsDisabled == 0 && numEnemyUnitsDisabled == totalEnemyUnits)
            {
                medals[2].SetActive(true);
            }
            else if (numPlayerUnitsDisabled == 1 && numEnemyUnitsDisabled >= totalEnemyUnits - 1)
            {
                medals[1].SetActive(true);
            }
            else if (numPlayerUnitsDisabled == 2 && numEnemyUnitsDisabled >= totalEnemyUnits - 2)
            {
                medals[0].SetActive(true);
            }
        }

        if (this.tcScript.isLevelComplete)
        {
            titleSuccessText.gameObject.SetActive(true);
            titleFailText.gameObject.SetActive(false);
            titleSuccessText.text = "MISSION COMPLETE";
            retryButton.SetActive(false);
            rewardText.SetActive(true);

            // Give reward based on difficulty level
            Text rewardTextScript = rewardText.GetComponent<Text>();

            this.tcScript.AddDoubleDollars(rewardDoubleDollarAmount);
            rewardTextScript.text = "Rewarded §" + rewardDoubleDollarAmount.ToString("N0");

        }
        else if (this.tcScript.isLevelFailed)
        {
            titleSuccessText.gameObject.SetActive(false);
            titleFailText.gameObject.SetActive(true);
            titleFailText.text = "MISSION FAILED";
            retryButton.SetActive(true);
            rewardText.SetActive(false);
        }
    }


	public void ShowPanel () {
		this.gameObject.SetActive (true);
		this.tcScript.isMenuOpen = true;

        UpdatePanelForLevel(this.tcScript.currentLevel);

		//this.tcScript.ShowNavBar ();
	}

	public void HidePanel () {
		this.gameObject.SetActive (false);

		this.tcScript.isLevelCompletionPanelQueued = false;
	}

	public void LevelCompletionPanel_MapButton_Tapped () {
		this.tcScript.PlayUIAudioClip ();

		this.tcScript.isMenuOpen = false;

		this.tcScript.ShowLevelSelectionPanel ();

		this.HidePanel ();

		this.tcScript.ShowNavBar ();
	}

	public void LevelCompletionPanel_ResetButton_Tapped () {
		this.tcScript.PlayUIAudioClip ();

		this.tcScript.isMenuOpen = false;

		this.tcScript.ResetTacticalCombat ();

        this.tcScript.UnpauseTacticalCombat();

		this.HidePanel ();

		this.tcScript.HideNavBar ();
	}

	public void LevelCompletionPanel_NextLevelButton_Tapped () {
		this.tcScript.PlayUIAudioClip ();

		this.tcScript.isMenuOpen = false;

		this.tcScript.currentLevel++;

		//this.tcScript.ResetTacticalCombat ();
		//this.tcScript.ShowUnitSelectionPanel ();

		this.tcScript.ShowLevelSelectionPanel ();

		this.HidePanel ();

		this.tcScript.ShowNavBar ();
	}
}
