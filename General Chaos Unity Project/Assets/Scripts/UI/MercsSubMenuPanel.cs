using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercsSubMenuPanel : MonoBehaviour {

    private TacticalCombat tcScript;

    private bool isInitialized = false;
    void InitializeIfNeeded()
    {
        if (this.tcScript == null)
        {
            Camera mainCamera = Camera.main;
            this.tcScript = mainCamera.GetComponent<TacticalCombat>();
        }

        if (!isInitialized)
        {
            isInitialized = true;
        }
    }

    public void ShowPanel()
    {
        InitializeIfNeeded();

        this.gameObject.SetActive(true);
        this.tcScript.isMenuOpen = true;
    }

    public void HidePanel()
    {
        InitializeIfNeeded();

        this.gameObject.SetActive(false);
    }

    public void Button_Close_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        HidePanel();
    }

	// Use this for initialization
	void Start () {
        InitializeIfNeeded();
	}
	
	// Update is called once per frame
	void Update () {
		
    }

    public void MercsButton_Tapped()
    {
        tcScript.PlayUIAudioClip();

        tcScript.ShowUnitSelectionPanel();
        tcScript.unitSelectionPanel.transform.SetAsLastSibling();
        tcScript.navbarPanel.transform.SetAsLastSibling();

    }

    public void TeamsButton_Tapped()
    {
        tcScript.PlayUIAudioClip();

        tcScript.ShowTeamPanel();
    }

    public void ArmouryButton_Tapped()
    {
        tcScript.PlayUIAudioClip();

    }
}
