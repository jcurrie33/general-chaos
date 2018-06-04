using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public enum ObjectType {
		OBJECT_UNIT = 0,
		OBJECT_OBSTACLE,
		OBJECT_WEAPON,
		OBJECT_COLLECTABLE
	};
	
	public enum UnitType {
		UNIT_NONE = 0,
		UNIT_MACHINEGUNNER,
		UNIT_BAZOOKA,
		UNIT_GRENADIER,
		UNIT_FLAMETHROWER,
		UNIT_GRENADELAUNCHER,
		UNIT_MEDIC,
		UNIT_KAMIKAZE,
		UNIT_BRAWLER,
		UNIT_SWAT
	};
	
	public enum UnitState {
		UNIT_STATE_IDLE = 0,
		UNIT_STATE_WALKING,
		UNIT_STATE_FIRING,
		UNIT_STATE_DISABLED,
		UNIT_STATE_DEAD,
		UNIT_STATE_PAUSED,
		UNIT_STATE_PREPARING_FOR_CLOSE_COMBAT,
		UNIT_STATE_CLOSE_COMBAT
	};
	
	public enum Direction {
		DIRECTION_NONE = 0,
		DIRECTION_UP,
		DIRECTION_UP_RIGHT,
		DIRECTION_RIGHT,
		DIRECTION_DOWN_RIGHT,
		DIRECTION_DOWN,
		DIRECTION_DOWN_LEFT,
		DIRECTION_LEFT,
		DIRECTION_UP_LEFT
	};

	public enum UnitMeleeAttackType {
		UNIT_MELEE_ATTACK_TYPE_LOW = 0,
		UNIT_MELEE_ATTACK_TYPE_MEDIUM,
		UNIT_MELEE_ATTACK_TYPE_HIGH
	};

	// Global constants	
	public const float MINIMUM_TIME_WITHIN_RANGE_TO_TRIGGER_CLOSECOMBAT = 3.0f;
	public const float UNIT_FIRERATE_MULTIPLIER = 20.0f;
	public const float UNIT_RANGE_MULTIPLIER = 0.18f;//9.0f;
	public const float UNIT_ATTACK_MULTIPLIER = 0.3f;
	public const float UNIT_PROJECTILE_SPEED_MULTIPLIER = 0.2f;//10.0f;
	public const float UNIT_SPEED_MULTIPLIER = 1.3f;
	public const float ANIMATION_SPEED_MULTIPLIER = 0.45f;
	public const float STOPPING_DISTANCE_TO_MOVE_TARGET = 0.2f;//10.0f;
	public const float TIME_TO_REGISTER_TAP_HOLD = 1.0f;
	public const float TIME_DELAY_BEFORE_AI_ATTACKING = 5.0f;
	public const float TIME_DELAY_BEFORE_HEALTH_REGENERATION = 0.005f;
	public const float HEALTH_REGENERATION_AMOUNT = 10.0f;
	public const float MINIMUM_TIME_BETWEEN_LOST_HEARTS = 1.0f;
	public const float MINIMUM_RANGE_FOR_BALLISTIC_PROJECTILES = 0.1f;//0.4f; // as percentage of Range
	public const float MINIMUM_RANGE_FOR_NONBALLISTIC_PROJECTILES = 0.1f; // as percentage of Range
	public Vector2 PROJECTILE_CREATION_POINT_OFFSET;
	public Vector2 MEDIC_PARTICLES_CREATION_POINT_OFFSET;
	public const float MACHINEGUNNER_ATTACK_VARIANCE = 2.0f;
	public const float GRENADELAUNCHER_ATTACK_VARIANCE = 1.0f;//2.0f;
	public const float PROJECTILE_SPEED = 20.0f;//1000.0f;

	public const float MEDIC_HEALTH_REGENERATION_AMOUNT = 1.2f;//25.0f;
	public const float MEDIC_HEALTH_REGENERATION_DISTANCE = 1.6f;//80.0f;
	public const float MEDIC_HEALTH_REGENERATION_COOLDOWN_TIME = 0.1f;//5.0f;
	public const float MEDIC_HEALTH_PARTICLE_COOLDOWN_TIME = 1.0f;//5.0f;

	public const float DELAY_BETWEEN_CALCULATING_COVER = 0.33f;
	private float timeAtLastCoverCalculation = 0;

	
	public bool isTrackedForDebug = false;
//	public GameObject unitNameLabel;
	public ObjectType objectType = ObjectType.OBJECT_UNIT;
	public UnitType unitType = UnitType.UNIT_NONE;
	public int level = 1;
	public int assignedTeam = 0;
	public bool isPlayerControlled = false;
	public bool isSelected = false;
	public bool isMoving = false;
	public Direction direction = Direction.DIRECTION_NONE;

	public bool isJumping;
	public Vector2 jumpVelocity;
	public Vector2 jumpOrigin;
	public float timeAtLastJump;

	public Vector2 damageOrigin;
	public bool isTakingDamage;
	public float timeAtTakeDamage;

	public Vector3 initialPosition;

	public GameObject moveTargetGameObject;
	public Vector3 moveTargetPosition;
	public bool isMoveTargetPositionSet = false;

	public GameObject attackTargetGameObject;
	public Vector3 attackTargetPosition;
	public bool isAttackTargetPositionSet = false;
	public bool isIntentionallyAttackingExplosiveObstacle = false;

	public bool isHealthAutoRegenerating = false;

	public UnitState currentState;
	public int debugView_currentState {get {return (int) currentState; }}
	public float previousHealth;
	public float currentHealth;
	public float previousArmor;
	public float currentArmor;
	public int previousLives;
	public int currentLives;
	public int previousHearts;
	public int currentHearts;
	public bool previousIsFacingLeft;
	public bool isFacingLeft = false;
	public bool isFacingDown = false;
	private bool forceUpdateIsFacingLeft = true;

	public float timeAtLastAttack = 0;
	public float timeAtLastLostHeart = 0;
	public float timeAtLastHealthRegeneration = 0;
	public float timeAtLastMedicHealthRegeneration = 0;
	public float timeAtLastMedicParticle = 0;


	private Animator animator;
	private TacticalCombat tcScript;

	public GameObject healthBar;
	public GameObject healthBarChild;
	public GameObject armorBar;
	public GameObject armorBarChild;
	public GameObject lives1;
	public GameObject lives2;
	public GameObject lives3;
	public GameObject healthBG;
	private GameObject unitBonusArrow;

	public GameObject shadow;
	private Renderer shadowRenderer;
	private SpriteRenderer shadowSpriteRenderer;

	public GameObject closeCombatRangeUI;
	private Renderer closeCombatRangeUIRenderer;
	private SpriteRenderer closeCombatRangeUISpriteRenderer;

	private AudioSource audioSource;

	public bool shouldStopWhenWithinFiringRange;
	
	public GameObject flamethrowerParticle;
	public bool isFlamethrowerActive = false;

	private Unit enemyCloseCombatUnit;
	private float closeCombatRangeDuration;
	private float timeAtLastCloseCombatRange;
	private Dictionary<Unit,float> timeAtEnteringCloseCombatRange;

	private bool previousIsUnderCover;
	private bool isUnderCover;
	public bool coverBonusDirtyFlag = true;

	private float health = -1.0f;
	private float attack = -1.0f;
	private float firerate = -1.0f;
	private float projectileSpeed = -1.0f;
	private float range = -1.0f;
	private float speed = -1.0f;
	private float melee = -1.0f;

	private PolyNavAgent _agent;
	public PolyNavAgent agent{
		get
		{
			if (!_agent)
				_agent = GetComponent<PolyNavAgent>();
			return _agent;			
		}
	}
	
	private UnitAI _unitAI;
	public UnitAI unitAI {
		get {
			if (!_unitAI) {
				_unitAI = this.gameObject.GetComponent<UnitAI> ();
			}
			return _unitAI;			
		}
	}

	private bool didStart = false;
	
	// Use this for initialization
	void Start () {
		if (!didStart) {
			didStart = true;

			// Force updating UI once initially
			this.previousHearts = -1;
			this.previousLives = -1;
			this.previousHealth = -1;
			this.previousArmor = -1;
			this.forceUpdateIsFacingLeft = true;

			Utilities.DebugLog ("Unit.Start ()");
			PROJECTILE_CREATION_POINT_OFFSET = new Vector2 (0, 0.7f/*35.0f*/);
			MEDIC_PARTICLES_CREATION_POINT_OFFSET = new Vector2 (0, 0.9f/*45.0f*/);

			this.animator = this.GetComponent<Animator> ();
			Camera mainCamera = Camera.main;
			this.tcScript = mainCamera.GetComponent<TacticalCombat> ();

			this.initialPosition = this.gameObject.transform.position;

			if (!this.healthBar) {
				PopulateUIObjects ();

				// Hide cover bonus arrow initially
				this.previousIsUnderCover = false;
				Utilities.Hide (this.unitBonusArrow);

				this.closeCombatRangeUI = (GameObject)Instantiate(this.tcScript.closeCombatRangeUIPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
				this.closeCombatRangeUIRenderer = closeCombatRangeUI.GetComponent<Renderer> ();
				this.closeCombatRangeUISpriteRenderer = closeCombatRangeUI.GetComponent<SpriteRenderer> ();

                if (!this.shadow)
                {
                    this.shadow = (GameObject)Instantiate(this.tcScript.unitShadowPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
                    this.shadowRenderer = shadow.GetComponent<Renderer>();
                    this.shadowSpriteRenderer = shadow.GetComponent<SpriteRenderer>();
                }

				this.audioSource = this.gameObject.GetComponent<AudioSource> ();
			}

			timeAtEnteringCloseCombatRange = new Dictionary<Unit,float> ();

			Reset ();
		}
	}

	public void MarkWithinCloseCombatRangeOfUnit (Unit enemyUnit) {
		timeAtLastCloseCombatRange = Time.time;

		if (enemyUnit == enemyCloseCombatUnit) {
			closeCombatRangeDuration += Time.deltaTime;
		} else {
			enemyCloseCombatUnit = enemyUnit;
			closeCombatRangeDuration = 0;
		}

		if (closeCombatRangeDuration > MINIMUM_TIME_WITHIN_RANGE_TO_TRIGGER_CLOSECOMBAT) {
			this.tcScript.PrepareCloseCombatForUnits (this, enemyCloseCombatUnit);
		}
	}
		
	public void PlayAudioType (TacticalCombat.AudioType audioType) {
        // Only play VO for player-controlled units
        if (!this.isPlayerControlled && audioType != TacticalCombat.AudioType.AudioType_SFX_Attack && audioType != TacticalCombat.AudioType.AudioType_SFX_Explode) {
            return;
        }

		AudioClip clip = this.tcScript.GetRandomAudioClipForUnitTypeWithAudioType (this.unitType, audioType);
        if (clip)
        {
            float volume = 1.0f;
            if (audioType == TacticalCombat.AudioType.AudioType_SFX_Attack)
            {
                volume = 0.07f;
                if (this.unitType == UnitType.UNIT_MACHINEGUNNER) {
                    volume = 0.03f;
                }
            }
            else if (audioType == TacticalCombat.AudioType.AudioType_SFX_Explode)
            {
                volume = 0.07f;
            }
            else if (audioType == TacticalCombat.AudioType.AudioType_Unit_TakeDamage)
            {
                volume = 0.05f;
            }
            else if (audioType == TacticalCombat.AudioType.AudioType_Unit_Dying)
            {
                volume = 0.07f;
            }
            else 
            {
                volume = 0.1f;
            }
            this.audioSource.PlayOneShot(clip, volume);
            //		this.audioSource.clip = clip;
            //		this.audioSource.Play ();
        }
	}

	public void PopulateUIObjects () {


		this.healthBar = gameObject.transform.Find("health-ui/ui-unit-health-bar").gameObject;
		this.healthBarChild = gameObject.transform.Find("health-ui/ui-unit-health-bar/ui-unit-health-bar-child").gameObject;
		this.armorBar = gameObject.transform.Find("health-ui/ui-unit-armor-bar").gameObject;
		this.armorBarChild = gameObject.transform.Find("health-ui/ui-unit-armor-bar/ui-unit-armor-bar-child").gameObject;
		this.lives1 = gameObject.transform.Find("health-ui/ui-unit-lives-1").gameObject;
		this.lives2 = gameObject.transform.Find("health-ui/ui-unit-lives-2").gameObject;
		this.lives3 = gameObject.transform.Find("health-ui/ui-unit-lives-3").gameObject;
		this.lives3.SetActive (false);
		this.healthBG = gameObject.transform.Find("health-ui/ui-unit-health-bg").gameObject;
		this.unitBonusArrow = gameObject.transform.Find("unit-bonus-arrow").gameObject;

		this.healthBarChild.GetComponent<SpriteRenderer> ().sortingOrder = 32767;
		this.armorBarChild.GetComponent<SpriteRenderer> ().sortingOrder = 32767;
		this.lives1.GetComponent<SpriteRenderer> ().sortingOrder = 32767;
		this.lives2.GetComponent<SpriteRenderer> ().sortingOrder = 32767;
		this.lives3.GetComponent<SpriteRenderer> ().sortingOrder = 32767;
		this.healthBG.GetComponent<SpriteRenderer> ().sortingOrder = 32767-10;
		this.unitBonusArrow.GetComponent<SpriteRenderer> ().sortingOrder = 32767;
	}
	
	// Update is called once per frame
	void Update () {
		if (tcScript.isLevelComplete || tcScript.isLevelFailed || tcScript.isMenuOpen) {
			return;
		}

		Utilities.DebugLog ("Unit.Update ()");
		// Sort depth of units
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

		if (currentState == UnitState.UNIT_STATE_PAUSED) {
			return;
		}

		if (Time.time > this.timeAtLastCoverCalculation + DELAY_BETWEEN_CALCULATING_COVER) {
			CalculateIsUnderCover ();
			this.timeAtLastCoverCalculation = Time.time;
		}

		if (this.tcScript.isCloseCombatActive) {
			RunCloseCombatUpdate ();
		} else {
			RunTacticalCombatUpdate ();
		}

	}

	public void UpdateUI () {
		if (!didStart) {
			Start ();
		}

		UnityEngine.Profiling.Profiler.BeginSample ("Unit.UpdateHealthUI");
		UpdateHealthUI ();
		UnityEngine.Profiling.Profiler.EndSample ();

		UpdateHeartsUI ();

		UpdateLivesUI ();

		UpdateCloseCombatRangeUI ();

		UpdateCoverBonusUI ();

		UpdateShadow ();
	}

	void LateUpdate () {
		UpdateUI ();
	}
	
	void OnEnable(){
		this.agent.OnDestinationReached += OnDestinationReached;
		this.agent.OnDestinationInvalid += OnDestinationReached;
	}
	
	void OnDisable(){
		this.agent.OnDestinationReached -= OnDestinationReached;
		this.agent.OnDestinationInvalid -= OnDestinationReached;
	}

	void OnDestinationReached () {
		StopRunning ();
		this.moveTargetGameObject = null;
		this.isMoveTargetPositionSet = false;
	}

	public void MoveTowardsTarget (GameObject target) {
		if (this.currentState != Unit.UnitState.UNIT_STATE_DEAD && this.currentState != Unit.UnitState.UNIT_STATE_DISABLED && this.currentState != Unit.UnitState.UNIT_STATE_PAUSED) {
			this.agent.SetDestination (target.transform.position);
            this.agent.maxSpeed = this.Speed () * UNIT_SPEED_MULTIPLIER;
			this.StartRunning ();
		}
	}
	
	public void MoveTowardsPoint (Vector3 position) {
		if (this.currentState != Unit.UnitState.UNIT_STATE_DEAD && this.currentState != Unit.UnitState.UNIT_STATE_DISABLED && this.currentState != Unit.UnitState.UNIT_STATE_PAUSED) {
			this.agent.SetDestination (position);
            this.agent.maxSpeed = this.Speed () * UNIT_SPEED_MULTIPLIER;
			this.StartRunning ();
		}
	}

	public void RunTacticalCombatUpdate () {
		Utilities.DebugLog ("Unit.RunTacticalCombatUpdate ()");
		// If Medic, heal nearby units
		if (this.unitType == UnitType.UNIT_MEDIC) {
			HealNearbyUnitsIfReady ();
		}

		// Regenerate health if Auto-Regenerating
		if (this.isHealthAutoRegenerating) {
			RegenerateHealthIfReady ();
		}

		// Make flamethrower follow unit and face target
		if (this.unitType == UnitType.UNIT_FLAMETHROWER) {
			FollowTargetWithFlamethrower ();
		}

		// Face in the direction the unit is attacking or moving
		if (this.attackTargetGameObject) {
			FaceTowardsPosition (this.attackTargetGameObject.transform.position);
		} else if (this.isAttackTargetPositionSet) {
			FaceTowardsPosition (this.attackTargetPosition);
		} else if (this.moveTargetGameObject) {
			FaceTowardsPosition (this.moveTargetGameObject.transform.position);
		} else if (this.isMoveTargetPositionSet) {
			FaceTowardsPosition (this.moveTargetPosition);
		}

	}
	
	public void RunCloseCombatUpdate () {
		Utilities.DebugLog ("Unit.RunCloseCombatUpdate ()");

		// Push the character backwards after taking damage
		if (this.isTakingDamage) {
			float t = 2.0f * (Time.time - timeAtTakeDamage);
			float cube = (1.6f * t - 2.0f) + 0.5f;
			float square = (1.6f * t - 2.0f) + 0.4f;
			float xOffset = 0.6f * cube * cube * cube + 0.4f * square * square + 1.0f;
			float dir = (this.isFacingLeft ? 1.0f : -1.0f);
            this.transform.position = new Vector3 (this.damageOrigin.x + dir * xOffset * TacticalCombat.MELEE_ATTACK_DAMAGE_PUSH_DISTANCE, this.transform.position.y, this.transform.position.z);

			if (t >= 1.0f) {
				this.isTakingDamage = false;
			}
		}

		// Handle jumping
		if (this.isJumping) {
			this.transform.position = new Vector3 (this.transform.position.x + this.jumpVelocity.x * TacticalCombat.CLOSECOMBAT_UNIT_WALK_SPEED
				, this.transform.position.y + this.jumpVelocity.y * TacticalCombat.CLOSECOMBAT_UNIT_JUMP_SPEED
				, this.transform.position.z /*+ adjustedTouchVector.z * CLOSECOMBAT_UNIT_WALK_SPEED*/);
			this.jumpVelocity = new Vector2 (this.jumpVelocity.x, this.jumpVelocity.y - TacticalCombat.CLOSECOMBAT_UNIT_JUMP_GRAVITY);

			if (this.jumpVelocity.y < 0 && this.transform.position.y <= this.jumpOrigin.y) {
				this.transform.position = new Vector3 (this.transform.position.x
					, this.jumpOrigin.y
					, this.transform.position.z);
				this.StopJumping ();
				this.timeAtLastJump = Time.time;
			}
		}

		// Run AI gameplay for CloseCombat enemy unit
		if (!this.isPlayerControlled && this.tcScript.isAIEnabled && Time.time > timeAtLastAttack + 0.8f && !this.isTakingDamage) {
			timeAtLastAttack = Time.time;
//			int random = (int)Mathf.Floor (UnityEngine.Random.Range (0.0f, 4.0f));
//			// 25% chance of jumping
//			if (random == 3) {
//				random = (int)Mathf.Floor (UnityEngine.Random.Range (3.0f, 6.0f));
//			}
			int random = (int)Mathf.Floor (UnityEngine.Random.Range (0.0f, 6.0f));
			switch (random) {
			case 0:
				StartMeleeAttackHigh ();
				break;
			case 1:
				StartMeleeAttackMedium ();
				break;
			case 2:
				StartMeleeAttackLow ();
				break;
			case 3:
				JumpInRandomDirection ();
				break;
			case 4:
				JumpInRandomDirection ();
				break;
			case 5:
				//JumpInRandomDirection ();
				break;
			}
			
		}


		// Keep units from getting too far from center
		if (this.transform.position.x > this.tcScript.closeCombatOriginPosition.x + TacticalCombat.MAX_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS) {
			this.transform.position = new Vector3 (this.tcScript.closeCombatOriginPosition.x + TacticalCombat.MAX_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS
				, this.transform.position.y
				, this.transform.position.z);
		}
		if (this.transform.position.x < this.tcScript.closeCombatOriginPosition.x - TacticalCombat.MAX_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS) {
			this.transform.position = new Vector3 (this.tcScript.closeCombatOriginPosition.x - TacticalCombat.MAX_DISTANCE_BETWEEN_CLOSE_COMBAT_UNITS
				, this.transform.position.y
				, this.transform.position.z);
		}
	}

	public void JumpInRandomDirection () {
		// Jump
		this.StartJumping ();
		this.jumpOrigin = new Vector2 (this.transform.position.x, this.transform.position.y);

		int randomJumpDirection = (int)Mathf.Floor (UnityEngine.Random.Range (0.0f, 3.0f));
		float xVelocity = 0;
		switch (randomJumpDirection) {
		case 0:
			xVelocity = 0;
			break;
		case 1:
			xVelocity = -100.0f;
			break;
		case 2:
			xVelocity = 100.0f;
			break;
		}
		this.jumpVelocity = new Vector2 (xVelocity, 1.0f);
	}

	public void StopMovingOrAttacking () {
		Utilities.DebugLog ("Unit.StopMovingOrAttacking ()");
		StopRunning ();
		this.moveTargetGameObject = null;
		this.isMoveTargetPositionSet = false;
		StopAttacking ();
		this.attackTargetGameObject = null;
		this.isAttackTargetPositionSet = false;
		HideFlamethrowerParticle ();
	}

	public void SelectOnlyThisUnit () {
		Utilities.DebugLog ("Unit.SelectOnlyThisUnit ()");
		this.tcScript.DeselectAllUnits ();
		this.tcScript.SelectUnit (this);
		PlayAudioType (TacticalCombat.AudioType.AudioType_Unit_Selected);
	}

	public void ApplyMedpack () {
		Utilities.DebugLog ("Unit.ApplyMedpack ()");
		if (this.currentState == Unit.UnitState.UNIT_STATE_DISABLED) {
			this.currentHealth = Health ();
			ReviveFromMedic ();
			ShowMedicParticle ();
			
			if (!this.tcScript.isPaused) {
				this.tcScript.numMedpacks--;
			}
			this.tcScript.isMedpackSelected = false;
		} else if (this.currentHealth < Health ()) {
			this.currentHealth = Health ();
			ShowMedicParticle ();

			if (!this.tcScript.isPaused) {
				this.tcScript.numMedpacks--;
			}
			this.tcScript.isMedpackSelected = false;
		} else {
			this.tcScript.isMedpackSelected = false;
		}
	}
	
	public void HealNearbyUnitsIfReady () {
		Utilities.DebugLog ("Unit.HealNearbyUnitsIfReady ()");
		if (this.currentState != UnitState.UNIT_STATE_DISABLED && this.currentState != UnitState.UNIT_STATE_DEAD && Time.time > timeAtLastMedicHealthRegeneration + MEDIC_HEALTH_REGENERATION_COOLDOWN_TIME) {
			bool hasUnitBeenHealed = false;
			if (this.tcScript) {
				foreach (Unit unit in this.tcScript.unitsList) {
					if (this != unit) {
						if (this.isPlayerControlled) {
							if (unit.isPlayerControlled) {
								// Heal unit if it needs healing and is within range
								Utilities.DebugLog ("AdjustedDistance () bbbbbbb");
								if (unit.currentHealth < Health () && TacticalCombat.AdjustedDistance (this.gameObject.transform.position, unit.gameObject.transform.position) < MEDIC_HEALTH_REGENERATION_DISTANCE) {
									unit.HealFromMedic ();
									hasUnitBeenHealed = true;
								}
							}
						} else {
							if (!unit.isPlayerControlled) {
								// Heal unit if it needs healing and is within range
								Utilities.DebugLog ("AdjustedDistance () ccccccc");
								if (unit.currentHealth < Health () && TacticalCombat.AdjustedDistance (this.gameObject.transform.position, unit.gameObject.transform.position) < MEDIC_HEALTH_REGENERATION_DISTANCE) {
									unit.HealFromMedic ();
									hasUnitBeenHealed = true;
								}
							}
						}
					}
				}
			}
			if (hasUnitBeenHealed) {
				timeAtLastMedicHealthRegeneration = Time.time;
			}
		}
	}

	public void RegenerateHealthIfReady () {
		Utilities.DebugLog ("Unit.RegenerateHealthIfReady ()");
		if (Time.time > timeAtLastHealthRegeneration + TIME_DELAY_BEFORE_HEALTH_REGENERATION) {
			timeAtLastHealthRegeneration = Time.time;
			this.currentHealth += HEALTH_REGENERATION_AMOUNT;
			if (this.currentHealth > Health ()) {
				this.currentHealth = Health ();
			}
		}
	}

	public void FollowTargetWithFlamethrower () {
		Utilities.DebugLog ("Unit.FollowTargetWithFlamethrower ()");
		if (this.flamethrowerParticle) {
			flamethrowerParticle.transform.position = new Vector3 (this.transform.position.x + ProjectileCreationPointOffset ().x, this.transform.position.y + ProjectileCreationPointOffset ().y, -1.0f);
		
			float speed = 10.0f;
			Vector3 vectorToTarget = new Vector3 (0, 0, 0);
			if (isAttackTargetPositionSet) {
				vectorToTarget = this.attackTargetPosition - transform.position;
			} else if (this.attackTargetGameObject) {
				vectorToTarget = this.attackTargetGameObject.transform.position - transform.position;
			}
			float angle = Mathf.Atan2 (vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
			Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
			flamethrowerParticle.transform.rotation = Quaternion.Slerp (flamethrowerParticle.transform.rotation, q, Time.deltaTime * speed);
		
			FaceTowardsPosition (this.attackTargetPosition);
			//flamethrowerParticle.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(flamethrowerParticle.transform.position.y * 100f) * -1;
			flamethrowerParticle.GetComponent<ParticleSystem> ().GetComponent<Renderer> ().sortingOrder = 32767;//Mathf.RoundToInt(transform.position.y * 100f) * -1
		}
	}

	public void CalculateIsUnderCover () {
		Utilities.DebugLog ("Unit.IsUnderCover ()");
		if (this.tcScript) {
			foreach (TerrainObstacle obstacle in this.tcScript.obstaclesList) {
				Utilities.DebugLog ("AdjustedDistance () eeeeeee");
				float distanceFromPosition = TacticalCombat.AdjustedDistance ((Vector2)obstacle.gameObject.transform.position, this.gameObject.transform.position);
				if (((obstacle.isIntact && obstacle.shouldProvideCoverWhenIntact) || (!obstacle.isIntact && obstacle.shouldProvideCoverWhenDestroyed)) && distanceFromPosition <= TacticalCombat.OBSTACLE_COVER_RANGE) {
					this.isUnderCover = true;
					return;
				}
			}
		}
		this.isUnderCover = false;
	}

	public bool IsUnderCover () {
		return this.isUnderCover;
	}

	public void AttackPosition (Vector2 attackPosition) {
		Utilities.DebugLog ("Unit.AttackPosition ()");
		if ((this.isPlayerControlled || this.tcScript.isAIEnabled) && this.unitType != UnitType.UNIT_BRAWLER && this.currentState != UnitState.UNIT_STATE_DEAD && this.currentState != UnitState.UNIT_STATE_DISABLED && this.currentState != UnitState.UNIT_STATE_PREPARING_FOR_CLOSE_COMBAT && this.currentState != UnitState.UNIT_STATE_CLOSE_COMBAT) {
            if (!this.isMoving || (this.unitType != UnitType.UNIT_BAZOOKA && this.unitType != UnitType.UNIT_FLAMETHROWER && this.unitType != UnitType.UNIT_GRENADIER && this.unitType != UnitType.UNIT_GRENADELAUNCHER && this.unitType != UnitType.UNIT_FLAMETHROWER)) {
				if (this.unitType != UnitType.UNIT_KAMIKAZE && this.unitType != UnitType.UNIT_BRAWLER) {
					StartAttacking ();
					
					//if (unitType == UnitType.UNIT_FLAMETHROWER) {
					//	ShowFlamethrowerParticle ();
					//}
					// Fire according to firerate (even Flamethrower shoots invisible projectile)
					if (Time.time - timeAtLastAttack > 1.0f / Firerate () * UNIT_FIRERATE_MULTIPLIER) {
						timeAtLastAttack = Time.time;

						if (this.unitType == Unit.UnitType.UNIT_MACHINEGUNNER) {
							float randomX = UnityEngine.Random.Range (0.0f, MACHINEGUNNER_ATTACK_VARIANCE) - 0.5f * MACHINEGUNNER_ATTACK_VARIANCE;
							float randomY = UnityEngine.Random.Range (0.0f, MACHINEGUNNER_ATTACK_VARIANCE) - 0.5f * MACHINEGUNNER_ATTACK_VARIANCE;
							attackPosition = new Vector2 (attackPosition.x + randomX, attackPosition.y + randomY);
						}

						if (this.unitType == Unit.UnitType.UNIT_GRENADELAUNCHER) {
							float randomX = UnityEngine.Random.Range (0.0f, GRENADELAUNCHER_ATTACK_VARIANCE) - 0.5f * GRENADELAUNCHER_ATTACK_VARIANCE;
							float randomY = UnityEngine.Random.Range (0.0f, GRENADELAUNCHER_ATTACK_VARIANCE) - 0.5f * GRENADELAUNCHER_ATTACK_VARIANCE;
							attackPosition = new Vector2 (attackPosition.x + randomX, attackPosition.y + randomY);
						}

						FireBulletAtPosition ((Vector2)(attackPosition));
						FaceTowardsPosition (attackPosition);
					}
				}
			}
		}
	}

	public bool IsWithinRangeOfMoveTarget () {
		Utilities.DebugLog ("Unit.IsWithinRangeOfMoveTarget ()");
		
		if ((this.moveTargetGameObject && TacticalCombat.AdjustedDistance (this.gameObject.transform.position, this.moveTargetGameObject.transform.position) <= 0.2 /*Range () * UNIT_RANGE_MULTIPLIER * 0.9*/)
		    || (this.isMoveTargetPositionSet && TacticalCombat.AdjustedDistance (this.gameObject.transform.position, this.moveTargetPosition) <= 0.2 /*Range () * UNIT_RANGE_MULTIPLIER * 0.9*/)) {
					
			return true;
		}
		
		//Utilities.DebugLog ("NOT Within range of move target");
		return false;
	}

	public bool IsWithinRangeOfGameObject (GameObject gameObject) {
		if (TacticalCombat.AdjustedDistance (this.gameObject.transform.position, gameObject.transform.position) <= Range () * UNIT_RANGE_MULTIPLIER * 0.9) {
			return true;
		}
		return false;
	}

	public bool IsWithinRangeOfAttackTarget () {
		Utilities.DebugLog ("Unit.IsWithinRangeOfAttackTarget ()");

		if ((this.attackTargetGameObject && TacticalCombat.AdjustedDistance (this.gameObject.transform.position, this.attackTargetGameObject.transform.position) <= Range () * UNIT_RANGE_MULTIPLIER * 0.9)
		    || (this.isAttackTargetPositionSet && TacticalCombat.AdjustedDistance (this.gameObject.transform.position, this.attackTargetPosition) <= Range () * UNIT_RANGE_MULTIPLIER * 0.9)) {
		
			//Utilities.DebugLog ("Within range of attack target");
				
			return true;
		}

		//Utilities.DebugLog ("NOT Within range of attack target");
		return false;
	}

	public void HideHealthUI () {
		Utilities.DebugLog ("Unit.HideHealthUI ()");
		Utilities.Hide (this.healthBar);
		Utilities.Hide (this.armorBar);
		Utilities.Hide (this.lives1);
		Utilities.Hide (this.lives2);
		Utilities.Hide (this.lives3);
		Utilities.Hide (this.healthBG);
	}

	public void ShowHealthUI () {
		Utilities.DebugLog ("Unit.ShowHealthUI ()");
		Utilities.Show (this.healthBar);
		Utilities.Show (this.armorBar);
		Utilities.Show (this.lives1);
		Utilities.Show (this.lives2);
		//Utilities.Show (this.lives3);
		Utilities.Show (this.healthBG);
		UpdateHealthUI ();
		UpdateLivesUI ();
	}

	public void FireBulletAtPosition (Vector2 position) {
		Utilities.DebugLog ("Unit.FireBulletAtPosition ()");
		if (!this.tcScript) {
			return;
		}

        // Track number of shots fired by all player units
        if (this.isPlayerControlled)
        {
            this.tcScript.numShotsFired++;
        }

		Vector3 projectileStartPosition = new Vector3 (this.gameObject.transform.position.x + ProjectileCreationPointOffset ().x, this.gameObject.transform.position.y + ProjectileCreationPointOffset ().y, -0.1f);

		// Get direction of shot
		Vector2 dir = position - (Vector2)(projectileStartPosition);
		dir = new Vector2 (TacticalCombat.VERTICAL_SQUASH_RATIO * dir.x, dir.y);
		dir.Normalize();
			

		// Check if shooting would hit an explosive obstacle
		if (UnitAI.AI_shouldAvoidTargetingExplosiveObstaclesNearFriendlies) {
			RaycastHit2D[] hits = Physics2D.RaycastAll (projectileStartPosition, dir);
			int currentIndex = 0;
			foreach (RaycastHit2D hit in hits) {
				Unit unit = hit.collider.gameObject.GetComponent<Unit> ();
				if (hit.collider != null && unit == null && currentIndex > 1) {
					TerrainObstacle obstacle = hit.collider.gameObject.GetComponent<TerrainObstacle> ();
					if (obstacle && obstacle.isDestructible && obstacle.isIntact && obstacle.shouldCauseExplosionWhenDestroyed) {
						if (isIntentionallyAttackingExplosiveObstacle) {
							Debug.Log ("Intentionally attacking explosive obstacle");
						} else {
							Debug.Log ("Don't attack explosive obstacle unless intentional!");
							return;
						}
					}
				}
				currentIndex++;
//				if (currentIndex > 1) {
//					break;
//				}
			}
		}

		PlayAudioType (TacticalCombat.AudioType.AudioType_SFX_Attack);

		GameObject bullet;
		if (this.unitType == UnitType.UNIT_GRENADIER) {
			bullet = (GameObject)Instantiate(this.tcScript.grenadePrefab, projectileStartPosition, this.gameObject.transform.rotation);
		} else if (this.unitType == UnitType.UNIT_GRENADELAUNCHER) {
			bullet = (GameObject)Instantiate(this.tcScript.grenadeLauncherPrefab, projectileStartPosition, this.gameObject.transform.rotation);
		} else if (this.unitType == UnitType.UNIT_BAZOOKA) {
			bullet = (GameObject)Instantiate(this.tcScript.bazookaPrefab, projectileStartPosition, this.gameObject.transform.rotation);
		} else if (this.unitType == UnitType.UNIT_FLAMETHROWER) {
			bullet = (GameObject)Instantiate(this.tcScript.invisibleProjectilePrefab, projectileStartPosition, this.gameObject.transform.rotation);
		} else {
			bullet = (GameObject)Instantiate(this.tcScript.bulletPrefab, projectileStartPosition, this.gameObject.transform.rotation);
		}

		bullet.GetComponent<Renderer> ().sortingOrder = 32767;
		Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();

		// Rotate the projectile to face the direction it is going
		if (this.unitType != UnitType.UNIT_GRENADIER /*&& this.unitType != UnitType.UNIT_GRENADELAUNCHER*/) {
			float speed = PROJECTILE_SPEED;
			Vector3 vectorToTarget = new Vector3 (position.x - this.gameObject.transform.position.x, position.y - this.gameObject.transform.position.y, 0 - this.gameObject.transform.position.z);
			float angle = Mathf.Atan2 (vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
			Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
			bullet.transform.rotation = q;//Quaternion.Slerp (bullet.transform.rotation, q, Time.deltaTime * speed);
		}




		Projectile projectile = bullet.GetComponent<Projectile> ();
		projectile.currentState = Projectile.ProjectileState.PROJECTILE_STATE_ACTIVE;
		projectile.initialPosition = bullet.transform.position;
		projectile.basePosition = projectile.initialPosition;
		projectile.range = Range () * UNIT_RANGE_MULTIPLIER;
		projectile.attack = Attack () * UNIT_ATTACK_MULTIPLIER;
		projectile.isPlayerControlled = this.isPlayerControlled;
		projectile.direction = dir;
		projectile.speed = ProjectileSpeed () * UNIT_PROJECTILE_SPEED_MULTIPLIER;
        projectile.sourceUnit = this;
		
		Utilities.DebugLog ("AdjustedDistance () hhhhhhh");
		float distanceToTarget = TacticalCombat.AdjustedDistance (this.gameObject.transform.position, position);


		projectile.isBullet = (this.unitType == UnitType.UNIT_MACHINEGUNNER || this.unitType == UnitType.UNIT_MEDIC || this.unitType == UnitType.UNIT_SWAT);
		projectile.isBazooka = (this.unitType == UnitType.UNIT_BAZOOKA);
		projectile.isFlamethrower = (this.unitType == UnitType.UNIT_FLAMETHROWER);
		projectile.isGrenadeLauncher = (this.unitType == UnitType.UNIT_GRENADELAUNCHER);


		if (this.unitType == UnitType.UNIT_GRENADIER || this.unitType == UnitType.UNIT_GRENADELAUNCHER) {
			projectile.isBallisticProjectile = true;
			if (this.unitType == UnitType.UNIT_GRENADELAUNCHER) {
				projectile.isBouncingProjectile = true;
			} else {
				projectile.isBouncingProjectile = false;
			}

			if (distanceToTarget < projectile.range && distanceToTarget > MINIMUM_RANGE_FOR_BALLISTIC_PROJECTILES * projectile.range) {
				projectile.range = distanceToTarget;
			} else if (distanceToTarget < MINIMUM_RANGE_FOR_BALLISTIC_PROJECTILES * projectile.range) {
				projectile.range = MINIMUM_RANGE_FOR_BALLISTIC_PROJECTILES * projectile.range;
			}
			//bulletRigidbody.AddForce(dir * ProjectileSpeed () * UNIT_PROJECTILE_SPEED_MULTIPLIER, ForceMode2D.Force);
		} else {
			projectile.isBallisticProjectile = false;
			projectile.isBouncingProjectile = false;

			if (distanceToTarget < projectile.range && distanceToTarget > MINIMUM_RANGE_FOR_NONBALLISTIC_PROJECTILES * projectile.range) {
				// Stop non-ballistic projectiles at their target
				//projectile.range = distanceToTarget * 1.05f;
			} else if (distanceToTarget < MINIMUM_RANGE_FOR_NONBALLISTIC_PROJECTILES * projectile.range) {
				projectile.range = MINIMUM_RANGE_FOR_NONBALLISTIC_PROJECTILES * projectile.range;
			}
			//bulletRigidbody.AddForce(dir * ProjectileSpeed () * UNIT_PROJECTILE_SPEED_MULTIPLIER, ForceMode2D.Force);
		}

		this.tcScript.projectilesList.Add (projectile);
	}

	public void FaceTowardsPosition (Vector2 position) {
		Utilities.DebugLog ("Unit.FaceTowardsPosition ()");
		// Face the animation in the proper direction
		if (position.x < transform.position.x) {
			FaceLeft ();
		} else {
			FaceRight ();
		}
		if (position.y < transform.position.y) {
			FaceDown ();
		} else {
			FaceUp ();
		}
	}

	public void LoseLife () {
		Utilities.DebugLog ("Unit.LoseLife ()");
		this.currentLives--;
		UpdateLivesUI ();
	}

	public void GainLife () {
		Utilities.DebugLog ("Unit.GainLife ()");
		this.currentLives++;
		UpdateLivesUI ();
	}

	public void ResetLives () {
		Utilities.DebugLog ("Unit.ResetLives ()");
		this.currentLives = 2;
		UpdateLivesUI ();
	}
	
	public void LoseHeart () {
		Utilities.DebugLog ("Unit.LoseHeart ()");
		if (Time.time > this.timeAtLastLostHeart + MINIMUM_TIME_BETWEEN_LOST_HEARTS) {
			this.timeAtLastLostHeart = Time.time;
			this.currentHearts--;
			if (this.currentHearts == 0) {
				KnockDown ();
				this.tcScript.EndCloseCombat ();
			}
			//Utilities.DebugLog ("Lose Heart");
			UpdateHeartsUI ();
		}
	}
	
	public void GainHeart () {
		Utilities.DebugLog ("Unit.GainHeart ()");
		this.currentHearts++;
		UpdateHeartsUI ();
	}
	
	public void ResetHearts () {
		Utilities.DebugLog ("Unit.ResetHearts ()");
		this.currentHearts = 5;
		UpdateHeartsUI ();
	}

	public void UpdateLivesUI () {
		Utilities.DebugLog ("Unit.UpdateLivesUI ()");
		if (this.currentLives != this.previousLives) {
			this.previousLives = this.currentLives;
			if (this.currentLives == 3) {
				//Utilities.Show (this.lives3)Utilities.Hide.Hide (this.lives2)Utilities.Hide.Hide (this.lives1);
			} else if (this.currentLives == 2) {
				Utilities.Hide (this.lives3);
				Utilities.Show (this.lives2);
				Utilities.Hide (this.lives1);
			} else if (this.currentLives == 1) {
				Utilities.Hide (this.lives3);
				Utilities.Hide (this.lives2);
				Utilities.Show (this.lives1);
			} else if (this.currentLives == 0) {
				Utilities.Hide (this.lives3);
				Utilities.Hide (this.lives2);
				Utilities.Hide (this.lives1);
			}
		}
	}

	public void UpdateHeartsUI () {
		Utilities.DebugLog ("Unit.UpdateHeartsUI ()");
        if (this.tcScript)
        {
            this.tcScript.UpdateHeartsUI();
        }
	}

	public void UpdateHealthUI () {
		Utilities.DebugLog ("Unit.UpdateHealthUI ()");
		int flip = (this.isFacingLeft ? -1 : 1);

		if (this.isFacingLeft != this.previousIsFacingLeft || forceUpdateIsFacingLeft) {
			if (this.isFacingLeft) {
				this.healthBar.transform.localPosition = new Vector3 (0.24f, 0, 0);
				this.armorBar.transform.localPosition = new Vector3 (0.24f, 0, 0);
			} else {
				this.healthBar.transform.localPosition = new Vector3 (-0.24f, 0, 0);
				this.armorBar.transform.localPosition = new Vector3 (-0.24f, 0, 0);
			}
		}

		if (this.currentHealth != this.previousHealth || this.isFacingLeft != this.previousIsFacingLeft || forceUpdateIsFacingLeft) {
			this.previousHealth = this.currentHealth;

			float healthPercentage = this.currentHealth / Health ();
			this.healthBar.transform.localScale = new Vector3 (healthPercentage * flip, 1, 1);
			this.healthBarChild.GetComponent<SpriteRenderer> ().color = new Color (1.0f - healthPercentage, healthPercentage, 0, 1);
		}

		if (this.currentArmor != this.previousArmor || this.isFacingLeft != this.previousIsFacingLeft || forceUpdateIsFacingLeft) {
			this.previousArmor = this.currentArmor;

			float armorPercentage = this.currentArmor / Armor ();
			this.armorBar.transform.localScale = new Vector3 (armorPercentage * flip, 1, 1);
			this.lives1.transform.localScale = new Vector3 (flip, 1, 1);
			this.lives2.transform.localScale = new Vector3 (flip, 1, 1);
			//this.lives3.transform.localScale = new Vector3 (flip, 1, 1);
		}

		if (this.isFacingLeft != this.previousIsFacingLeft || forceUpdateIsFacingLeft) {
			forceUpdateIsFacingLeft = false;
			this.previousIsFacingLeft = this.isFacingLeft;
		}
	}

	public void UpdateCoverBonusUI () {

		if (this.isUnderCover != this.previousIsUnderCover || this.coverBonusDirtyFlag) {
			this.previousIsUnderCover = this.isUnderCover;
			this.coverBonusDirtyFlag = false;

			if (IsUnderCover () && !this.tcScript.isCloseCombatActive) {
				Utilities.Show (this.unitBonusArrow);
			} else {
				Utilities.Hide (this.unitBonusArrow);
			}
		}
	}

	public void UpdateCloseCombatRangeUI () {
		if (TacticalCombat.DEBUG_SHOULD_DISABLE_CLOSE_COMBAT || this.currentState == Unit.UnitState.UNIT_STATE_DEAD || this.currentState == Unit.UnitState.UNIT_STATE_DISABLED || this.currentState == Unit.UnitState.UNIT_STATE_PAUSED) {
			this.closeCombatRangeUISpriteRenderer.color = new Color(1.0f,1.0f,1.0f,0);
			return;
		}

		UnityEngine.Profiling.Profiler.BeginSample ("Unit.UpdateCloseCombatRangeUI sort");
		this.closeCombatRangeUI.transform.localPosition = this.transform.position;
		this.closeCombatRangeUIRenderer.sortingOrder = -32767;
		UnityEngine.Profiling.Profiler.EndSample ();

		UnityEngine.Profiling.Profiler.BeginSample ("Unit.UpdateCloseCombatRangeUI distance");
		// Update tracking for when units enter Close Combat range
		foreach (Unit enemyUnit in this.tcScript.unitsList) {
			// Check only active units.
			if (enemyUnit.currentState != Unit.UnitState.UNIT_STATE_DEAD && enemyUnit.currentState != Unit.UnitState.UNIT_STATE_DISABLED && enemyUnit.currentState != Unit.UnitState.UNIT_STATE_PAUSED) {	
				// Check only enemy units. Kamikaze unit triggers explosion rather than Close Combat.
				if (this.isPlayerControlled != enemyUnit.isPlayerControlled && this.unitType != Unit.UnitType.UNIT_KAMIKAZE && enemyUnit.unitType != Unit.UnitType.UNIT_KAMIKAZE) {
					float distance = TacticalCombat.AdjustedDistance ((Vector2)this.gameObject.transform.position, (Vector2)enemyUnit.gameObject.transform.position);
					if (distance <= TacticalCombat.MIN_DISTANCE_TO_TRIGGER_CLOSE_COMBAT) {
						if (!timeAtEnteringCloseCombatRange.ContainsKey (enemyUnit)) {
							timeAtEnteringCloseCombatRange.Add (enemyUnit, Time.time);
						}
					} else {
						timeAtEnteringCloseCombatRange.Remove (enemyUnit);
					}
				}
			} else {
				if (timeAtEnteringCloseCombatRange.ContainsKey (enemyUnit)) {
					timeAtEnteringCloseCombatRange.Remove (enemyUnit);
				}
			}
		}
		UnityEngine.Profiling.Profiler.EndSample ();

		UnityEngine.Profiling.Profiler.BeginSample ("Unit.UpdateCloseCombatRangeUI maxtime");
		// Determine the longest time any unit has been within Close Combat range
		float maxTimeWithinCloseCombatRange = 0;
		foreach (KeyValuePair<Unit, float> keyValuePair in timeAtEnteringCloseCombatRange) {
			float duration = Time.time - keyValuePair.Value;
			if (duration > maxTimeWithinCloseCombatRange) {
				maxTimeWithinCloseCombatRange = duration;
			}
		}
		UnityEngine.Profiling.Profiler.EndSample ();

		// Show Close Combat range indicator as appropriate
		float alpha = maxTimeWithinCloseCombatRange / MINIMUM_TIME_WITHIN_RANGE_TO_TRIGGER_CLOSECOMBAT;
		//alpha = alpha * alpha * alpha * alpha * alpha * alpha * alpha * alpha * alpha * alpha;
		this.closeCombatRangeUISpriteRenderer.color = new Color(1.0f,1.0f,1.0f,alpha);

//		// Trigger Close Combat if time is surpassed
//		if (maxTimeWithinCloseCombatRange >= MINIMUM_TIME_WITHIN_RANGE_TO_TRIGGER_CLOSECOMBAT) {
//			this.tcScript.PrepareCloseCombatForUnits (this, enemyCloseCombatUnit);
//		}
	}

	public void UpdateShadow () {
		if (this.currentState == Unit.UnitState.UNIT_STATE_DEAD || this.currentState == Unit.UnitState.UNIT_STATE_DISABLED
			|| (this.tcScript.isCloseCombatActive && this != this.tcScript.closeCombatPlayerUnit && this != this.tcScript.closeCombatEnemyUnit)) {

			this.shadowSpriteRenderer.color = new Color(1.0f,1.0f,1.0f,0);
			return;
		}

		if (this.isJumping) {
			this.shadow.transform.localPosition = new Vector3 (this.transform.position.x, this.jumpOrigin.y, this.transform.position.z);
			this.shadowRenderer.sortingOrder = -32767;

			float jumpHeight = this.transform.position.y - this.jumpOrigin.y;
			float shadowScale = 0.45f * (1 - Mathf.Log10 (jumpHeight + 1));
			this.shadow.transform.localScale = new Vector3 (shadowScale, shadowScale, shadowScale);

			float alpha = shadowScale + 0.15f;
			this.shadowSpriteRenderer.color = new Color(1.0f,1.0f,1.0f,alpha);
		} else {
			this.shadow.transform.localPosition = this.transform.position;

			float shadowScale = 0.45f;
			this.shadow.transform.localScale = new Vector3 (shadowScale, shadowScale, shadowScale);

			float alpha = 0.6f;
			this.shadowSpriteRenderer.color = new Color(1.0f,1.0f,1.0f,alpha);
		}

	}

//	public void AttackNearestEnemyUnit () {
//		Utilities.DebugLog ("Unit.AttackNearestEnemyUnit ()");
//		Unit nearestEnemy = this.tcScript.GetNearestEnemyForUnit (this);
//		if (nearestEnemy) {
//			this.attackTargetGameObject = nearestEnemy.gameObject;
//			this.isAttackTargetPositionSet = false;
//		}
//	}
//	
//	public void AttackNearestEnemyUnitWhenWithinFiringRange () {
//		Utilities.DebugLog ("Unit.AttackNearestEnemyUnitWhenWithinFiringRange ()");
//		// TODO: This method isn't programmed properly yet
//		Unit nearestEnemy = this.tcScript.GetNearestEnemyForUnit (this);
//		if (nearestEnemy) {
//			this.attackTargetGameObject = nearestEnemy.gameObject;
//			this.isAttackTargetPositionSet = false;
//		}
//	}
//	
//	public void AttackGameObjectWhenWithinFiringRange (GameObject gameObject) {
//		Utilities.DebugLog ("Unit.AttackGameObjectWhenWithinFiringRange ()");
//		// TODO: This method isn't programmed properly yet
//		this.attackTargetGameObject = gameObject;
//		this.isAttackTargetPositionSet = false;
//	}
//
//	public void MoveTowardsNearestEnemyUnit () {
//		Utilities.DebugLog ("Unit.MoveTowardsNearestEnemyUnit ()");
//		if (this.currentState != UnitState.UNIT_STATE_DEAD && this.currentState != UnitState.UNIT_STATE_DISABLED  && this.currentState != UnitState.UNIT_STATE_PAUSED) {
//			if (this.tcScript) {
//				Unit nearestEnemy = this.tcScript.GetNearestEnemyForUnit (this);
//				if (nearestEnemy) {
//					this.shouldStopWhenWithinFiringRange = false;
//					StartRunning ();
//					this.moveTargetGameObject = nearestEnemy.gameObject;
//					this.isMoveTargetPositionSet = false;
//					this.agent.SetDestination (nearestEnemy.gameObject.transform.position);
//					this.agent.maxSpeed = this.Speed ();
//				}
//			}
//		}
//	}
//	
//	public void MoveWithinFiringRangeOfNearestEnemyUnit () {
//		Utilities.DebugLog ("Unit.MoveWithinFiringRangeOfNearestEnemyUnit ()");
//		// TODO: This method isn't programmed properly yet
//		MoveTowardsNearestEnemyUnit ();
//		this.shouldStopWhenWithinFiringRange = true;
//	}
//	
//	public void MoveWithinFiringRangeOfGameObject (GameObject gameObject) {
//		Utilities.DebugLog ("Unit.MoveWithinFiringRangeOfGameObject ()");
//		// TODO: This method isn't programmed properly yet
//		MoveTowardsGameObject (gameObject);
//		this.shouldStopWhenWithinFiringRange = true;
//	}
//	
//	public void MoveTowardsGameObject (GameObject gameObject) {
//		Utilities.DebugLog ("Unit.MoveTowardsGameObject ()");
//		if (this.currentState != UnitState.UNIT_STATE_DEAD && this.currentState != UnitState.UNIT_STATE_DISABLED  && this.currentState != UnitState.UNIT_STATE_PAUSED) {
//			this.shouldStopWhenWithinFiringRange = false;
//			StartRunning ();
//			this.moveTargetGameObject = gameObject;
//			this.isMoveTargetPositionSet = false;
//			this.agent.SetDestination (gameObject.transform.position);
//			this.agent.maxSpeed = this.Speed ();
//		}
//	}
//
//	public void MoveWithinFiringRangeOfAttackTarget () {
//		Utilities.DebugLog ("Unit.MoveWithinFiringRangeOfAttackTarget ()");
//		MoveTowardsAttackTarget ();
//		this.shouldStopWhenWithinFiringRange = true;
//	}

	public void ShowMedicParticle () {
		Utilities.DebugLog ("Unit.ShowMedicParticle ()");
		GameObject medicParticles = (GameObject)Instantiate (this.tcScript.medicParticlesPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
		medicParticles.transform.position = new Vector3 (medicParticles.transform.position.x + MEDIC_PARTICLES_CREATION_POINT_OFFSET.x, medicParticles.transform.position.y + MEDIC_PARTICLES_CREATION_POINT_OFFSET.y, -0.1f);
		medicParticles.GetComponent<ParticleSystem> ().GetComponent<Renderer> ().sortingOrder = 32767;
		Destroy (medicParticles, 1.0f);
	}

	public void ShowFlamethrowerParticle () {
		isFlamethrowerActive = true;
		Utilities.DebugLog ("Unit.ShowFlamethrowerParticle ()");
		if (this.flamethrowerParticle) {
			Utilities.Show (this.flamethrowerParticle);
		} else {
			flamethrowerParticle = (GameObject)Instantiate (this.tcScript.flamethrowerParticlesPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
			flamethrowerParticle.transform.position = new Vector3 (flamethrowerParticle.transform.position.x + ProjectileCreationPointOffset ().x, flamethrowerParticle.transform.position.y + ProjectileCreationPointOffset ().y, -0.1f);
			flamethrowerParticle.GetComponent<ParticleSystem> ().GetComponent<Renderer> ().sortingOrder = 32767;
		}
	}

	public void HideFlamethrowerParticle () {
		isFlamethrowerActive = false;
		Utilities.DebugLog ("Unit.HideFlamethrowerParticle ()");
		if (this.flamethrowerParticle) {
			Utilities.Hide (this.flamethrowerParticle);
		}
	}

	public void DestroyFlamethrowerParticle () {
		isFlamethrowerActive = false;
		Utilities.DebugLog ("Unit.DestroyFlamethrowerParticle ()");
		if (this.flamethrowerParticle) {
			Destroy (this.flamethrowerParticle);
		}
	}

	// Reset all stats for this unit
	public void Reset () {
		Utilities.DebugLog ("Unit.Reset ()");
		if (!this.animator) {
			this.animator = this.GetComponent<Animator>();
		}
		this.currentState = UnitState.UNIT_STATE_IDLE;
		this.previousHealth = -1; // Force UI update
		this.currentHealth = Health ();
		this.previousArmor = -1; // Force UI update
		this.currentArmor = Armor ();
		this.isSelected = false;
		this.isMoving = false;
		this.direction = Direction.DIRECTION_NONE;
		this.isJumping = false;
		this.timeAtLastJump = 0;
		this.isTakingDamage = false;

		if (!this.isPlayerControlled && TacticalCombat.DEBUG_SHOULD_GIVE_AI_LOW_HEALTH) {
			this.currentHealth = TacticalCombat.DEBUG_AI_LOW_HEALTH_PERCENTAGE * Health ();
			this.currentArmor = 0;
		}

		this.isMoveTargetPositionSet = false;
		this.moveTargetGameObject = null;
		this.isAttackTargetPositionSet = false;
		this.attackTargetGameObject = null;

		this.gameObject.transform.position = this.initialPosition;

		this.timeAtLastAttack = 0;
		this.timeAtLastLostHeart = 0;
		this.timeAtLastHealthRegeneration = 0;
		this.timeAtLastMedicHealthRegeneration = 0;
		this.timeAtLastMedicParticle = 0;

		this.shouldStopWhenWithinFiringRange = false;

		this.isFlamethrowerActive = false;

		// Stop pathfinding movement
		agent.Stop ();

		// Reset animation states
		SetUnitTypeForAnimation ();
		StopRunning ();
		StopAttacking ();
		StopCloseCombat ();
		Revive ();
		ResetLives (); // Must be done after call to Revive
		ResetHearts ();
	}

	// Called from Projectile.cs
	public void TakeDamage (float damage) {
		Utilities.DebugLog ("Unit.TakeDamage ()");

		// Apply multiplier if Unit is under cover
		if (IsUnderCover ()) {
			damage *= TacticalCombat.OBSTACLE_COVER_DAMAGE_MULTIPLIER;
		}

		if (currentState != UnitState.UNIT_STATE_DEAD && currentState != UnitState.UNIT_STATE_DISABLED) {
			if (currentArmor > 0) {
				float remainingDamage = 0;
				if (damage > currentArmor) {
					remainingDamage = damage - currentArmor;
				}
				currentArmor -= damage;
				if (currentArmor < 0) {
					currentArmor = 0;
				}
				if (currentHealth > 0 && remainingDamage > 0) {

					PlayAudioType (TacticalCombat.AudioType.AudioType_Unit_TakeDamage);

					currentHealth -= remainingDamage;
					if (currentHealth <= 0) {
						currentHealth = 0;
						KnockDown ();
					}
				}
			} else {
				if (currentHealth > 0 && damage > 0) {

					PlayAudioType (TacticalCombat.AudioType.AudioType_Unit_TakeDamage);

					currentHealth -= damage;
					if (currentHealth <= 0) {
						currentHealth = 0;
						KnockDown ();
					}
				}
			}

			InitHitPointTextWithValue (Mathf.RoundToInt (damage));
		}
	}

	void InitHitPointTextWithValue (int value) {
		GameObject temp = Instantiate (this.tcScript.hitPointTextPrefab) as GameObject;
		Text tempText = temp.GetComponentInChildren<Text> ();
		tempText.text = value.ToString ();
		RectTransform tempRect = temp.GetComponent<RectTransform> ();
		temp.transform.SetParent (this.tcScript.canvas.transform);
		
		//first you need the RectTransform component of your canvas
		RectTransform CanvasRect = this.tcScript.canvas.GetComponent<RectTransform> ();
		//then you calculate the position of the UI element
		//0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
		Vector2 ViewportPosition = Camera.main.WorldToViewportPoint (this.gameObject.transform.position);
		float randomX = (Random.value * 2.0f - 1.0f) * 12.0f;
		float randomY = (Random.value * 2.0f - 1.0f) * 8.0f;
		Vector2 WorldObject_ScreenPosition = new Vector2 (
			((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)) + randomX,
			((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)) + randomY);
		//now you can set the position of the ui element
		tempRect.anchoredPosition = WorldObject_ScreenPosition;

		//tempRect.transform.localPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, this.gameObject.transform.position);//this.gameObject.transform.localPosition;
		tempRect.transform.localScale = this.tcScript.hitPointTextPrefab.transform.localScale;

		temp.GetComponentInChildren<Animator> ().SetTrigger ("Hit");

		Destroy (tempRect.gameObject, 1);
	}

//	public void TakeMeleeDamage () {
//		Utilities.DebugLog ("Unit.TakeMeleeDamage ()");
//
//		StartCoroutine (TakeMeleeDamageAfterDelay (0.16f));
//
//	}

	public IEnumerator TakeMeleeDamageAfterDelay (float delay) {
		yield return new WaitForSeconds (delay);

		bool shouldTakeDamage = true;

		// Don't take damage if already taking damage
		if (this.isTakingDamage)
			shouldTakeDamage = false;

		// Don't take damage if characters are not next to each other vertically
		if (Mathf.Abs (this.tcScript.closeCombatPlayerUnit.transform.position.y - this.tcScript.closeCombatEnemyUnit.transform.position.y) > 1.0f) {
			shouldTakeDamage = false;
		}

		if (this.isPlayerControlled) {
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Placeholder Melee Attack Low"))
				shouldTakeDamage = false;
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Placeholder Melee Attack Medium"))
				shouldTakeDamage = false;
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Placeholder Melee Attack High"))
				shouldTakeDamage = false;
		}

		if (shouldTakeDamage) {
			LoseHeart ();

			this.damageOrigin = new Vector2 (this.transform.position.x, this.transform.position.y);
			this.isTakingDamage = true;
			this.timeAtTakeDamage = Time.time;

			StartMeleeDamaged ();
		}
	}
	
	public void HandleIncomingMeleeAttack (UnitMeleeAttackType attackType) {
		Utilities.DebugLog ("Unit.HandleIncomingMeleeAttack ()");
//		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Placeholder Melee Idle")) {
//			TakeMeleeDamage ();
//		}
		if ( attackType == UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_LOW 
		    && !(animator.GetCurrentAnimatorStateInfo (0).IsName ("Placeholder Melee Block Low")) ) {

            StartCoroutine (TakeMeleeDamageAfterDelay (TacticalCombat.MELEE_ATTACK_LOW_DELAY_BEFORE_TAKING_DAMAGE));
		}
		if ( attackType == UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_MEDIUM 
		    && !(animator.GetCurrentAnimatorStateInfo (0).IsName ("Placeholder Melee Block Medium")) ) {

            StartCoroutine (TakeMeleeDamageAfterDelay (TacticalCombat.MELEE_ATTACK_MEDIUM_DELAY_BEFORE_TAKING_DAMAGE));
		}
		if ( attackType == UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_HIGH 
		    && !(animator.GetCurrentAnimatorStateInfo (0).IsName ("Placeholder Melee Block High")) ) {

            StartCoroutine (TakeMeleeDamageAfterDelay (TacticalCombat.MELEE_ATTACK_HIGH_DELAY_BEFORE_TAKING_DAMAGE));
		}
	}
	
	public void FaceLeft () {
		Utilities.DebugLog ("Unit.FaceLeft ()");
		if (!this.isFacingLeft) {
			if (animator) {
				animator.transform.Rotate(0, 180, 0);
			}
			this.isFacingLeft = true;
		}
	}

	public void FaceRight () {
		Utilities.DebugLog ("Unit.FaceRight ()");
		if (this.isFacingLeft) {
			if (animator) {
				animator.transform.Rotate(0, 180, 0);
			}
			this.isFacingLeft = false;
		}
	}

	public void FaceUp () {
		if (animator) {
			animator.SetBool ("IsFacingUp", true);
		}
		this.isFacingDown = false;
	}

	public void FaceDown () {
		if (animator) {
			animator.SetBool ("IsFacingUp", false);
		}
		this.isFacingDown = true;
	}

	public void StartRunning () {
		Utilities.DebugLog ("Unit.StartRunning ()");
		this.isMoving = true;
		if (animator) {
			animator.SetFloat ("Speed", Speed () * ANIMATION_SPEED_MULTIPLIER);
			animator.SetBool ("IsRunning", true);
		}
	}
	
	public void StopRunning () {
		Utilities.DebugLog ("Unit.StopRunning ()");
		this.isMoving = false;
		this.agent.Stop ();
		if (animator) {
			animator.SetBool ("IsRunning", false);
		}
	}

	public void StartJumping () {
		Utilities.DebugLog ("Unit.StartJumping ()");
		this.isJumping = true;
		if (animator) {
			animator.SetFloat ("Speed", Speed () * ANIMATION_SPEED_MULTIPLIER);
			animator.SetBool ("IsJumping", true);
		}
	}

	public void StopJumping () {
		Utilities.DebugLog ("Unit.StopJumping ()");
		this.isJumping = false;
		if (animator) {
			animator.SetBool ("IsJumping", false);
		}
	}
	
	public void StartAttacking () {
		Utilities.DebugLog ("Unit.StartAttacking ()");
		if (animator) {
			animator.SetFloat ("FireRate", 2.0f);//Firerate () * 0.01f);
			animator.SetBool ("IsAttacking", true);
		}
	}
	
	public void StopAttacking () {
		Utilities.DebugLog ("Unit.StopAttacking ()");
		if (animator) {
			animator.SetBool ("IsAttacking", false);
		}
	}

	public void SetUnitTypeForAnimation () {
		if (animator) {
			animator.SetBool ("IsMachineGunner", false);

			switch (unitType) {
			case UnitType.UNIT_MACHINEGUNNER:
				animator.SetBool ("IsMachineGunner", true);
				break;
			case UnitType.UNIT_BAZOOKA:
				break;
			case UnitType.UNIT_GRENADIER:
				break;
			case UnitType.UNIT_FLAMETHROWER:
				break;
			case UnitType.UNIT_GRENADELAUNCHER:
				break;
			case UnitType.UNIT_MEDIC:
				break;
			case UnitType.UNIT_KAMIKAZE:
				break;
			case UnitType.UNIT_BRAWLER:
				break;
			case UnitType.UNIT_SWAT:
				break;
			}
		}
	}

	public void StartCloseCombat () {
		Utilities.DebugLog ("Unit.StartCloseCombat ()");
		if (animator) {
			animator.SetBool ("IsCloseCombatActive", true);
		}
		HideFlamethrowerParticle ();
	}
	
	public void StopCloseCombat () {
		Utilities.DebugLog ("Unit.StopCloseCombat ()");
		if (this.currentState == UnitState.UNIT_STATE_CLOSE_COMBAT) {
			this.currentState = UnitState.UNIT_STATE_IDLE;
		}
		if (animator) {
			animator.SetBool ("IsCloseCombatActive", false);
		}
	}

	public void StartMeleeAttackHigh () {
		Utilities.DebugLog ("Unit.StartMeleeAttackHigh ()");
		if (animator) {
			animator.SetTrigger ("MeleeAttackHigh");
		}
		if (this.isPlayerControlled) {
			this.tcScript.MeleeAttackEnemy (UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_HIGH);
		} else {
			this.tcScript.MeleeAttackPlayer (UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_HIGH);
		}
	}
	
	public void StartMeleeAttackMedium () {
		Utilities.DebugLog ("Unit.StartMeleeAttackMedium ()");
		if (animator) {
			animator.SetTrigger ("MeleeAttackMedium");
		}
		if (this.isPlayerControlled) {
			this.tcScript.MeleeAttackEnemy (UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_MEDIUM);
		} else {
			this.tcScript.MeleeAttackPlayer (UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_MEDIUM);
		}
	}
	
	public void StartMeleeAttackLow () {
		Utilities.DebugLog ("Unit.StartMeleeAttackLow ()");
		if (animator) {
			animator.SetTrigger ("MeleeAttackLow");
		}
		if (this.isPlayerControlled) {
			this.tcScript.MeleeAttackEnemy (UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_LOW);
		} else {
			this.tcScript.MeleeAttackPlayer (UnitMeleeAttackType.UNIT_MELEE_ATTACK_TYPE_LOW);
		}
	}

//	public bool IsAnimatorPlaying (){
//		if (animator) {
//			return animator.GetCurrentAnimatorStateInfo(0).length >
//				animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//		}
//		return false;
//	}

	public bool IsAnimatorPlaying (string stateName){
		if (animator) {
			AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo (0);
			return currentState.shortNameHash == Animator.StringToHash (stateName);
		}
		return false;
	}

	public bool IsAnimatorPlayingMeleeAttack () {
		if (IsAnimatorPlaying ("Placeholder Melee Attack High")) {
			return true;
		}
		if (IsAnimatorPlaying ("Placeholder Melee Attack Medium")) {
			return true;
		}
		if (IsAnimatorPlaying ("Placeholder Melee Attack Low")) {
			return true;
		}
		return false;
	}
	
	public void StartMeleeBlockHigh () {
		Utilities.DebugLog ("Unit.StartMeleeBlockHigh ()");
		if (animator) {
			animator.SetTrigger ("MeleeBlockHigh");
		}
	}
	
	public void StartMeleeBlockMedium () {
		Utilities.DebugLog ("Unit.StartMeleeBlockMedium ()");
		if (animator) {
			animator.SetTrigger ("MeleeBlockMedium");
		}
	}
	
	public void StartMeleeBlockLow () {
		Utilities.DebugLog ("Unit.StartMeleeBlockLow ()");
		if (animator) {
			animator.SetTrigger ("MeleeBlockLow");
		}
	}

	public void StartMeleeDamaged () {
		Utilities.DebugLog ("Unit.StartMeleeDamaged ()");
		if (animator) {
			animator.SetTrigger ("MeleeDamaged");
		}
	}

	public void KnockDown () {
		Utilities.DebugLog ("Unit.KnockDown ()");
		//LoseLife ();
		this.currentArmor = 0;
		this.currentHealth = 0;

		if (this.isPlayerControlled && this.isSelected) {
			this.tcScript.DeselectAllUnits ();
		}
		
		if (this.currentLives > 0) {
			Disable ();
		} else {
			Kill ();
		}
	}
	
	public void Disable () {
		Utilities.DebugLog ("Unit.Disable ()");
		//Utilities.DebugLog ("Unit.Disable() called");
		this.currentState = UnitState.UNIT_STATE_DISABLED;
		if (animator) {
			animator.SetBool ("IsDisabled", true);
		}
		this.agent.Stop ();

		PlayAudioType (TacticalCombat.AudioType.AudioType_Unit_Dying);

		HideHealthUI ();
		Utilities.Hide (this.unitBonusArrow);
		HideFlamethrowerParticle ();
	}

	public void Kill () {
		Utilities.DebugLog ("Unit.Kill ()");
		//Utilities.DebugLog ("Unit.Kill() called");
		this.currentState = UnitState.UNIT_STATE_DEAD;
		if (animator) {
			animator.SetBool ("IsKilled", true);
		}
		this.agent.Stop ();

		PlayAudioType (TacticalCombat.AudioType.AudioType_Unit_Dying);

		HideHealthUI ();
		Utilities.Hide (this.unitBonusArrow);
		HideFlamethrowerParticle ();
	}
	
	public void Revive () {
		Utilities.DebugLog ("Unit.Revive ()");
		//LoseLife ();
		//Utilities.DebugLog ("Unit.Revive() called");
		this.currentHealth = Health ();
		this.currentState = UnitState.UNIT_STATE_IDLE;
		if (animator) {
			animator.SetBool ("IsDisabled", false);
			animator.SetBool ("IsKilled", false);
		}

		if (!this.isPlayerControlled && TacticalCombat.DEBUG_SHOULD_GIVE_AI_LOW_HEALTH) {
			this.currentHealth = TacticalCombat.DEBUG_AI_LOW_HEALTH_PERCENTAGE * Health ();
			this.currentArmor = 0;
		}

		ShowHealthUI ();
	}

	public void ReviveFromMedic () {
		Utilities.DebugLog ("Unit.ReviveFromMedic ()");
		ResetHearts ();
		LoseLife ();
		Revive ();
	}
	
	public void HealFromMedic () {
		Utilities.DebugLog ("Unit.HealFromMedic ()");
		if (this.currentHealth < Health ()) {
			this.currentHealth += MEDIC_HEALTH_REGENERATION_AMOUNT;
			if (this.currentHealth >= Health ()) {
				this.currentHealth = Health ();
				if (this.currentState == UnitState.UNIT_STATE_DISABLED) {
					ReviveFromMedic ();
				}
			}
			if (Time.time > timeAtLastMedicParticle + MEDIC_HEALTH_PARTICLE_COOLDOWN_TIME) {
				timeAtLastMedicParticle = Time.time;
				ShowMedicParticle ();
			}
		}
	}

	public void MoveUp () {
		Utilities.DebugLog ("Unit.MoveUp ()");
		//Utilities.DebugLog (this.Name () + " MoveUp()");
		this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, this.gameObject.transform.position.y + Speed (), this.gameObject.transform.position.z);
	}
	
	public void MoveRight () {
		Utilities.DebugLog ("Unit.MoveRight ()");
		//Utilities.DebugLog (this.Name () + " MoveRight()");
		this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x + Speed (), this.gameObject.transform.position.y, this.gameObject.transform.position.z);
	}
	
	public void MoveDown () {
		Utilities.DebugLog ("Unit.MoveDown ()");
		//Utilities.DebugLog (this.Name () + " MoveDown()");
		this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, this.gameObject.transform.position.y - Speed (), this.gameObject.transform.position.z);
	}
	
	public void MoveLeft () {
		Utilities.DebugLog ("Unit.MoveLeft ()");
		//Utilities.DebugLog (this.Name () + " MoveLeft()");
		this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x - Speed (), this.gameObject.transform.position.y, this.gameObject.transform.position.z);
	}
	
	public string Name () {
		Utilities.DebugLog ("Unit.Name ()");
		string name = "None";
		switch (unitType) {
			case UnitType.UNIT_MACHINEGUNNER:
				name = "Machinegunner";
				break;
			case UnitType.UNIT_BAZOOKA:
				name = "Bazooka";
				break;
			case UnitType.UNIT_GRENADIER:
				name = "Grenadier";
				break;
			case UnitType.UNIT_FLAMETHROWER:
				name = "Flamethrower";
				break;
			case UnitType.UNIT_GRENADELAUNCHER:
				name = "Grenade Launcher";
				break;
			case UnitType.UNIT_MEDIC:
				name = "Medic";
				break;
			case UnitType.UNIT_KAMIKAZE:
				name = "Kamikaze";
				break;
			case UnitType.UNIT_BRAWLER:
				name = "Brawler";
				break;
			case UnitType.UNIT_SWAT:
				name = "SWAT";
				break;
		}
		return name;
	}
	
	public string Abbreviation () {
		Utilities.DebugLog ("Unit.Abbreviation ()");
		string name = "NA";
		switch (unitType) {
		case UnitType.UNIT_MACHINEGUNNER:
			name = "MG";
			break;
		case UnitType.UNIT_BAZOOKA:
			name = "BZ";
			break;
		case UnitType.UNIT_GRENADIER:
			name = "GR";
			break;
		case UnitType.UNIT_FLAMETHROWER:
			name = "FT";
			break;
		case UnitType.UNIT_GRENADELAUNCHER:
			name = "GL";
			break;
		case UnitType.UNIT_MEDIC:
			name = "MD";
			break;
		case UnitType.UNIT_KAMIKAZE:
			name = "KK";
			break;
		case UnitType.UNIT_BRAWLER:
			name = "BR";
			break;
		case UnitType.UNIT_SWAT:
			name = "SW";
			break;
		}
		return name;
	}

	public Vector2 ProjectileCreationPointOffset () {
		Vector2 offset = PROJECTILE_CREATION_POINT_OFFSET;
		switch (unitType) {
		case UnitType.UNIT_MACHINEGUNNER:
			if (isFacingLeft && isFacingDown) { // Down-left
				offset = new Vector2 (offset.x - 0.4f, offset.y - 0.40f);
			} else if (isFacingLeft && !isFacingDown) { // Up-left
				offset = new Vector2 (offset.x - 0.5f, offset.y + 0.25f);
			} else if (!isFacingLeft && isFacingDown) { // Down-right
				offset = new Vector2 (offset.x + 0.4f, offset.y - 0.40f);
			} else if (!isFacingLeft && !isFacingDown) { // Up-right
				offset = new Vector2 (offset.x + 0.5f, offset.y + 0.25f);
			}
			break;
		case UnitType.UNIT_BAZOOKA:
			if (isFacingLeft && isFacingDown) { // Down-left
				offset = new Vector2 (offset.x - 0.5f, offset.y - 0.15f);
			} else if (isFacingLeft && !isFacingDown) { // Up-left
				offset = new Vector2 (offset.x - 0.5f, offset.y + 0.45f);
			} else if (!isFacingLeft && isFacingDown) { // Down-right
				offset = new Vector2 (offset.x + 0.5f, offset.y - 0.15f);
			} else if (!isFacingLeft && !isFacingDown) { // Up-right
				offset = new Vector2 (offset.x + 0.5f, offset.y + 0.65f);
			}
			break;
		case UnitType.UNIT_GRENADIER:
			if (isFacingLeft && isFacingDown) { // Down-left
				offset = new Vector2 (offset.x - 0.15f, offset.y - 0.15f);
			} else if (isFacingLeft && !isFacingDown) { // Up-left
				offset = new Vector2 (offset.x - 0.15f, offset.y + 0.15f);
			} else if (!isFacingLeft && isFacingDown) { // Down-right
				offset = new Vector2 (offset.x + 0.15f, offset.y - 0.15f);
			} else if (!isFacingLeft && !isFacingDown) { // Up-right
				offset = new Vector2 (offset.x + 0.15f, offset.y + 0.15f);
			}
			break;
		case UnitType.UNIT_FLAMETHROWER:
			if (isFacingLeft && isFacingDown) { // Down-left
				offset = new Vector2 (offset.x - 0.4f, offset.y - 0.35f);
			} else if (isFacingLeft && !isFacingDown) { // Up-left
				offset = new Vector2 (offset.x - 0.5f, offset.y + 0.25f);
			} else if (!isFacingLeft && isFacingDown) { // Down-right
				offset = new Vector2 (offset.x + 0.4f, offset.y - 0.35f);
			} else if (!isFacingLeft && !isFacingDown) { // Up-right
				offset = new Vector2 (offset.x + 0.5f, offset.y + 0.25f);
			}
			break;
		case UnitType.UNIT_GRENADELAUNCHER:
			if (isFacingLeft && isFacingDown) { // Down-left
				offset = new Vector2 (offset.x - 0.5f, offset.y - 0.15f);
			} else if (isFacingLeft && !isFacingDown) { // Up-left
				offset = new Vector2 (offset.x - 0.5f, offset.y + 0.45f);
			} else if (!isFacingLeft && isFacingDown) { // Down-right
				offset = new Vector2 (offset.x + 0.5f, offset.y - 0.15f);
			} else if (!isFacingLeft && !isFacingDown) { // Up-right
				offset = new Vector2 (offset.x + 0.5f, offset.y + 0.45f);
			}
			break;
		case UnitType.UNIT_MEDIC:
			if (isFacingLeft && isFacingDown) {
				offset = new Vector2 (offset.x - 0.5f, offset.y - 0.15f);
			} else if (isFacingLeft && !isFacingDown) {
				offset = new Vector2 (offset.x - 0.5f, offset.y + 0.45f);
			} else if (!isFacingLeft && isFacingDown) {
				offset = new Vector2 (offset.x + 0.5f, offset.y - 0.15f);
			} else if (!isFacingLeft && !isFacingDown) {
				offset = new Vector2 (offset.x + 0.5f, offset.y + 0.45f);
			}
			break;
		case UnitType.UNIT_KAMIKAZE:
			if (isFacingLeft && isFacingDown) {
				offset = new Vector2 (offset.x - 0.5f, offset.y - 0.15f);
			} else if (isFacingLeft && !isFacingDown) {
				offset = new Vector2 (offset.x - 0.5f, offset.y + 0.45f);
			} else if (!isFacingLeft && isFacingDown) {
				offset = new Vector2 (offset.x + 0.5f, offset.y - 0.15f);
			} else if (!isFacingLeft && !isFacingDown) {
				offset = new Vector2 (offset.x + 0.5f, offset.y + 0.45f);
			}
			break;
		case UnitType.UNIT_BRAWLER:
			if (isFacingLeft && isFacingDown) {
				offset = new Vector2 (offset.x - 0.5f, offset.y - 0.15f);
			} else if (isFacingLeft && !isFacingDown) {
				offset = new Vector2 (offset.x - 0.5f, offset.y + 0.45f);
			} else if (!isFacingLeft && isFacingDown) {
				offset = new Vector2 (offset.x + 0.5f, offset.y - 0.15f);
			} else if (!isFacingLeft && !isFacingDown) {
				offset = new Vector2 (offset.x + 0.5f, offset.y + 0.45f);
			}
			break;
		case UnitType.UNIT_SWAT:
			if (isFacingLeft && isFacingDown) {
				offset = new Vector2 (offset.x - 0.5f, offset.y - 0.15f);
			} else if (isFacingLeft && !isFacingDown) {
				offset = new Vector2 (offset.x - 0.5f, offset.y + 0.45f);
			} else if (!isFacingLeft && isFacingDown) {
				offset = new Vector2 (offset.x + 0.5f, offset.y - 0.15f);
			} else if (!isFacingLeft && !isFacingDown) {
				offset = new Vector2 (offset.x + 0.5f, offset.y + 0.45f);
			}
			break;
		}
		return offset;
	}
	
	public float BaseHealth () {
		return UnitStatsData.unitStatsDataDictionary [Name ()].health;
//		Utilities.DebugLog ("Unit.BaseHealth ()");
//		float health = 100.0f;
//		switch (unitType) {
//			case UnitType.UNIT_MACHINEGUNNER:
//				health = UNIT_MACHINEGUNNER_HEALTH;
//				break;
//			case UnitType.UNIT_BAZOOKA:
//				health = UNIT_BAZOOKA_HEALTH;
//				break;
//			case UnitType.UNIT_GRENADIER:
//				health = UNIT_GRENADIER_HEALTH;
//				break;
//			case UnitType.UNIT_FLAMETHROWER:
//				health = UNIT_FLAMETHROWER_HEALTH;
//				break;
//			case UnitType.UNIT_GRENADELAUNCHER:
//				health = UNIT_GRENADELAUNCHER_HEALTH;
//				break;
//			case UnitType.UNIT_MEDIC:
//				health = UNIT_MEDIC_HEALTH;
//				break;
//			case UnitType.UNIT_KAMIKAZE:
//				health = UNIT_KAMIKAZE_HEALTH;
//				break;
//			case UnitType.UNIT_BRAWLER:
//				health = UNIT_BRAWLER_HEALTH;
//				break;
//			case UnitType.UNIT_SWAT:
//				health = UNIT_SWAT_HEALTH;
//				break;
//		}
//		return (this.health > 0 ? this.health : health);
	}

	public float Health () {
		float health = BaseHealth ();
		// No multipliers currently on Health
		return health;
	}

	public float BaseArmor () {
		return 50.0f;
	}

	public float Armor () {
		float armor = BaseArmor ();
		// No multipliers currently on Armor
		return armor;
	}
	
	public float BaseAttack () {
		return UnitStatsData.unitStatsDataDictionary [Name ()].attack;
//		Utilities.DebugLog ("Unit.BaseAttack ()");
//		float attack = 100.0f;
//		switch (unitType) {
//			case UnitType.UNIT_MACHINEGUNNER:
//				attack = UNIT_MACHINEGUNNER_ATTACK;
//				break;
//			case UnitType.UNIT_BAZOOKA:
//				attack = UNIT_BAZOOKA_ATTACK;
//				break;
//			case UnitType.UNIT_GRENADIER:
//				attack = UNIT_GRENADIER_ATTACK;
//				break;
//			case UnitType.UNIT_FLAMETHROWER:
//				attack = UNIT_FLAMETHROWER_ATTACK;
//				break;
//			case UnitType.UNIT_GRENADELAUNCHER:
//				attack = UNIT_GRENADELAUNCHER_ATTACK;
//				break;
//			case UnitType.UNIT_MEDIC:
//				attack = UNIT_MEDIC_ATTACK;
//				break;
//			case UnitType.UNIT_KAMIKAZE:
//				attack = UNIT_KAMIKAZE_ATTACK;
//				break;
//			case UnitType.UNIT_BRAWLER:
//				attack = UNIT_BRAWLER_ATTACK;
//				break;
//			case UnitType.UNIT_SWAT:
//				attack = UNIT_SWAT_ATTACK;
//				break;
//		}
//		return (this.attack > 0 ? this.attack : attack);
	}

	public float Attack () {
		float attack = BaseAttack ();
		// Apply multiplier if Unit is under cover
		if (IsUnderCover ()) {
			attack *= TacticalCombat.OBSTACLE_COVER_ATTACK_MULTIPLIER;
		}
		return attack;
	}
	
	public float BaseFirerate () {
		return UnitStatsData.unitStatsDataDictionary [Name ()].firerate;
//		Utilities.DebugLog ("Unit.BaseFirerate ()");
//		float firerate = 100.0f;
//		switch (unitType) {
//			case UnitType.UNIT_MACHINEGUNNER:
//				firerate = UNIT_MACHINEGUNNER_FIRERATE;
//				break;
//			case UnitType.UNIT_BAZOOKA:
//				firerate = UNIT_BAZOOKA_FIRERATE;
//				break;
//			case UnitType.UNIT_GRENADIER:
//				firerate = UNIT_GRENADIER_FIRERATE;
//				break;
//			case UnitType.UNIT_FLAMETHROWER:
//				firerate = UNIT_FLAMETHROWER_FIRERATE;
//				break;
//			case UnitType.UNIT_GRENADELAUNCHER:
//				firerate = UNIT_GRENADELAUNCHER_FIRERATE;
//				break;
//			case UnitType.UNIT_MEDIC:
//				firerate = UNIT_MEDIC_FIRERATE;
//				break;
//			case UnitType.UNIT_KAMIKAZE:
//				firerate = UNIT_KAMIKAZE_FIRERATE;
//				break;
//			case UnitType.UNIT_BRAWLER:
//				firerate = UNIT_BRAWLER_FIRERATE;
//				break;
//			case UnitType.UNIT_SWAT:
//				firerate = UNIT_SWAT_FIRERATE;
//				break;
//		}
//		return (this.firerate > 0 ? this.firerate : firerate);
	}

	public float Firerate () {
		float firerate = BaseFirerate ();
		// No multipliers currently on Firerate
		return firerate;
	}
	
	public float BaseProjectileSpeed () {
		return UnitStatsData.unitStatsDataDictionary [Name ()].projectileSpeed;
//		Utilities.DebugLog ("Unit.BaseProjectileSpeed ()");
//		float projectileSpeed = 100.0f;
//		switch (unitType) {
//			case UnitType.UNIT_MACHINEGUNNER:
//				projectileSpeed = UNIT_MACHINEGUNNER_PROJECTILE_SPEED;
//				break;
//			case UnitType.UNIT_BAZOOKA:
//				projectileSpeed = UNIT_BAZOOKA_PROJECTILE_SPEED;
//				break;
//			case UnitType.UNIT_GRENADIER:
//				projectileSpeed = UNIT_GRENADIER_PROJECTILE_SPEED;
//				break;
//			case UnitType.UNIT_FLAMETHROWER:
//				projectileSpeed = UNIT_FLAMETHROWER_PROJECTILE_SPEED;
//				break;
//			case UnitType.UNIT_GRENADELAUNCHER:
//				projectileSpeed = UNIT_GRENADELAUNCHER_PROJECTILE_SPEED;
//				break;
//			case UnitType.UNIT_MEDIC:
//				projectileSpeed = UNIT_MEDIC_PROJECTILE_SPEED;
//				break;
//			case UnitType.UNIT_KAMIKAZE:
//				projectileSpeed = UNIT_KAMIKAZE_PROJECTILE_SPEED;
//				break;
//			case UnitType.UNIT_BRAWLER:
//				projectileSpeed = UNIT_BRAWLER_PROJECTILE_SPEED;
//				break;
//			case UnitType.UNIT_SWAT:
//				projectileSpeed = UNIT_SWAT_PROJECTILE_SPEED;
//				break;
//		}
//		return (this.projectileSpeed > 0 ? this.projectileSpeed : projectileSpeed);
	}

	public float ProjectileSpeed () {
		float projectileSpeed = BaseProjectileSpeed ();
		// No multipliers currently on ProjectileSpeed
		return projectileSpeed;
	}
	
	public float BaseRange () {
		return UnitStatsData.unitStatsDataDictionary [Name ()].range;
//		Utilities.DebugLog ("Unit.BaseRange ()");
//		float range = 100.0f;
//		switch (unitType) {
//			case UnitType.UNIT_MACHINEGUNNER:
//				range = UNIT_MACHINEGUNNER_RANGE;
//				break;
//			case UnitType.UNIT_BAZOOKA:
//				range = UNIT_BAZOOKA_RANGE;
//				break;
//			case UnitType.UNIT_GRENADIER:
//				range = UNIT_GRENADIER_RANGE;
//				break;
//			case UnitType.UNIT_FLAMETHROWER:
//				range = UNIT_FLAMETHROWER_RANGE;
//				break;
//			case UnitType.UNIT_GRENADELAUNCHER:
//				range = UNIT_GRENADELAUNCHER_RANGE;
//				break;
//			case UnitType.UNIT_MEDIC:
//				range = UNIT_MEDIC_RANGE;
//				break;
//			case UnitType.UNIT_KAMIKAZE:
//				range = UNIT_KAMIKAZE_RANGE;
//				break;
//			case UnitType.UNIT_BRAWLER:
//				range = UNIT_BRAWLER_RANGE;
//				break;
//			case UnitType.UNIT_SWAT:
//				range = UNIT_SWAT_RANGE;
//				break;
//		}
//		return (this.range > 0 ? this.range : range);
	}

	public float Range () {
		float range = BaseRange ();
		// Apply multiplier if Unit is under cover
		if (IsUnderCover ()) {
			range *= TacticalCombat.OBSTACLE_COVER_RANGE_MULTIPLIER;
		}
		return range;
	}
	
	public float BaseSpeed () {
		return UnitStatsData.unitStatsDataDictionary [Name ()].speed;
//		Utilities.DebugLog ("Unit.BaseSpeed ()");
//		float speed = 100.0f;
//		switch (unitType) {
//			case UnitType.UNIT_MACHINEGUNNER:
//				speed = UNIT_MACHINEGUNNER_SPEED;
//				break;
//			case UnitType.UNIT_BAZOOKA:
//				speed = UNIT_BAZOOKA_SPEED;
//				break;
//			case UnitType.UNIT_GRENADIER:
//				speed = UNIT_GRENADIER_SPEED;
//				break;
//			case UnitType.UNIT_FLAMETHROWER:
//				speed = UNIT_FLAMETHROWER_SPEED;
//				break;
//			case UnitType.UNIT_GRENADELAUNCHER:
//				speed = UNIT_GRENADELAUNCHER_SPEED;
//				break;
//			case UnitType.UNIT_MEDIC:
//				speed = UNIT_MEDIC_SPEED;
//				break;
//			case UnitType.UNIT_KAMIKAZE:
//				speed = UNIT_KAMIKAZE_SPEED;
//				break;
//			case UnitType.UNIT_BRAWLER:
//				speed = UNIT_BRAWLER_SPEED;
//				break;
//			case UnitType.UNIT_SWAT:
//				speed = UNIT_SWAT_SPEED;
//				break;
//		}
//		return (this.speed > 0 ? this.speed : speed);
	}

	public float Speed () {
		float speed = BaseSpeed ();
		// Apply multiplier if Unit is Kamikaze attacking target
		if (this.unitType == UnitType.UNIT_KAMIKAZE && this.attackTargetGameObject) {
			speed *= 1.5f;
		}
		if (this.unitType == UnitType.UNIT_FLAMETHROWER && this.isFlamethrowerActive) {
			speed *= 0.5f;
		}
		return speed;
	}
	
	public float BaseMelee () {
		return UnitStatsData.unitStatsDataDictionary [Name ()].melee;
//		Utilities.DebugLog ("Unit.BaseMelee ()");
//		float melee = 100.0f;
//		switch (unitType) {
//		case UnitType.UNIT_MACHINEGUNNER:
//			melee = UNIT_MACHINEGUNNER_MELEE;
//			break;
//		case UnitType.UNIT_BAZOOKA:
//			melee = UNIT_BAZOOKA_MELEE;
//			break;
//		case UnitType.UNIT_GRENADIER:
//			melee = UNIT_GRENADIER_MELEE;
//			break;
//		case UnitType.UNIT_FLAMETHROWER:
//			melee = UNIT_FLAMETHROWER_MELEE;
//			break;
//		case UnitType.UNIT_GRENADELAUNCHER:
//			melee = UNIT_GRENADELAUNCHER_MELEE;
//			break;
//		case UnitType.UNIT_MEDIC:
//			melee = UNIT_MEDIC_MELEE;
//			break;
//		case UnitType.UNIT_KAMIKAZE:
//			melee = UNIT_KAMIKAZE_MELEE;
//			break;
//		case UnitType.UNIT_BRAWLER:
//			melee = UNIT_BRAWLER_MELEE;
//			break;
//		case UnitType.UNIT_SWAT:
//			melee = UNIT_SWAT_MELEE;
//			break;
//		}
//		return (this.melee > 0 ? this.melee : melee);
	}

	public float Melee () {
		float melee = BaseMelee ();
		// No multipliers currently on Melee
		return melee;
	}
	
	public void Health (float value) {
		UnitStatsData.unitStatsDataDictionary [Name ()].health = value;
//		Utilities.DebugLog ("Unit.Health ()");
//		this.health = value;
		this.currentHealth = value;
	}
	
	public void Attack (float value) {
		UnitStatsData.unitStatsDataDictionary [Name ()].attack = value;
//		Utilities.DebugLog ("Unit.Attack ()");
//		this.attack = value;
	}
	
	public void Firerate (float value) {
		UnitStatsData.unitStatsDataDictionary [Name ()].firerate = value;
//		Utilities.DebugLog ("Unit.Firerate ()");
//		this.firerate = value;
	}
	
	public void ProjectileSpeed (float value) {
		UnitStatsData.unitStatsDataDictionary [Name ()].projectileSpeed = value;
//		Utilities.DebugLog ("Unit.ProjectileSpeed ()");
//		this.projectileSpeed = value;
	}
	
	public void Range (float value) {
		UnitStatsData.unitStatsDataDictionary [Name ()].range = value;
//		Utilities.DebugLog ("Unit.Range ()");
//		this.range = value;
	}
	
	public void Speed (float value) {
		UnitStatsData.unitStatsDataDictionary [Name ()].speed = value;
//		Utilities.DebugLog ("Unit.Speed ()");
//		this.speed = value;
	}

	public void Melee (float value) {
		UnitStatsData.unitStatsDataDictionary [Name ()].melee = value;
//		Utilities.DebugLog ("Unit.Melee ()");
//		this.melee = value;
	}
}
