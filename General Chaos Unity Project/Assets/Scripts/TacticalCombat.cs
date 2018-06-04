using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class TacticalCombat : MonoBehaviour {

	public static bool DEBUG_SHOULD_DISABLE_CLOSE_COMBAT = false;
	public static bool DEBUG_SHOULD_GIVE_AI_LOW_HEALTH = false;
	public static float DEBUG_AI_LOW_HEALTH_PERCENTAGE = 0.32f;
	public static bool DEBUG_SHOULD_AI_CONTROL_BOTH_SIDES = false;
	public static bool DEBUG_SHOULD_PLAYER_AI_AUTO_ATTACK_NEAREST_ENEMY = false; // Default setting = true

    private float perspectiveZoomSpeed = 0.01f;        // The rate of change of the field of view in perspective mode.
    private float orthoZoomSpeed = 0.01f;        // The rate of change of the orthographic size in orthographic mode.
    private float cameraPanSpeed = 0.01f;        // The rate of change of the orthographic size in orthographic mode.
    public const bool IS_CAMERA_AUTO_ZOOM_ENABLED = false;
    public const float CAMERA_ZOOM_TACTICAL_COMBAT = 6.0f;
    public const float CAMERA_ZOOM_TACTICAL_COMBAT_MIN = 2.0f; // How zoomed-in camera can get
    public const float CAMERA_ZOOM_TACTICAL_COMBAT_MAX = 6.0f; // How zoomed-out camera can get
    public const float CAMERA_ZOOM_CLOSE_COMBAT = 2.3f;//1.5f;
    public const float CAMERA_OFFSET_Y_CLOSE_COMBAT = 2.0f;//0.7f;
    public const float CAMERA_ZOOM_SPEED = 0.2f;//0.08f;
    public const float CAMERA_PAN_SPEED = 1.0f;//0.1f;

	public enum UIPanelType {
        UI_PANEL_TYPE_NONE                  = 0,
        UI_PANEL_TYPE_NAVBAR                = (1 << 0),
        UI_PANEL_TYPE_MAINMENU              = (1 << 1),
        UI_PANEL_TYPE_LEVELSELECTION        = (1 << 2),
        UI_PANEL_TYPE_STARTMISSION          = (1 << 3),
        UI_PANEL_TYPE_MISSIONOBJECTIVES     = (1 << 4),
        UI_PANEL_TYPE_TEAM                  = (1 << 5),
        UI_PANEL_TYPE_UNITSELECTION         = (1 << 6),
        UI_PANEL_TYPE_OPTIONS               = (1 << 7),
        UI_PANEL_TYPE_SHOP                  = (1 << 8),
        UI_PANEL_TYPE_LEVELCOMPLETION       = (1 << 9),
        UI_PANEL_TYPE_MERCS_SUBMENU         = (1 << 10)
	};

	public enum UnitConfiguration {
		UNIT_CONFIGURATION_2BAZ_2MG_1GR = 0,
		UNIT_CONFIGURATION_2MG_2FT_1BAZ,
		UNIT_CONFIGURATION_1BAZ_2GR_2GL,
		UNIT_CONFIGURATION_1MG_1FT_1BAZ_1MED_1GL,
		UNIT_CONFIGURATION_1K_1GR_1GL_1BR,
		UNIT_CONFIGURATION_1MED_2MG_2GL,
		UNIT_CONFIGURATION_COUNT
	};

	public enum Terrain {
		TERRAIN_CITY = 0,
		TERRAIN_DESERT,
	};
	
	public enum LevelObjective {
        LevelObjective_KillAllEnemies = 0,
        LevelObjective_WaitForReinforcements,
        LevelObjective_Destroy_Tank,
        LevelObjective_Destroy_Helicopter,
        LevelObjective_Destroy_SatelliteTruck,
        LevelObjective_Destroy_ArmoredVehicle,
        LevelObjective_Destroy_Limo,
        LevelObjective_Protect_Tank,
        LevelObjective_Protect_Helicopter,
        LevelObjective_Protect_SatelliteTruck,
        LevelObjective_Protect_ArmoredVehicle,
        LevelObjective_Protect_Limo,
    };

	public enum AudioType {
		AudioType_Unit_Selected,
		AudioType_Unit_Moved,
		AudioType_Unit_Attack,
		AudioType_Unit_TakeDamage,
		AudioType_Unit_Dying,
		AudioType_SFX_Attack,
		AudioType_SFX_Explode,
		AudioType_BGM_Default
	};

	public float levelObjectiveDuration;
	public bool isLevelComplete = false;
	public bool isLevelFailed = false;
	public bool isMenuOpen = false;
	public bool isCountdownActive = false;
	public bool wasHeldTapRegistered = false;

	public float currentZoom = 6.0f;
	public const float MAX_ZOOM = 6.0f;
	public const float MIN_ZOOM = 1.0f;

	// Global constants
	public const float TIME_TO_REGISTER_TAP_HOLD = 0.2f;
	public const float KAMIKAZE_KNOCKDOWN_RANGE = 4.0f;
	public const float KAMIKAZE_KNOCKDOWN_DAMAGE = 200.0f;

	public const float MIN_DISTANCE_TO_TRIGGER_CLOSE_COMBAT = 1.0f;//2.6f;
	public const float DELAY_BEFORE_TRIGGERING_CLOSE_COMBAT = 0.5f;//0.8f;
	public const float DELAY_BEFORE_SHOWING_LEVEL_COMPLETION_PANEL = 1.5f;

    // Close combat settings
    public const float MELEE_ATTACK_DAMAGE_PUSH_DISTANCE = 2.0f;//1.0f;
    public const float MELEE_ATTACK_LOW_DELAY_BEFORE_TAKING_DAMAGE = 0.1f;//0.15f;
    public const float MELEE_ATTACK_MEDIUM_DELAY_BEFORE_TAKING_DAMAGE = 0.1f;//0.20f;
    public const float MELEE_ATTACK_HIGH_DELAY_BEFORE_TAKING_DAMAGE = 0.1f;//0.30f;
    public const float MIN_DISTANCE_TO_TRIGGER_LOW_MELEE_ATTACK_DAMAGE = 1.8f;//0.85f;
    public const float MIN_DISTANCE_TO_TRIGGER_MEDIUM_MELEE_ATTACK_DAMAGE = 1.8f;//0.75f;
    public const float MIN_DISTANCE_TO_TRIGGER_HIGH_MELEE_ATTACK_DAMAGE = 1.8f;//0.95f;
    public const float MIN_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS = 0.6f;//0.28f;
	public const float MAX_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS = 2.5f;
    public const float MIN_TIME_BETWEEN_CLOSE_COMBAT_ROUNDS = 8.0f;
	public const float JOYSTICK_MAX_RADIUS = 130.0f;
	public const float JOYSTICK_MAX_TOUCH_RADIUS = 250.0f;
	public const float CLOSECOMBAT_UNIT_WALK_SPEED = 0.0008f;
    public const float CLOSECOMBAT_UNIT_JUMP_SPEED = 0.50f;//0.30f;
	public const float CLOSECOMBAT_UNIT_JUMP_GRAVITY = 0.130f;
	public const float CLOSECOMBAT_MIN_DELAY_BETWEEN_MULTIPLE_JUMPS = 0.09f;
	public const float JOYSTICK_MIN_RADIUS_TO_TRIGGER_WALK = 40.0f;
 	// How far up do we have to move to jump
	public const float CLOSECOMBAT_JOYSTICK_VERTICAL_JUMP_SENSITIVITY = 55.0f;
	// How far left or right to we have to move to jump left or right
	public const float CLOSECOMBAT_JOYSTICK_HORIZONTAL_JUMP_SENSITIVITY = 25.0f;

	public const int WAYPOINT_DIVISIONS = 100;
	public const bool SHOULD_ALLOW_MIRRORED_TERRAIN = true;
	public const bool SHOULD_FORCE_MIRRORED_TERRAIN = false;
	public Vector2 CAMERA_TARGET_POSITION; 
	public const float UNIT_SELECTION_WIDTH = 1.0f;
	public const float UNIT_SELECTION_HEIGHT = 2.0f;
	private const int INITIAL_CURRENCY_DOUBLEDOLLARS = 0;
	private const int INITIAL_CURRENCY_DIAMONDS = 0;

	// Audio Clips
	public List<AudioClip> FT_Selected_AudioClipsList;
	public List<AudioClip> FT_Moved_AudioClipsList;
	public List<AudioClip> FT_Attack_AudioClipsList;
	public List<AudioClip> FT_TakeDamage_AudioClipsList;
	public List<AudioClip> FT_Dying_AudioClipsList;
	public List<AudioClip> MG_Selected_AudioClipsList;
	public List<AudioClip> MG_Moved_AudioClipsList;
	public List<AudioClip> MG_Attack_AudioClipsList;
	public List<AudioClip> MG_TakeDamage_AudioClipsList;
	public List<AudioClip> MG_Dying_AudioClipsList;
	public List<AudioClip> BZ_Selected_AudioClipsList;
	public List<AudioClip> BZ_Moved_AudioClipsList;
	public List<AudioClip> BZ_Attack_AudioClipsList;
	public List<AudioClip> BZ_TakeDamage_AudioClipsList;
	public List<AudioClip> BZ_Dying_AudioClipsList;
	public List<AudioClip> GR_Selected_AudioClipsList;
	public List<AudioClip> GR_Moved_AudioClipsList;
	public List<AudioClip> GR_Attack_AudioClipsList;
	public List<AudioClip> GR_TakeDamage_AudioClipsList;
	public List<AudioClip> GR_Dying_AudioClipsList;
	public List<AudioClip> GL_Selected_AudioClipsList;
	public List<AudioClip> GL_Moved_AudioClipsList;
	public List<AudioClip> GL_Attack_AudioClipsList;
	public List<AudioClip> GL_TakeDamage_AudioClipsList;
	public List<AudioClip> GL_Dying_AudioClipsList;

	public List<AudioClip> FT_AttackSFX_AudioClipsList;
	public List<AudioClip> MG_AttackSFX_AudioClipsList;
	public List<AudioClip> BZ_AttackSFX_AudioClipsList;
	public List<AudioClip> GR_AttackSFX_AudioClipsList;
	public List<AudioClip> GL_AttackSFX_AudioClipsList;

	public List<AudioClip> BZ_ExplosionSFX_AudioClipsList;
	public List<AudioClip> GR_ExplosionSFX_AudioClipsList;
	public List<AudioClip> GL_ExplosionSFX_AudioClipsList;

	public List<AudioClip> BGM_AudioClipsList;

	public AudioClip UI_AudioClip;


	public List<Unit> unitsList;
	public List<UnitDataObject> unitDataObjectsList;
	public List<Weapon> weaponList;
	public List<Armor> armorList;
	public List<Projectile> projectilesList;
	public List<TerrainObstacle> obstaclesList;
	public List<GameObject> levelsList;

	public Dictionary<string, UnitStatsData> unitStatsDataDictionary;

	// GameObjects
	public GameObject unitNameText;
	public GameObject selectionArrow;
	public GameObject closeCombatCloud;

	public GameObject mainMenuPanel;
    public GameObject levelSelectionPanel;
    public GameObject missionObjectivesPanel;
	public GameObject unitSelectionPanel;
	public GameObject levelCompletionPanel;
	public GameObject closeCombatPanel;
	public GameObject debugPanel;
	public GameObject navbarPanel;
	public GameObject teamPanel;
    public GameObject startMissionPanel;
    public GameObject mercsSubMenuPanel;
    public GameObject shopPanel;

//	public GameObject mainMenuButton;
//	public GameObject mapButton;
//	public GameObject unitSelectionButton;
//	public GameObject optionsButton;
//	public GameObject shopButton;
	public GameObject desertTerrainBG;
	public GameObject cityTerrainBG;
	public Terrain currentTerrain;
	public bool isCurrentTerrainMirrored;

	// Prefabs
	public GameObject unitPrefab;
	public GameObject unitShadowPrefab;
	public GameObject bulletPrefab;
	public GameObject bazookaPrefab;
	public GameObject grenadePrefab;
	public GameObject grenadeLauncherPrefab;
	public GameObject invisibleProjectilePrefab;
	public GameObject explosionPrefab;
	public GameObject kamikazeExplosionPrefab;
	public GameObject medicParticlesPrefab;
	public GameObject flamethrowerParticlesPrefab;
	public GameObject targetXPrefab;
	public GameObject closeCombatRangeUIPrefab;
	public GameObject hitPointTextPrefab;
	
	public List<GameObject> obstaclePrefabsList;

    public LevelObjective[][] levelObjectives;
    public int[][] isObjectiveComplete;
	
	public int currentLevel = 1;
	public int selectedTeam = 1;
	public StartMissionPanel.LevelDifficulty selectedDifficulty = StartMissionPanel.LevelDifficulty.LEVEL_DIFFICULTY_EASY;
	public int maxUnlockLevel = 1;
	public float timeAtMouseDown = 0;
	public float timeAtLevelStart = 0;
	public float timeAtCloseCombatEnd = 0;
	public bool previousIsCloseCombatActive;
	public bool isCloseCombatActive = false;
	public bool isAIEnabled = true;
	public bool isPaused = false;
	public bool wasPausedFromDebug = false;
	public float cameraTargetZoom = 6.0f;
	public Vector3 cameraTargetPosition;

	// Items and currency
	public Inventory inventory;
	public int doubleDollars = 0;
	public int diamonds = 0;
	public int numMedpacks = 3;
	public bool isMedpackSelected = false;
    public float numShotsFired = 0;

	public bool isUIExpanded = false;

	public AnimatorOverrideController animController_MG;
	public AnimatorOverrideController animController_FT;
	public AnimatorOverrideController animController_BZ;
	public AnimatorOverrideController animController_GR;
	public AnimatorOverrideController animController_GL;

	private int uniqueLevelIdentifier = 1;
	private float timeToTriggerCloseCombat = 0.0f;
	public Unit closeCombatPlayerUnit;
	public Unit closeCombatEnemyUnit;
	private Vector2 touchStartPosition;
	private bool didTouchBeginWithinJoypad;

	// UI elements stored in Start() for later access
	public GameObject canvas;

    private GameObject buttonPause;
    private GameObject buttonUnpause;
    private GameObject buttonMedkit;
    private GameObject buttonMedkitSelected;
    private GameObject medkitCounter0;
    private GameObject medkitCounter1;
    private GameObject medkitCounter2;
    private GameObject medkitCounter3;
    private GameObject medkitCounter4;
    private GameObject medkitCounter5;
    private GameObject medkitCounter6;
    private GameObject medkitCounter7;
    private GameObject medkitCounter8;
    private GameObject medkitCounter9;
	private GameObject buttonMinimize;
	private GameObject enableDebugToggle;
	private GameObject debugPauseButton;
	private GameObject debugZoomInButton;
	private GameObject debugZoomOutButton;
	private GameObject debugUnlockButton;

	private GameObject countdown1;
	private GameObject countdown2;
	private GameObject countdown3;

	private GameObject playerHearts1;
	private GameObject playerHearts2;
	private GameObject playerHearts3;
	private GameObject playerHearts4;
	private GameObject playerHearts5;

	private GameObject enemyHearts1;
	private GameObject enemyHearts2;
	private GameObject enemyHearts3;
	private GameObject enemyHearts4;
	private GameObject enemyHearts5;

	private GameObject joystickBG;
	private GameObject joystick;

	private GameObject dollarsCurrencyText;
	private GameObject diamondsCurrencyText;

	private Unit previouslySelectedUnit;
	private int previouslySelectedUnitIndex;

	private UnitConfiguration currentUnitConfiguration;

	public Vector2 closeCombatOriginPosition;

	bool shouldCallDelayedReset = false;

	
	public static float OBSTACLE_COVER_RANGE = 2.4f;//120.0f;
	public static float OBSTACLE_COVER_ATTACK_MULTIPLIER = 1.5f;
	public static float OBSTACLE_COVER_RANGE_MULTIPLIER = 1.5f;
	public static float OBSTACLE_COVER_DAMAGE_MULTIPLIER = 0.5f;
	
	public static float VERTICAL_SQUASH_RATIO = 0.7f;
	public static float AdjustedDistance (Vector2 position1, Vector2 position2) {
		Utilities.DebugLog ("AdjustedDistance ()");
		float dx = position2.x - position1.x;
		float dy = (1.0f / VERTICAL_SQUASH_RATIO) * (position2.y - position1.y);
		return Mathf.Sqrt (dx * dx + dy * dy);
	}

	public static Vector2 PointInCollider (PolygonCollider2D collider) {
		Utilities.DebugLog ("PointInCollider ()");
		var bounds = collider.bounds;
		var center = bounds.center;
		
		float x = 0;
		float y = 0;
		int attempt = 0;
		do {
			x = UnityEngine.Random.Range(center.x - bounds.extents.x, center.x + bounds.extents.x);
			y = UnityEngine.Random.Range(center.y - bounds.extents.y, center.y + bounds.extents.y);
			attempt++;
		} while (!collider.OverlapPoint(new Vector2(x, y)) && attempt <= 100);
		Utilities.DebugLog("Attemps: " + attempt);
		
		return new Vector2(x, y);
	}

    public string GetLevelObjectiveDescription (LevelObjective levelObjective) {
        switch (levelObjective) {
            case LevelObjective.LevelObjective_KillAllEnemies:
                return "eliminate all resistance";
            case LevelObjective.LevelObjective_WaitForReinforcements:
                return "wait for reinforcements";
            case LevelObjective.LevelObjective_Destroy_Tank:
                return "destroy tank";
            case LevelObjective.LevelObjective_Destroy_Helicopter:
                return "destroy helicopter";
            case LevelObjective.LevelObjective_Destroy_SatelliteTruck:
                return "destroy satellite truck";
            case LevelObjective.LevelObjective_Destroy_ArmoredVehicle:
                return "destroy armored vehicle";
            case LevelObjective.LevelObjective_Destroy_Limo:
                return "destroy limo";
            case LevelObjective.LevelObjective_Protect_Tank:
                return "protect tank";
            case LevelObjective.LevelObjective_Protect_Helicopter:
                return "protect helicopter";
            case LevelObjective.LevelObjective_Protect_SatelliteTruck:
                return "protect satellite truck";
            case LevelObjective.LevelObjective_Protect_ArmoredVehicle:
                return "protect armored vehicle";
            case LevelObjective.LevelObjective_Protect_Limo:
                return "protect limo";
        }
        return "";
    }

	public AudioClip GetRandomAudioClipForUnitTypeWithAudioType (Unit.UnitType unitType, AudioType audioType) {
		int randomIndex = 0;
		AudioClip clip = null;
		if (unitType == Unit.UnitType.UNIT_FLAMETHROWER) {
            if (audioType == AudioType.AudioType_Unit_Selected) {
                if (FT_Selected_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, FT_Selected_AudioClipsList.Count);
                    clip = FT_Selected_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Moved) {
                if (FT_Moved_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, FT_Moved_AudioClipsList.Count);
                    clip = FT_Moved_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Attack) {
                if (FT_Attack_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, FT_Attack_AudioClipsList.Count);
                    clip = FT_Attack_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_TakeDamage) {
                if (FT_TakeDamage_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, FT_TakeDamage_AudioClipsList.Count);
                    clip = FT_TakeDamage_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Dying) {
                if (FT_Dying_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, FT_Dying_AudioClipsList.Count);
                    clip = FT_Dying_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_SFX_Attack) {
                if (FT_AttackSFX_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range (0, FT_AttackSFX_AudioClipsList.Count);
                    clip = FT_AttackSFX_AudioClipsList [randomIndex];
                }
                else
                {
                    clip = null;
                }
			}
		} else if (unitType == Unit.UnitType.UNIT_MACHINEGUNNER) {
            if (audioType == AudioType.AudioType_Unit_Selected) {
                if (MG_Selected_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, MG_Selected_AudioClipsList.Count);
                    clip = MG_Selected_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Moved) {
                if (MG_Moved_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, MG_Moved_AudioClipsList.Count);
                    clip = MG_Moved_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Attack) {
                if (MG_Attack_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, MG_Attack_AudioClipsList.Count);
                    clip = MG_Attack_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_TakeDamage) {
                if (MG_TakeDamage_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, MG_TakeDamage_AudioClipsList.Count);
                    clip = MG_TakeDamage_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Dying) {
                if (MG_Dying_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, MG_Dying_AudioClipsList.Count);
                    clip = MG_Dying_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_SFX_Attack) {
                if (MG_AttackSFX_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, MG_AttackSFX_AudioClipsList.Count);
                    clip = MG_AttackSFX_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
			}
		} else if (unitType == Unit.UnitType.UNIT_GRENADIER) {
            if (audioType == AudioType.AudioType_Unit_Selected) {
                if (GR_Selected_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GR_Selected_AudioClipsList.Count);
                    clip = GR_Selected_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Moved) {
                if (GR_Moved_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GR_Moved_AudioClipsList.Count);
                    clip = GR_Moved_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Attack) {
                if (GR_Attack_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GR_Attack_AudioClipsList.Count);
                    clip = GR_Attack_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_TakeDamage) {
                if (GR_TakeDamage_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GR_TakeDamage_AudioClipsList.Count);
                    clip = GR_TakeDamage_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Dying) {
                if (GR_Dying_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GR_Dying_AudioClipsList.Count);
                    clip = GR_Dying_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_SFX_Attack) {
                if (GR_AttackSFX_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GR_AttackSFX_AudioClipsList.Count);
                    clip = GR_AttackSFX_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_SFX_Explode) {
                if (GR_ExplosionSFX_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GR_ExplosionSFX_AudioClipsList.Count);
                    clip = GR_ExplosionSFX_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
			}
		} else if (unitType == Unit.UnitType.UNIT_GRENADELAUNCHER) {
            if (audioType == AudioType.AudioType_Unit_Selected) {
                if (GL_Selected_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GL_Selected_AudioClipsList.Count);
                    clip = GL_Selected_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Moved) {
                if (GL_Moved_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GL_Moved_AudioClipsList.Count);
                    clip = GL_Moved_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Attack) {
                if (GL_Attack_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GL_Attack_AudioClipsList.Count);
                    clip = GL_Attack_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_TakeDamage) {
                if (GL_TakeDamage_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GL_TakeDamage_AudioClipsList.Count);
                    clip = GL_TakeDamage_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Dying) {
                if (GL_Dying_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GL_Dying_AudioClipsList.Count);
                    clip = GL_Dying_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_SFX_Attack) {
                if (GL_AttackSFX_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GL_AttackSFX_AudioClipsList.Count);
                    clip = GL_AttackSFX_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_SFX_Explode) {
                if (GL_ExplosionSFX_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, GL_ExplosionSFX_AudioClipsList.Count);
                    clip = GL_ExplosionSFX_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
			}
		} else if (unitType == Unit.UnitType.UNIT_BAZOOKA) {
            if (audioType == AudioType.AudioType_Unit_Selected) {
                if (BZ_Selected_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, BZ_Selected_AudioClipsList.Count);
                    clip = BZ_Selected_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Moved) {
                if (BZ_Moved_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, BZ_Moved_AudioClipsList.Count);
                    clip = BZ_Moved_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Attack) {
                if (BZ_Attack_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, BZ_Attack_AudioClipsList.Count);
                    clip = BZ_Attack_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_TakeDamage) {
                if (BZ_TakeDamage_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, BZ_TakeDamage_AudioClipsList.Count);
                    clip = BZ_TakeDamage_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_Unit_Dying) {
                if (BZ_Dying_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, BZ_Dying_AudioClipsList.Count);
                    clip = BZ_Dying_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_SFX_Attack) {
                if (BZ_AttackSFX_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, BZ_AttackSFX_AudioClipsList.Count);
                    clip = BZ_AttackSFX_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
            } else if (audioType == AudioType.AudioType_SFX_Explode) {
                if (BZ_ExplosionSFX_AudioClipsList.Count > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, BZ_ExplosionSFX_AudioClipsList.Count);
                    clip = BZ_ExplosionSFX_AudioClipsList[randomIndex];
                }
                else
                {
                    clip = null;
                }
			}
		}

		// Background music
        if (audioType == AudioType.AudioType_BGM_Default) {
            if (BGM_AudioClipsList.Count > 0)
            {
                randomIndex = UnityEngine.Random.Range(0, BGM_AudioClipsList.Count);
                clip = BGM_AudioClipsList[randomIndex];
            }
            else
            {
                clip = null;
            }
		}

		return clip;
	}


	// The Awake function is called on all objects in the scene before any object's Start function is called.
	void Awake () {
		Application.targetFrameRate = 30;

		UnitStatsData.SetupDataDictionary ();

		unitDataObjectsList = new List<UnitDataObject> ();
		weaponList = new List<Weapon> ();
		armorList = new List<Armor> ();
		AddPlayerUnitDataObjectsToTeam (1);

	}


	// Use this for initialization
	void Start () {
		Utilities.DebugLog ("Start ()");
		// Disable screen dimming
		// TODO: Selectively set this so that screen CAN timeout while on menus
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

        levelObjectives = new LevelObjective[][] {
            new LevelObjective[] { LevelObjective.LevelObjective_KillAllEnemies }, // 1
            new LevelObjective[] { LevelObjective.LevelObjective_KillAllEnemies }, // 2
            new LevelObjective[] { LevelObjective.LevelObjective_KillAllEnemies }, // 3
            new LevelObjective[] { LevelObjective.LevelObjective_Destroy_Tank, LevelObjective.LevelObjective_KillAllEnemies }, // 4
            new LevelObjective[] { LevelObjective.LevelObjective_Protect_Tank }, // 5
            new LevelObjective[] { LevelObjective.LevelObjective_Destroy_Tank, LevelObjective.LevelObjective_KillAllEnemies }, // 6
            new LevelObjective[] { LevelObjective.LevelObjective_KillAllEnemies }, // 7
            new LevelObjective[] { LevelObjective.LevelObjective_KillAllEnemies }, // 8
            new LevelObjective[] { LevelObjective.LevelObjective_Protect_Tank, LevelObjective.LevelObjective_KillAllEnemies }, // 9
            new LevelObjective[] { LevelObjective.LevelObjective_KillAllEnemies }, // 10
            new LevelObjective[] { LevelObjective.LevelObjective_KillAllEnemies }, // 11
            new LevelObjective[] { LevelObjective.LevelObjective_KillAllEnemies } // 12
        };
        isObjectiveComplete = new int[][] {
            new int[] {0},
            new int[] {0},
            new int[] {0},
            new int[] {0,0},
            new int[] {0},
            new int[] {0,0},
            new int[] {0},
            new int[] {0},
            new int[] {0,0},
            new int[] {0},
            new int[] {0},
            new int[] {0},
        };

		// Store UI elements for later access

		CAMERA_TARGET_POSITION = new Vector2 (0, 0);

		unitsList = new List<Unit> ();
		projectilesList = new List<Projectile> ();
		obstaclesList = new List<TerrainObstacle> ();

		foreach (GameObject level in levelsList) {
			level.SetActive (false);
		}

		this.inventory = new Inventory ();
        buttonPause = GameObject.Find ("ui-pixel-overlay-btn-pause");
        buttonUnpause = GameObject.Find("ui-pixel-overlay-btn-unpause");
        buttonMedkit = GameObject.Find("ui-pixel-overlay-btn-medkit");
        buttonMedkitSelected = GameObject.Find("ui-pixel-overlay-btn-medkit-selected");
        medkitCounter0 = GameObject.Find("ui-pixel-overlay-medkit-counter-0");
        medkitCounter1 = GameObject.Find("ui-pixel-overlay-medkit-counter-1");
        medkitCounter2 = GameObject.Find("ui-pixel-overlay-medkit-counter-2");
        medkitCounter3 = GameObject.Find("ui-pixel-overlay-medkit-counter-3");
        medkitCounter4 = GameObject.Find("ui-pixel-overlay-medkit-counter-4");
        medkitCounter5 = GameObject.Find("ui-pixel-overlay-medkit-counter-5");
        medkitCounter6 = GameObject.Find("ui-pixel-overlay-medkit-counter-6");
        medkitCounter7 = GameObject.Find("ui-pixel-overlay-medkit-counter-7");
        medkitCounter8 = GameObject.Find("ui-pixel-overlay-medkit-counter-8");
        medkitCounter9 = GameObject.Find("ui-pixel-overlay-medkit-counter-9");
        buttonUnpause.SetActive(false);
		buttonMinimize = GameObject.Find ("ui-button-minimize");
		enableDebugToggle = GameObject.Find ("EnableDebugToggle");
		debugPauseButton = GameObject.Find ("DebugPauseButton");
		debugZoomInButton = GameObject.Find ("DebugZoomInButton");
		debugZoomOutButton = GameObject.Find ("DebugZoomOutButton");
		debugUnlockButton = GameObject.Find ("DebugUnlockButton");

		countdown1 = GameObject.Find ("countdown-1");
		countdown2 = GameObject.Find ("countdown-2");
		countdown3 = GameObject.Find ("countdown-3");

		playerHearts1 = GameObject.Find ("ui-player-hearts-1");
		playerHearts2 = GameObject.Find ("ui-player-hearts-2");
		playerHearts3 = GameObject.Find ("ui-player-hearts-3");
		playerHearts4 = GameObject.Find ("ui-player-hearts-4");
		playerHearts5 = GameObject.Find ("ui-player-hearts-5");

		enemyHearts1 = GameObject.Find ("ui-enemy-hearts-1");
		enemyHearts2 = GameObject.Find ("ui-enemy-hearts-2");
		enemyHearts3 = GameObject.Find ("ui-enemy-hearts-3");
		enemyHearts4 = GameObject.Find ("ui-enemy-hearts-4");
		enemyHearts5 = GameObject.Find ("ui-enemy-hearts-5");

		joystickBG = GameObject.Find ("JoystickBG");
		joystick = GameObject.Find ("Joystick");

		canvas = GameObject.Find ("Canvas");

		dollarsCurrencyText = navbarPanel.transform.Find ("DollarsCurrencyText").gameObject;
		diamondsCurrencyText = navbarPanel.transform.Find ("DiamondsCurrencyText").gameObject;
		SetDoubleDollars (INITIAL_CURRENCY_DOUBLEDOLLARS);
		SetDiamonds (INITIAL_CURRENCY_DIAMONDS);

		previouslySelectedUnitIndex = -1;

		mainMenuPanel.SetActive (false);
		levelCompletionPanel.SetActive (false);
        levelSelectionPanel.SetActive (false);
        missionObjectivesPanel.SetActive(false);
		unitSelectionPanel.SetActive (false);
		closeCombatPanel.SetActive (false);
		debugPanel.SetActive (false);
		navbarPanel.SetActive (false);
		teamPanel.SetActive (false);
        startMissionPanel.SetActive (false);
        mercsSubMenuPanel.SetActive(false);
        shopPanel.SetActive(false);
		isLevelCompletionPanelQueued = false;
		isLevelSelectionPanelQueued = false;
		isUnitSelectionPanelQueued = false;
		isMenuOpen = false;
		ShowMainMenuPanel ();
		//StartCoroutine (ShowLevelSelectionPanelAfterDelay (0));
		//ResetTacticalCombat ();

		Utilities.Hide (countdown1);
		Utilities.Hide (countdown2);
		Utilities.Hide (countdown3);
	}

	// Update is called once per frame
	void Update () {
		if (isLevelComplete || isLevelFailed || isMenuOpen || isCountdownActive) {
			return;
		}

		damageWithFalloffIterationCount = 0;

		Utilities.DebugLog ("Update ()");
		EaseCameraForZoomAndPan ();

		if (timeToTriggerCloseCombat > 0.0f && Time.time >= timeToTriggerCloseCombat && !isCloseCombatActive) {
			
			timeToTriggerCloseCombat = 0;



			float meleeWinToLoseRatio = closeCombatPlayerUnit.Melee () / closeCombatEnemyUnit.Melee ();
			float meleeEnterPercentage = 100.0f;//15.0f;
			float meleeWinPercentage = meleeWinToLoseRatio * 55.0f;
			float meleeLosePercentage = (1.0f - meleeWinToLoseRatio) * 55.0f;

			float randomValue = UnityEngine.Random.Range (0.0f, 100.0f);
			// Close combat resolution: 15% - Go into Melee, 30% - Auto win, 20% - Auto lose, 35% - Bump
			if (randomValue < meleeEnterPercentage) { // Go into Melee
				TriggerCloseCombatForUnits (closeCombatPlayerUnit, closeCombatEnemyUnit);
			} else if (randomValue < meleeEnterPercentage + meleeWinPercentage) { // Player Auto win
				closeCombatEnemyUnit.KnockDown ();
				EndCloseCombat ();
			} else if (randomValue < meleeEnterPercentage + meleeWinPercentage + meleeLosePercentage) { // Player Auto lose
				closeCombatPlayerUnit.KnockDown ();
				EndCloseCombat ();
			} else { // Bump
				EndCloseCombat ();
			}
		}
			
		UnityEngine.Profiling.Profiler.BeginSample ("TacticalCombat.Update Tactical Combat update");
		if (!isCloseCombatActive) {
			RunTacticalCombatUpdate ();
		}
		UnityEngine.Profiling.Profiler.EndSample ();
			
		UnityEngine.Profiling.Profiler.BeginSample ("TacticalCombat.Update Check distance for Close Combat");
		if (!isCloseCombatActive && timeToTriggerCloseCombat <= 0.0f && !DEBUG_SHOULD_DISABLE_CLOSE_COMBAT) {
			CheckUnitDistanceForTriggeringCloseCombat ();
		}
		UnityEngine.Profiling.Profiler.EndSample ();

		UnityEngine.Profiling.Profiler.BeginSample ("TacticalCombat.Update Close Combat update");
		if (isCloseCombatActive) {
			RunCloseCombatUpdate ();
		}
		UnityEngine.Profiling.Profiler.EndSample ();


		UpdateUI ();


		// Sort background to bottom
		desertTerrainBG.GetComponent<SpriteRenderer> ().sortingOrder = -32767;
		cityTerrainBG.GetComponent<SpriteRenderer> ().sortingOrder = -32767;

		if (shouldCallDelayedReset) {
			DelayedReset ();
		}

//		Profiler.BeginSample ("TacticalCombat.Update UpdateUnitNameLabelUI");
//		UpdateUnitNameLabelUI ();
//		Profiler.EndSample ();

		UpdateColorIfPlayerControlled ();
	}

	public void UpdateColorIfPlayerControlled () {

		// Change tint of Units based on being Player or Enemy units
		foreach (Unit unit in unitsList) {
			SpriteRenderer spriteRenderer = unit.gameObject.GetComponent<SpriteRenderer> ();
			if (spriteRenderer) {
				if (unit.isPlayerControlled) {
//					if (unit.IsUnderCover ()) {
//						spriteRenderer.color = new Color(0.75f,0.75f,1f,1f);
//					} else {
//						spriteRenderer.color = new Color(0.75f,1f,1f,1f);
//					}
				} else {
					if (unit.IsUnderCover ()) {
//						spriteRenderer.color = new Color(0.85f,0.25f,0,1f);
					} else {
//						spriteRenderer.color = new Color(0.85f,0,0,1f);
					}
				}
			}
		}
	}

//	public void UpdateUnitNameLabelUI () {
//		Utilities.DebugLog ("UpdateUnitNameLabelUI ()");
//		foreach (Unit unit in unitsList) {
//			//this is your object that you want to have the UI element hovering over
//			Profiler.BeginSample ("UpdateUnitNameLabelUI 1");
//			GameObject WorldObject = unit.gameObject;
//			Profiler.EndSample ();
//		
//			//this is the ui element
//			Profiler.BeginSample ("UpdateUnitNameLabelUI 2");
//			RectTransform UI_Element = unit.unitNameLabel.GetComponent<RectTransform> ();
//			Profiler.EndSample ();
//		
//			//first you need the RectTransform component of your canvas
//			Profiler.BeginSample ("UpdateUnitNameLabelUI 3");
//			RectTransform CanvasRect = canvas.GetComponent<RectTransform> ();
//			Profiler.EndSample ();
//		
//			//then you calculate the position of the UI element
//			//0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
//
//			Profiler.BeginSample ("UpdateUnitNameLabelUI 4");
//			Vector2 ViewportPosition = Camera.main.WorldToViewportPoint (WorldObject.transform.position);
//			Profiler.EndSample ();
//			Profiler.BeginSample ("UpdateUnitNameLabelUI 5");
//			Vector2 WorldObject_ScreenPosition = new Vector2 (
//			((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
//				((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
//			Profiler.EndSample ();
//		
//			//now you can set the position of the ui element
//			Profiler.BeginSample ("UpdateUnitNameLabelUI 6");
//			UI_Element.anchoredPosition = WorldObject_ScreenPosition;
//			Profiler.EndSample ();
//		}
//	}

	public void RunTacticalCombatUpdate () {
		Utilities.DebugLog ("RunTacticalCombatUpdate ()");

		HandlePlayerInputForTacticalCombat ();

		UpdateSelectionArrowPosition ();

		UnityEngine.Profiling.Profiler.BeginSample ("UpdateUnitAI");
		UpdateUnitAI ();
		UnityEngine.Profiling.Profiler.EndSample ();

		CheckForLevelCompletion ();
    }

    public void MarkObjectiveFail(int level, int objective)
    {
        this.isObjectiveComplete[level - 1][objective - 1] = -1;
    }

    public void MarkObjectiveEmpty(int level, int objective)
    {
        this.isObjectiveComplete[level - 1][objective - 1] = 0;
    }

    public void MarkObjectiveSuccess(int level, int objective)
    {
        this.isObjectiveComplete[level - 1][objective - 1] = 1;
    }
	
	public bool IsPositionOverUnit (Vector3 position, Unit unit) {
		if (position.x >= unit.gameObject.transform.position.x - 0.5f * UNIT_SELECTION_WIDTH
		    && position.x <= unit.gameObject.transform.position.x + 0.5f * UNIT_SELECTION_WIDTH
		    && position.y >= unit.gameObject.transform.position.y - 0.5f * UNIT_SELECTION_HEIGHT
		    && position.y <= unit.gameObject.transform.position.y + 0.5f * UNIT_SELECTION_HEIGHT) {
			return true;
		}
		return false;
	}
	
	public bool IsPositionOverAnyUnit (Vector3 position) {
		foreach (Unit unit in unitsList) {
			if (IsPositionOverUnit (position, unit)) {
				return true;
			}
		}
		return false;
	}
	
	public Vector3 AdjustPositionInFromEdges (Vector3 position) {
		float cameraHeight = 2.0f * Camera.main.orthographicSize;
		float cameraWidth = cameraHeight * Camera.main.aspect;
		float tapXPercentage = (position.x / cameraWidth);
		float tapYPercentage = (position.y / cameraHeight);
		
		// Adjust mouse position to disallow moving units to edges of screen
		if (tapXPercentage > 0.45f) {
			position = new Vector3 (0.45f * cameraWidth, position.y, position.z);
		} else if (tapXPercentage < -0.45f) {
			position = new Vector3 (-0.45f * cameraWidth, position.y, position.z);
		}
		if (tapYPercentage > 0.30f) {
			position = new Vector3 (position.x, 0.30f * cameraHeight, position.z);
		} else if (tapYPercentage < -0.42f) {
			position = new Vector3 (position.x, -0.42f * cameraHeight, position.z);
		}
		
		return position;
	}
	
	public bool IsQuickTap () {
		return this.timeAtMouseDown != 0 && Time.time - this.timeAtMouseDown < TIME_TO_REGISTER_TAP_HOLD;
	}
	
	public bool IsHeldTap () {
		return this.timeAtMouseDown != 0 && Time.time - this.timeAtMouseDown >= TIME_TO_REGISTER_TAP_HOLD && !this.wasHeldTapRegistered;
	}

	public bool IsHeldTapIgnoringAlreadyRegistered () {
		return this.timeAtMouseDown != 0 && Time.time - this.timeAtMouseDown >= TIME_TO_REGISTER_TAP_HOLD;
	}

	public void OnButtonDown (Vector3 touchPosition) {
		Utilities.DebugLog ("Input.GetButtonDown ('Fire1')");
		timeAtMouseDown = Time.time;
		this.wasHeldTapRegistered = false;


		List<Unit> selectableUnitsList = new List<Unit> ();
		foreach (Unit unit in unitsList) {
			if (IsPositionOverUnit (touchPosition, unit) && unit.isPlayerControlled) {
				if (isMedpackSelected) {
					unit.ApplyMedpack ();
				} else if (unit.isSelected) {
					unit.StopMovingOrAttacking ();
				} else {
					if (unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED && (unit.currentState != Unit.UnitState.UNIT_STATE_PAUSED || this.isPaused)) {
						selectableUnitsList.Add (unit);
					}
				}
			}
		}

		int index = 0;
		Unit selectedUnit = null;
		// If we selected the last unit in the list previously, start at the first again
		if (previouslySelectedUnitIndex == selectableUnitsList.Count - 1) {
			previouslySelectedUnitIndex = -1;
		}
		// Go through the units in the list until we find a different one to select
		foreach (Unit unit in selectableUnitsList) {
			// Skip ahead until we get to the previously selected unit
			if (index <= previouslySelectedUnitIndex) {
				index++;
				continue;
			}
			// Select the unit if it's different
			if (unit != previouslySelectedUnit) {
				unit.SelectOnlyThisUnit ();
				previouslySelectedUnit = unit;
				previouslySelectedUnitIndex = index;
				break;
			}
			index++;
		}
		// If we somehow made it through without selecting a unit, select the first unit
		if (selectedUnit == null && selectableUnitsList.Count > 0) {
			selectableUnitsList [0].SelectOnlyThisUnit ();
			previouslySelectedUnit = selectableUnitsList [0];
			previouslySelectedUnitIndex = 0;
		}

	}

	public void OnButtonHeld (Vector3 touchPosition) {
		Utilities.DebugLog ("Input.GetButton ('Fire1')");

		if (IsHeldTap ()) {
			this.wasHeldTapRegistered = true;
			if (IsPositionOverAnyUnit (touchPosition)) {
				foreach (Unit unit in unitsList) {
					if (IsPositionOverUnit (touchPosition, unit) && !unit.isPlayerControlled) {
						// If tap-hold on unit, attack this unit
						MakeSelectedUnitAttackTarget (unit);
					}
				}
			} else {
				// If tap-hold on empty space, attack this point
				MakeSelectedUnitAttackPoint (touchPosition);
			}
		}

		//if (this.isPaused) {
		//	// TODO: Move selected unit to position
		//	foreach (Unit unit in unitsList) {
		//		if (unit.isSelected) {
		//			unit.gameObject.transform.position = touchPosition;
		//		}
		//	}
		//}
	}

	public void OnButtonUp (Vector3 touchPosition) {
		Utilities.DebugLog ("Input.GetButtonUp ('Fire1')");

		if (IsQuickTap ()) {
			if (IsPositionOverAnyUnit (touchPosition)) {
				foreach (Unit unit in unitsList) {
					if (IsPositionOverUnit (touchPosition, unit)) {
						if (this.isPaused) {
							//DeselectAllUnits ();
						} else {
							if (unit.isPlayerControlled) {
								if (unit.currentState == Unit.UnitState.UNIT_STATE_DEAD || unit.currentState == Unit.UnitState.UNIT_STATE_DISABLED || unit.currentState == Unit.UnitState.UNIT_STATE_PAUSED) {
									// If quick tap on disabled player unit, move toward this point
									MoveSelectedUnitTowardsPoint (AdjustPositionInFromEdges (touchPosition));
								}
							} else {
								// If quick tap on unit, move toward this unit
								MoveSelectedUnitTowardsTarget (unit);
							}
						}
					}
				}
			} else {
				// If quick tap on empty space, move toward this point
				MoveSelectedUnitTowardsPoint (AdjustPositionInFromEdges (touchPosition));
			}
		}

		//if (this.isPaused && IsHeldTapIgnoringAlreadyRegistered ()) {
		//	cameraTargetPosition = touchPosition;
		//}
		
		timeAtMouseDown = 0;
	}
	
	public void HandlePlayerInputForTacticalCombat () {
		Utilities.DebugLog ("HandlePlayerInputForTacticalCombat ()");
		// Get the current screen position of the mouse from Input
		Vector3 mousePos2D = Input.mousePosition;
		// The Camera's z position set the how far to push the mouse into 3D
		mousePos2D.z = -Camera.main.transform.position.z;
		// Convert the point from 2D screen space into 3D game world space
		Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);


		if (Input.GetButtonDown ("Fire1")) {
			OnButtonDown (mousePos3D);
		}

		if (Input.GetButton ("Fire1")) {
			OnButtonHeld (mousePos3D);
		}

		if (Input.GetButtonUp ("Fire1")) {
			OnButtonUp (mousePos3D);
		}
		
		//		if (Input.GetMouseButton (0)) {
		//			foreach (Unit unit in unitsList) {
		//				if (unit.isSelected) {
		//					unit.agent.SetDestination (Camera.main.ScreenToWorldPoint (mousePos2D));
		//				}
		//			}
		//		}


        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (Camera.main.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                Camera.main.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                Camera.main.orthographicSize = Mathf.Max(CAMERA_ZOOM_TACTICAL_COMBAT_MIN, Mathf.Min(Camera.main.orthographicSize, CAMERA_ZOOM_TACTICAL_COMBAT_MAX));

                ClampCameraPositionWithinBoundsOfLevel();
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                Camera.main.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 0.1f, 179.9f);
            }
        } else {


        // Handle panning
            if (Input.touchCount > 0)
            {
                Touch touchZero = Input.GetTouch(0);

                float cameraTargetX = CAMERA_TARGET_POSITION.x + (-0.007f) * touchZero.deltaPosition.x;
                float cameraTargetY = CAMERA_TARGET_POSITION.y + (-0.007f) * touchZero.deltaPosition.y;
                CAMERA_TARGET_POSITION = new Vector2(cameraTargetX, cameraTargetY);

                ClampCameraPositionWithinBoundsOfLevel();
            }
        }

	}

    public void ClampCameraPositionWithinBoundsOfLevel ()
    {
        // Note: maxPositionX and maxPositionY are equations dictated by trial and error by 
        //  scaling the camera with the given level dimensions in the editor, and seeing what 
        //  keeps them in frame, then fitting a line to the points
        float maxPositionX = -1.8f * Camera.main.orthographicSize + 11.0f;
        float minPositionX = -maxPositionX;
        float cameraTargetX = CAMERA_TARGET_POSITION.x;
        float clampedTargetX = Mathf.Max(minPositionX, Mathf.Min(cameraTargetX, maxPositionX));
        float maxPositionY = -Camera.main.orthographicSize + 6.2f;
        float minPositionY = -maxPositionY;
        float cameraTargetY = CAMERA_TARGET_POSITION.y;
        float clampedTargetY = Mathf.Max(minPositionY, Mathf.Min(cameraTargetY, maxPositionY));

        CAMERA_TARGET_POSITION = new Vector2(clampedTargetX, clampedTargetY);
        Camera.main.transform.position = new Vector3(CAMERA_TARGET_POSITION.x, CAMERA_TARGET_POSITION.y, -10);
    }

	public void UpdateUnitAI () {
		Utilities.DebugLog ("UpdateUnitAI ()");
		foreach (Unit unit in unitsList) {
			if (unit && unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED && unit.currentState != Unit.UnitState.UNIT_STATE_PAUSED) {
				unit.unitAI.UpdateAI ();
			}
		}
	}

	public int GetNumMovingAIUnits () {
		Utilities.DebugLog ("GetNumMovingAIUnits ()");
		int totalMovingAIUnits = 0;
		foreach (Unit unit in unitsList) {
			if (!unit.isPlayerControlled) {
				if (unit.isMoving) {
					totalMovingAIUnits++;
				}
			}
		}
		//Utilities.DebugLog ("Num moving AI units = " + totalMovingAIUnits);
		return totalMovingAIUnits;
	}

	public void UpdateSelectionArrowPosition () {
		Utilities.DebugLog ("UpdateSelectionArrowPosition ()");
		// The selection arrow should follow the selected unit
		foreach (Unit unit in unitsList) {
			if (unit.isSelected) {
				selectionArrow.transform.position = unit.gameObject.transform.position;
				selectionArrow.GetComponent<SpriteRenderer> ().sortingOrder = 32767;
			}
		}
	}

	public void CheckForLevelCompletion () {
		Utilities.DebugLog ("CheckForLevelCompletion ()");
		if (unitsList.Count == 0) {
			return;
		}

		// Check if all Player units are disabled
		bool isEveryPlayerUnitDisabled = true;
		bool isEveryEnemyUnitDisabled = true;
        bool shouldMarkIncompleteObjectivesAsFailed = false;
		foreach (Unit unit in unitsList) {
			if (unit.isPlayerControlled) {
				if (unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED) {
					isEveryPlayerUnitDisabled = false;
				}
			} else {
				if (unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED) {
					isEveryEnemyUnitDisabled = false;
				}
			}
		}
        if (isEveryPlayerUnitDisabled) {
            shouldMarkIncompleteObjectivesAsFailed = true;
            isLevelFailed = true;
		}

        isLevelComplete = true;
        int i = 0;
        foreach (LevelObjective levelObjective in levelObjectives[currentLevel - 1]) {
            if (levelObjective == LevelObjective.LevelObjective_KillAllEnemies)
            {
                if (isEveryEnemyUnitDisabled)
                {
                    MarkObjectiveSuccess(this.currentLevel, i + 1);
                }
                else
                {
                    isLevelComplete = false;
                    if (shouldMarkIncompleteObjectivesAsFailed)
                    {
                        MarkObjectiveFail(this.currentLevel, i + 1);
                    }
                }
            }
            else if (levelObjective == LevelObjective.LevelObjective_Destroy_Tank)
            {
                bool isTankDestroyed = false;
                foreach (TerrainObstacle obstacle in obstaclesList)
                {
                    if (obstacle.obstacleType == TerrainObstacle.ObstacleType.OBSTACLE_TANK)
                    {
                        if (!obstacle.isIntact)
                        {
                            isTankDestroyed = true;
                        }
                    }
                }
                if (isTankDestroyed)
                {
                    MarkObjectiveSuccess(this.currentLevel, i + 1);
                }
                else
                {
                    isLevelComplete = false;
                    if (shouldMarkIncompleteObjectivesAsFailed)
                    {
                        MarkObjectiveFail(this.currentLevel, i + 1);
                    }
                }
            }
            else if (levelObjective == LevelObjective.LevelObjective_Protect_Tank)
            {
                bool isTankDestroyed = false;
                foreach (TerrainObstacle obstacle in obstaclesList)
                {
                    if (obstacle.obstacleType == TerrainObstacle.ObstacleType.OBSTACLE_TANK)
                    {
                        if (!obstacle.isIntact)
                        {
                            isTankDestroyed = true;
                        }
                    }
                }
                if (isTankDestroyed)
                {
                    isLevelFailed = true;
                    MarkObjectiveFail(this.currentLevel, i + 1);
                }
                float timeElapsed = Mathf.Floor((Time.time - timeAtLevelStart) * 10.0f) / 10.0f;
                if (!isTankDestroyed && timeElapsed >= levelObjectiveDuration)
                {
                    MarkObjectiveSuccess(this.currentLevel, i + 1);
                }
                else
                {
                    isLevelComplete = false;
                    if (shouldMarkIncompleteObjectivesAsFailed)
                    {
                        MarkObjectiveFail(this.currentLevel, i + 1);
                    }
                }
                if (isEveryEnemyUnitDisabled)
                {
                    MarkObjectiveSuccess(this.currentLevel, i + 1);
                    isLevelComplete = true;
                }
            }
            i++;
        }

		

		if (!isLevelCompletionPanelQueued && (isLevelComplete || isLevelFailed)) {
			StartCoroutine (ShowLevelCompletionPanelAfterDelay (DELAY_BEFORE_SHOWING_LEVEL_COMPLETION_PANEL));
			isLevelCompletionPanelQueued = true;
			
			// Unlock next level on Map
			if (isLevelComplete) {
				maxUnlockLevel = currentLevel + 1;
			}
		}
	}

	public void ShowMainMenuPanel () {
		mainMenuPanel.gameObject.SetActive (true);
	}

	public void HideMainMenuPanel () {
		mainMenuPanel.gameObject.SetActive (false);
	}

	public void ShowLevelSelectionPanel () {
		StartCoroutine (ShowLevelSelectionPanelAfterDelay (0));
	}
	
	public bool isLevelSelectionPanelQueued = false;
	public IEnumerator ShowLevelSelectionPanelAfterDelay (float delay) {
		yield return new WaitForSeconds (delay);

		DestroyOldObjects ();

		LevelSelectionPanel levelSelectionPanelScript = levelSelectionPanel.GetComponent<LevelSelectionPanel> ();
		levelSelectionPanelScript.ShowPanel ();
	}

	public void HideLevelSelectionPanel () {
		StartCoroutine (HideLevelSelectionPanelAfterDelay (0.1f));
	}
	public IEnumerator HideLevelSelectionPanelAfterDelay (float delay) {
		yield return new WaitForSeconds (delay);

		DestroyOldObjects ();

		LevelSelectionPanel levelSelectionPanelScript = levelSelectionPanel.GetComponent<LevelSelectionPanel> ();
		levelSelectionPanelScript.HidePanel ();
	}

	public void ShowUnitSelectionPanel () {
		StartCoroutine (ShowUnitSelectionPanelAfterDelay (0));
	}

	public bool isUnitSelectionPanelQueued = false;
	public IEnumerator ShowUnitSelectionPanelAfterDelay (float delay) {
		yield return new WaitForSeconds (delay);

		DestroyOldObjects ();

		UnitSelectionPanel unitSelectionPanelScript = unitSelectionPanel.GetComponent<UnitSelectionPanel> ();
		unitSelectionPanelScript.ShowPanel ();

		HideLevelSelectionPanel ();
	}

	public void ShowStartMissionPanel () {
		StartCoroutine (ShowStartMissionPanelAfterDelay (0));
	}

	public bool isStartMissionPanelQueued = false;
	public IEnumerator ShowStartMissionPanelAfterDelay (float delay) {
		yield return new WaitForSeconds (delay);

		DestroyOldObjects ();

		StartMissionPanel startMissionPanelScript = startMissionPanel.GetComponent<StartMissionPanel> ();
		startMissionPanelScript.ShowPanel ();
		this.levelSelectionPanel.transform.SetAsLastSibling ();
		this.startMissionPanel.transform.SetAsLastSibling ();
		this.navbarPanel.transform.SetAsLastSibling ();
    }

    public void ShowMissionObjectivesPanel()
    {
        MissionObjectivesPanel missionObjectivesPanelScript = this.missionObjectivesPanel.GetComponent<MissionObjectivesPanel>();
        missionObjectivesPanelScript.ShowPanel();
    }

	public void ShowTeamPanel () {
		TeamPanel teamPanelScript = this.teamPanel.GetComponent<TeamPanel> ();
		teamPanelScript.ShowPanel ();
		this.levelSelectionPanel.transform.SetAsLastSibling ();
		this.startMissionPanel.transform.SetAsLastSibling ();
		this.teamPanel.transform.SetAsLastSibling ();
		this.navbarPanel.transform.SetAsLastSibling ();
	}

	public bool isLevelCompletionPanelQueued = false;
	public IEnumerator ShowLevelCompletionPanelAfterDelay (float delay) {
		yield return new WaitForSeconds (delay);
		
		PauseTacticalCombat ();

		foreach (Unit unit in unitsList) {
			unit.StopMovingOrAttacking ();
		}

		Utilities.DebugLog ("ShowLevelCompletionPanelAfterDelay ()");
		LevelCompletionPanel levelCompletionPanelScript = levelCompletionPanel.GetComponent<LevelCompletionPanel> ();
		levelCompletionPanelScript.ShowPanel ();
		this.navbarPanel.transform.SetAsLastSibling ();
	}

	public void ButtonBlockLow_OnPress () {
		closeCombatPlayerUnit.StartMeleeBlockLow ();
	}
	public void ButtonBlockMedium_OnPress () {
		closeCombatPlayerUnit.StartMeleeBlockMedium ();
	}
	public void ButtonBlockHigh_OnPress () {
		closeCombatPlayerUnit.StartMeleeBlockHigh ();
	}
	public void ButtonAttackLow_OnPress () {
		//if (!Application.isEditor) {
			closeCombatPlayerUnit.StartMeleeAttackLow ();
		//}
	}
	public void ButtonAttackMedium_OnPress () {
		//if (!Application.isEditor) {
			closeCombatPlayerUnit.StartMeleeAttackMedium ();
		//}
	}
	public void ButtonAttackHigh_OnPress () {
		//if (!Application.isEditor) {
			closeCombatPlayerUnit.StartMeleeAttackHigh ();
		//}
	}

	public bool IsMouseMoving () {
		return (Input.GetAxisRaw ("Mouse X") != 0 || Input.GetAxisRaw ("Mouse Y") != 0);
	}

	public void RunCloseCombatUpdate () {
		Utilities.DebugLog ("RunCloseCombatUpdate ()");
		RunCloseCombatAIUpdate ();


		// Get joystick object info
		Vector3 joystickCenter = Vector3.zero;
		if (joystickBG) {
			joystickCenter = joystickBG.transform.position;
		}


		// Get touch/mouse position
		Vector3 touchPosition = Vector3.zero;
		bool isKeyboardArrowPressed = false;
		if (Input.touchCount > 0) {
			// Touch
			touchPosition = Input.GetTouch (0).position;
		} else if (Application.isEditor) {
			if (Input.GetMouseButton (0)) {
				// Mouse
				Vector3 mousePosition = Input.mousePosition;
				touchPosition = mousePosition;
			} else {
				// Keyboard Arrow keys
				float horizontalInput = Input.GetAxis ("Horizontal");
				float verticalInput = Input.GetAxis ("Vertical");
				if (horizontalInput != 0 || verticalInput != 0) {
//					Debug.Log ("Arrow keys NON-ZERO");
					isKeyboardArrowPressed = true;
					touchPosition = new Vector3 (horizontalInput * JOYSTICK_MAX_RADIUS + joystickCenter.x, verticalInput * JOYSTICK_MAX_RADIUS + joystickCenter.y, joystickCenter.z);
				}
			}
		}


		if (Application.isEditor) {
			// Ctrl or Left mouse  (Medium)
			if (Input.GetButtonDown ("Fire1")) {
//				Debug.Log ("FIRE1");
				//closeCombatPlayerUnit.StartMeleeAttackMedium ();
			}

			// Option/Alt  (Low)
			if (Input.GetButtonDown ("Fire2")) {
//				Debug.Log ("FIRE2");
				//closeCombatPlayerUnit.StartMeleeAttackLow ();
			}

			// Shift  (High)
			if (Input.GetButtonDown ("Fire3")) {
//				Debug.Log ("FIRE3");
				//closeCombatPlayerUnit.StartMeleeAttackHigh ();
			}
		}

		// Get touch vector
		Vector3 touchVector = new Vector3 (touchPosition.x - joystickCenter.x, touchPosition.y - joystickCenter.y, touchPosition.z - joystickCenter.z);
		float touchVectorLength = Mathf.Sqrt (touchVector.x * touchVector.x + touchVector.y * touchVector.y + touchVector.z * touchVector.z);
		//Debug.Log ("touchVector = (" + touchVector.x + ", " + touchVector.y + ", " + touchVector.z + ")");

		Vector3 unitTouchVector = new Vector3 (0, 0, 0);
		if (touchVectorLength > 0) {
			unitTouchVector = new Vector3 (touchVector.x / touchVectorLength, touchVector.y / touchVectorLength, touchVector.z / touchVectorLength);
		}
		float adjustedTouchVectorLength = touchVectorLength;
		if (touchVectorLength > JOYSTICK_MAX_RADIUS) {
			adjustedTouchVectorLength = JOYSTICK_MAX_RADIUS;
		}
		Vector3 adjustedTouchVector = new Vector3 (unitTouchVector.x * adjustedTouchVectorLength, unitTouchVector.y * adjustedTouchVectorLength, unitTouchVector.z * adjustedTouchVectorLength);



		// Handle touches
		if (Input.touchCount > 0 || Input.GetMouseButton (0) || Input.GetMouseButtonUp (0) || isKeyboardArrowPressed) {

			if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown (0)) {
				// Get touch/mouse position at start
				this.touchStartPosition = touchPosition;

				// Decide whether the initial touch was over the joystick
				if (touchVectorLength < JOYSTICK_MAX_TOUCH_RADIUS) {
					didTouchBeginWithinJoypad = true;
				} else {
					didTouchBeginWithinJoypad = false;
				}
			}

			if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || (Input.GetMouseButton (0) && IsMouseMoving ()) || isKeyboardArrowPressed) {
				// If touch returns within joystick radius, count as starting over joypad
				if (touchVectorLength < JOYSTICK_MAX_RADIUS) {
					didTouchBeginWithinJoypad = true;
				}

				if (didTouchBeginWithinJoypad) {
					if (touchVectorLength > JOYSTICK_MAX_RADIUS) {
						joystick.transform.position = new Vector3 (JOYSTICK_MAX_RADIUS * unitTouchVector.x + joystickCenter.x, JOYSTICK_MAX_RADIUS * unitTouchVector.y + joystickCenter.y, joystick.transform.position.z);
					} else {
						float modifiedRadius = 0.5f * (touchVectorLength + Mathf.Sqrt (touchVectorLength));
						joystick.transform.position = touchPosition;
					}
				}
			}
			
			if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {
				didTouchBeginWithinJoypad = false;

//				// Swipe detection
//				// Get touch/mouse position at end
//				Vector2 touchEndPosition = touchPosition;
//
//				Vector2 touchDeltaPosition = new Vector2 (touchEndPosition.x - this.touchStartPosition.x, touchEndPosition.y - this.touchStartPosition.y);//Input.GetTouch(0).deltaPosition;
//				float minimumDistanceRequiredToRegisterSwipe = 15.0f;
//				
//				if (touchDeltaPosition.x > minimumDistanceRequiredToRegisterSwipe) {
//					//Utilities.DebugLog ("Swipe Right");
//					if (touchDeltaPosition.y > minimumDistanceRequiredToRegisterSwipe) {
//						//Utilities.DebugLog ("High");
//						closeCombatPlayerUnit.StartMeleeAttackHigh ();
//					} else if (touchDeltaPosition.y < -minimumDistanceRequiredToRegisterSwipe) {
//						//Utilities.DebugLog ("Low");
//						closeCombatPlayerUnit.StartMeleeAttackLow ();
//					} else {
//						//Utilities.DebugLog ("Medium");
//						closeCombatPlayerUnit.StartMeleeAttackMedium ();
//					}
//				} else if (touchDeltaPosition.x < -minimumDistanceRequiredToRegisterSwipe) {
//					//Utilities.DebugLog ("Swipe Left");
//					if (touchDeltaPosition.y > minimumDistanceRequiredToRegisterSwipe) {
//						//Utilities.DebugLog ("High");
//						closeCombatPlayerUnit.StartMeleeBlockHigh ();
//					} else if (touchDeltaPosition.y < -minimumDistanceRequiredToRegisterSwipe) {
//						//Utilities.DebugLog ("Low");
//						closeCombatPlayerUnit.StartMeleeBlockLow ();
//					} else {
//						//Utilities.DebugLog ("Medium");
//						closeCombatPlayerUnit.StartMeleeBlockMedium ();
//					}
//					/*if (touchStartPosition.y < closeCombatPlayerUnit.gameObject.transform.position.y + 45 - 20) {
//							Utilities.DebugLog ("High");
//							closeCombatPlayerUnit.StartMeleeBlockHigh ();
//						} else if (touchStartPosition.y > closeCombatPlayerUnit.gameObject.transform.position.y + 45 + 20) {
//							Utilities.DebugLog ("Low");
//							closeCombatPlayerUnit.StartMeleeBlockLow ();
//						} else {
//							Utilities.DebugLog ("Medium");
//							closeCombatPlayerUnit.StartMeleeBlockMedium ();
//						}*/
//				}

				// Reset joystick position on touch up
				if (joystick) {
					joystick.transform.position = joystickCenter;
				}

			}


			// Move player unit according to joystick position
			if (joystick && adjustedTouchVectorLength >= JOYSTICK_MIN_RADIUS_TO_TRIGGER_WALK && !closeCombatPlayerUnit.IsAnimatorPlayingMeleeAttack ()) {

				if (didTouchBeginWithinJoypad) {
					if (!closeCombatPlayerUnit.isJumping) {
						// Restrict motion to the x-axis
						closeCombatPlayerUnit.transform.position = new Vector3 (closeCombatPlayerUnit.transform.position.x + adjustedTouchVector.x * CLOSECOMBAT_UNIT_WALK_SPEED
							, closeCombatPlayerUnit.transform.position.y /*+ adjustedTouchVector.y * CLOSECOMBAT_UNIT_WALK_SPEED*/
							, closeCombatPlayerUnit.transform.position.z /*+ adjustedTouchVector.z * CLOSECOMBAT_UNIT_WALK_SPEED*/);
						closeCombatPlayerUnit.StartRunning ();

						if (Time.time - closeCombatPlayerUnit.timeAtLastJump > CLOSECOMBAT_MIN_DELAY_BETWEEN_MULTIPLE_JUMPS && adjustedTouchVector.y > CLOSECOMBAT_JOYSTICK_VERTICAL_JUMP_SENSITIVITY) {
							closeCombatPlayerUnit.StartJumping ();
							closeCombatPlayerUnit.jumpOrigin = new Vector2 (closeCombatPlayerUnit.transform.position.x, closeCombatPlayerUnit.transform.position.y);

							float xVelocity = 0;
							if (adjustedTouchVector.x < -CLOSECOMBAT_JOYSTICK_HORIZONTAL_JUMP_SENSITIVITY) {
								xVelocity = -100.0f;
							} else if (adjustedTouchVector.x > CLOSECOMBAT_JOYSTICK_HORIZONTAL_JUMP_SENSITIVITY) {
								xVelocity = 100.0f;
							}
							closeCombatPlayerUnit.jumpVelocity = new Vector2 (xVelocity, 1.0f);
						}
					}
				}

			} else {
				closeCombatPlayerUnit.StopRunning ();
			}
		}

		// Keep units from getting too close
		if (closeCombatPlayerUnit.transform.position.x > closeCombatEnemyUnit.transform.position.x - MIN_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS) {
			closeCombatPlayerUnit.transform.position = new Vector3 (closeCombatEnemyUnit.transform.position.x - MIN_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS
				, closeCombatPlayerUnit.transform.position.y
				, closeCombatPlayerUnit.transform.position.z);
		}

//		// Keep units from getting too far apart
//		if (closeCombatPlayerUnit.transform.position.x < this.closeCombatOriginPosition.x - MAX_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS) {
//			closeCombatPlayerUnit.transform.position = new Vector3 (this.closeCombatOriginPosition.x - MAX_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS
//				, closeCombatPlayerUnit.transform.position.y
//				, closeCombatPlayerUnit.transform.position.z);
//		}
//		if (closeCombatEnemyUnit.transform.position.x > this.closeCombatOriginPosition.x + MAX_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS) {
//			closeCombatEnemyUnit.transform.position = new Vector3 (this.closeCombatOriginPosition.x + MAX_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS
//				, closeCombatEnemyUnit.transform.position.y
//				, closeCombatEnemyUnit.transform.position.z);
//		}

		// Stop player running when the joystick isn't moved enough to run
		if (adjustedTouchVectorLength < JOYSTICK_MIN_RADIUS_TO_TRIGGER_WALK
			|| Mathf.Abs (joystick.transform.position.x - joystickCenter.x) < JOYSTICK_MIN_RADIUS_TO_TRIGGER_WALK) {

			closeCombatPlayerUnit.StopRunning ();
		}

	}

	public void RunCloseCombatAIUpdate () {
		Utilities.DebugLog ("RunCloseCombatAIUpdate ()");

		//closeCombatEnemyUnit.StartPunching ();
	}

	public void EaseCameraForZoomAndPan () {
		Utilities.DebugLog ("EaseCameraForZoomAndPan ()");
		//if (this.isPaused) {
		//	Utilities.DebugLog ("EaseCameraForZoomAndPan () isPaused", true);
		//}

		//if (this.wasPausedFromDebug) {
		//	Utilities.DebugLog ("EaseCameraForZoomAndPan () wasPausedFromDebug", true);
		//}

		//if (isCloseCombatActive) {
		//	Utilities.DebugLog ("EaseCameraForZoomAndPan () isCloseCombatActive", true);
		//}

		if (this.wasPausedFromDebug && !isCloseCombatActive) {
			if (Camera.main.orthographicSize > this.cameraTargetZoom) {
				Camera.main.orthographicSize -= CAMERA_ZOOM_SPEED;
				if (Camera.main.orthographicSize < this.cameraTargetZoom) {
					Camera.main.orthographicSize = this.cameraTargetZoom;
				}
			} else if (Camera.main.orthographicSize < this.cameraTargetZoom) {
				Camera.main.orthographicSize += CAMERA_ZOOM_SPEED;
				if (Camera.main.orthographicSize > this.cameraTargetZoom) {
					Camera.main.orthographicSize = this.cameraTargetZoom;
				}
			}

			float cameraX = Camera.main.transform.position.x;
			float cameraY = Camera.main.transform.position.y;
			if (Camera.main.transform.position.x < cameraTargetPosition.x) {
				cameraX += CAMERA_PAN_SPEED * Math.Abs (Camera.main.transform.position.x - cameraTargetPosition.x);
			} else if (Camera.main.transform.position.x > cameraTargetPosition.x) {
				cameraX -= CAMERA_PAN_SPEED * Math.Abs (Camera.main.transform.position.x - cameraTargetPosition.x);
			}
			if (Camera.main.transform.position.y < cameraTargetPosition.y) {
				cameraY += CAMERA_PAN_SPEED * Math.Abs (Camera.main.transform.position.y - cameraTargetPosition.y);
			} else if (Camera.main.transform.position.y > cameraTargetPosition.y) {
				cameraY -= CAMERA_PAN_SPEED * Math.Abs (Camera.main.transform.position.y - cameraTargetPosition.y);
			}
			Camera.main.transform.position = new Vector3 (cameraX, cameraY, -10);
		} else {
			if (isCloseCombatActive) {
				if (Camera.main.orthographicSize > CAMERA_ZOOM_CLOSE_COMBAT) {
					Camera.main.orthographicSize -= CAMERA_ZOOM_SPEED;
					if (Camera.main.orthographicSize < CAMERA_ZOOM_CLOSE_COMBAT) {
						Camera.main.orthographicSize = CAMERA_ZOOM_CLOSE_COMBAT;
					}
				}
            } else {
                if (IS_CAMERA_AUTO_ZOOM_ENABLED)
                {
                    if (Camera.main.orthographicSize < CAMERA_ZOOM_TACTICAL_COMBAT)
                    {
                        Camera.main.orthographicSize += CAMERA_ZOOM_SPEED;
                        if (Camera.main.orthographicSize > CAMERA_ZOOM_TACTICAL_COMBAT)
                        {
                            Camera.main.orthographicSize = CAMERA_ZOOM_TACTICAL_COMBAT;
                        }
                    }
                }
			}

			float cameraX = Camera.main.transform.position.x;
			float cameraY = Camera.main.transform.position.y;
			if (Camera.main.transform.position.x < CAMERA_TARGET_POSITION.x) {
				cameraX += CAMERA_PAN_SPEED * Math.Abs (Camera.main.transform.position.x - CAMERA_TARGET_POSITION.x);
			} else if (Camera.main.transform.position.x > CAMERA_TARGET_POSITION.x) {
				cameraX -= CAMERA_PAN_SPEED * Math.Abs (Camera.main.transform.position.x - CAMERA_TARGET_POSITION.x);
			}
			if (Camera.main.transform.position.y < CAMERA_TARGET_POSITION.y) {
				cameraY += CAMERA_PAN_SPEED * Math.Abs (Camera.main.transform.position.y - CAMERA_TARGET_POSITION.y);
			} else if (Camera.main.transform.position.y > CAMERA_TARGET_POSITION.y) {
				cameraY -= CAMERA_PAN_SPEED * Math.Abs (Camera.main.transform.position.y - CAMERA_TARGET_POSITION.y);
			}
			Camera.main.transform.position = new Vector3 (cameraX, cameraY, -10);
		}

	}

	public void ResetCameraPositionAndZoom () {
		Utilities.DebugLog ("ResetCameraPositionAndZoom ()");
		Camera.main.transform.position = new Vector3 (0, 0, -10);
		Camera.main.orthographicSize = CAMERA_ZOOM_TACTICAL_COMBAT;
	}

	public void KnockDownUnitsWithinRange (Vector2 position, float range) {
		Utilities.DebugLog ("KnockDownUnitsWithinRange ()");
		foreach (Unit unit in unitsList) {
			Utilities.DebugLog ("AdjustedDistance () 222222");
			float distanceFromPosition = AdjustedDistance ((Vector2)unit.gameObject.transform.position, position);
			if (distanceFromPosition <= range) {
				unit.KnockDown ();
			}
		}
	}
	
	public void DealDamageToUnitsWithinRange (Vector2 position, float range, float damage) {
		Utilities.DebugLog ("DealDamageToUnitsWithinRange ()");
		foreach (Unit unit in unitsList) {
			Utilities.DebugLog ("AdjustedDistance () 3333333");
			float distanceFromPosition = AdjustedDistance ((Vector2)unit.gameObject.transform.position, position);
			if (distanceFromPosition <= range) {
				//Utilities.DebugLog ("DealDamageToUnitsWithinRange: " + damage);
				unit.TakeDamage (damage);
			}
		}
	}

	public static int damageWithFalloffIterationCount = 0;
	public void DealDamageWithFalloffToUnitsWithinRange (Vector2 position, float range, float damage, GameObject sourceOfDamage) {
		damageWithFalloffIterationCount++;
		if (damageWithFalloffIterationCount > 10) {
			// TODO: Instead of limiting iteration count, obstacles should be added to an array to indicate they've been checked
			Debug.Log ("Max count of DealDamageWithFalloffToUnitsWithinRange reached. Aborting!");
			return;
		}
		Utilities.DebugLog ("DealDamageWithFalloffToUnitsWithinRange ()");
		foreach (Unit unit in unitsList) {
			Unit unitSourceOfDamage = sourceOfDamage.GetComponent<Unit> ();
			if (!unitSourceOfDamage || unitSourceOfDamage != unit) {
				Utilities.DebugLog ("AdjustedDistance () 4444444");
				float distanceFromPosition = AdjustedDistance ((Vector2)unit.gameObject.transform.position, position);
				if (distanceFromPosition <= range) {
					float dist = distanceFromPosition / range;
					float damageWithFalloff = (1 - dist * dist) * damage;
					unit.TakeDamage (damageWithFalloff);
				}
			}
		}
		foreach (TerrainObstacle obstacle in obstaclesList) {
			TerrainObstacle obstacleSourceOfDamage = sourceOfDamage.GetComponent<TerrainObstacle> ();
			if (!obstacleSourceOfDamage || obstacleSourceOfDamage != obstacle) {
				Utilities.DebugLog ("AdjustedDistance () 5555555");
				float distanceFromPosition = AdjustedDistance ((Vector2)obstacle.gameObject.transform.position, position);
				if (distanceFromPosition <= range) {
					float dist = distanceFromPosition / range;
					float damageWithFalloff = (1 - dist * dist) * damage;
					if (obstacle.isIntact && !obstacle.isImmuneToExplosiveDamage) {
						obstacle.TakeDamage (damageWithFalloff);
					}
				}
			}
		}
	}

	public void CheckUnitDistanceForTriggeringCloseCombat () {
		Utilities.DebugLog ("CheckUnitDistanceForTriggeringCloseCombat ()");
		foreach (Unit unit in unitsList) {
			if (/*unit.isPlayerControlled && */unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED && unit.currentState != Unit.UnitState.UNIT_STATE_PAUSED) {
				foreach (Unit enemyUnit in unitsList) {
					if (unit.isPlayerControlled != enemyUnit.isPlayerControlled) {
						if (/*!enemyUnit.isPlayerControlled && */enemyUnit.currentState != Unit.UnitState.UNIT_STATE_DEAD && enemyUnit.currentState != Unit.UnitState.UNIT_STATE_DISABLED) {
							Utilities.DebugLog ("AdjustedDistance () 6666666");
							float distance = AdjustedDistance ((Vector2)unit.gameObject.transform.position, (Vector2)enemyUnit.gameObject.transform.position);
							if (distance <= MIN_DISTANCE_TO_TRIGGER_CLOSE_COMBAT) {
								// Kamikaze unit triggers explosion rather than Close Combat
								if (unit.unitType == Unit.UnitType.UNIT_KAMIKAZE) {

									GameObject explosion = (GameObject)Instantiate (this.kamikazeExplosionPrefab, unit.gameObject.transform.position, unit.gameObject.transform.rotation);
									explosion.transform.position = new Vector3 (explosion.transform.position.x, explosion.transform.position.y, -0.1f);
									explosion.GetComponent<ParticleSystem> ().GetComponent<Renderer> ().sortingOrder = 32767;
									//Destroy (explosion, 1.0f);
									//KnockDownUnitsWithinRange ((Vector2)unit.gameObject.transform.position, KAMIKAZE_KNOCKDOWN_RANGE);
									unit.TakeDamage (100000.0f); // Kill the Kamikaze unit
									DealDamageWithFalloffToUnitsWithinRange ((Vector2)unit.gameObject.transform.position, KAMIKAZE_KNOCKDOWN_RANGE, KAMIKAZE_KNOCKDOWN_DAMAGE, unit.gameObject);

								} else if (enemyUnit.unitType != Unit.UnitType.UNIT_KAMIKAZE) {

									unit.MarkWithinCloseCombatRangeOfUnit (enemyUnit);
									enemyUnit.MarkWithinCloseCombatRangeOfUnit (unit);

								}

								unit.StopRunning ();
							}
						}
					}
				}
			}
		}
	}

	public void PrepareCloseCombatForUnits (Unit unit, Unit enemyUnit) {
		// If we recently called PrepareCloseCombatForUnits, don't call again
		// If we recently ended Close Combat, don't call
		if (timeToTriggerCloseCombat > Time.time || Time.time < timeAtCloseCombatEnd + MIN_TIME_BETWEEN_CLOSE_COMBAT_ROUNDS) {
			return;
		}

		Vector2 midPoint = new Vector2 (0.5f * (unit.gameObject.transform.position.x + enemyUnit.gameObject.transform.position.x), 0.5f * (unit.gameObject.transform.position.y + enemyUnit.gameObject.transform.position.y));
		closeCombatCloud.transform.position = midPoint;

		unit.currentState = Unit.UnitState.UNIT_STATE_PREPARING_FOR_CLOSE_COMBAT;
		enemyUnit.currentState = Unit.UnitState.UNIT_STATE_PREPARING_FOR_CLOSE_COMBAT;

		unit.HideFlamethrowerParticle ();
		enemyUnit.HideFlamethrowerParticle ();

		Utilities.Hide (unit.gameObject);
		Utilities.Hide (enemyUnit.gameObject);
		Utilities.Show (closeCombatCloud);
		Utilities.Hide (selectionArrow);

		if (unit.isPlayerControlled) {
			closeCombatPlayerUnit = unit;
			closeCombatEnemyUnit = enemyUnit;
		} else {
			closeCombatPlayerUnit = enemyUnit;
			closeCombatEnemyUnit = unit;
		}
		// Setup delay to trigger close combat
		timeToTriggerCloseCombat = Time.time + DELAY_BEFORE_TRIGGERING_CLOSE_COMBAT;
	}

	public void TriggerCloseCombatForUnits (Unit unit, Unit enemyUnit) {
		Utilities.DebugLog ("TriggerCloseCombatForUnits ()");

		// Hide cloud and arrow
		Utilities.Hide (closeCombatCloud);
		Utilities.Hide (selectionArrow);

		// Show units
		Utilities.Show (unit.gameObject);
		Utilities.Show (enemyUnit.gameObject);

		// Disable pathfinding
		PolyNav2D.DisablePathfinding ();

		// Hide Close Combat Range UI
		Utilities.Hide (unit.closeCombatRangeUI);
		Utilities.Hide (enemyUnit.closeCombatRangeUI);

		// Hide health UI for each unit
		unit.HideHealthUI ();
		enemyUnit.HideHealthUI ();

		// Hide Debug UI
        unitNameText.SetActive (false);

		// Hide all other units
		foreach (Unit unitToHide in unitsList) {
			unitToHide.HideFlamethrowerParticle ();
			if (unitToHide != closeCombatPlayerUnit && unitToHide != closeCombatEnemyUnit) {
				Utilities.Hide (unitToHide.gameObject);
			}
		}

		// Stop all units' running
		// TODO: Change this functionality to only pause units' running
		foreach (Unit unitToStop in unitsList) {
			unitToStop.StopRunning ();
		}

		closeCombatPanel.SetActive (true);

		// Make units face each other
		unit.FaceRight ();
		enemyUnit.FaceLeft ();

		closeCombatOriginPosition = new Vector2 (closeCombatCloud.transform.position.x + 0, closeCombatCloud.transform.position.y - 0.4f);
		unit.gameObject.transform.position = new Vector2 (closeCombatOriginPosition.x - 0.5f, closeCombatOriginPosition.y);
		enemyUnit.gameObject.transform.position = new Vector2 (closeCombatOriginPosition.x + 0.5f, closeCombatOriginPosition.y);

		// Change to Melee animations
		unit.StopAttacking ();
		unit.StopRunning ();
		unit.StartCloseCombat ();
		enemyUnit.StopAttacking ();
		enemyUnit.StopRunning ();
		enemyUnit.StartCloseCombat ();
		// Add delay before AI player attacks
		enemyUnit.timeAtLastAttack = Time.time + 0.5f;

		PauseTacticalCombat ();

		// Reset joystick position to center
		Vector3 joystickCenter = Vector3.zero;
		if (joystickBG) {
			joystickCenter = joystickBG.transform.position;
		}
		if (joystick) {
			joystick.transform.position = joystickCenter;
		}

		unit.currentState = Unit.UnitState.UNIT_STATE_CLOSE_COMBAT;
		enemyUnit.currentState = Unit.UnitState.UNIT_STATE_CLOSE_COMBAT;
		SetIsCloseCombatActive (true);
        CAMERA_TARGET_POSITION = new Vector2 (0.5f * (unit.gameObject.transform.position.x + enemyUnit.gameObject.transform.position.x), 0.5f * (unit.gameObject.transform.position.y + enemyUnit.gameObject.transform.position.y) + CAMERA_OFFSET_Y_CLOSE_COMBAT);
	}

	public void SetIsCloseCombatActive (bool value) {
		isCloseCombatActive = value;
		if (value == false) {
			timeAtCloseCombatEnd = Time.time;
		}
		foreach (Unit unit in unitsList) {
			unit.coverBonusDirtyFlag = true;
		}
	}

	public void PauseTacticalCombat () {
		Utilities.DebugLog ("PauseTacticalCombat ()");

		isPaused = true;

		// Pause any alive units
		foreach (Unit aliveUnit in unitsList) {
			if (aliveUnit.currentState != Unit.UnitState.UNIT_STATE_DEAD && aliveUnit.currentState != Unit.UnitState.UNIT_STATE_DISABLED) {
				aliveUnit.currentState = Unit.UnitState.UNIT_STATE_PAUSED;
			}
		}
		
		// Pause any active projectiles
		foreach (Projectile activeProjectile in projectilesList) {
			if (activeProjectile) {
				if (activeProjectile.currentState == Projectile.ProjectileState.PROJECTILE_STATE_ACTIVE) {
					activeProjectile.currentState = Projectile.ProjectileState.PROJECTILE_STATE_PAUSED;
					// Fade out projectile
					activeProjectile.GetComponent<SpriteRenderer> ().color = new Color(1.0f,1.0f,1.0f,0.2f);
				}
			}
		}
	}

	public void UnpauseTacticalCombat () {
		Utilities.DebugLog ("UnpauseTacticalCombat ()");

		isPaused = false;

		// Unpause any paused units
		foreach (Unit pausedUnit in unitsList) {
			if (pausedUnit.currentState == Unit.UnitState.UNIT_STATE_PAUSED) {
				pausedUnit.currentState = Unit.UnitState.UNIT_STATE_IDLE;
			}
		}
	
		// Unpause any paused projectiles
		foreach (Projectile pausedProjectile in projectilesList) {
			if (pausedProjectile) {
				if (pausedProjectile.currentState == Projectile.ProjectileState.PROJECTILE_STATE_PAUSED) {
					pausedProjectile.currentState = Projectile.ProjectileState.PROJECTILE_STATE_ACTIVE;
					// Fade out projectile
					pausedProjectile.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				}
			}
		}
	}

	public void EndCloseCombat () {
		Utilities.DebugLog ("EndCloseCombat ()");

		// In case we did an AutoWin, hide the cloud
		Utilities.Hide (closeCombatCloud);
		closeCombatPanel.SetActive (false);

		// Stop close combat for both units
		closeCombatPlayerUnit.StopCloseCombat ();
		closeCombatEnemyUnit.StopCloseCombat ();

		// Show all units
		foreach (Unit unit in unitsList) {
			Utilities.Show (unit.gameObject);
		}

		// Re-enable pathfinding
		PolyNav2D.EnablePathfinding ();

		// Show Debug UI
        unitNameText.SetActive (true);

		UnpauseTacticalCombat ();

		SetIsCloseCombatActive (false);
		CAMERA_TARGET_POSITION = new Vector2 (0, 0);
	}

	public Unit GetNearestEnemyForUnit (Unit chosenUnit) {
		Utilities.DebugLog ("GetNearestEnemyForUnit ()");
		float nearestDistance = 1000000.0f;
		Unit nearestUnit = null;
		foreach (Unit unit in unitsList) {
			if (unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED && (chosenUnit.isPlayerControlled != unit.isPlayerControlled)) {
				Utilities.DebugLog ("AdjustedDistance () 7777777");
				float currentDistance = AdjustedDistance ((Vector2)chosenUnit.gameObject.transform.position, (Vector2)unit.gameObject.transform.position);

				// Adjust distance to prioritize attacking player units in this order: 
				//   Kamikaze, Medic, Machinegunner, Bazooka, Flamethrower
				if (UnitAI.AI_shouldPrioritizeAttackingUnitsByType) {
					if (unit.unitType == Unit.UnitType.UNIT_KAMIKAZE) {
						currentDistance -= 6.0f;//300.0f;
					} else if (unit.unitType == Unit.UnitType.UNIT_MEDIC) {
						currentDistance -= 5.0f;//250.0f;
					} else if (unit.unitType == Unit.UnitType.UNIT_MACHINEGUNNER) {
						currentDistance -= 4.0f;//200.0f;
					} else if (unit.unitType == Unit.UnitType.UNIT_BAZOOKA) {
						currentDistance -= 3.0f;//150.0f;
					} else if (unit.unitType == Unit.UnitType.UNIT_FLAMETHROWER) {
						currentDistance -= 2.0f;//100.0f;
					}
				}

				if (currentDistance < nearestDistance) {
					nearestDistance = currentDistance;
					nearestUnit = unit;
				}
			}
		}

		return nearestUnit;
	}

	public float GetDistanceOfNearestFriendlyFromObstacle (TerrainObstacle obstacle, bool isPlayerControlled) {
		Utilities.DebugLog ("GetDistanceOfNearestFriendlyUnitForUnit ()");
		float nearestDistance = 1000000.0f;
		foreach (Unit unit in unitsList) {
			if (unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED && (isPlayerControlled == unit.isPlayerControlled)) {
				Utilities.DebugLog ("AdjustedDistance () 7777777");
				float currentDistance = AdjustedDistance ((Vector2)obstacle.gameObject.transform.position, (Vector2)unit.gameObject.transform.position);

				if (currentDistance < nearestDistance) {
					nearestDistance = currentDistance;
				}
			}
		}

		return nearestDistance;
	}
	
	public TerrainObstacle GetNearestExplosiveObstacleForUnit (Unit chosenUnit) {
		Utilities.DebugLog ("GetNearestExplosiveObstacleForUnit ()");
		float nearestDistance = 1000000.0f;
		TerrainObstacle nearestObstacle = null;
		foreach (TerrainObstacle obstacle in obstaclesList) {
			if (obstacle && obstacle.isDestructible && obstacle.isIntact && obstacle.shouldCauseExplosionWhenDestroyed) {
				Utilities.DebugLog ("AdjustedDistance () 888888");
				float currentDistance = AdjustedDistance ((Vector2)chosenUnit.gameObject.transform.position, (Vector2)obstacle.gameObject.transform.position);
				// Adjust distance based on obstacle radius, to account for larger explosive range
				currentDistance -= obstacle.radius;
				if (currentDistance < nearestDistance) {
					nearestDistance = currentDistance;
					nearestObstacle = obstacle;
				}
			}
		}
		
		return nearestObstacle;
	}

	public void UpdateUI () {

        UpdateMedpackUI();

		if (isCloseCombatActive) {
			enableDebugToggle.SetActive (false);

			buttonMinimize.SetActive (false);

			debugPauseButton.SetActive (false);
			debugZoomInButton.SetActive (false);
			debugZoomOutButton.SetActive (false);
			debugUnlockButton.SetActive (false);
		} else {
			if (isUIExpanded) {

				enableDebugToggle.SetActive (true);

				buttonMinimize.SetActive (true);

				debugPauseButton.SetActive (true);
				debugZoomInButton.SetActive (true);
				debugZoomOutButton.SetActive (true);
				debugUnlockButton.SetActive (true);
			} else {

				enableDebugToggle.SetActive (false);

				buttonMinimize.SetActive (false);

				debugPauseButton.SetActive (false);
				debugZoomInButton.SetActive (false);
				debugZoomOutButton.SetActive (false);
				debugUnlockButton.SetActive (false);
			}
		}
	}

	public void UpdateMedpackUI () {
		Utilities.DebugLog ("UpdateMedpackUI ()");

        buttonMedkit.SetActive(false);
        buttonMedkitSelected.SetActive(false);
        medkitCounter0.SetActive(false);
        medkitCounter1.SetActive(false);
        medkitCounter2.SetActive(false);
        medkitCounter3.SetActive(false);
        medkitCounter4.SetActive(false);
        medkitCounter5.SetActive(false);
        medkitCounter6.SetActive(false);
        medkitCounter7.SetActive(false);
        medkitCounter8.SetActive(false);
        medkitCounter9.SetActive(false);

        if (!isCloseCombatActive)
        {
            if (isMedpackSelected)
            {
                buttonMedkitSelected.SetActive(true);
            } else {
                buttonMedkit.SetActive(true);
            }

            switch (numMedpacks)
            {
                case 0:
                    medkitCounter0.SetActive(true);
                    break;
                case 1:
                    medkitCounter1.SetActive(true);
                    break;
                case 2:
                    medkitCounter2.SetActive(true);
                    break;
                case 3:
                    medkitCounter3.SetActive(true);
                    break;
                case 4:
                    medkitCounter4.SetActive(true);
                    break;
                case 5:
                    medkitCounter5.SetActive(true);
                    break;
                case 6:
                    medkitCounter6.SetActive(true);
                    break;
                case 7:
                    medkitCounter7.SetActive(true);
                    break;
                case 8:
                    medkitCounter8.SetActive(true);
                    break;
                case 9:
                    medkitCounter9.SetActive(true);
                    break;
            }
        }
	}

	public void UpdateHeartsUI () {
		Utilities.DebugLog ("UpdateHeartsUI ()");

		if (isCloseCombatActive) {
			if (this.closeCombatPlayerUnit.currentHearts != this.closeCombatPlayerUnit.previousHearts || this.isCloseCombatActive != this.previousIsCloseCombatActive) {
				this.closeCombatPlayerUnit.previousHearts = this.closeCombatPlayerUnit.currentHearts;

				if (this.closeCombatPlayerUnit.currentHearts == 5) {
					Utilities.Show (playerHearts5);
				} else {
					Utilities.Hide (playerHearts5);
				}
				if (this.closeCombatPlayerUnit.currentHearts == 4) {
					Utilities.Show (playerHearts4);
				} else {
					Utilities.Hide (playerHearts4);
				}
				if (this.closeCombatPlayerUnit.currentHearts == 3) {
					Utilities.Show (playerHearts3);
				} else {
					Utilities.Hide (playerHearts3);
				}
				if (this.closeCombatPlayerUnit.currentHearts == 2) {
					Utilities.Show (playerHearts2);
				} else {
					Utilities.Hide (playerHearts2);
				}
				if (this.closeCombatPlayerUnit.currentHearts == 1) {
					Utilities.Show (playerHearts1);
				} else {
					Utilities.Hide (playerHearts1);
				}
			}
		

			if (this.closeCombatEnemyUnit.currentHearts != this.closeCombatEnemyUnit.previousHearts || this.isCloseCombatActive != this.previousIsCloseCombatActive) {
				this.closeCombatEnemyUnit.previousHearts = this.closeCombatEnemyUnit.currentHearts;

				if (this.closeCombatEnemyUnit.currentHearts == 5) {
					Utilities.Show (enemyHearts5);
				} else {
					Utilities.Hide (enemyHearts5);
				}
				if (this.closeCombatEnemyUnit.currentHearts == 4) {
					Utilities.Show (enemyHearts4);
				} else {
					Utilities.Hide (enemyHearts4);
				}
				if (this.closeCombatEnemyUnit.currentHearts == 3) {
					Utilities.Show (enemyHearts3);
				} else {
					Utilities.Hide (enemyHearts3);
				}
				if (this.closeCombatEnemyUnit.currentHearts == 2) {
					Utilities.Show (enemyHearts2);
				} else {
					Utilities.Hide (enemyHearts2);
				}
				if (this.closeCombatEnemyUnit.currentHearts == 1) {
					Utilities.Show (enemyHearts1);
				} else {
					Utilities.Hide (enemyHearts1);
				}
			}

			if (this.closeCombatPlayerUnit.currentHearts != this.closeCombatPlayerUnit.previousHearts
			    || this.closeCombatEnemyUnit.currentHearts != this.closeCombatEnemyUnit.previousHearts
			    || this.isCloseCombatActive != this.previousIsCloseCombatActive) {

				this.previousIsCloseCombatActive = this.isCloseCombatActive;
			}
		}
	}

	public void MeleeAttackPlayer (Unit.UnitMeleeAttackType attackType) {
		Utilities.DebugLog ("MeleeAttackPlayer ()");
		float distance = Mathf.Abs (closeCombatPlayerUnit.transform.position.x - closeCombatEnemyUnit.transform.position.x);
		switch (attackType) {
		case Unit.UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_LOW:
			if (distance < MIN_DISTANCE_TO_TRIGGER_LOW_MELEE_ATTACK_DAMAGE) {
				closeCombatPlayerUnit.HandleIncomingMeleeAttack (attackType);
			}
			break;
		case Unit.UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_MEDIUM:
			if (distance < MIN_DISTANCE_TO_TRIGGER_MEDIUM_MELEE_ATTACK_DAMAGE) {
				closeCombatPlayerUnit.HandleIncomingMeleeAttack (attackType);
			}
			break;
		case Unit.UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_HIGH:
			if (distance < MIN_DISTANCE_TO_TRIGGER_HIGH_MELEE_ATTACK_DAMAGE) {
				closeCombatPlayerUnit.HandleIncomingMeleeAttack (attackType);
			}
			break;
		}
	}

	public void MeleeAttackEnemy (Unit.UnitMeleeAttackType attackType) {
		Utilities.DebugLog ("MeleeAttackEnemy ()");
		float distance = Mathf.Abs (closeCombatPlayerUnit.transform.position.x - closeCombatEnemyUnit.transform.position.x);
		switch (attackType) {
		case Unit.UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_LOW:
			if (distance < MIN_DISTANCE_TO_TRIGGER_LOW_MELEE_ATTACK_DAMAGE) {
				closeCombatEnemyUnit.HandleIncomingMeleeAttack (attackType);
			}
			break;
		case Unit.UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_MEDIUM:
			if (distance < MIN_DISTANCE_TO_TRIGGER_MEDIUM_MELEE_ATTACK_DAMAGE) {
				closeCombatEnemyUnit.HandleIncomingMeleeAttack (attackType);
			}
			break;
		case Unit.UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_HIGH:
			if (distance < MIN_DISTANCE_TO_TRIGGER_HIGH_MELEE_ATTACK_DAMAGE) {
				closeCombatEnemyUnit.HandleIncomingMeleeAttack (attackType);
			}
			break;
		}
	}

	public Vector3 GetRandomObstaclePosition () {
		Utilities.DebugLog ("GetObstaclePosition ()");
		
		GameObject obstacleCreationRegion;
		if (currentTerrain == Terrain.TERRAIN_CITY) {
			if (isCurrentTerrainMirrored) {
				obstacleCreationRegion = GameObject.Find ("ObstacleCreationRegionCityMirrored").gameObject;
			} else {
				obstacleCreationRegion = GameObject.Find ("ObstacleCreationRegionCity").gameObject;
			}
		} else {
			if (isCurrentTerrainMirrored) {
				obstacleCreationRegion = GameObject.Find ("ObstacleCreationRegionDesertMirrored").gameObject;
			} else {
				obstacleCreationRegion = GameObject.Find ("ObstacleCreationRegionDesert").gameObject;
			}
		}
		
		PolygonCollider2D collider = obstacleCreationRegion.GetComponent<PolygonCollider2D> ();
		Vector2 pointVector2 = PointInCollider (collider);
		Vector3 point = new Vector3 (pointVector2.x, pointVector2.y, 10.0f);
		return point;
	}

	public Vector3 GetRandomEnemyUnitPosition () {
		Utilities.DebugLog ("GetRandomEnemyUnitPosition ()");
		
		GameObject enemyUnitStartRegion;
		if (currentTerrain == Terrain.TERRAIN_CITY) {
			if (isCurrentTerrainMirrored) {
				enemyUnitStartRegion = GameObject.Find("EnemyUnitStartRegionCityMirrored").gameObject;
			} else {
				enemyUnitStartRegion = GameObject.Find("EnemyUnitStartRegionCity").gameObject;
			}
		} else {
			if (isCurrentTerrainMirrored) {
				enemyUnitStartRegion = GameObject.Find("EnemyUnitStartRegionDesertMirrored").gameObject;
			} else {
				enemyUnitStartRegion = GameObject.Find("EnemyUnitStartRegionDesert").gameObject;
			}
		}
		
		PolygonCollider2D collider = enemyUnitStartRegion.GetComponent<PolygonCollider2D> ();
		Vector2 pointVector2 = PointInCollider (collider);
		Vector3 point = new Vector3 (pointVector2.x, pointVector2.y, 10.0f);
		return point;

		//return new Vector3 (0.5f, 0.5f);
		float x, y;
		x = UnityEngine.Random.Range (0.6f, 0.95f);
		y = UnityEngine.Random.Range (0.6f, 0.8f);
		Vector3 pos = new Vector3(x, y, 10.0f);
		pos = Camera.main.ViewportToWorldPoint(pos);
		/*while (IsPositionNearUnit (pos)) {
			x = UnityEngine.Random.Range (0.6f, 0.95f);
			y = UnityEngine.Random.Range (0.6f, 0.8f);
			pos = new Vector3(x, y, 10.0f);
			pos = Camera.main.ViewportToWorldPoint(pos);
		}*/
		return pos;
	}

	public Vector3 GetRandomPlayerUnitPosition () {
		Utilities.DebugLog ("GetRandomPlayerUnitPosition ()");

		GameObject playerUnitStartRegion;
		if (currentTerrain == Terrain.TERRAIN_CITY) {
			if (isCurrentTerrainMirrored) {
				playerUnitStartRegion = GameObject.Find("PlayerUnitStartRegionCityMirrored").gameObject;
			} else {
				playerUnitStartRegion = GameObject.Find("PlayerUnitStartRegionCity").gameObject;
			}
		} else {
			if (isCurrentTerrainMirrored) {
				playerUnitStartRegion = GameObject.Find("PlayerUnitStartRegionDesertMirrored").gameObject;
			} else {
				playerUnitStartRegion = GameObject.Find("PlayerUnitStartRegionDesert").gameObject;
			}
		}
		
		PolygonCollider2D collider = playerUnitStartRegion.GetComponent<PolygonCollider2D> ();
		Vector2 pointVector2 = PointInCollider (collider);
		Vector3 point = new Vector3 (pointVector2.x, pointVector2.y, 10.0f);
		return point;

		//return new Vector3 (0.5f, 0.5f);
		float x, y;
		x = UnityEngine.Random.Range (0.05f, 0.4f);
		y = UnityEngine.Random.Range (0.05f, 0.4f);
		Vector3 pos = new Vector3(x, y, 10.0f);
		pos = Camera.main.ViewportToWorldPoint(pos);
		/*while (IsPositionNearUnit (pos)) {
			x = UnityEngine.Random.Range (0.05f, 0.4f);
			y = UnityEngine.Random.Range (0.05f, 0.4f);
			pos = new Vector3(x, y, 10.0f);
			pos = Camera.main.ViewportToWorldPoint(pos);
		}*/
		return pos;
	}

	public bool IsPositionNearUnit (Vector3 position) {
		Utilities.DebugLog ("IsPositionNearUnit ()");
		foreach (Unit unit in unitsList) {
			Utilities.DebugLog ("AdjustedDistance () 999999");
			if (AdjustedDistance ((Vector2)position, (Vector2)unit.gameObject.transform.position) < 0.6f/*30.0*/ || 
			    (AdjustedDistance ((Vector2)position, (Vector2)unit.gameObject.transform.position) < 1.8f/*90.0*/ && Mathf.Abs (position.x - unit.gameObject.transform.position.x) < 0.5f/*25.0*/)) {
				return true;
			}
		}
		return false;
	}

	public UnitDataObject AddPlayerUnitDataObjectOfType (Unit.UnitType unitType, int team) {
		Utilities.DebugLog ("AddPlayerUnitDataObjectOfType ()");
		UnitDataObject unitDataObject = new UnitDataObject ();
		unitDataObject.unitType = unitType;
		unitDataObject.assignedTeam = team;

		// Add default weapon to character
		Weapon weapon = new Weapon ();
		switch (unitType) {
		case Unit.UnitType.UNIT_BAZOOKA:
			weapon.weaponClass = Weapon.WeaponClass.WEAPON_CLASS_BAZOOKA;
			break;
		case Unit.UnitType.UNIT_FLAMETHROWER:
			weapon.weaponClass = Weapon.WeaponClass.WEAPON_CLASS_FLAMETHROWER;
			break;
		case Unit.UnitType.UNIT_GRENADELAUNCHER:
			weapon.weaponClass = Weapon.WeaponClass.WEAPON_CLASS_GRENADELAUNCHER;
			break;
		case Unit.UnitType.UNIT_GRENADIER:
			weapon.weaponClass = Weapon.WeaponClass.WEAPON_CLASS_GRENADE;
			break;
		case Unit.UnitType.UNIT_MACHINEGUNNER:
			weapon.weaponClass = Weapon.WeaponClass.WEAPON_CLASS_MACHINEGUN;
			break;
		}
		weapon.UpdateStats ();
		weapon.EquipToUnit (unitDataObject);
		weaponList.Add (weapon);
		unitDataObject.equippedWeapon = weapon;

		// Add default armor to character
		Armor armor = new Armor ();
		int rand = UnityEngine.Random.Range (0, 2);
		if (rand == 0) {
			armor.armorClass = Armor.ArmorClass.ARMOR_CLASS_BULLET_RESISTANT;
		} else if (rand == 1) {
			armor.armorClass = Armor.ArmorClass.ARMOR_CLASS_EXPLOSION_RESISTANT;
		} else if (rand == 2) {
			armor.armorClass = Armor.ArmorClass.ARMOR_CLASS_FLAME_RESISTANT;
		}
		armor.UpdateStats ();
		armor.EquipToUnit (unitDataObject);
		armorList.Add (armor);
		unitDataObject.equippedArmor = armor;

		unitDataObject.InitializeIfNeeded ();
		unitDataObjectsList.Add (unitDataObject);

		return unitDataObject;
	}

	public void AddPlayerUnitOfType (Unit.UnitType unitType) {
		Utilities.DebugLog ("AddPlayerUnitOfType ()");
		GameObject unitGameObject1 = (GameObject)Instantiate(unitPrefab, GetRandomPlayerUnitPosition (), Quaternion.identity);
		Unit unit1 = unitGameObject1.GetComponent<Unit> ();
		unit1.isPlayerControlled = true;
		unit1.unitType = unitType;
		unit1.Reset ();
		unitsList.Add (unit1);

//		GameObject newTextInstance = (GameObject)Instantiate(unitNameText, new Vector3(0,0,0), Quaternion.identity);
//		newTextInstance.transform.SetParent(canvas.transform);
//		newTextInstance.gameObject.SetActive(true);
//		Text newTextInstanceText = newTextInstance.GetComponent<Text> ();
//		newTextInstanceText.text = unit1.Abbreviation ();
//		unit1.unitNameLabel = newTextInstance;
	}
	
	public void AddEnemyUnitOfType (Unit.UnitType unitType) {
		Utilities.DebugLog ("AddEnemyUnitOfType ()");
		GameObject unitGameObject1 = (GameObject)Instantiate(unitPrefab, GetRandomEnemyUnitPosition (), Quaternion.identity);
		Unit unit1 = unitGameObject1.GetComponent<Unit> ();
		unit1.isPlayerControlled = false;
		unit1.unitType = unitType;
		unitsList.Add (unit1);
		
//		GameObject newTextInstance = (GameObject)Instantiate(unitNameText, new Vector3(0,0,0), Quaternion.identity);
//		newTextInstance.transform.SetParent(canvas.transform);
//		newTextInstance.gameObject.SetActive(true);
//		Text newTextInstanceText = newTextInstance.GetComponent<Text> ();
//		newTextInstanceText.text = unit1.Abbreviation ();
//		unit1.unitNameLabel = newTextInstance;
	}

	public void StartCountdown () {
		isCountdownActive = true;
		PauseTacticalCombat ();

		Utilities.Hide (countdown1);
		Utilities.Hide (countdown2);
		Utilities.Show (countdown3);

		countdown3.gameObject.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
		iTween.ScaleTo(countdown3.gameObject, iTween.Hash("x", 0.5f, "y", 0.5f, "time", 1.0f, "easeType", "easeInOutExpo", "oncomplete", "StartCountdown2", "oncompletetarget", this.gameObject));
	}

	public void StartCountdown2 () {
		Utilities.Hide (countdown1);
		Utilities.Show (countdown2);
		Utilities.Hide (countdown3);

		countdown2.gameObject.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
		iTween.ScaleTo(countdown2.gameObject, iTween.Hash("x", 0.5f, "y", 0.5f, "time", 1.0f, "easeType", "easeInOutExpo", "oncomplete", "StartCountdown1", "oncompletetarget", this.gameObject));
	}

	public void StartCountdown1 () {
		Utilities.Show (countdown1);
		Utilities.Hide (countdown2);
		Utilities.Hide (countdown3);

		countdown1.gameObject.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
		iTween.ScaleTo(countdown1.gameObject, iTween.Hash("x", 0.5f, "y", 0.5f, "time", 1.0f, "easeType", "easeInOutExpo", "oncomplete", "StartCountdownComplete", "oncompletetarget", this.gameObject));
	}

	public void StartCountdownComplete () {
		Utilities.Hide (countdown1);
		Utilities.Hide (countdown2);
		Utilities.Hide (countdown3);

		UnpauseTacticalCombat ();

		timeAtLevelStart = Time.time;

		shouldCallDelayedReset = true;

		isCountdownActive = false;
	}

	public void ResetTacticalCombat () {
		Utilities.DebugLog ("ResetTacticalCombat ()");

		levelCompletionPanel.SetActive (false);
		isMenuOpen = false;
		isLevelCompletionPanelQueued = false;

		isLevelComplete = false;
		isLevelFailed = false;

		Utilities.Hide (closeCombatCloud);

		if (isCloseCombatActive) {
			EndCloseCombat ();
		}
			
		ResetCameraPositionAndZoom ();

		DeselectAllUnits ();

		DestroyOldObjects ();

		//ChooseRandomTerrain ();
		ChooseTerrainForCurrentLevel ();

		//PopulateRandomObstacles ();
		PopulateObstaclesForCurrentLevel ();

		//AddPlayerAndEnemyUnits ();
		PopulateUnitsForCurrentLevel ();

		// Change tint of Units based on being Player or Enemy units
		foreach (Unit unit in unitsList) {
			SpriteRenderer spriteRenderer = unit.gameObject.GetComponent<SpriteRenderer> ();
			if (spriteRenderer) {
				if (unit.isPlayerControlled) {
					//spriteRenderer.color = new Color(0.75f,1f,1f,1f);
				} else {
					//spriteRenderer.color = new Color(0.85f,0,0,1f);
				}
			}
		}

		// Reset health packs
		numMedpacks = 3;

        // Reset num shots fired
        numShotsFired = 0;

		// Update UI prior to countdown
		foreach (Unit unit in unitsList) {
			unit.UpdateUI ();
		}
//		UpdateUnitNameLabelUI ();
		UpdateColorIfPlayerControlled ();


		//StartCountdown ();

	}

	public void DestroyOldObjects () {
		
		// Destroy old projectiles
		foreach (Projectile projectile in projectilesList) {
			if (projectile) {
				Destroy (projectile.gameObject, 0);
			}
		}
		projectilesList.Clear ();
		
		// Destroy old obstacles
		foreach (TerrainObstacle obstacle in obstaclesList) {
			Destroy (obstacle.gameObject, 0);
		}
		obstaclesList.Clear ();
		
		// Destroy old units
		foreach (Unit unit in unitsList) {
			unit.DestroyFlamethrowerParticle ();
//			Destroy (unit.unitNameLabel);
            Destroy(unit.shadow);
			Destroy (unit.gameObject);
			Destroy (unit.closeCombatRangeUI);
		}
		unitsList.Clear ();

		// Destroy old Level
		int levelCount = 1;
		foreach (GameObject level in levelsList) {
			GameObject levelGameObject = GameObject.Find("Level"+levelCount+"_"+uniqueLevelIdentifier);
			if (levelGameObject) {
				Destroy (levelGameObject);
			}
			levelCount++;
		}
	}

	public void ChooseRandomTerrain () {
		// Choose random terrain
		int randomTerrain = UnityEngine.Random.Range (0, 2);
		if (randomTerrain == 0) {
			desertTerrainBG.SetActive (false);
			cityTerrainBG.SetActive (true);
			currentTerrain = Terrain.TERRAIN_CITY;
		} else if (randomTerrain == 1) {
			desertTerrainBG.SetActive (true);
			cityTerrainBG.SetActive (false);
			currentTerrain = Terrain.TERRAIN_DESERT;
		}
	}

	public void AddPlayerUnitDataObjectsToTeam (int team) {
		UnitDataObject unit1 = AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_MACHINEGUNNER, team);

		AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_BAZOOKA, team);
		AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_GRENADELAUNCHER, team);
		AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_FLAMETHROWER, team);
		AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_GRENADIER, team);

		AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_FLAMETHROWER, 2);
		AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_FLAMETHROWER, 2);
		AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_GRENADIER, 2);
		AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_MACHINEGUNNER, 2);
		AddPlayerUnitDataObjectOfType (Unit.UnitType.UNIT_MACHINEGUNNER, 2);
	}

	public void AddPlayerAndEnemyUnits () {
		// Add all player units
		AddPlayerUnitOfType (Unit.UnitType.UNIT_MACHINEGUNNER);
		AddPlayerUnitOfType (Unit.UnitType.UNIT_BAZOOKA);
		AddPlayerUnitOfType (Unit.UnitType.UNIT_GRENADELAUNCHER);
		AddPlayerUnitOfType (Unit.UnitType.UNIT_FLAMETHROWER);
		AddPlayerUnitOfType (Unit.UnitType.UNIT_GRENADIER);
		
		//		AddPlayerUnitOfType (Unit.UnitType.UNIT_BRAWLER);
		//		AddPlayerUnitOfType (Unit.UnitType.UNIT_KAMIKAZE);
		//		AddPlayerUnitOfType (Unit.UnitType.UNIT_MEDIC);
		//AddPlayerUnitOfType (Unit.UnitType.UNIT_SWAT);
		//		AddPlayerUnitOfType (Unit.UnitType.UNIT_KAMIKAZE);
		//		AddPlayerUnitOfType (Unit.UnitType.UNIT_KAMIKAZE);
		//		AddPlayerUnitOfType (Unit.UnitType.UNIT_KAMIKAZE);
		//		AddPlayerUnitOfType (Unit.UnitType.UNIT_KAMIKAZE);
		//		AddPlayerUnitOfType (Unit.UnitType.UNIT_KAMIKAZE);
		
		// Choose random unit configuration
		currentUnitConfiguration = (UnitConfiguration)UnityEngine.Random.Range (0, (int)UnitConfiguration.UNIT_CONFIGURATION_COUNT);
		switch (currentUnitConfiguration) {
		case UnitConfiguration.UNIT_CONFIGURATION_1BAZ_2GR_2GL:
			AddEnemyUnitOfType (Unit.UnitType.UNIT_BAZOOKA);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADIER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADIER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADELAUNCHER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADELAUNCHER);
			break;
		case UnitConfiguration.UNIT_CONFIGURATION_1K_1GR_1GL_1BR:
			AddEnemyUnitOfType (Unit.UnitType.UNIT_KAMIKAZE);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADIER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADELAUNCHER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_BRAWLER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MACHINEGUNNER);
			break;
		case UnitConfiguration.UNIT_CONFIGURATION_1MED_2MG_2GL:
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MEDIC);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MACHINEGUNNER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MACHINEGUNNER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADELAUNCHER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADELAUNCHER);
			break;
		case UnitConfiguration.UNIT_CONFIGURATION_1MG_1FT_1BAZ_1MED_1GL:
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MACHINEGUNNER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_FLAMETHROWER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_BAZOOKA);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MEDIC);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADELAUNCHER);
			break;
		case UnitConfiguration.UNIT_CONFIGURATION_2BAZ_2MG_1GR:
			AddEnemyUnitOfType (Unit.UnitType.UNIT_BAZOOKA);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_BAZOOKA);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MACHINEGUNNER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MACHINEGUNNER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_GRENADIER);
			break;
		case UnitConfiguration.UNIT_CONFIGURATION_2MG_2FT_1BAZ:
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MACHINEGUNNER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_MACHINEGUNNER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_FLAMETHROWER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_FLAMETHROWER);
			AddEnemyUnitOfType (Unit.UnitType.UNIT_BAZOOKA);
			break;
		}
	}

	public void ActivateTerrain (Terrain terrain) {
		switch (terrain) {
		case Terrain.TERRAIN_CITY:
			desertTerrainBG.SetActive (false);
			cityTerrainBG.SetActive (true);
			break;
		case Terrain.TERRAIN_DESERT:
			desertTerrainBG.SetActive (true);
			cityTerrainBG.SetActive (false);
			break;
		}
	}
	
	public void ChooseTerrainForCurrentLevel () {
		uniqueLevelIdentifier++;
		
		GameObject level = (GameObject)Instantiate (levelsList[currentLevel-1]);//(GameObject)Instantiate (Resources.Load("Level6"));
		level.SetActive (true);
		level.name = "Level"+currentLevel+"_"+uniqueLevelIdentifier;

		if (currentLevel == 1) {
			ActivateTerrain (Terrain.TERRAIN_CITY);
			SetTerrainToMirrored (false);
		} else if (currentLevel == 2) {
			ActivateTerrain (Terrain.TERRAIN_CITY);
			SetTerrainToMirrored (true);
		} else if (currentLevel == 3) {
			ActivateTerrain (Terrain.TERRAIN_DESERT);
			SetTerrainToMirrored (false);
		} else if (currentLevel == 4) {
			ActivateTerrain (Terrain.TERRAIN_DESERT);
			SetTerrainToMirrored (true);
		} else if (currentLevel == 5) {
			ActivateTerrain (Terrain.TERRAIN_CITY);
			this.levelObjectiveDuration = 45.0f;
			SetTerrainToMirrored (false);
		} else if (currentLevel == 6) {
			ActivateTerrain (Terrain.TERRAIN_CITY);
			SetTerrainToMirrored (true);
		} else if (currentLevel == 7) {
			ActivateTerrain (Terrain.TERRAIN_DESERT);
			SetTerrainToMirrored (false);
		} else if (currentLevel == 8) {
			ActivateTerrain (Terrain.TERRAIN_DESERT);
			SetTerrainToMirrored (true);
		} else if (currentLevel == 9) {
			ActivateTerrain (Terrain.TERRAIN_CITY);
			SetTerrainToMirrored (false);
			levelObjectiveDuration = 45.0f;
		} else if (currentLevel == 10) {
			ActivateTerrain (Terrain.TERRAIN_CITY);
			SetTerrainToMirrored (true);
		} else if (currentLevel == 11) {
			ActivateTerrain (Terrain.TERRAIN_DESERT);
			SetTerrainToMirrored (false);
		} else if (currentLevel == 12) {
			ActivateTerrain (Terrain.TERRAIN_DESERT);
			SetTerrainToMirrored (true);
		}
	}
	
	public void PopulateObstaclesForCurrentLevel () {
		GameObject obstaclesGameObject = GameObject.Find("Level"+currentLevel+"_"+uniqueLevelIdentifier+"/Obstacles").gameObject;
		foreach (Transform obstacleTransform in obstaclesGameObject.transform) {
			TerrainObstacle obstacle = obstacleTransform.gameObject.GetComponent<TerrainObstacle> ();
			obstaclesList.Add (obstacle);
		}
	}

	public void PopulateUnitsForCurrentLevel () {
		// TODO: Swap these units out and use DataObjects
		GameObject playerUnitsGameObject = GameObject.Find("Level"+currentLevel+"_"+uniqueLevelIdentifier+"/PlayerUnits").gameObject;
		int currentUnitDataObjectIndex = 0;
		foreach (Transform playerUnitTransform in playerUnitsGameObject.transform) {
			Unit playerUnit = playerUnitTransform.gameObject.GetComponent<Unit> ();

			// Set unit type based on team data
			for (int i = currentUnitDataObjectIndex; i < this.unitDataObjectsList.Count; i++) {
				currentUnitDataObjectIndex++;
				UnitDataObject unitDataObject = this.unitDataObjectsList [i];
				if (unitDataObject.assignedTeam == this.selectedTeam) {
					playerUnit.unitType = unitDataObject.unitType;

					Animator unitAnimator = playerUnit.GetComponent<Animator> ();
					switch (playerUnit.unitType) {
					case Unit.UnitType.UNIT_BAZOOKA:
						unitAnimator.runtimeAnimatorController = this.animController_BZ;
						break;
					case Unit.UnitType.UNIT_FLAMETHROWER:
						unitAnimator.runtimeAnimatorController = this.animController_FT;
						break;
					case Unit.UnitType.UNIT_GRENADELAUNCHER:
						unitAnimator.runtimeAnimatorController = this.animController_GL;
						break;
					case Unit.UnitType.UNIT_GRENADIER:
						unitAnimator.runtimeAnimatorController = this.animController_GR;
						break;
					case Unit.UnitType.UNIT_MACHINEGUNNER:
						unitAnimator.runtimeAnimatorController = this.animController_MG;
						break;
					}

					break;
				}
			}
			unitsList.Add (playerUnit);
			
//			GameObject newTextInstance = (GameObject)Instantiate(unitNameText, new Vector3(0,0,0), Quaternion.identity);
//			newTextInstance.transform.SetParent(canvas.transform);
//			newTextInstance.gameObject.SetActive(true);
//			Text newTextInstanceText = newTextInstance.GetComponent<Text> ();
//			newTextInstanceText.text = playerUnit.Abbreviation ();
//			playerUnit.unitNameLabel = newTextInstance;
		}

		GameObject AIUnitsGameObject = GameObject.Find("Level"+currentLevel+"_"+uniqueLevelIdentifier+"/AIUnits").gameObject;
		foreach (Transform AIUnitTransform in AIUnitsGameObject.transform) {
			Unit AIUnit = AIUnitTransform.gameObject.GetComponent<Unit> ();

			Animator unitAnimator = AIUnit.GetComponent<Animator> ();
			switch (AIUnit.unitType) {
			case Unit.UnitType.UNIT_BAZOOKA:
				unitAnimator.runtimeAnimatorController = this.animController_BZ;
				break;
			case Unit.UnitType.UNIT_FLAMETHROWER:
				unitAnimator.runtimeAnimatorController = this.animController_FT;
				break;
			case Unit.UnitType.UNIT_GRENADELAUNCHER:
				unitAnimator.runtimeAnimatorController = this.animController_GL;
				break;
			case Unit.UnitType.UNIT_GRENADIER:
				unitAnimator.runtimeAnimatorController = this.animController_GR;
				break;
			case Unit.UnitType.UNIT_MACHINEGUNNER:
				unitAnimator.runtimeAnimatorController = this.animController_MG;
				break;
			}

			unitsList.Add (AIUnit);
			
//			GameObject newTextInstance = (GameObject)Instantiate(unitNameText, new Vector3(0,0,0), Quaternion.identity);
//			newTextInstance.transform.SetParent(canvas.transform);
//			newTextInstance.gameObject.SetActive(true);
//			Text newTextInstanceText = newTextInstance.GetComponent<Text> ();
//			newTextInstanceText.text = AIUnit.Abbreviation ();
//			AIUnit.unitNameLabel = newTextInstance;
		}
	}

	public void PopulateRandomObstacles () {
		// Populate random obstacles
		int maxNumObstacles = 5;
		int minNumObstacles = 2;
		
		Vector2[] obstaclePositions = new Vector2[5] {new Vector2 (-249.0f * 0.02f, 91.0f * 0.02f), new Vector2 (-125.0f * 0.02f, -39.0f * 0.02f), new Vector2 (56.0f * 0.02f, 98.0f * 0.02f), new Vector2 (100.0f * 0.02f, -78.0f * 0.02f), new Vector2 (263.0f * 0.02f, -41.0f * 0.02f)};
		//		List<Vector2> obstaclePositionList = new List<Vector2> ();
		//		obstaclePositionList.Add (new Vector2 (-249, 91));
		//		obstaclePositionList.Add (new Vector2 (-125, -39));
		//		obstaclePositionList.Add (new Vector2 (56, 98));
		//		obstaclePositionList.Add (new Vector2 (100, -78));
		//		obstaclePositionList.Add (new Vector2 (263, -41));
		
		int numObstaclesCreated = 0;
		int opportunitiesToCreateObstacle = maxNumObstacles;
		for (int i = 0; i < maxNumObstacles; i++) {
			int randomObstaclePrefabIndex = UnityEngine.Random.Range (0, obstaclePrefabsList.Count);
			float x = UnityEngine.Random.Range (0.05f, 0.95f);
			float y = UnityEngine.Random.Range (0.05f, 0.8f);
			Vector3 pos = new Vector3(x, y, 10.0f);
			pos = Camera.main.ViewportToWorldPoint(pos);
			
			//pos = obstaclePositions[i];
			pos = GetRandomObstaclePosition ();
			
			int randomShouldShowObstacle = UnityEngine.Random.Range (0, 4);
			if (randomShouldShowObstacle > 0 || (numObstaclesCreated < minNumObstacles && opportunitiesToCreateObstacle <= minNumObstacles)) {
				numObstaclesCreated++;
				GameObject obstacleGameObject = (GameObject)Instantiate (obstaclePrefabsList[randomObstaclePrefabIndex], pos, Quaternion.identity);
				TerrainObstacle obstacle = obstacleGameObject.GetComponent<TerrainObstacle> ();
				obstaclesList.Add (obstacle);
			}
			opportunitiesToCreateObstacle--;
		}
	}

	public void SetTerrainToMirrored (bool isMirrored) {
		float scale = Mathf.Abs (desertTerrainBG.transform.localScale.x);
		if (!isMirrored && !SHOULD_FORCE_MIRRORED_TERRAIN) {
			Utilities.DebugLog ("Terrain is NOT mirrored.");
			isCurrentTerrainMirrored = false;
			desertTerrainBG.transform.localScale = new Vector3 (scale, scale, scale);
			cityTerrainBG.transform.localScale = new Vector3 (scale, scale, scale);
			
			foreach (Transform transform in desertTerrainBG.transform) {
				PolyNavObstacle polyNavObstacle = transform.gameObject.GetComponent<PolyNavObstacle> ();
				if (polyNavObstacle) {
					polyNavObstacle.invertPolygon = true;
				}
			}
			
			foreach (Transform transform in cityTerrainBG.transform) {
				PolyNavObstacle polyNavObstacle = transform.gameObject.GetComponent<PolyNavObstacle> ();
				if (polyNavObstacle) {
					polyNavObstacle.invertPolygon = true;
				}
			}
			
			foreach (TerrainObstacle obstacle in obstaclesList) {
				PolyNavObstacle polyNavObstacle = obstacle.GetComponent<PolyNavObstacle> ();
				polyNavObstacle.invertPolygon = true;
			}
		} else if (SHOULD_ALLOW_MIRRORED_TERRAIN) {
			Utilities.DebugLog ("Terrain IS mirrored.");
			isCurrentTerrainMirrored = true;
			desertTerrainBG.transform.localScale = new Vector3 (-scale, scale, scale);
			cityTerrainBG.transform.localScale = new Vector3 (-scale, scale, scale);
			
			foreach (Transform transform in desertTerrainBG.transform) {
				PolyNavObstacle polyNavObstacle = transform.gameObject.GetComponent<PolyNavObstacle> ();
				if (polyNavObstacle) {
					polyNavObstacle.invertPolygon = false;
				}
			}
			
			foreach (Transform transform in cityTerrainBG.transform) {
				PolyNavObstacle polyNavObstacle = transform.gameObject.GetComponent<PolyNavObstacle> ();
				if (polyNavObstacle) {
					polyNavObstacle.invertPolygon = false;
				}
			}
			
			foreach (TerrainObstacle obstacle in obstaclesList) {
				float obstacleScale = Mathf.Abs (obstacle.transform.localScale.x);
				obstacle.transform.position = new Vector2 (-obstacle.transform.position.x, obstacle.transform.position.y);
				obstacle.transform.localScale = new Vector3 (-obstacleScale, obstacleScale, obstacleScale);
				PolyNavObstacle polyNavObstacle = obstacle.GetComponent<PolyNavObstacle> ();
				polyNavObstacle.invertPolygon = false;
			}
		} else {
			Utilities.DebugLog ("???");
		}
	}

	public void DelayedReset () {
		Utilities.DebugLog ("DelayedReset ()");
		shouldCallDelayedReset = false;
		
		
		// Randomly flip the terrain
//		int randomTerrainFlip = UnityEngine.Random.Range (0, 2);
//		bool isMirrored = (randomTerrainFlip == 0 ? false : true);
//		SetTerrainToMirrored (isMirrored);

	}

	public TerrainObstacle GetExplosiveObstacleNearPlayerUnitIfExists () {
		Utilities.DebugLog ("GetExplosiveObstacleNearPlayerUnitIfExists ()");
		foreach (Unit unit in unitsList) {
			if (unit.isPlayerControlled && unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED) {
				TerrainObstacle nearestObstacle = GetNearestExplosiveObstacleForUnit (unit);
				if (nearestObstacle) {
					Utilities.DebugLog ("AdjustedDistance () aaaaaaa");
					float distanceToObstacle = AdjustedDistance ((Vector2)unit.gameObject.transform.position, (Vector2)nearestObstacle.gameObject.transform.position);
					// Adjust distance based on obstacle radius, to account for larger explosive range
					distanceToObstacle -= nearestObstacle.radius;
					if (distanceToObstacle <= UnitAI.AI_MAX_DISTANCE_TO_TARGET_EXPLOSIVE_OBSTACLES) {
						return nearestObstacle;
					}
				}
			}
		}
		return null; // No nearby explosive obstacle found
	}

	public void DeselectAllUnits () {
		Utilities.DebugLog ("DeselectAllUnits ()");
		//Utilities.DebugLog ("DeselectAllUnits()");
		Utilities.Hide (selectionArrow);

		foreach (Unit unit in unitsList) {
			unit.isSelected = false;
		}
	}

	public void SelectUnit (Unit unit) {
		Utilities.DebugLog ("SelectUnit ()");
		//Utilities.DebugLog ("SelectUnit(" + unit.Name () + ")");
		unit.isSelected = true;
		Utilities.Show (selectionArrow);
		selectionArrow.transform.position = unit.gameObject.transform.position;

        Text unitTypeText = unitNameText.GetComponent<Text> ();
        String unitNameString = "Selected: " + unit.Name();
        unitTypeText.text = unitNameString.ToLower();

		DebugPanel debugPanelScript = debugPanel.GetComponent<DebugPanel> ();
		debugPanelScript.UpdateDebugUIForUnit (unit);

	}

	public void MoveSelectedUnitTowardsTarget (Unit targetUnit) {
		Utilities.DebugLog ("MoveSelectedUnitTowardsTarget ()");
		foreach (Unit unit in unitsList) {
			if (unit.isSelected) {
				unit.moveTargetGameObject = targetUnit.gameObject;
				unit.isMoveTargetPositionSet = false;
				unit.MoveTowardsTarget (targetUnit.gameObject);

				unit.PlayAudioType (AudioType.AudioType_Unit_Moved);
			}
		}
	}

	public void MoveSelectedUnitTowardsPoint (Vector3 position) {
		Utilities.DebugLog ("MoveSelectedUnitTowardsPoint ()");
		foreach (Unit unit in unitsList) {
			if (unit.isSelected) {
				unit.moveTargetPosition = position;
				unit.isMoveTargetPositionSet = true;
				unit.moveTargetGameObject = null;
				unit.MoveTowardsPoint (position);

				unit.PlayAudioType (AudioType.AudioType_Unit_Moved);
			}
		}
	}

	public void MakeSelectedUnitAttackTarget (Unit targetUnit) {
		Utilities.DebugLog ("MakeSelectedUnitAttackTarget ()");
		foreach (Unit unit in unitsList) {
			if (unit.isSelected && unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED && unit.currentState != Unit.UnitState.UNIT_STATE_PAUSED
			    && targetUnit.currentState != Unit.UnitState.UNIT_STATE_DEAD && targetUnit.currentState != Unit.UnitState.UNIT_STATE_DISABLED && targetUnit.currentState != Unit.UnitState.UNIT_STATE_PAUSED) {
				unit.attackTargetGameObject = targetUnit.gameObject;
				unit.isAttackTargetPositionSet = false;

				unit.PlayAudioType (AudioType.AudioType_Unit_Attack);

				unit.isMoveTargetPositionSet = false;
				unit.moveTargetGameObject = null;

				// Show Target X at location
				GameObject targetX = (GameObject)Instantiate (this.targetXPrefab, targetUnit.gameObject.transform.position, targetUnit.gameObject.transform.rotation);
				Destroy (targetX, 0.3f);
			}
		}
	}

	public void MakeSelectedUnitAttackPoint (Vector3 position) {
		Utilities.DebugLog ("MakeSelectedUnitAttackPoint ()");
		foreach (Unit unit in unitsList) {
			if (unit.isSelected && unit.currentState != Unit.UnitState.UNIT_STATE_DEAD && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED && unit.currentState != Unit.UnitState.UNIT_STATE_PAUSED) {
				// Set the target position to the current position of the Mouse
				unit.attackTargetPosition = position;
				unit.isAttackTargetPositionSet = true;
				unit.attackTargetGameObject = null;

				unit.PlayAudioType (AudioType.AudioType_Unit_Attack);

				unit.isMoveTargetPositionSet = false;
				unit.moveTargetGameObject = null;

				// Show Target X at location
				GameObject targetX = (GameObject)Instantiate (this.targetXPrefab, position, Quaternion.identity);
				Destroy (targetX, 0.3f);
			}
		}
	}

	public void Button_OverlayMedkit_Tapped () {
        if (numMedpacks > 0)
        {
            Utilities.DebugLog("Button_OverlayMedkit_Tapped ()");
            isMedpackSelected = !isMedpackSelected;
            PlayUIAudioClip();
        }
	}

	public void OnExpandButtonTapped () {
		isUIExpanded = true;
		PlayUIAudioClip ();
	}

	public void OnMinimizeButtonTapped () {
		isUIExpanded = false;
		PlayUIAudioClip ();
	}

//	void InitHitPointText () {
//		GameObject temp = Instantiate (hitPointTextPrefab) as GameObject;
//		RectTransform tempRect = temp.GetComponent<RectTransform> ();
//		temp.transform.SetParent (canvas);
//		tempRect.transform.localPosition = hitPointTextPrefab.transform.localPosition;
//		tempRect.transform.localScale = hitPointTextPrefab.transform.localScale;
//	}

	public void PlayUIAudioClip () {
		AudioClip clip = UI_AudioClip;
		AudioSource audioSource = this.gameObject.GetComponent<AudioSource> ();
		float volume = 1.0f;
		audioSource.PlayOneShot (clip, volume);
//		audioSource.clip = clip;
//		audioSource.Play ();
	}

	public void ShowNavBar () {
//		mainMenuButton.gameObject.SetActive (true);
//		mapButton.gameObject.SetActive (true);
//		unitSelectionButton.gameObject.SetActive (true);
//		optionsButton.gameObject.SetActive (true);
//		shopButton.gameObject.SetActive (true);
		navbarPanel.gameObject.SetActive (true);
	}

	public void HideNavBar () {
//		mainMenuButton.gameObject.SetActive (false);
//		mapButton.gameObject.SetActive (false);
//		unitSelectionButton.gameObject.SetActive (false);
//		optionsButton.gameObject.SetActive (false);
//		shopButton.gameObject.SetActive (false);
		navbarPanel.gameObject.SetActive (false);
	}

	public void TitleScreen_Button_Tapped () {
		ShowNavBar ();

		NavBar_MapButton_Tapped ();
	}

	public void NavBar_MainMenuButton_Tapped () {
		PlayUIAudioClip ();

		HideAllPanelsExcept (UIPanelType.UI_PANEL_TYPE_MAINMENU);

		ShowMainMenuPanel ();
	}

	public void NavBar_MapButton_Tapped () {
		PlayUIAudioClip ();

		ShowLevelSelectionPanel ();

        // Bring Map screen to front, then navbar in front of that
		this.levelSelectionPanel.transform.SetAsLastSibling ();
		this.navbarPanel.transform.SetAsLastSibling ();

		//HideAllPanelsExcept (UIPanelType.UI_PANEL_TYPE_LEVELSELECTION);
	}

	public void NavBar_MercsButton_Tapped () {
		PlayUIAudioClip ();

        MercsSubMenuPanel mercsSubMenuPanelScript = mercsSubMenuPanel.GetComponent<MercsSubMenuPanel>();
        mercsSubMenuPanelScript.ShowPanel();

        // Bring Mercs sub-menu to front, then navbar in front of that
        this.mercsSubMenuPanel.transform.SetAsLastSibling();
        this.navbarPanel.transform.SetAsLastSibling();

		//ShowUnitSelectionPanel ();
		//this.unitSelectionPanel.transform.SetAsLastSibling ();
		//this.navbarPanel.transform.SetAsLastSibling ();
        HideAllPanelsExcept (UIPanelType.UI_PANEL_TYPE_MERCS_SUBMENU | UIPanelType.UI_PANEL_TYPE_NAVBAR | UIPanelType.UI_PANEL_TYPE_LEVELSELECTION);
	}

	public void NavBar_OptionsButton_Tapped () {
		PlayUIAudioClip ();
	}

	public void NavBar_ShopButton_Tapped () {
        PlayUIAudioClip ();

        ShopPanel shopPanelScript = shopPanel.GetComponent<ShopPanel>();
        shopPanelScript.ShowPanel();

        // Bring Shop to front, then navbar in front of that
        this.shopPanel.transform.SetAsLastSibling();
        this.navbarPanel.transform.SetAsLastSibling();

        HideAllPanelsExcept(UIPanelType.UI_PANEL_TYPE_SHOP | UIPanelType.UI_PANEL_TYPE_NAVBAR | UIPanelType.UI_PANEL_TYPE_LEVELSELECTION);
	}

	public void NavBar_SurvivalButton_Tapped () {
		PlayUIAudioClip ();
	}

	public void HideAllPanelsExcept (UIPanelType panelType) {
        if ((panelType & UIPanelType.UI_PANEL_TYPE_MAINMENU) == 0) {
			HideMainMenuPanel ();
        }
        if ((panelType & UIPanelType.UI_PANEL_TYPE_MISSIONOBJECTIVES) == 0)
        {
            MissionObjectivesPanel missionObjectivesPanelScript = missionObjectivesPanel.GetComponent<MissionObjectivesPanel>();
            missionObjectivesPanelScript.HidePanel();
        }
        if ((panelType & UIPanelType.UI_PANEL_TYPE_NAVBAR) == 0) {
			navbarPanel.gameObject.SetActive (false);
		}
        if ((panelType & UIPanelType.UI_PANEL_TYPE_LEVELSELECTION) == 0) {
			LevelSelectionPanel levelSelectionPanelScript = levelSelectionPanel.GetComponent<LevelSelectionPanel> ();
			levelSelectionPanelScript.HidePanel ();
		}
        if ((panelType & UIPanelType.UI_PANEL_TYPE_STARTMISSION) == 0) {
			StartMissionPanel startMissionPanelScript = startMissionPanel.GetComponent<StartMissionPanel> ();
			startMissionPanelScript.HidePanel ();
		}
        if ((panelType & UIPanelType.UI_PANEL_TYPE_TEAM) == 0) {
			TeamPanel teamPanelScript = teamPanel.GetComponent<TeamPanel> ();
			teamPanelScript.HidePanel ();
		}
        if ((panelType & UIPanelType.UI_PANEL_TYPE_UNITSELECTION) == 0) {
			UnitSelectionPanel unitSelectionPanelScript = unitSelectionPanel.GetComponent<UnitSelectionPanel> ();
			unitSelectionPanelScript.HidePanel ();
		}
        if ((panelType & UIPanelType.UI_PANEL_TYPE_LEVELCOMPLETION) == 0) {
			LevelCompletionPanel levelCompletionPanelScript = levelCompletionPanel.GetComponent<LevelCompletionPanel> ();
			levelCompletionPanelScript.HidePanel ();
        }
        if ((panelType & UIPanelType.UI_PANEL_TYPE_MERCS_SUBMENU) == 0)
        {
            MercsSubMenuPanel mercsSubMenuPanelScript = mercsSubMenuPanel.GetComponent<MercsSubMenuPanel>();
            mercsSubMenuPanelScript.HidePanel();
        }
        if ((panelType & UIPanelType.UI_PANEL_TYPE_SHOP) == 0)
        {
            ShopPanel shopPanelScript = shopPanel.GetComponent<ShopPanel>();
            shopPanelScript.HidePanel();
        }
	}

	public void AddDoubleDollars (int amount) {
		this.doubleDollars += amount;
		UpdateCurrencyText ();
	}

	public void SubtractDoubleDollars (int amount) {
		this.doubleDollars -= amount;
		UpdateCurrencyText ();
	}

	public void SetDoubleDollars (int amount) {
		this.doubleDollars = amount;
		UpdateCurrencyText ();
	}

	public void AddDiamonds (int amount) {
		this.diamonds += amount;
		UpdateCurrencyText ();
	}

	public void SubtractDiamonds (int amount) {
		this.diamonds -= amount;
		UpdateCurrencyText ();
	}

	public void SetDiamonds (int amount) {
		this.diamonds = amount;
		UpdateCurrencyText ();
	}

	public void UpdateCurrencyText () {
		Text dollarsCurrencyTextScript = dollarsCurrencyText.GetComponent<Text> ();
		Text diamondsCurrencyTextScript = diamondsCurrencyText.GetComponent<Text> ();
		dollarsCurrencyTextScript.text = this.doubleDollars.ToString ("N0");
		diamondsCurrencyTextScript.text = this.diamonds.ToString ("N0");
	}

    public void Button_OverlayPause_Tapped()
    {
        if (isPaused)
        {
            buttonPause.SetActive(true);
            buttonUnpause.SetActive(false);
            UnpauseTacticalCombat();
        }
        else
        {
            buttonPause.SetActive(false);
            buttonUnpause.SetActive(true);
            this.wasPausedFromDebug = true;
            PauseTacticalCombat();
            foreach (Unit unit in unitsList)
            {
                unit.StopMovingOrAttacking();
            }
        }
    }

	public void DebugPauseButton_OnClick () {
		if (isPaused) {
			UnpauseTacticalCombat ();

			Button debugZoomInButtonScript = debugZoomInButton.GetComponent<Button> ();
			debugZoomInButtonScript.interactable = false;
			Button debugZoomOutButtonScript = debugZoomOutButton.GetComponent<Button> ();
			debugZoomOutButtonScript.interactable = false;
		} else {
			this.wasPausedFromDebug = true;
			PauseTacticalCombat ();
			foreach (Unit unit in unitsList) {
				unit.StopMovingOrAttacking ();
			}

			Button debugZoomInButtonScript = debugZoomInButton.GetComponent<Button> ();
			debugZoomInButtonScript.interactable = true;
			Button debugZoomOutButtonScript = debugZoomOutButton.GetComponent<Button> ();
			debugZoomOutButtonScript.interactable = true;
		}
	}

	public void DebugZoomInButton_OnClick () {
		currentZoom -= 0.5f;
		if (currentZoom < MIN_ZOOM) {
			currentZoom = MIN_ZOOM;
		}
		cameraTargetZoom = currentZoom;
//		foreach (Unit unit in unitsList) {
//			if (unit.isSelected) {
//				cameraTargetPosition = unit.gameObject.transform.position;
//			}
//		}
	}

	public void DebugZoomOutButton_OnClick () {
		currentZoom += 0.5f;
		if (currentZoom >= MAX_ZOOM) {
			currentZoom = MAX_ZOOM;
			cameraTargetPosition = new Vector3 (0, 0, 0);
		}
		cameraTargetZoom = currentZoom;
	}

	public void DebugUnlockButton_OnClick () {
		AddDiamonds (99999);
		AddDoubleDollars (99999);
		maxUnlockLevel = 999;
	}

	public void DebugPanel_ResetButton_OnClick () {
		PlayUIAudioClip ();

		isMenuOpen = false;

		ShowLevelSelectionPanel ();

		ShowNavBar ();
	}
}
