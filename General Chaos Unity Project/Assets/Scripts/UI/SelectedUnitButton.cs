using UnityEngine;
using System.Collections;

public class SelectedUnitButton : MonoBehaviour {

	public Unit.UnitType unitType;
	public int level;
	public UnitDataObject assignedUnitDataObject = null;

	private GameObject unitProfilePicBZ;
	private GameObject unitProfilePicFT;
	private GameObject unitProfilePicGL;
	private GameObject unitProfilePicGR;
	private GameObject unitProfilePicMG;
	private GameObject level1;
	private GameObject level2;
	private GameObject level3;
	private GameObject closeButtonImage;

	private TacticalCombat tcScript;

	private bool isInitialized = false;
	void InitializeIfNeeded () {
		if (this.tcScript == null) {
			Camera mainCamera = Camera.main;
			this.tcScript = mainCamera.GetComponent<TacticalCombat> ();
		}

		if (unitProfilePicBZ == null) {
			unitProfilePicBZ = transform.Find ("UnitProfilePicBZ").gameObject;
			unitProfilePicFT = transform.Find ("UnitProfilePicFT").gameObject;
			unitProfilePicGL = transform.Find ("UnitProfilePicGL").gameObject;
			unitProfilePicGR = transform.Find ("UnitProfilePicGR").gameObject;
			unitProfilePicMG = transform.Find ("UnitProfilePicMG").gameObject;
			level1 = transform.Find ("Level1").gameObject;
			level2 = transform.Find ("Level2").gameObject;
			level3 = transform.Find ("Level3").gameObject;
			closeButtonImage = transform.Find ("CloseButtonImage").gameObject;

			ClearButton ();
		}
	}

	// Use this for initialization
	void Start () {
		InitializeIfNeeded ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void Button_Tapped () {
		this.tcScript.PlayUIAudioClip ();

		// TODO: Open unit selection panel
	}

	public void UnassignUnit () {
		if (this.assignedUnitDataObject != null) {
			this.assignedUnitDataObject.assignedTeam = 0;
			this.assignedUnitDataObject = null;
		}
		SetUnitType (Unit.UnitType.UNIT_NONE);
		SetLevel (0);
		closeButtonImage.SetActive (false);
	}

	public void ClearButton () {
		this.assignedUnitDataObject = null;
		SetUnitType (Unit.UnitType.UNIT_NONE);
		SetLevel (0);
		closeButtonImage.SetActive (false);
	}

	public void SetUnitDataObject (UnitDataObject unitDataObject) {

		InitializeIfNeeded ();

		this.assignedUnitDataObject = unitDataObject;
		SetUnitType (this.assignedUnitDataObject.unitType);
		SetLevel (this.assignedUnitDataObject.level);
		closeButtonImage.SetActive (true);
	}

	public void HideCloseButton () {
		closeButtonImage.SetActive (false);
	}

	public void SetUnitType (Unit.UnitType unitType) {
		InitializeIfNeeded ();

		this.unitType = unitType;

		// Hide all units
		unitProfilePicBZ.SetActive (false);
		unitProfilePicFT.SetActive (false);
		unitProfilePicGL.SetActive (false);
		unitProfilePicGR.SetActive (false);
		unitProfilePicMG.SetActive (false);

		// Show appropriate unit
		switch (this.unitType) {
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

	public void SetLevel (int level) {
		this.level = level;

		// Hide all level icons
		level1.SetActive (false);
		level2.SetActive (false);
		level3.SetActive (false);

		// Show appropriate level icon
		switch (this.level) {
		case 0: 
			// Hide all level icons
			break;
		case 1:
			level1.SetActive (true);
			level1.transform.SetAsLastSibling ();
			break;
		case 2:
			level2.SetActive (true);
			level2.transform.SetAsLastSibling ();
			break;
		case 3:
			level3.SetActive (true);
			level3.transform.SetAsLastSibling ();
			break;
		}
	}
}
