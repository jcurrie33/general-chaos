using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	private TacticalCombat tcScript;
	public List<InventoryItem> inventoryItemList;

	// Use this for initialization
	void Start () {
		Camera mainCamera = Camera.main;
		this.tcScript = mainCamera.GetComponent<TacticalCombat> ();

		this.inventoryItemList = new List<InventoryItem> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
