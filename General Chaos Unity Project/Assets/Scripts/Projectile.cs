using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public enum ProjectileState {
		PROJECTILE_STATE_ACTIVE = 0,
		PROJECTILE_STATE_PENDING_DESTROY,
		PROJECTILE_STATE_PAUSED
	};
	
	public const float BALLISTIC_PROJECTILE_SPLASH_DAMAGE_RANGE = 2.0f;//100.0f;
	public const float BALLISTIC_PROJECTILE_SPLASH_DAMAGE_PERCENTAGE = 1.0f;//0.4f;

	public TacticalCombat tcScript;
	
	public float range = 0.0f;
	public float attack = 100.0f;
	public Vector2 initialPosition;
	public bool isPlayerControlled = false;
	public bool isBallisticProjectile = false;
	public bool isBouncingProjectile = false;
	public bool isBullet = false;
	public bool isBazooka = false;
	public bool isFlamethrower = false;
	public bool isGrenadeLauncher = false;
	public Vector2 direction;
	public float speed;
	public Vector2 basePosition;
	public Vector2 nonAdjustedPosition;
    public Unit sourceUnit;

	public ProjectileState currentState = ProjectileState.PROJECTILE_STATE_ACTIVE;

	private Quaternion targetRotation;
	private float ballisticHeight = 2.0f;//100.0f;
	private float ballisticShift = 0;
	private float fullBallisticShift = 0;
	private float distanceTravelled = 0;
	private float percentTravelled = 0;
	private float timeAtStart = 0;
	private float timeAtDestroy = 0;
	public bool isWaitingToBeDestroyed = false;

	// Use this for initialization
	void Start () {
		Utilities.DebugLog ("Projectile.Start ()");
		Camera mainCamera = Camera.main;
		this.tcScript = mainCamera.GetComponent<TacticalCombat> ();
		this.nonAdjustedPosition = this.gameObject.transform.position;
		this.timeAtStart = (float)Time.time;
		this.timeAtDestroy = 0;
		this.isWaitingToBeDestroyed = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (tcScript.isLevelComplete || tcScript.isLevelFailed || tcScript.isMenuOpen) {
			return;
		}

		if (this.isWaitingToBeDestroyed) {
			TrailRenderer trailRenderer = this.gameObject.GetComponent<TrailRenderer> ();
			if (trailRenderer) {
				float currentTime = (float)Time.time;
				float timeSinceDestroy = currentTime - timeAtDestroy;
				float t = Mathf.Min (1.0f, timeSinceDestroy / 1.0f + 0.5f);
				Color newColor = Color.Lerp (Color.white, Color.clear, t);
				trailRenderer.material.SetColor("_TintColor", newColor);
			}
			return;
		}

		Utilities.DebugLog ("Projectile.Update ()");
		if (this.currentState == ProjectileState.PROJECTILE_STATE_ACTIVE) {
			Utilities.DebugLog ("AdjustedDistance () 1111111");
			this.distanceTravelled = TacticalCombat.AdjustedDistance (this.basePosition, this.initialPosition);
			this.percentTravelled = this.distanceTravelled / this.range;

			if (this.isBallisticProjectile) {
				this.basePosition = new Vector2 (this.basePosition.x + this.direction.x * this.speed * Time.deltaTime, this.basePosition.y + this.direction.y * this.speed * TacticalCombat.VERTICAL_SQUASH_RATIO * Time.deltaTime);

				if (this.isBouncingProjectile && percentTravelled != 0) {
					this.ballisticShift = Mathf.Abs (Mathf.Sin (Mathf.PI * this.distanceTravelled * this.distanceTravelled * this.distanceTravelled / (this.range * this.range * this.range)) / (this.distanceTravelled * this.distanceTravelled) * (0.55f * this.range * this.range));
				} else {
					this.ballisticShift = Mathf.Sin (Mathf.PI * percentTravelled) * ((18.0f/*900.0f*/ - this.range)/10.0f/*500.0f*/);
				}
				this.fullBallisticShift = this.ballisticShift * this.ballisticHeight;
				//Utilities.DebugLog ("ballistic shift = " + fullBallisticShift + ", basePosition.y = " + this.basePosition.y);

				Vector2 oldPosition = this.gameObject.transform.position;
				Vector2 newPosition = new Vector2 (this.basePosition.x, this.basePosition.y + this.fullBallisticShift);

				// Rotate Grenade Launcher projectile to face its current direction in arc
				if (this.isGrenadeLauncher) {
					Vector3 vectorToTarget = new Vector3 (newPosition.x - oldPosition.x, newPosition.y - oldPosition.y, 0);
					float angle = Mathf.Atan2 (vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
					Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
					targetRotation = q;
					this.gameObject.transform.rotation = Quaternion.Lerp (this.gameObject.transform.rotation, targetRotation, 0.5f);
				}

				this.gameObject.transform.position = newPosition;
			} else if (this.isBazooka) {
				float currentTime = (float)Time.time;
				float timeSinceStart = currentTime - timeAtStart;
				float t = Mathf.Min (1.0f, timeSinceStart / 2.0f);
				t = 10.0f * t * t * t * t;
				float bazookaSpeed = Mathf.Lerp (0.2f * this.speed, 1.6f * this.speed, t);
				this.basePosition = new Vector2 (this.basePosition.x + this.direction.x * bazookaSpeed * Time.deltaTime, this.basePosition.y + this.direction.y * bazookaSpeed * TacticalCombat.VERTICAL_SQUASH_RATIO * Time.deltaTime);
				this.gameObject.transform.position = this.basePosition;
			} else {
				this.basePosition = new Vector2 (this.basePosition.x + this.direction.x * this.speed * Time.deltaTime, this.basePosition.y + this.direction.y * this.speed * TacticalCombat.VERTICAL_SQUASH_RATIO * Time.deltaTime);
				this.gameObject.transform.position = this.basePosition;
			}

			if (this.isBouncingProjectile) {
				if (this.distanceTravelled > 1.5f * this.range) {
					DestroyProjectileAccordingToType ();
				}
			} else {
				if (this.distanceTravelled > this.range) {
					DestroyProjectileAccordingToType ();
				}
			}
		}
	}

	void DestroyProjectileAccordingToType () {
        // Play SFX
        sourceUnit.PlayAudioType(TacticalCombat.AudioType.AudioType_SFX_Explode);

		if (this.isBallisticProjectile || this.isBazooka) {
			ExplodeProjectile ();
		} else if (this.isBullet) {
			this.isWaitingToBeDestroyed = true;
			this.timeAtDestroy = (float)Time.time;
			Utilities.Hide (this.gameObject);
			GameObject.Destroy (this.gameObject, 1.5f);
		} else {
			Destroy (gameObject);
		}
	}

	void ExplodeProjectile () {
		Utilities.DebugLog ("Projectile.ExplodeProjectile ()");
		if (this.tcScript) {
			GameObject explosion = (GameObject)Instantiate(this.tcScript.explosionPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
			explosion.transform.position = new Vector3 (explosion.transform.position.x, explosion.transform.position.y, -0.1f);
			ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem> ();
			if (particleSystem) {
				particleSystem.GetComponent<Renderer> ().sortingOrder = 32767;
			}
			this.currentState = ProjectileState.PROJECTILE_STATE_PENDING_DESTROY;
			//Destroy (explosion, 1.0f);
			Destroy (gameObject, 0.0f);

			// Deal splash damage
			float damageRange = BALLISTIC_PROJECTILE_SPLASH_DAMAGE_RANGE;
			if (this.isBazooka) {
				damageRange *= 0.4f;
				if (particleSystem) {
					particleSystem.startSize = 0.85f;
				}
			}
			if (this.isGrenadeLauncher) {
				damageRange *= 0.3f;
				if (particleSystem) {
					particleSystem.startSize = 0.7f;
				}
			}
			this.tcScript.DealDamageWithFalloffToUnitsWithinRange (this.gameObject.transform.position, damageRange, this.attack * BALLISTIC_PROJECTILE_SPLASH_DAMAGE_PERCENTAGE, this.gameObject);
		}
	}
	
	// Handle collisions
	void OnTriggerEnter2D (Collider2D other) {
		Utilities.DebugLog ("Projectile.OnTriggerEnter2D ()");
		if (this.currentState == ProjectileState.PROJECTILE_STATE_ACTIVE) {

			// Check collision between projectile and Unit
			Unit unit = other.gameObject.GetComponent<Unit> ();
			if (unit) {
				if (unit.isPlayerControlled != this.isPlayerControlled 
				    && (!this.isBallisticProjectile || this.percentTravelled > 0.9f)
				    && unit.currentState != Unit.UnitState.UNIT_STATE_PREPARING_FOR_CLOSE_COMBAT 
				    && unit.currentState != Unit.UnitState.UNIT_STATE_CLOSE_COMBAT 
				    && unit.currentState != Unit.UnitState.UNIT_STATE_DEAD
				    && unit.currentState != Unit.UnitState.UNIT_STATE_DISABLED) {

					// For bazooka only, decrease damage as it approaches its max range
					float attackStrength = this.attack;
					if (this.isBazooka) {
						attackStrength = attackStrength * ((1.0f - this.percentTravelled) * 0.5f + 0.5f);
					}
					unit.TakeDamage (attackStrength);

					if (this.tcScript) {
						this.tcScript.projectilesList.Remove (this);
					}

					DestroyProjectileAccordingToType ();
				}
			}

			// Check collision between projectile and obstacle
			TerrainObstacle obstacle = other.gameObject.GetComponent<TerrainObstacle> ();
			if (obstacle) {
				if (obstacle.isDestructible
				    && (!this.isBallisticProjectile || this.percentTravelled > 0.9f)
				    && (!this.isBazooka || this.distanceTravelled > 1.0f/*50.0f*/)) {

					// Stop projectile if applicable
					if ((obstacle.isIntact && obstacle.shouldStopExplosivesWhenIntact && this.isBallisticProjectile && this.fullBallisticShift * 1.0f < obstacle.height * 2.5f)
					    || (obstacle.isIntact && obstacle.shouldStopFireWhenIntact && this.isFlamethrower)
					    || (obstacle.isIntact && obstacle.shouldStopBulletsWhenIntact && !this.isBallisticProjectile && !this.isFlamethrower)
					    || (!obstacle.isIntact && obstacle.shouldStopExposivesWhenDestroyed && this.isBallisticProjectile)
					    || (!obstacle.isIntact && obstacle.shouldStopFireWhenDestroyed && this.isFlamethrower)
					    || (!obstacle.isIntact && obstacle.shouldStopBulletsWhenDestroyed && !this.isBallisticProjectile && !this.isFlamethrower)) {

						DestroyProjectileAccordingToType ();
					}

					// Cause damage to obstacle if applicable
					if ((obstacle.isIntact && !obstacle.isImmuneToExplosiveDamage && this.isBallisticProjectile && this.fullBallisticShift * 1.0f < obstacle.height * 2.5f)
					    || (obstacle.isIntact && !obstacle.isImmuneToFireDamage && this.isFlamethrower)
					    || (obstacle.isIntact && !obstacle.isImmuneToBulletDamage && !this.isBallisticProjectile && !this.isFlamethrower)) {

						obstacle.TakeDamage (this.attack);
					}
				}
			} else {
				// Check collision between projectile and non-TerrainObstacle PolyNavObstacle
				PolyNavObstacle polyNavObstacle = other.gameObject.GetComponent<PolyNavObstacle> ();
				if (polyNavObstacle) {
					//DestroyProjectileAccordingToType ();

					float GENERIC_POLYNAV_OBSTACLE_HEIGHT = 200.0f;

					if ((!this.isBallisticProjectile || this.percentTravelled > 0.9f)
				    && (!this.isBazooka || this.distanceTravelled > 1.0f/*50.0f*/)) {

						// Stop projectile if applicable
						if ((this.isBallisticProjectile && this.fullBallisticShift * 1.0f < GENERIC_POLYNAV_OBSTACLE_HEIGHT * 2.5f)
						    || (this.isFlamethrower)
						    || (!this.isBallisticProjectile && !this.isFlamethrower)) {

							DestroyProjectileAccordingToType ();
						}
					}

				}
			}
		}
	}
	
	public void Pause () {
		Utilities.DebugLog ("Projectile.Pause ()");
		this.currentState = ProjectileState.PROJECTILE_STATE_PAUSED;
	}
	
	public void Resume () {
		Utilities.DebugLog ("Projectile.Resume ()");
		this.currentState = ProjectileState.PROJECTILE_STATE_ACTIVE;
	}
}
