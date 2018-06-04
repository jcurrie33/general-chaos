using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour {

	public enum InventoryItemType {
		INVENTORY_ITEM_TYPE_DEFAULT = 0,
		INVENTORY_ITEM_TYPE_WEAPON,
		INVENTORY_ITEM_TYPE_ARMOR,
		INVENTORY_ITEM_TYPE_MEDKIT
	};

	public enum InventoryItemState {
		INVENTORY_ITEM_STATE_NOT_PURCHASED = 0,
		INVENTORY_ITEM_STATE_PURCHASED,
		INVENTORY_ITEM_STATE_EQUIPPED
	};

	public enum InventoryItemRarity {
		INVENTORY_ITEM_RARITY_COMMON = 0,
		INVENTORY_ITEM_RARITY_RARE,
		INVENTORY_ITEM_RARITY_EPIC
	};
		
	public InventoryItemType type = InventoryItemType.INVENTORY_ITEM_TYPE_DEFAULT;
	public InventoryItemState state = InventoryItemState.INVENTORY_ITEM_STATE_NOT_PURCHASED;
	public InventoryItemRarity rarity = InventoryItemRarity.INVENTORY_ITEM_RARITY_COMMON;
	public int level = 1;
	public int unallocatedStatPoints = 0;
	public int count = 1;
	public int doubleDollarCostToPurchase = 2000;
	public int doubleDollarCostToUpgradeToLevel2 = 1000;
	public int doubleDollarCostToUpgradeToLevel3 = 5000;
	public int diamondCostToPurchase = 0;
	public int diamondCostToUpgradeToLevel2 = 0;
	public int diamondCostToUpgradeToLevel3 = 0;
	public UnitDataObject equippedByUnit = null;

	private float SELL_PERCENTAGE = 0.5f;

	private TacticalCombat tcScript;

	private bool isInitialized = false;
	void InitializeIfNeeded () {
		if (this.tcScript == null) {
			Camera mainCamera = Camera.main;
			this.tcScript = mainCamera.GetComponent<TacticalCombat> ();
		}
	}

	// Use this for initialization
	void Start () {
		InitializeIfNeeded ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Purchase () {
		InitializeIfNeeded ();

		if (this.tcScript.doubleDollars >= doubleDollarCostToPurchase
			&& this.tcScript.diamonds >= diamondCostToPurchase) {

			this.tcScript.SubtractDoubleDollars (doubleDollarCostToPurchase);
			this.tcScript.SubtractDiamonds (diamondCostToPurchase);
			this.state = InventoryItemState.INVENTORY_ITEM_STATE_PURCHASED;
			this.level = 1;
			this.UpdateStats ();
		}
	}

	public void EquipToUnit (UnitDataObject unit) {
		InitializeIfNeeded ();

		this.equippedByUnit = unit;
		if (this.type == InventoryItemType.INVENTORY_ITEM_TYPE_ARMOR) {
			unit.equippedArmor = (Armor)this;
		} else if (this.type == InventoryItemType.INVENTORY_ITEM_TYPE_WEAPON) {
			unit.equippedWeapon = (Weapon)this;
		}

		this.state = InventoryItemState.INVENTORY_ITEM_STATE_EQUIPPED;
		this.level = 1;
		this.UpdateStats ();
	}

	public void Sell () {
		InitializeIfNeeded ();

		if (this.level >= 3) {
			this.tcScript.AddDoubleDollars (Mathf.FloorToInt (SELL_PERCENTAGE * doubleDollarCostToUpgradeToLevel3));
			this.tcScript.AddDiamonds (Mathf.FloorToInt (SELL_PERCENTAGE * diamondCostToUpgradeToLevel3));
		}
		if (this.level >= 2) {
			this.tcScript.AddDoubleDollars (Mathf.FloorToInt (SELL_PERCENTAGE * doubleDollarCostToUpgradeToLevel2));
			this.tcScript.AddDiamonds (Mathf.FloorToInt (SELL_PERCENTAGE * diamondCostToUpgradeToLevel2));
		}
		if (this.level >= 1) {
			this.tcScript.AddDoubleDollars (Mathf.FloorToInt (SELL_PERCENTAGE * doubleDollarCostToPurchase));
			this.tcScript.AddDiamonds (Mathf.FloorToInt (SELL_PERCENTAGE * diamondCostToPurchase));
		}
		this.state = InventoryItemState.INVENTORY_ITEM_STATE_NOT_PURCHASED;
	}

	public void PrepareToUpgrade () {
		InitializeIfNeeded ();

		if (this.level == 1) {
			if (this.tcScript.doubleDollars >= doubleDollarCostToUpgradeToLevel2
				&& this.tcScript.diamonds >= diamondCostToUpgradeToLevel2) {

				this.level++;
				this.UpdateUnallocatedStatPoints ();
				this.UpdateStats ();
			}
		} else if (this.level == 2) {
			if (this.tcScript.doubleDollars >= doubleDollarCostToUpgradeToLevel3
				&& this.tcScript.diamonds >= diamondCostToUpgradeToLevel3) {

				this.level++;
				this.UpdateUnallocatedStatPoints ();
				this.UpdateStats ();
			}
		}
	}

	public void ConfirmUpgrade () {
		InitializeIfNeeded ();

		// NOTE: By this point, the level is already set to the next level
		if (this.level == 2) {
			if (this.tcScript.doubleDollars >= doubleDollarCostToUpgradeToLevel2
			    && this.tcScript.diamonds >= diamondCostToUpgradeToLevel2) {

				this.tcScript.SubtractDoubleDollars (doubleDollarCostToUpgradeToLevel2);
				this.tcScript.SubtractDiamonds (diamondCostToUpgradeToLevel2);
			}
		} else if (this.level == 3) {
			if (this.tcScript.doubleDollars >= doubleDollarCostToUpgradeToLevel3
			    && this.tcScript.diamonds >= diamondCostToUpgradeToLevel3) {

				this.tcScript.SubtractDoubleDollars (doubleDollarCostToUpgradeToLevel3);
				this.tcScript.SubtractDiamonds (diamondCostToUpgradeToLevel3);
			}
		}
	}

	public void CancelUpgrade () {
		InitializeIfNeeded ();

		if (this.level == 2) {
			this.level--;
			this.UpdateStats ();
		} else if (this.level == 3) {
			this.level--;
			this.UpdateStats ();
		}
	}

	public void UpdateUnallocatedStatPoints () {
		this.unallocatedStatPoints = GetStatPointsAvailableToAllocate ();
	}

	public virtual void UpdateStats () {
		// Needs to be extended by child class
	}

	public virtual int GetStatPointsAvailableToAllocate () {
		// Needs to be extended by child class
		return 0;
	}

	public void InitAsRandomItemWithTypeAndRarity (InventoryItemType type, InventoryItemRarity rarity) {
		// Needs to be extended by child class
	}


}
