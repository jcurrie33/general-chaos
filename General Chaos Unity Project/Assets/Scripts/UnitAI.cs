using UnityEngine;
using System.Collections;

public class UnitAI : MonoBehaviour {
	
	
	
	public enum UnitAIState {
		UnitAIState_Idle = 0,
		UnitAIState_MovingToward_MoveTarget,
		UnitAIState_MovingToward_AttackTarget,
		UnitAIState
	};


	// AI Constants
	public static float AI_MAX_DISTANCE_TO_TARGET_EXPLOSIVE_OBSTACLES = 0.8f;//40.0f;
	public static float AI_MIN_DISTANCE_FROM_FRIENDLY_TO_TARGET_EXPLOSIVE_OBSTACLES = 4.0f;
	
	// AI Strength settings
	public static bool AI_shouldTargetExplosiveObstacles = true;
	public static bool AI_shouldPrioritizeAttackingUnitsByType = true;
	public static int AI_numSimultaneousActions = 100;
	public static float AI_delayBetweenActions_AllUnits = 1.2f;
	public static float AI_delayBetweenActions_SingleUnit = 5.0f;
	public static bool AI_shouldAvoidExplosiveObstacles = true; // TODO: Add this functionality
	public static bool AI_shouldAvoidTargetingExplosiveObstaclesNearFriendlies = true; // TODO: Add this functionality
	public static bool AI_shouldSpreadOut = true; // TODO: Add this functionality
	public static bool AI_shouldKeepLongRangeUnitsInRear = true; // TODO: Add this functionality
	public static bool AI_shouldKeepShortRangeUnitsInFront = true; // TODO: Add this functionality
	public static bool AI_shouldSendMedicToWeakestUnit = true; // TODO: Add this functionality
	public static bool AI_shouldMoveTowardsCover = true; // TODO: Add this functionality
	public static bool AI_shouldStraifeBetweenShots = true; // TODO: Add this functionality
	public static bool AI_shouldMoveRandomlyIfStuck = true; // TODO: Add this functionality
	public static bool AI_shouldGrenadiersMoveBehindWalls = true; // TODO: Add this functionality
	
	private static float timeAtLastAIAction_AllUnits_Player = 0;
	private static float timeAtLastAIAction_AllUnits_AI = 0;

	private float timeAtLastAIAction_SingleUnit = 0;
	
	private TacticalCombat _tcScript;
	public TacticalCombat tcScript {
		get {
			if (!_tcScript) {
				Camera mainCamera = Camera.main;
				_tcScript = mainCamera.GetComponent<TacticalCombat> ();
			}
			return _tcScript;			
		}
	}

	private Unit _unit;
	public Unit unit {
		get {
			if (!_unit) {
				_unit = this.gameObject.GetComponent<Unit> ();
			}
			return _unit;			
		}
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateAI () {
		Utilities.DebugLog ("UnitAI.UpdateAI ()");
		if (tcScript.isAIEnabled && !unit.isPlayerControlled) {
			UpdateEnemyAIForUnit ();
		} else if (unit.isPlayerControlled) {
			UpdatePlayerAIForUnit ();
		}
	}

	private void MarkLastAIActionTime () {
		timeAtLastAIAction_SingleUnit = Time.time;
		if (unit.isPlayerControlled) {
			timeAtLastAIAction_AllUnits_Player = Time.time;
		} else {
			timeAtLastAIAction_AllUnits_AI = Time.time;
		}

	}

	private bool IsTimeForNextAIAction () {
		float timeAtLastAIAction_AllUnits = (unit.isPlayerControlled ? timeAtLastAIAction_AllUnits_Player : timeAtLastAIAction_AllUnits_AI);
		return Time.time > timeAtLastAIAction_SingleUnit + AI_delayBetweenActions_SingleUnit && Time.time > timeAtLastAIAction_AllUnits + AI_delayBetweenActions_AllUnits;
	}

	private void UpdateEnemyAIForUnit () {
		if (unit.isTrackedForDebug) {
			Utilities.DebugLog ("Place breakpoint here to debug this unit.");
		}

		// Remove target if it is disabled
		if (unit.attackTargetGameObject) {
			Unit targetUnit = unit.attackTargetGameObject.GetComponent<Unit> ();
			if (targetUnit) {
				if (targetUnit.currentState == Unit.UnitState.UNIT_STATE_DEAD || targetUnit.currentState == Unit.UnitState.UNIT_STATE_DISABLED || targetUnit.currentState == Unit.UnitState.UNIT_STATE_PAUSED) {
					unit.attackTargetGameObject = null;
					unit.isIntentionallyAttackingExplosiveObstacle = false;
				}
			} else {
				TerrainObstacle targetObstacle = unit.attackTargetGameObject.GetComponent<TerrainObstacle> ();
				if (targetObstacle) {
					if (!targetObstacle.isIntact) {
						unit.attackTargetGameObject = null;
						unit.isIntentionallyAttackingExplosiveObstacle = false;
					}
				}
			}
		}

		if (unit.attackTargetGameObject || unit.isAttackTargetPositionSet) {
			if (unit.isMoveTargetPositionSet) {
				// If following a move command, keep firing at attack target, regardless of range
				if (unit.attackTargetGameObject) {
					unit.AttackPosition (unit.attackTargetGameObject.transform.position);
				}
				if (unit.isAttackTargetPositionSet) {
					unit.AttackPosition (unit.attackTargetPosition);
				}
			} else {
				if (unit.IsWithinRangeOfAttackTarget ()) {
					// If not following a move command, attack target only once in range
					if (unit.unitType != Unit.UnitType.UNIT_KAMIKAZE && unit.unitType != Unit.UnitType.UNIT_BRAWLER) {
						unit.StopRunning ();
						unit.moveTargetGameObject = null;
						unit.isMoveTargetPositionSet = false;
						if (unit.attackTargetGameObject) {
							unit.AttackPosition (unit.attackTargetGameObject.transform.position);
						}
						if (unit.isAttackTargetPositionSet) {
							unit.AttackPosition (unit.attackTargetPosition);
						}
					}
				} else {
//					if (IsTimeForNextAIAction ()) {
						// If not following a move command, and not yet within range of attack target, move towards attack target
						if (unit.isAttackTargetPositionSet) {
							unit.MoveTowardsPoint (unit.attackTargetPosition);
//							MarkLastAIActionTime ();
						}
						if (unit.attackTargetGameObject) {
							unit.MoveTowardsTarget (unit.attackTargetGameObject);
//							MarkLastAIActionTime ();
						}
//					}
				}
			}
		}
		
		// If moving and reached target, stop.
		if (unit.IsWithinRangeOfMoveTarget ()) {
			unit.StopRunning ();
			unit.moveTargetGameObject = null;
			unit.isMoveTargetPositionSet = false;
		}





		// Have AI players attack nearest enemy
		// Move towards new closest target if current attack or move target is dead or disabled
		Unit nearestEnemy = tcScript.GetNearestEnemyForUnit (unit);
		if (nearestEnemy /*&& GetNumMovingAIUnits () < AI_numSimultaneousActions*/ && IsTimeForNextAIAction () && !unit.isMoving) {
			if (!unit.attackTargetGameObject || 
			    (unit.attackTargetGameObject && unit.attackTargetGameObject != nearestEnemy.gameObject) ||
			    !unit.moveTargetGameObject || 
			    (unit.moveTargetGameObject && unit.moveTargetGameObject != nearestEnemy.gameObject)) {
				
				if (AI_shouldTargetExplosiveObstacles) {
					TerrainObstacle nearestExplosiveObstacle = tcScript.GetExplosiveObstacleNearPlayerUnitIfExists ();
					if (nearestExplosiveObstacle) {

						if (AI_shouldAvoidTargetingExplosiveObstaclesNearFriendlies) {
							float distanceToNearestFriendly = tcScript.GetDistanceOfNearestFriendlyFromObstacle (nearestExplosiveObstacle, unit.isPlayerControlled);
							if (distanceToNearestFriendly > AI_MIN_DISTANCE_FROM_FRIENDLY_TO_TARGET_EXPLOSIVE_OBSTACLES) {
								unit.attackTargetGameObject = nearestExplosiveObstacle.gameObject;
								unit.isIntentionallyAttackingExplosiveObstacle = true;
							} else {
								Debug.Log ("NOT TARGETING BARREL!");
							}

						} else {
							unit.attackTargetGameObject = nearestExplosiveObstacle.gameObject;
							unit.isIntentionallyAttackingExplosiveObstacle = true;
							//unit.MoveTowardsTarget (nearestExplosiveObstacle.gameObject);
						}
					} else {
						unit.attackTargetGameObject = nearestEnemy.gameObject;
						unit.isIntentionallyAttackingExplosiveObstacle = false;
						//unit.MoveTowardsTarget (nearestEnemy.gameObject);
					}
				} else {
					unit.attackTargetGameObject = nearestEnemy.gameObject;
					unit.isIntentionallyAttackingExplosiveObstacle = false;
					//unit.MoveTowardsTarget (nearestEnemy.gameObject);
				}
				MarkLastAIActionTime ();
				Utilities.DebugLog ("AI: Move within firing range of nearest enemy.");
				
			}
		}
		
//		if (unit.IsWithinRangeOfAttackTarget () || unit.IsWithinRangeOfMoveTarget ()) {
//			if (unit.unitType != Unit.UnitType.UNIT_KAMIKAZE && unit.unitType != Unit.UnitType.UNIT_BRAWLER) {
//				
//				unit.StopRunning ();
//				unit.moveTargetGameObject = null;
//				unit.isMoveTargetPositionSet = false;
//				
//				TerrainObstacle nearestExplosiveObstacle = tcScript.GetExplosiveObstacleNearPlayerUnitIfExists ();
//				if (nearestExplosiveObstacle) {
//					Utilities.DebugLog ("AI: Attacking explosive obstacle near player unit.");
//					unit.AttackPosition (nearestExplosiveObstacle.gameObject.transform.position);
//				} else {
//					Utilities.DebugLog ("AI: Attacking nearest enemy unit.");
//					unit.AttackPosition (nearestEnemy.gameObject.transform.position);
//				}
//				
//			}
//		} else if (/*GetNumMovingAIUnits () < AI_numSimultaneousActions &&*/ Time.time > timeAtLastAIAction_SingleUnit + AI_delayBetweenActions_SingleUnit && Time.time > timeAtLastAIAction_AllUnits + AI_delayBetweenActions_AllUnits && !unit.isMoving) {
//			
//			if (AI_shouldTargetExplosiveObstacles) {
//				TerrainObstacle nearestExplosiveObstacle = tcScript.GetExplosiveObstacleNearPlayerUnitIfExists ();
//				if (nearestExplosiveObstacle) {
//					unit.MoveWithinFiringRangeOfGameObject (nearestExplosiveObstacle.gameObject);
//				} else {
//					unit.MoveWithinFiringRangeOfNearestEnemyUnit ();
//				}
//			} else {
//				unit.MoveWithinFiringRangeOfNearestEnemyUnit ();
//				//Utilities.DebugLog ("MoveWithinFiringRangeOfNearestEnemyUnit");
//			}
//			timeAtLastAIAction_SingleUnit = Time.time;
//			timeAtLastAIAction_AllUnits = Time.time;
//			Utilities.DebugLog ("AI: Attacking explosive obstacle near player unit. (2)");
//			
//		}

	}

	private void UpdatePlayerAIForUnit () {
		if (TacticalCombat.DEBUG_SHOULD_AI_CONTROL_BOTH_SIDES) {
			UpdateEnemyAIForUnit ();
			return;
		}

		if (unit.attackTargetGameObject || unit.isAttackTargetPositionSet) {
			if (unit.isMoveTargetPositionSet) {
				// If following a move command, keep firing at attack target, regardless of range
				if (unit.attackTargetGameObject) {
					unit.AttackPosition (unit.attackTargetGameObject.transform.position);
				}
				if (unit.isAttackTargetPositionSet) {
					unit.AttackPosition (unit.attackTargetPosition);
				}
			} else {
				if (unit.IsWithinRangeOfAttackTarget ()) {
					// If not following a move command, attack target only once in range
					if (unit.unitType != Unit.UnitType.UNIT_KAMIKAZE && unit.unitType != Unit.UnitType.UNIT_BRAWLER) {
						unit.StopRunning ();
						unit.moveTargetGameObject = null;
						unit.isMoveTargetPositionSet = false;
						if (unit.attackTargetGameObject) {
							unit.AttackPosition (unit.attackTargetGameObject.transform.position);
						}
						if (unit.isAttackTargetPositionSet) {
							unit.AttackPosition (unit.attackTargetPosition);
						}
					}
				} else {
					// If not following a move command, and not yet within range of attack target, move towards attack target
					if (unit.isAttackTargetPositionSet) {
						unit.MoveTowardsPoint (unit.attackTargetPosition);
					}
					if (unit.attackTargetGameObject) {
						unit.MoveTowardsTarget (unit.attackTargetGameObject);
					}
				}
			}
		} else {
			Unit nearestEnemy = tcScript.GetNearestEnemyForUnit (unit);
			if (TacticalCombat.DEBUG_SHOULD_PLAYER_AI_AUTO_ATTACK_NEAREST_ENEMY && nearestEnemy) {
				if (unit.IsWithinRangeOfGameObject (nearestEnemy.gameObject)) {
					unit.AttackPosition (nearestEnemy.gameObject.transform.position);
				}
			}
		}

		// If moving and reached target, stop.
		if (unit.IsWithinRangeOfMoveTarget ()) {
			unit.StopRunning ();
			unit.moveTargetGameObject = null;
			unit.isMoveTargetPositionSet = false;
		}

	}
}
