using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitStatsData {

	public const string UNIT_MACHINEGUNNER_NAME = "Machinegunner";
	public const float UNIT_MACHINEGUNNER_HEALTH = 100.0f;
	public const float UNIT_MACHINEGUNNER_ATTACK = 24.0f;
	public const float UNIT_MACHINEGUNNER_FIRERATE = 80.0f;
	public const float UNIT_MACHINEGUNNER_PROJECTILE_SPEED = 61.0f;
	public const float UNIT_MACHINEGUNNER_RANGE = 24.0f;
	public const float UNIT_MACHINEGUNNER_SPEED = 45.0f * 0.02f * 1.2f * 0.7f;
	public const float UNIT_MACHINEGUNNER_MELEE = 5.0f;

	public const string UNIT_BAZOOKA_NAME = "Bazooka";
	public const float UNIT_BAZOOKA_HEALTH = 79.0f;
	public const float UNIT_BAZOOKA_ATTACK = 85.0f;
	public const float UNIT_BAZOOKA_FIRERATE = 10.0f;
	public const float UNIT_BAZOOKA_PROJECTILE_SPEED = 66.0f;
	public const float UNIT_BAZOOKA_RANGE = 100.0f;
	public const float UNIT_BAZOOKA_SPEED = 41.0f * 0.02f * 1.2f * 0.7f;
	public const float UNIT_BAZOOKA_MELEE = 5.0f;

	public const string UNIT_GRENADIER_NAME = "Grenadier";
	public const float UNIT_GRENADIER_HEALTH = 100.0f;
	public const float UNIT_GRENADIER_ATTACK = 65.0f;
	public const float UNIT_GRENADIER_FIRERATE = 10.0f;
	public const float UNIT_GRENADIER_PROJECTILE_SPEED = 32.0f;
	public const float UNIT_GRENADIER_RANGE = 32.0f;
	public const float UNIT_GRENADIER_SPEED = 60.0f * 0.02f * 1.2f * 0.7f;
	public const float UNIT_GRENADIER_MELEE = 5.0f;

	public const string UNIT_FLAMETHROWER_NAME = "Flamethrower";
	public const float UNIT_FLAMETHROWER_HEALTH = 100.0f;
	public const float UNIT_FLAMETHROWER_ATTACK = 40.0f;
	public const float UNIT_FLAMETHROWER_FIRERATE = 71.0f;
	public const float UNIT_FLAMETHROWER_PROJECTILE_SPEED = 61.0f;
    public const float UNIT_FLAMETHROWER_RANGE = 8.0f;//10.0f;
	public const float UNIT_FLAMETHROWER_SPEED = 55.0f * 0.02f * 1.2f * 0.7f;
	public const float UNIT_FLAMETHROWER_MELEE = 5.0f;

	public const string UNIT_GRENADELAUNCHER_NAME = "Grenade Launcher";
	public const float UNIT_GRENADELAUNCHER_HEALTH = 56.0f;
	public const float UNIT_GRENADELAUNCHER_ATTACK = 20.0f;//51.0f;
	public const float UNIT_GRENADELAUNCHER_FIRERATE = 35.0f;//18.0f;
	public const float UNIT_GRENADELAUNCHER_PROJECTILE_SPEED = 18.0f;
	public const float UNIT_GRENADELAUNCHER_RANGE = 40.0f;
	public const float UNIT_GRENADELAUNCHER_SPEED = 41.0f * 0.02f * 1.2f * 0.7f;
	public const float UNIT_GRENADELAUNCHER_MELEE = 5.0f;

	public const string UNIT_MEDIC_NAME = "Medic";
	public const float UNIT_MEDIC_HEALTH = 100.0f;
	public const float UNIT_MEDIC_ATTACK = 16.0f;
	public const float UNIT_MEDIC_FIRERATE = 16.0f;
	public const float UNIT_MEDIC_PROJECTILE_SPEED = 43.0f;
	public const float UNIT_MEDIC_RANGE = 20.0f;
	public const float UNIT_MEDIC_SPEED = 50.0f * 0.02f * 1.2f * 0.7f;
	public const float UNIT_MEDIC_MELEE = 5.0f;

	public const string UNIT_KAMIKAZE_NAME = "Kamikaze";
	public const float UNIT_KAMIKAZE_HEALTH = 100.0f;
	public const float UNIT_KAMIKAZE_ATTACK = 1.0f;
	public const float UNIT_KAMIKAZE_FIRERATE = 1.0f;
	public const float UNIT_KAMIKAZE_PROJECTILE_SPEED = 40.0f;
	public const float UNIT_KAMIKAZE_RANGE = 9.0f;
	public const float UNIT_KAMIKAZE_SPEED = 50.0f * 0.02f * 1.2f * 0.7f;//64.0f;
	public const float UNIT_KAMIKAZE_MELEE = 0.0f;

	public const string UNIT_BRAWLER_NAME = "Brawler";
	public const float UNIT_BRAWLER_HEALTH = 20.0f;
	public const float UNIT_BRAWLER_ATTACK = 40.0f;
	public const float UNIT_BRAWLER_FIRERATE = 0.0f;
	public const float UNIT_BRAWLER_PROJECTILE_SPEED = 0.0f;
	public const float UNIT_BRAWLER_RANGE = 0.0f;
	public const float UNIT_BRAWLER_SPEED = 86.0f * 0.02f * 1.2f * 0.7f;
	public const float UNIT_BRAWLER_MELEE = 15.0f;

	public const string UNIT_SWAT_NAME = "Swat";
	public const float UNIT_SWAT_HEALTH = 100.0f;
	public const float UNIT_SWAT_ATTACK = 30.0f;
	public const float UNIT_SWAT_FIRERATE = 20.0f;
	public const float UNIT_SWAT_PROJECTILE_SPEED = 40.0f;
	public const float UNIT_SWAT_RANGE = 22.0f;
	public const float UNIT_SWAT_SPEED = 20.0f * 0.02f * 1.2f * 0.7f;
	public const float UNIT_SWAT_MELEE = 5.0f;



	public float health = -1.0f;
	public float attack = -1.0f;
	public float firerate = -1.0f;
	public float projectileSpeed = -1.0f;
	public float range = -1.0f;
	public float speed = -1.0f;
	public float melee = -1.0f;

	public static Dictionary<string, UnitStatsData> unitStatsDataDictionary;


		
	public static void SetupDataDictionary () {
		unitStatsDataDictionary = new Dictionary<string, UnitStatsData> ();

		UnitStatsData machinegunnerData = new UnitStatsData ();
		machinegunnerData.health = UNIT_MACHINEGUNNER_HEALTH;
		machinegunnerData.attack = UNIT_MACHINEGUNNER_ATTACK;
		machinegunnerData.firerate = UNIT_MACHINEGUNNER_FIRERATE;
		machinegunnerData.projectileSpeed = UNIT_MACHINEGUNNER_PROJECTILE_SPEED;
		machinegunnerData.range = UNIT_MACHINEGUNNER_RANGE;
		machinegunnerData.speed = UNIT_MACHINEGUNNER_SPEED;
		machinegunnerData.melee = UNIT_MACHINEGUNNER_MELEE;
		unitStatsDataDictionary.Add (UNIT_MACHINEGUNNER_NAME, machinegunnerData);

		UnitStatsData bazookaData = new UnitStatsData ();
		bazookaData.health = UNIT_BAZOOKA_HEALTH;
		bazookaData.attack = UNIT_BAZOOKA_ATTACK;
		bazookaData.firerate = UNIT_BAZOOKA_FIRERATE;
		bazookaData.projectileSpeed = UNIT_BAZOOKA_PROJECTILE_SPEED;
		bazookaData.range = UNIT_BAZOOKA_RANGE;
		bazookaData.speed = UNIT_BAZOOKA_SPEED;
		bazookaData.melee = UNIT_BAZOOKA_MELEE;
		unitStatsDataDictionary.Add (UNIT_BAZOOKA_NAME, bazookaData);

		UnitStatsData grenadierData = new UnitStatsData ();
		grenadierData.health = UNIT_GRENADIER_HEALTH;
		grenadierData.attack = UNIT_GRENADIER_ATTACK;
		grenadierData.firerate = UNIT_GRENADIER_FIRERATE;
		grenadierData.projectileSpeed = UNIT_GRENADIER_PROJECTILE_SPEED;
		grenadierData.range = UNIT_GRENADIER_RANGE;
		grenadierData.speed = UNIT_GRENADIER_SPEED;
		grenadierData.melee = UNIT_GRENADIER_MELEE;
		unitStatsDataDictionary.Add (UNIT_GRENADIER_NAME, grenadierData);

		UnitStatsData flamethrowerData = new UnitStatsData ();
		flamethrowerData.health = UNIT_FLAMETHROWER_HEALTH;
		flamethrowerData.attack = UNIT_FLAMETHROWER_ATTACK;
		flamethrowerData.firerate = UNIT_FLAMETHROWER_FIRERATE;
		flamethrowerData.projectileSpeed = UNIT_FLAMETHROWER_PROJECTILE_SPEED;
		flamethrowerData.range = UNIT_FLAMETHROWER_RANGE;
		flamethrowerData.speed = UNIT_FLAMETHROWER_SPEED;
		flamethrowerData.melee = UNIT_FLAMETHROWER_MELEE;
		unitStatsDataDictionary.Add (UNIT_FLAMETHROWER_NAME, flamethrowerData);

		UnitStatsData grenadeLauncherData = new UnitStatsData ();
		grenadeLauncherData.health = UNIT_GRENADELAUNCHER_HEALTH;
		grenadeLauncherData.attack = UNIT_GRENADELAUNCHER_ATTACK;
		grenadeLauncherData.firerate = UNIT_GRENADELAUNCHER_FIRERATE;
		grenadeLauncherData.projectileSpeed = UNIT_GRENADELAUNCHER_PROJECTILE_SPEED;
		grenadeLauncherData.range = UNIT_GRENADELAUNCHER_RANGE;
		grenadeLauncherData.speed = UNIT_GRENADELAUNCHER_SPEED;
		grenadeLauncherData.melee = UNIT_GRENADELAUNCHER_MELEE;
		unitStatsDataDictionary.Add (UNIT_GRENADELAUNCHER_NAME, grenadeLauncherData);

		UnitStatsData medicData = new UnitStatsData ();
		medicData.health = UNIT_MEDIC_HEALTH;
		medicData.attack = UNIT_MEDIC_ATTACK;
		medicData.firerate = UNIT_MEDIC_FIRERATE;
		medicData.projectileSpeed = UNIT_MEDIC_PROJECTILE_SPEED;
		medicData.range = UNIT_MEDIC_RANGE;
		medicData.speed = UNIT_MEDIC_SPEED;
		medicData.melee = UNIT_MEDIC_MELEE;
		unitStatsDataDictionary.Add (UNIT_MEDIC_NAME, medicData);

		UnitStatsData kamikazeData = new UnitStatsData ();
		kamikazeData.health = UNIT_KAMIKAZE_HEALTH;
		kamikazeData.attack = UNIT_KAMIKAZE_ATTACK;
		kamikazeData.firerate = UNIT_KAMIKAZE_FIRERATE;
		kamikazeData.projectileSpeed = UNIT_KAMIKAZE_PROJECTILE_SPEED;
		kamikazeData.range = UNIT_KAMIKAZE_RANGE;
		kamikazeData.speed = UNIT_KAMIKAZE_SPEED;
		kamikazeData.melee = UNIT_KAMIKAZE_MELEE;
		unitStatsDataDictionary.Add (UNIT_KAMIKAZE_NAME, kamikazeData);

		UnitStatsData brawlerData = new UnitStatsData ();
		brawlerData.health = UNIT_BRAWLER_HEALTH;
		brawlerData.attack = UNIT_BRAWLER_ATTACK;
		brawlerData.firerate = UNIT_BRAWLER_FIRERATE;
		brawlerData.projectileSpeed = UNIT_BRAWLER_PROJECTILE_SPEED;
		brawlerData.range = UNIT_BRAWLER_RANGE;
		brawlerData.speed = UNIT_BRAWLER_SPEED;
		brawlerData.melee = UNIT_BRAWLER_MELEE;
		unitStatsDataDictionary.Add (UNIT_BRAWLER_NAME, brawlerData);

		UnitStatsData swatData = new UnitStatsData ();
		swatData.health = UNIT_SWAT_HEALTH;
		swatData.attack = UNIT_SWAT_ATTACK;
		swatData.firerate = UNIT_SWAT_FIRERATE;
		swatData.projectileSpeed = UNIT_SWAT_PROJECTILE_SPEED;
		swatData.range = UNIT_SWAT_RANGE;
		swatData.speed = UNIT_SWAT_SPEED;
		swatData.melee = UNIT_SWAT_MELEE;
		unitStatsDataDictionary.Add (UNIT_SWAT_NAME, swatData);
	}
}
