using UnityEngine;
using System.Collections;

public class Armor : InventoryItem {

	public enum ArmorClass {
		ARMOR_CLASS_DEFAULT = 0,
		ARMOR_CLASS_BULLET_RESISTANT,
		ARMOR_CLASS_FLAME_RESISTANT,
		ARMOR_CLASS_EXPLOSION_RESISTANT
	};

	public enum ArmorVariant {
		ARMOR_VARIANT_DEFAULT = 0,
		ARMOR_VARIANT_LIGHT,
		ARMOR_VARIANT_HEAVY
	};

	public ArmorClass armorClass = ArmorClass.ARMOR_CLASS_DEFAULT;
	public ArmorVariant variant = ArmorVariant.ARMOR_VARIANT_DEFAULT;
	public int health = 50;
	public int bulletResistance = 0;
	public int flameResistance = 0;
	public int explosionResistance = 0;
	public int baseBulletResistance = 0;
	public int baseFlameResistance = 0;
	public int baseExplosionResistance = 0;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string Name () {
		Utilities.DebugLog ("Armor.Name ()");
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

		// Add variant description, eg "Light "
		if (this.variant == ArmorVariant.ARMOR_VARIANT_DEFAULT) {
			// Add nothing if Default
		} else if (this.variant == ArmorVariant.ARMOR_VARIANT_LIGHT) {
			name += "Light ";
		} else if (this.variant == ArmorVariant.ARMOR_VARIANT_HEAVY) {
			name += "Heavy ";
		}

		// Add class description, eg "Flame-Resistant "
		if (this.armorClass == ArmorClass.ARMOR_CLASS_DEFAULT) {
			// Add nothing if Default
		} else if (this.armorClass == ArmorClass.ARMOR_CLASS_BULLET_RESISTANT) {
			name += "Bullet-Resistant ";
		} else if (this.armorClass == ArmorClass.ARMOR_CLASS_EXPLOSION_RESISTANT) {
			name += "Explosion-Resistant ";
		} else if (this.armorClass == ArmorClass.ARMOR_CLASS_FLAME_RESISTANT) {
			name += "Flame-Resistant ";
		}

		// Add "Armor"
		name += "Armor";

		return name;
	}

	// Over-ridden from base class
	public override void UpdateStats () {
		// Update health
		if (this.variant == ArmorVariant.ARMOR_VARIANT_DEFAULT) {
			this.health = 75;
		} else if (this.variant == ArmorVariant.ARMOR_VARIANT_LIGHT) {
			this.health = 50;
		} else if (this.variant == ArmorVariant.ARMOR_VARIANT_HEAVY) {
			this.health = 100;
		}

		// Determine the base stat amount to apply
		int baseStatAmount = 0;
		if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_COMMON) {
			baseStatAmount = 20;
		} else if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_RARE) {
			baseStatAmount = 40;
		} else if (this.rarity == InventoryItemRarity.INVENTORY_ITEM_RARITY_EPIC) {
			baseStatAmount = 80;
		}

		// Apply the base stat amount to the appropriate stat
		if (this.armorClass == ArmorClass.ARMOR_CLASS_DEFAULT) {
			this.baseBulletResistance = Mathf.CeilToInt (baseStatAmount / 3.0f);
			this.baseFlameResistance = Mathf.CeilToInt (baseStatAmount / 3.0f);
			this.baseExplosionResistance = Mathf.CeilToInt (baseStatAmount / 3.0f);
		} else if (this.armorClass == ArmorClass.ARMOR_CLASS_BULLET_RESISTANT) {
			this.baseBulletResistance = baseStatAmount;
			this.baseFlameResistance = 0;
			this.baseExplosionResistance = 0;
		} else if (this.armorClass == ArmorClass.ARMOR_CLASS_FLAME_RESISTANT) {
			this.baseBulletResistance = 0;
			this.baseFlameResistance = baseStatAmount;
			this.baseExplosionResistance = 0;
		} else if (this.armorClass == ArmorClass.ARMOR_CLASS_EXPLOSION_RESISTANT) {
			this.baseBulletResistance = 0;
			this.baseFlameResistance = 0;
			this.baseExplosionResistance = baseStatAmount;
		}

		// Determine how many stat points are available to allocate
		int statPointsAvailableToAllocate = GetStatPointsAvailableToAllocate ();

		// Determine how many stat points have been allocated
		int totalAllocatedPoints = 0;
		totalAllocatedPoints += this.bulletResistance;
		totalAllocatedPoints += this.flameResistance;
		totalAllocatedPoints += this.explosionResistance;

		// If allocated points are less than the base, set to the base
		if (totalAllocatedPoints < baseStatAmount) {
			this.bulletResistance = this.baseBulletResistance;
			this.flameResistance = this.baseFlameResistance;
			this.explosionResistance = this.baseExplosionResistance;
		}

		// Determine how many stat points have been allocated NOW
		int updatedTotalAllocatedPoints = 0;
		updatedTotalAllocatedPoints += this.bulletResistance;
		updatedTotalAllocatedPoints += this.flameResistance;
		updatedTotalAllocatedPoints += this.explosionResistance;

		// If updated allocated points is less than the base plus the total that can be allocated, determine how many
		if (updatedTotalAllocatedPoints < baseStatAmount + statPointsAvailableToAllocate) {
			this.unallocatedStatPoints = baseStatAmount + statPointsAvailableToAllocate - updatedTotalAllocatedPoints;
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

	public void AllocateStatPointToBulletResistance () {
		if (unallocatedStatPoints > 0 && this.bulletResistance < 100) {
			this.unallocatedStatPoints--;
			this.bulletResistance++;
		}
	}

	public void AllocateStatPointToFlameResistance () {
		if (unallocatedStatPoints > 0 && this.flameResistance < 100) {
			this.unallocatedStatPoints--;
			this.flameResistance++;
		}
	}

	public void AllocateStatPointToExplosionResistance () {
		if (unallocatedStatPoints > 0 && this.explosionResistance < 100) {
			this.unallocatedStatPoints--;
			this.explosionResistance++;
		}
	}

	public void UnallocateAllStatPoints () {
		this.bulletResistance = this.baseBulletResistance;
		this.flameResistance = this.baseFlameResistance;
		this.explosionResistance = this.baseExplosionResistance;
		this.unallocatedStatPoints = GetStatPointsAvailableToAllocate ();
	}
}
