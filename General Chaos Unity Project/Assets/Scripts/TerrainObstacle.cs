using UnityEngine;
using System.Collections;

public class TerrainObstacle : MonoBehaviour {

	public const float BALLISTIC_PROJECTILE_SPLASH_DAMAGE_RANGE = 2.0f;//100.0f;
	public const float PARTICLE_RADIUS = 14.0f;
	
	public enum ObstacleType {
		OBSTACLE_NONE = 0,
		OBSTACLE_ROCK,
		OBSTACLE_TALLWALL,
		OBSTACLE_SHORTWALL,
		OBSTACLE_OILDRUM,
		OBSTACLE_FALLENTREE,
		OBSTACLE_CRATE,
		OBSTACLE_SANDBAGS,
		OBSTACLE_TIRES,
		OBSTACLE_DIRTMOUND,

		OBSTACLE_ARMORED_VEHICLE,
		OBSTACLE_LIMO,
		OBSTACLE_HELICOPTER,
		OBSTACLE_TANK, 					// Added
		OBSTACLE_SATELLITE_TRUCK,
	};

	public ObstacleType obstacleType = ObstacleType.OBSTACLE_NONE;
	public bool isIntact = true;
	public bool isDestructible = false;
	public GameObject destroyedPrefab;
	public bool isImmuneToBulletDamage = false;
	public bool isImmuneToExplosiveDamage = false;
	public bool isImmuneToFireDamage = false;
	public float health = 100.0f;
	public float radius = 0.4f;//20.0f; // used for spacing out obstacle placement
	public float height = 0.6f;//30.0f; // (used for stopping ballistic projectiles)

	public bool shouldStopUnitMovementWhenIntact = false;
	public bool shouldStopBulletsWhenIntact = false;
	public bool shouldStopExplosivesWhenIntact = false;
	public bool shouldStopFireWhenIntact = false;
	public bool shouldProvideCoverWhenIntact = false; // (decide if fire should behave differently in terms of cover

	public bool shouldStopUnitMovementWhenDestroyed = false;
	public bool shouldStopBulletsWhenDestroyed = false;
	public bool shouldStopExposivesWhenDestroyed = false;
	public bool shouldStopFireWhenDestroyed = false;
	public bool shouldProvideCoverWhenDestroyed = false;
	public bool shouldCauseExplosionWhenDestroyed = false;
	public float explosionAttackStrength = 100.0f;

	public bool shouldRemoveFromListAndDestroyOnNextUpdate = false;
	public bool shouldAddToListOnNextUpdate = false;

	private TacticalCombat tcScript;


	// Use this for initialization
	void Start () {
		Utilities.DebugLog ("TerrainObstacle.Start ()");
		// Enable/disable pathfinding obstruction
		PolyNavObstacle polyNavObstacle = this.gameObject.GetComponent<PolyNavObstacle> ();
		if (polyNavObstacle) {
			polyNavObstacle.enabled = this.shouldStopUnitMovementWhenIntact;
		}
		
		Camera mainCamera = Camera.main;
		this.tcScript = mainCamera.GetComponent<TacticalCombat> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (tcScript.isLevelComplete || tcScript.isLevelFailed || tcScript.isMenuOpen) {
			return;
		}

		Utilities.DebugLog ("TerrainObstacle.Update ()");
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

		if (shouldAddToListOnNextUpdate) {
			shouldAddToListOnNextUpdate = false;
			this.tcScript.obstaclesList.Add (this);
		}

		if (shouldRemoveFromListAndDestroyOnNextUpdate) {
			shouldRemoveFromListAndDestroyOnNextUpdate = false;
			this.tcScript.obstaclesList.Remove (this);
			Destroy (this.gameObject, 0);
		}
	}

	public void DestroyObstacle () {
		Utilities.DebugLog ("TerrainObstacle.DestroyObstacle ()");
		if (this.destroyedPrefab) {
			GameObject destroyedObstacleGameObject = (GameObject)Instantiate (this.destroyedPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
			TerrainObstacle destroyedObstacle = destroyedObstacleGameObject.GetComponent<TerrainObstacle> ();
			float obstacleScaleSign = Mathf.Sign (this.transform.localScale.x);
			destroyedObstacle.transform.localScale = new Vector3 (obstacleScaleSign * destroyedObstacle.transform.localScale.x, destroyedObstacle.transform.localScale.y, destroyedObstacle.transform.localScale.z);

			// Enable/disable pathfinding obstruction
			PolyNavObstacle polyNavObstacle = destroyedObstacleGameObject.GetComponent<PolyNavObstacle> ();
			if (polyNavObstacle) {
				polyNavObstacle.enabled = this.shouldStopUnitMovementWhenDestroyed;
			}

			this.isIntact = false;
			this.shouldRemoveFromListAndDestroyOnNextUpdate = true;
			destroyedObstacle.shouldAddToListOnNextUpdate = true;
		} else {
			SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer> ();
			if (spriteRenderer) {
				spriteRenderer.color = new Color(1f,1f,1f,.2f); // 20% transparent
			}
			this.isIntact = false;
		}

	}

	public void TakeDamage (float damage) {
		Utilities.DebugLog ("TerrainObstacle.TakeDamage ()");
		this.health -= damage;
		if (this.health <= 0) {
			if (this.shouldCauseExplosionWhenDestroyed) {
				if (this.tcScript) {
					GameObject explosion = (GameObject)Instantiate (this.tcScript.explosionPrefab, this.transform.position, Quaternion.identity);
					ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem> ();
					if (particleSystem) {
						particleSystem.startSize = this.radius * PARTICLE_RADIUS;
					}
					
					//explosion.transform.position = new Vector2(explosion.transform.position.x + PROJECTILE_CREATION_POINT_OFFSET.x, explosion.transform.position.y + PROJECTILE_CREATION_POINT_OFFSET.y);
					//Destroy (explosion, 1.0f);
					
					// Deal splash damage
					Utilities.DebugLog ("Deal splash damage");
					//this.tcScript.DealDamageToUnitsWithinRange (this.gameObject.transform.position, BALLISTIC_PROJECTILE_SPLASH_DAMAGE_RANGE + obstacle.radius, this.attack * BALLISTIC_PROJECTILE_SPLASH_DAMAGE_PERCENTAGE * 20.0f);
					this.tcScript.DealDamageWithFalloffToUnitsWithinRange (this.gameObject.transform.position, BALLISTIC_PROJECTILE_SPLASH_DAMAGE_RANGE + this.radius * 1.5f, damage * 20.0f, this.gameObject);
				}
			}
			
			// Enable/disable pathfinding obstruction
			PolyNavObstacle polyNavObstacle = this.gameObject.GetComponent<PolyNavObstacle> ();
			polyNavObstacle.enabled = this.shouldStopUnitMovementWhenDestroyed;
			
			this.DestroyObstacle ();
		}
	}

}	