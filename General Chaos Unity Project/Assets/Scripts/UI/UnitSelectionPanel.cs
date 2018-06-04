using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UnitSelectionPanel : MonoBehaviour {

	public enum UnitDetailSubpanelType {
		UNITDETAILSUBPANEL_TYPE_NONE = 0,
		UNITDETAILSUBPANEL_TYPE_CHARACTER,
		UNITDETAILSUBPANEL_TYPE_WEAPON,
		UNITDETAILSUBPANEL_TYPE_ARMOR
	};

	public List<Image> imageList;

	public GameObject unitSelectionTabButtonPrefab;
	public List<GameObject> unitSelectionTabButtonList;
	public UnitDataObject selectedUnitDataObject;

//	private int currentDisplayedTeam = 0;
	private GameObject unitSelectionScrollViewContent;

	private GameObject unitDetailSubpanel;

	private GameObject characterTab;
	private GameObject weaponTab;
	private GameObject armorTab;

	private GameObject characterUpgradingText;
	private GameObject characterLevelValueText;
	private GameObject characterLevelLabelText;
	private GameObject unitProfilePicBZ;
	private GameObject unitProfilePicFT;
	private GameObject unitProfilePicGL;
	private GameObject unitProfilePicGR;
	private GameObject unitProfilePicMG;

	private GameObject weaponUpgradingText;
	private GameObject weaponLevelValueText;
	private GameObject weaponLevelLabelText;
	private GameObject weaponIcon;

	private GameObject armorUpgradingText;
	private GameObject armorLevelValueText;
	private GameObject armorLevelLabelText;
	private GameObject armorIcon;

	private GameObject statValuesText;
	private GameObject statLabelsText;
	private GameObject statButton1;
	private GameObject statButton2;
	private GameObject statButton3;
	private GameObject characterTabButton;
	private GameObject weaponTabButton;
	private GameObject armorTabButton;
	private GameObject upgradeButton;
	private GameObject addToTeamButton;


	private bool isUpgrading = false;
	private UnitDetailSubpanelType unitDetailSubpanelType = UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER;

	private TacticalCombat tcScript;

	private bool isInitialized = false;
	void InitializeIfNeeded () {
		if (this.tcScript == null) {
			Camera mainCamera = Camera.main;
			this.tcScript = mainCamera.GetComponent<TacticalCombat> ();
		}

		if (selectedUnitDataObject == null) {
			selectedUnitDataObject = this.tcScript.unitDataObjectsList [0];
		}

		if (unitDetailSubpanel == null) {
			unitDetailSubpanel = transform.Find ("UnitDetailSubpanel").gameObject;

			characterTab = unitDetailSubpanel.transform.Find ("CharacterTab").gameObject;
			weaponTab = unitDetailSubpanel.transform.Find ("WeaponTab").gameObject;
			armorTab = unitDetailSubpanel.transform.Find ("ArmorTab").gameObject;

			characterUpgradingText = characterTab.transform.Find ("CharacterUpgradingText").gameObject;
			characterLevelValueText = characterTab.transform.Find ("CharacterLevelValueText").gameObject;
			characterLevelLabelText = characterTab.transform.Find ("CharacterLevelLabelText").gameObject;
			unitProfilePicBZ = characterTab.transform.Find ("UnitProfilePicBZ").gameObject;
			unitProfilePicFT = characterTab.transform.Find ("UnitProfilePicFT").gameObject;
			unitProfilePicGL = characterTab.transform.Find ("UnitProfilePicGL").gameObject;
			unitProfilePicGR = characterTab.transform.Find ("UnitProfilePicGR").gameObject;
			unitProfilePicMG = characterTab.transform.Find ("UnitProfilePicMG").gameObject;

			weaponUpgradingText = weaponTab.transform.Find ("WeaponUpgradingText").gameObject;
			weaponLevelValueText = weaponTab.transform.Find ("WeaponLevelValueText").gameObject;
			weaponLevelLabelText = weaponTab.transform.Find ("WeaponLevelLabelText").gameObject;
			weaponIcon = weaponTab.transform.Find ("WeaponIcon").gameObject;

			armorUpgradingText = armorTab.transform.Find ("ArmorUpgradingText").gameObject;
			armorLevelValueText = armorTab.transform.Find ("ArmorLevelValueText").gameObject;
			armorLevelLabelText = armorTab.transform.Find ("ArmorLevelLabelText").gameObject;
			armorIcon = armorTab.transform.Find ("ArmorIcon").gameObject;

			statValuesText = unitDetailSubpanel.transform.Find ("StatValuesText").gameObject;
			statLabelsText = unitDetailSubpanel.transform.Find ("StatLabelsText").gameObject;
			statButton1 = unitDetailSubpanel.transform.Find ("StatButton1").gameObject;
			statButton2 = unitDetailSubpanel.transform.Find ("StatButton2").gameObject;
			statButton3 = unitDetailSubpanel.transform.Find ("StatButton3").gameObject;
			characterTabButton = unitDetailSubpanel.transform.Find ("CharacterTabButton").gameObject;
			weaponTabButton = unitDetailSubpanel.transform.Find ("WeaponTabButton").gameObject;
			armorTabButton = unitDetailSubpanel.transform.Find ("ArmorTabButton").gameObject;
			upgradeButton = unitDetailSubpanel.transform.Find ("UpgradeButton").gameObject;
			addToTeamButton = unitDetailSubpanel.transform.Find ("AddToTeamButton").gameObject;
		}
			
		if (unitSelectionScrollViewContent == null) {
			unitSelectionScrollViewContent = GameObject.Find ("UnitSelectionScrollViewContent");
		}

		if (!isInitialized && unitSelectionScrollViewContent != null) {
			isInitialized = true;

			this.unitSelectionTabButtonList = new List<GameObject> ();
			// Create buttons for each unit
			for (int i = 0; i < tcScript.unitDataObjectsList.Count; i++) {
				UnitDataObject unitDataObject = tcScript.unitDataObjectsList [i];
				GameObject unitSelectionTabButton = (GameObject)Instantiate (this.unitSelectionTabButtonPrefab, Vector3.zero, Quaternion.identity);

				unitSelectionTabButton.transform.parent = unitSelectionScrollViewContent.transform;
				RectTransform rectTransform = unitSelectionTabButton.GetComponent<RectTransform> ();
				rectTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, -400, 1320.0f/* * 0.25f*/);
				rectTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, -150 - 18 + 120 * i * 0.75f, 432.0f/* * 0.25f*/);
				unitSelectionTabButton.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);

				UnitSelectionTabButton unitSelectionTabButtonScript = unitSelectionTabButton.GetComponent<UnitSelectionTabButton> ();
				unitSelectionTabButtonScript.SetUnitDataObject (unitDataObject);

				Button tappableButtonScript = unitSelectionTabButton.GetComponent<Button> ();
				tappableButtonScript.onClick.AddListener (delegate {
					SelectedUnitButton_Tapped (unitSelectionTabButton);
				});

				this.unitSelectionTabButtonList.Add (unitSelectionTabButton);
			}

			// Set scrollview content size based on number of units
			RectTransform contentViewRectTransform = unitSelectionScrollViewContent.GetComponent<RectTransform> ();
			contentViewRectTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, 0, 1320.0f/* * 0.25f*/);
			contentViewRectTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, 0, 120.0f * tcScript.unitDataObjectsList.Count * 0.75f * 1.01f);

		}
	}

	void SelectedUnitButton_Tapped (GameObject unitSelectionTabButton) {
		UnitSelectionTabButton unitSelectionTabButtonScript = unitSelectionTabButton.GetComponent<UnitSelectionTabButton> ();

		selectedUnitDataObject = unitSelectionTabButtonScript.assignedUnitDataObject;

		CancelUpgrades ();
		SwitchDetailToTab (UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER);


	}

	// Use this for initialization
	void Start () {
		InitializeIfNeeded ();
		SwitchDetailToTab (UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER);
	}

	// Update is called once per frame
	void Update () {

	}

	public void ShowPanel () {
		InitializeIfNeeded ();

		this.gameObject.SetActive (true);
		this.tcScript.isMenuOpen = true;

		SwitchDetailToTab (UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER);
	}

	public void HidePanel () {
		InitializeIfNeeded ();

		this.gameObject.SetActive (false);

		this.tcScript.isUnitSelectionPanelQueued = false;
	}

	public void UpdateUpgradeButtonText () {
		Text upgradeButtonText = upgradeButton.transform.Find ("Text").gameObject.GetComponent<Text> ();

		if (isUpgrading) {
			upgradeButtonText.text = "CONFIRM UPGRADE";
		} else {
			switch (this.unitDetailSubpanelType) {
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
				// TODO: Show upgrade button for Unit
				upgradeButton.SetActive (false);

				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
				if (ReferenceEquals(selectedUnitDataObject.equippedWeapon, null)) {
					upgradeButton.SetActive (false);
				} else {
					if (selectedUnitDataObject.equippedWeapon.level == 1) {
						upgradeButtonText.text = "UPGRADE FOR §" + selectedUnitDataObject.equippedWeapon.doubleDollarCostToUpgradeToLevel2.ToString ("N0");
					} else if (selectedUnitDataObject.equippedWeapon.level == 2) {
						upgradeButtonText.text = "UPGRADE FOR §" + selectedUnitDataObject.equippedWeapon.doubleDollarCostToUpgradeToLevel3.ToString ("N0");
					} else {
						upgradeButton.SetActive (false);
					}
				}
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
				if (ReferenceEquals(selectedUnitDataObject.equippedArmor, null)) {
					upgradeButton.SetActive (false);
				} else {
					if (selectedUnitDataObject.equippedArmor.level == 1) {
						upgradeButtonText.text = "UPGRADE FOR §" + selectedUnitDataObject.equippedArmor.doubleDollarCostToUpgradeToLevel2.ToString ("N0");
					} else if (selectedUnitDataObject.equippedArmor.level == 2) {
						upgradeButtonText.text = "UPGRADE FOR §" + selectedUnitDataObject.equippedArmor.doubleDollarCostToUpgradeToLevel3.ToString ("N0");
					} else {
						upgradeButton.SetActive (false);
					}
				}
				break;
			}
		}
	}

	public void SwitchDetailToTab (UnitDetailSubpanelType tabType) {
		InitializeIfNeeded ();

		this.unitDetailSubpanelType = tabType;


		Text statLabelsTextScript = statLabelsText.GetComponent<Text> ();
		Text statValuesTextScript = statValuesText.GetComponent<Text> ();

		switch (this.unitDetailSubpanelType) {
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
			characterTab.transform.SetAsLastSibling ();
			upgradeButton.SetActive (false);
			statLabelsTextScript.text = "HEALTH\nSPEED";
			statValuesTextScript.text = selectedUnitDataObject.health + "\n"
			+ selectedUnitDataObject.speed;
			if (selectedUnitDataObject.level < 3) {
				upgradeButton.SetActive (true);
			}

			SetUnitType (selectedUnitDataObject.unitType);

			Text characterLevelValueTextScript = characterLevelValueText.GetComponent<Text> ();
			characterLevelValueTextScript.text = selectedUnitDataObject.level.ToString ();

			break;
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
			weaponTab.transform.SetAsLastSibling ();
			upgradeButton.SetActive (false);
			if (ReferenceEquals(selectedUnitDataObject.equippedWeapon, null)) {
				statLabelsTextScript.text = "";
				statValuesTextScript.text = "";

				weaponIcon.SetActive (false);
				weaponLevelValueText.SetActive (false);
				weaponLevelLabelText.SetActive (false);
			} else {
				statLabelsTextScript.text = "ATTACK\nFIRERATE\nRANGE";
				statValuesTextScript.text = selectedUnitDataObject.equippedWeapon.attack + "\n"
					+ selectedUnitDataObject.equippedWeapon.firerate + "\n"
					+ selectedUnitDataObject.equippedWeapon.range;
				if (selectedUnitDataObject.equippedWeapon.level < 3) {
					upgradeButton.SetActive (true);
				}

				weaponIcon.SetActive (true);
				weaponLevelValueText.SetActive (true);
				weaponLevelLabelText.SetActive (true);

				Text weaponLevelValueTextScript = weaponLevelValueText.GetComponent<Text> ();
				weaponLevelValueTextScript.text = selectedUnitDataObject.equippedWeapon.level.ToString ();

			}
			break;
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
			armorTab.transform.SetAsLastSibling ();
			upgradeButton.SetActive (false);
			if (ReferenceEquals(selectedUnitDataObject.equippedArmor, null)) {
				statLabelsTextScript.text = "";
				statValuesTextScript.text = "";
				armorIcon.SetActive (false);
				armorLevelValueText.SetActive (false);
				armorLevelLabelText.SetActive (false);
			} else {
				statLabelsTextScript.text = "BULLET RESIST.\nFLAME RESIST.\nEXPLOSION RESIST.";
				statValuesTextScript.text = selectedUnitDataObject.equippedArmor.bulletResistance + "\n"
					+ selectedUnitDataObject.equippedArmor.flameResistance + "\n"
					+ selectedUnitDataObject.equippedArmor.explosionResistance;
				if (selectedUnitDataObject.equippedArmor.level < 3) {
					upgradeButton.SetActive (true);
				}

				armorIcon.SetActive (true);
				armorLevelValueText.SetActive (true);
				armorLevelLabelText.SetActive (true);

				Text armorLevelValueTextScript = armorLevelValueText.GetComponent<Text> ();
				armorLevelValueTextScript.text = selectedUnitDataObject.equippedArmor.level.ToString ();

			}
			break;
		}

		statValuesText.transform.SetAsLastSibling ();
		statLabelsText.transform.SetAsLastSibling ();
		statButton1.transform.SetAsLastSibling ();
		statButton2.transform.SetAsLastSibling ();
		statButton3.transform.SetAsLastSibling ();
		characterTabButton.transform.SetAsLastSibling ();
		weaponTabButton.transform.SetAsLastSibling ();
		armorTabButton.transform.SetAsLastSibling ();
		upgradeButton.transform.SetAsLastSibling ();
		addToTeamButton.transform.SetAsLastSibling ();

		UpdateUpgradeButtonText ();
		UpdateBasedOnUpgradingStatus ();
	}

	public void SetUnitType (Unit.UnitType unitType) {
		InitializeIfNeeded ();

		// Hide all units
		unitProfilePicBZ.SetActive (false);
		unitProfilePicFT.SetActive (false);
		unitProfilePicGL.SetActive (false);
		unitProfilePicGR.SetActive (false);
		unitProfilePicMG.SetActive (false);

		// Show appropriate unit
		switch (unitType) {
		case Unit.UnitType.UNIT_NONE:
			// Show nothing
			break;
		case Unit.UnitType.UNIT_BAZOOKA:
			unitProfilePicBZ.SetActive (true);
			unitProfilePicBZ.transform.SetAsLastSibling ();
			break;
		case Unit.UnitType.UNIT_FLAMETHROWER:
			unitProfilePicFT.SetActive (true);
			unitProfilePicFT.transform.SetAsLastSibling ();
			break;
		case Unit.UnitType.UNIT_GRENADELAUNCHER:
			unitProfilePicGL.SetActive (true);
			unitProfilePicGL.transform.SetAsLastSibling ();
			break;
		case Unit.UnitType.UNIT_GRENADIER:
			unitProfilePicGR.SetActive (true);
			unitProfilePicGR.transform.SetAsLastSibling ();
			break;
		case Unit.UnitType.UNIT_MACHINEGUNNER:
			unitProfilePicMG.SetActive (true);
			unitProfilePicMG.transform.SetAsLastSibling ();
			break;
		}

	}




	// TODO: Remove below code:

	public void StartMissionButtonTapped () {
		this.tcScript.PlayUIAudioClip ();

		this.tcScript.isMenuOpen = false;

		this.tcScript.ResetTacticalCombat ();

		this.HidePanel ();

		this.tcScript.HideNavBar ();
	}

	public void ScrollButtonTapped () {
		this.tcScript.PlayUIAudioClip ();

		GameObject selectedButtonGO = EventSystem.current.currentSelectedGameObject;
		UnitSelectionScrollButton selectedButton = selectedButtonGO.GetComponent<UnitSelectionScrollButton> ();
		selectedButton.isSelected = !selectedButton.isSelected;
		imageList [selectedButton.index].gameObject.SetActive (selectedButton.isSelected);
		selectedButton.selectedImage.gameObject.SetActive (selectedButton.isSelected);
	}

	public void CancelUpgrades () {
		if (isUpgrading) {
			switch (this.unitDetailSubpanelType) {
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
				// TODO: Cancel upgrade
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
				this.selectedUnitDataObject.equippedWeapon.CancelUpgrade ();
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
				this.selectedUnitDataObject.equippedArmor.CancelUpgrade ();
				break;
			}

			isUpgrading = false;
			DisableStatButtons ();
		}
	}

	public void CharacterTabButton_Tapped () {
		CancelUpgrades ();
		SwitchDetailToTab (UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER);
	}

	public void WeaponTabButton_Tapped () {
		CancelUpgrades ();
		SwitchDetailToTab (UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON);
	}

	public void ArmorTabButton_Tapped () {
		CancelUpgrades ();
		SwitchDetailToTab (UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR);
	}

	public void EnableStatButtons () {
		Button upgradeButtonScript = upgradeButton.GetComponent<Button> ();
		upgradeButtonScript.interactable = false;
		Button statButton1Script = statButton1.GetComponent<Button> ();
		statButton1Script.interactable = true;
		Button statButton2Script = statButton2.GetComponent<Button> ();
		statButton2Script.interactable = true;
		Button statButton3Script = statButton3.GetComponent<Button> ();
		statButton3Script.interactable = true;
	}

	public void DisableStatButtons () {
		Button upgradeButtonScript = upgradeButton.GetComponent<Button> ();
		upgradeButtonScript.interactable = true;
		Button statButton1Script = statButton1.GetComponent<Button> ();
		statButton1Script.interactable = false;
		Button statButton2Script = statButton2.GetComponent<Button> ();
		statButton2Script.interactable = false;
		Button statButton3Script = statButton3.GetComponent<Button> ();
		statButton3Script.interactable = false;
	}

	public void StatButton1_Tapped () {
		Text statValuesTextScript = statValuesText.GetComponent<Text> ();

		switch (this.unitDetailSubpanelType) {
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
			// TODO: Allocate stat point to Health
			statValuesTextScript.text = selectedUnitDataObject.health + "\n"
				+ selectedUnitDataObject.speed;
			break;
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
			// Attack
			this.selectedUnitDataObject.equippedWeapon.AllocateStatPointToAttack ();
			if (this.selectedUnitDataObject.equippedWeapon.unallocatedStatPoints == 0) {
				DisableStatButtons ();
			}
			statValuesTextScript.text = selectedUnitDataObject.equippedWeapon.attack + "\n"
				+ selectedUnitDataObject.equippedWeapon.firerate + "\n"
				+ selectedUnitDataObject.equippedWeapon.range;
			break;
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
			// Bullet Resistance
			this.selectedUnitDataObject.equippedArmor.AllocateStatPointToBulletResistance ();
			if (this.selectedUnitDataObject.equippedArmor.unallocatedStatPoints == 0) {
				DisableStatButtons ();
			}
			statValuesTextScript.text = selectedUnitDataObject.equippedArmor.bulletResistance + "\n"
				+ selectedUnitDataObject.equippedArmor.flameResistance + "\n"
				+ selectedUnitDataObject.equippedArmor.explosionResistance;
			break;
		}
	}

	public void StatButton2_Tapped () {
		Text statValuesTextScript = statValuesText.GetComponent<Text> ();

		switch (this.unitDetailSubpanelType) {
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
			// TODO: Allocate stat point to Speed
			statValuesTextScript.text = selectedUnitDataObject.health + "\n"
				+ selectedUnitDataObject.speed;
			break;
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
			// Firerate
			this.selectedUnitDataObject.equippedWeapon.AllocateStatPointToFirerate ();
			if (this.selectedUnitDataObject.equippedWeapon.unallocatedStatPoints == 0) {
				DisableStatButtons ();
			}
			statValuesTextScript.text = selectedUnitDataObject.equippedWeapon.attack + "\n"
				+ selectedUnitDataObject.equippedWeapon.firerate + "\n"
				+ selectedUnitDataObject.equippedWeapon.range;
			break;
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
			// Flame Resistance
			this.selectedUnitDataObject.equippedArmor.AllocateStatPointToFlameResistance ();
			if (this.selectedUnitDataObject.equippedArmor.unallocatedStatPoints == 0) {
				DisableStatButtons ();
			}
			statValuesTextScript.text = selectedUnitDataObject.equippedArmor.bulletResistance + "\n"
				+ selectedUnitDataObject.equippedArmor.flameResistance + "\n"
				+ selectedUnitDataObject.equippedArmor.explosionResistance;
			break;
		}
	}

	public void StatButton3_Tapped () {
		Text statValuesTextScript = statValuesText.GetComponent<Text> ();

		switch (this.unitDetailSubpanelType) {
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
			// Do nothing
			break;
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
			// Range
			this.selectedUnitDataObject.equippedWeapon.AllocateStatPointToRange ();
			if (this.selectedUnitDataObject.equippedWeapon.unallocatedStatPoints == 0) {
				DisableStatButtons ();
			}
			statValuesTextScript.text = selectedUnitDataObject.equippedWeapon.attack + "\n"
				+ selectedUnitDataObject.equippedWeapon.firerate + "\n"
				+ selectedUnitDataObject.equippedWeapon.range;
			break;
		case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
			// Explosion Resistance
			this.selectedUnitDataObject.equippedArmor.AllocateStatPointToExplosionResistance ();
			if (this.selectedUnitDataObject.equippedArmor.unallocatedStatPoints == 0) {
				DisableStatButtons ();
			}
			statValuesTextScript.text = selectedUnitDataObject.equippedArmor.bulletResistance + "\n"
				+ selectedUnitDataObject.equippedArmor.flameResistance + "\n"
				+ selectedUnitDataObject.equippedArmor.explosionResistance;
			break;
		}
	}

	public void UpgradeButton_Tapped () {
		if (isUpgrading) {
			switch (this.unitDetailSubpanelType) {
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
				// TODO: Confirm upgrade
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
				this.selectedUnitDataObject.equippedWeapon.ConfirmUpgrade ();
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
				this.selectedUnitDataObject.equippedArmor.ConfirmUpgrade ();
				break;
			}
			SwitchDetailToTab (this.unitDetailSubpanelType);
		} else {
			switch (this.unitDetailSubpanelType) {
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
				// TODO: Prepare to upgrade
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
				this.selectedUnitDataObject.equippedWeapon.PrepareToUpgrade ();
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
				this.selectedUnitDataObject.equippedArmor.PrepareToUpgrade ();
				break;
			}
		}

		isUpgrading = !isUpgrading;

		UpdateUpgradeButtonText ();
		UpdateBasedOnUpgradingStatus ();

	}

	public void UpdateBasedOnUpgradingStatus () {
		if (isUpgrading) {
			statButton1.SetActive (false);
			statButton2.SetActive (false);
			statButton3.SetActive (false);

			EnableStatButtons ();

			switch (this.unitDetailSubpanelType) {
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
				statButton1.SetActive (true);
				statButton2.SetActive (true);
				Text characterUpgradingTextScript = characterUpgradingText.GetComponent<Text> ();
				characterUpgradingTextScript.text = "UPGRADING TO LEVEL " + (selectedUnitDataObject.level + 1);
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
				statButton1.SetActive (true);
				statButton2.SetActive (true);
				statButton3.SetActive (true);
				Text weaponUpgradingTextScript = weaponUpgradingText.GetComponent<Text> ();
				weaponUpgradingTextScript.text = "UPGRADING TO LEVEL " + (selectedUnitDataObject.equippedWeapon.level);
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
				statButton1.SetActive (true);
				statButton2.SetActive (true);
				statButton3.SetActive (true);
				Text armorUpgradingTextScript = armorUpgradingText.GetComponent<Text> ();
				armorUpgradingTextScript.text = "UPGRADING TO LEVEL " + (selectedUnitDataObject.equippedArmor.level);
				break;
			}
		} else {
			statButton1.SetActive (false);
			statButton2.SetActive (false);
			statButton3.SetActive (false);

			DisableStatButtons ();

			switch (this.unitDetailSubpanelType) {
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_CHARACTER:
				Text characterUpgradingTextScript = characterUpgradingText.GetComponent<Text> ();
				switch (selectedUnitDataObject.unitType) {
				case Unit.UnitType.UNIT_BAZOOKA:
					characterUpgradingTextScript.text = "BAZOOKA";
					break;
				case Unit.UnitType.UNIT_FLAMETHROWER:
					characterUpgradingTextScript.text = "FLAME THROWER";
					break;
				case Unit.UnitType.UNIT_GRENADELAUNCHER:
					characterUpgradingTextScript.text = "GRENADE LAUNCHER";
					break;
				case Unit.UnitType.UNIT_GRENADIER:
					characterUpgradingTextScript.text = "GRENADIER";
					break;
				case Unit.UnitType.UNIT_MACHINEGUNNER:
					characterUpgradingTextScript.text = "MACHINE GUNNER";
					break;
				}
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_WEAPON:
				Text weaponUpgradingTextScript = weaponUpgradingText.GetComponent<Text> ();
				if (ReferenceEquals(selectedUnitDataObject.equippedWeapon, null)) {
					weaponUpgradingTextScript.text = "NO WEAPON EQUIPPED";
				} else {
					weaponUpgradingTextScript.text = selectedUnitDataObject.equippedWeapon.Name ().ToUpper ();
				}
				break;
			case UnitDetailSubpanelType.UNITDETAILSUBPANEL_TYPE_ARMOR:
				Text armorUpgradingTextScript = armorUpgradingText.GetComponent<Text> ();
				if (ReferenceEquals(selectedUnitDataObject.equippedArmor, null)) {
					armorUpgradingTextScript.text = "NO ARMOR EQUIPPED";
				} else {
					armorUpgradingTextScript.text = selectedUnitDataObject.equippedArmor.Name ().ToUpper ();
				}
				break;
			}
		}
	}

	public void AddToTeamButton_Tapped () {

	}
}
