using UnityEngine;
using System.Collections;

public class Weapon : InventoryItem {

	public enum WeaponClass {
		WEAPON_CLASS_DEFAULT = 0,
		WEAPON_CLASS_MACHINEGUN,
		WEAPON_CLASS_GRENADE,
		WEAPON_CLASS_GRENADELAUNCHER,
		WEAPON_CLASS_FLAMETHROWER,
		WEAPON_CLASS_BAZOOKA
	};

	public WeaponClass weaponClass = WeaponClass.WEAPON_CLASS_DEFAULT;
	public int attack = 0;
	public int firerate = 0;
	public int range = 0;
	public int baseAttack = 0;
	public int baseFirerate = 0;
	public int baseRange = 0;



	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public string Name () {
		Utilities.DebugLog ("Weapon.Name ()");
		string name = "";

		// Add level description, eg "Level 1 "
		//name += "Level " + this.level + " ";

		// Add rarity description, eg "Epic "
		if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_COMMON) {
			// Add nothing if Common
		} else if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_RARE) {
			name += "Rare ";
		} else if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_EPIC) {
			name += "Epic ";
		}

		// Add class description, eg "Flamethrower"
		if (this.weaponClass == WeaponClass.WEAPON_CLASS_DEFAULT) {
			// Add nothing if Default
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_BAZOOKA) {
			if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_EPIC) {
				name += "Laser";
			} else {
				name += "Bazooka";
			}
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_FLAMETHROWER) {
			if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_EPIC) {
				name += "Blue Flamethrower";
			} else {
				name += "Flamethrower";
			}
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_GRENADE) {
			if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_EPIC) {
				name += "Frag Grenades";
			} else {
				name += "Grenades";
			}
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_GRENADELAUNCHER) {
			if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_EPIC) {
				name += "Bombardment Grenadelauncher";
			} else {
				name += "Grenadelauncher";
			}
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_MACHINEGUN) {
			if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_EPIC) {
				name += "Armor-Piercing Machinegun";
			} else {
				name += "Machinegun";
			}
		}

		return name;
	}

	// Over-ridden from base class
	public override void UpdateStats () {

		// Determine the base stat amount to add
		int baseStatAddition = 0;
		if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_COMMON) {
			baseStatAddition = -10;
		} else if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_RARE) {
			baseStatAddition = 0;
		} else if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_EPIC) {
			baseStatAddition = 10;
		}

		// Apply the base stat amount to the appropriate stat
		if (this.weaponClass == WeaponClass.WEAPON_CLASS_DEFAULT) {
			this.baseAttack = 50;
			this.baseFirerate = 50;
			this.baseRange = 50;
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_BAZOOKA) {
			this.baseAttack = Mathf.FloorToInt (UnitStatsData.UNIT_BAZOOKA_ATTACK);
			this.baseFirerate = Mathf.FloorToInt (UnitStatsData.UNIT_BAZOOKA_FIRERATE);
			this.baseRange = Mathf.FloorToInt (UnitStatsData.UNIT_BAZOOKA_RANGE);
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_FLAMETHROWER) {
			this.baseAttack = Mathf.FloorToInt (UnitStatsData.UNIT_FLAMETHROWER_ATTACK);
			this.baseFirerate = Mathf.FloorToInt (UnitStatsData.UNIT_FLAMETHROWER_FIRERATE);
			this.baseRange = Mathf.FloorToInt (UnitStatsData.UNIT_FLAMETHROWER_RANGE);
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_GRENADE) {
			this.baseAttack = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADIER_ATTACK);
			this.baseFirerate = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADIER_FIRERATE);
			this.baseRange = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADIER_RANGE);
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_GRENADELAUNCHER) {
			this.baseAttack = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADELAUNCHER_ATTACK);
			this.baseFirerate = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADELAUNCHER_FIRERATE);
			this.baseRange = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADELAUNCHER_RANGE);
		} else if (this.weaponClass == WeaponClass.WEAPON_CLASS_MACHINEGUN) {
			this.baseAttack = Mathf.FloorToInt (UnitStatsData.UNIT_MACHINEGUNNER_ATTACK);
			this.baseFirerate = Mathf.FloorToInt (UnitStatsData.UNIT_MACHINEGUNNER_FIRERATE);
			this.baseRange = Mathf.FloorToInt (UnitStatsData.UNIT_MACHINEGUNNER_RANGE);
		}
			
		// Add baseStatAddition to each base stat
		this.baseAttack += baseStatAddition;
		this.baseFirerate += baseStatAddition;
		this.baseRange += baseStatAddition;
		int totalBaseStatPoints = this.baseAttack + this.baseFirerate + this.baseRange;

		// Determine how many stat points are available to allocate
		int statPointsAvailableToAllocate = GetStatPointsAvailableToAllocate ();

		// Determine how many stat points have been allocated
		int totalAllocatedPoints = 0;
		totalAllocatedPoints += this.attack;
		totalAllocatedPoints += this.firerate;
		totalAllocatedPoints += this.range;

		// If allocated points are less than the base, set to the base
		if (totalAllocatedPoints < totalBaseStatPoints) {
			this.attack = this.baseAttack;
			this.firerate = this.baseFirerate;
			this.range = this.baseRange;
		}

		// Determine how many stat points have been allocated NOW
		int updatedTotalAllocatedPoints = 0;
		updatedTotalAllocatedPoints += this.attack;
		updatedTotalAllocatedPoints += this.firerate;
		updatedTotalAllocatedPoints += this.range;

		// If updated allocated points is less than the base plus the total that can be allocated, determine how many
		if (updatedTotalAllocatedPoints < totalBaseStatPoints + statPointsAvailableToAllocate) {
			this.unallocatedStatPoints = totalBaseStatPoints + statPointsAvailableToAllocate - updatedTotalAllocatedPoints;
		}
	}

	public override int GetStatPointsAvailableToAllocate () {
		// Determine how many stat points are available to allocate
		int statPointsAvailableToAllocate = 0;
		if (this.level == 1) {
			statPointsAvailableToAllocate = 0;
		} else if (this.level == 2) {
			statPointsAvailableToAllocate = 10;
		} else if (this.level == 3) {
			statPointsAvailableToAllocate = 20;
		}

		return statPointsAvailableToAllocate;
	}

	public void AllocateStatPointToAttack () {
		if (unallocatedStatPoints > 0 && this.attack < 100) {
			this.unallocatedStatPoints--;
			this.attack++;
		}
	}

	public void AllocateStatPointToFirerate () {
		if (unallocatedStatPoints > 0 && this.firerate < 100) {
			this.unallocatedStatPoints--;
			this.firerate++;
		}
	}

	public void AllocateStatPointToRange () {
		if (unallocatedStatPoints > 0 && this.range < 100) {
			this.unallocatedStatPoints--;
			this.range++;
		}
	}

	public void UnallocateAllStatPoints () {
		this.attack = this.baseAttack;
		this.firerate = this.baseFirerate;
		this.range = this.baseRange;
		this.unallocatedStatPoints = GetStatPointsAvailableToAllocate ();
	}
}
