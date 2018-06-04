using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour {

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
    void Start()
    {
        InitializeIfNeeded();
    }
	
	// Update is called once per frame
	void Update () {
		
    }

    public void Button_DD1_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        this.tcScript.AddDoubleDollars(500);
    }

    public void Button_DD2_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        this.tcScript.AddDoubleDollars(1650);
    }

    public void Button_DD3_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        this.tcScript.AddDoubleDollars(5750);
    }

    public void Button_CC1_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        this.tcScript.AddDiamonds(10);
    }

    public void Button_CC2_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        this.tcScript.AddDiamonds(36);
    }

    public void Button_CC3_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        this.tcScript.AddDiamonds(90);
    }

    public void Button_CC4_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        this.tcScript.AddDiamonds(210);
    }

    public void Button_CC5_Tapped()
    {
        this.tcScript.PlayUIAudioClip();

        this.tcScript.AddDiamonds(580);
    }
}
