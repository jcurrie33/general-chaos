using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitDataObject {

	public Unit.UnitType unitType = Unit.UnitType.UNIT_NONE;
	public int level = 1;
	public int assignedTeam = 0;
	public Weapon equippedWeapon;
	public Armor equippedArmor;

	public int health = 0;
	public int speed = 0;
	public int baseHealth = 0;
	public int baseSpeed = 0;

	private bool isInitialized = false;
	public void InitializeIfNeeded () {
		if (!isInitialized) {
			isInitialized = true;
		
			switch (this.unitType) {
			case Unit.UnitType.UNIT_NONE:
				// Set nothing
				break;
			case Unit.UnitType.UNIT_BAZOOKA:
				this.baseHealth = Mathf.FloorToInt (UnitStatsData.UNIT_BAZOOKA_HEALTH);
				this.baseSpeed = Mathf.FloorToInt (UnitStatsData.UNIT_BAZOOKA_SPEED * 100.0f);
				break;
			case Unit.UnitType.UNIT_FLAMETHROWER:
				this.baseHealth = Mathf.FloorToInt (UnitStatsData.UNIT_FLAMETHROWER_HEALTH);
				this.baseSpeed = Mathf.FloorToInt (UnitStatsData.UNIT_FLAMETHROWER_SPEED * 100.0f);
				break;
			case Unit.UnitType.UNIT_GRENADELAUNCHER:
				this.baseHealth = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADELAUNCHER_HEALTH);
				this.baseSpeed = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADELAUNCHER_SPEED * 100.0f);
				break;
			case Unit.UnitType.UNIT_GRENADIER:
				this.baseHealth = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADIER_HEALTH);
				this.baseSpeed = Mathf.FloorToInt (UnitStatsData.UNIT_GRENADIER_SPEED * 100.0f);
				break;
			case Unit.UnitType.UNIT_MACHINEGUNNER:
				this.baseHealth = Mathf.FloorToInt (UnitStatsData.UNIT_MACHINEGUNNER_HEALTH);
				this.baseSpeed = Mathf.FloorToInt (UnitStatsData.UNIT_MACHINEGUNNER_SPEED * 100.0f);
				break;
			}

			this.health = this.baseHealth;
			this.speed = this.baseSpeed;
		}
	}
}
