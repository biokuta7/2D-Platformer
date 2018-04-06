using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

	[System.Serializable]
	public class Stats {

		public bool godMode = false;
		public float maxHealth = 5.0f;
		[HideInInspector]
		public float health;
		[HideInInspector]
		public bool dead;
		public float secondsInvulnerableAfterDamage = 2.0f;
	}

	[System.Serializable]
	public class Movement {
		public float maxJumpHeight = 3f;
		public float minJumpHeight = 1.5f;
		public float timeToJumpApex = .4f;
		public float accelerationTimeGrounded = .1f;
		public float accelerationTimeAirborneMultiplier = 2f;
		public float jetpackSpeed = 5.0f;
		public float jetpackFuelRechargeAmount = 10.0f;
	}

	[System.Serializable]
	public class FX
	{

		public ParticleSystem jetpackParticles;
		public GameObject deathParticles;
	}

	public static Player instance;
	public Movement movement;
	public Stats stats;
	public FX fx;

    float moveSpeed = 6f;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    Vector2 movementInput;
    float velocityXSmoothing;
    bool jetpack;
    float jetpackFuel;

	bool forceApplied;

	private float invincibilityFrame = 0.0f;
	private bool invincible;

    Controller2D controller;
    Animator anim;
    Direction direction;
    bool facingRight;
	bool lookingDown;
	bool headBob = false;
    SpriteRenderer spriteRenderer;
	private Holster holster;


	private void Awake() {
		instance = this;
	}

    private void Start()
    {
		holster = GetComponentInChildren<Holster> ();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        InitVerticalValues();
        direction = Direction.RIGHT;
        facingRight = true;
		Camera c = GetComponentInChildren<Camera> ();

		FullHeal ();

		if (c != null) {
			c.transform.SetParent (null);	
		}

    }

    private void Update()
    {
		if (!WorldState.paused && !stats.dead) {
			//MOVEMENT
			GetInput ();
			Horizontal ();
			Vertical ();
			Jetpack ();
			ApplyMovement ();

			//STATS
			ManageStats();

			//ANIMATION
			Animation ();
		}
    }

    #region PUBLIC

	//FUEL
	public float GetFuelPercentage() { return Mathf.Clamp01(jetpackFuel / movement.jetpackFuelRechargeAmount); }
	public void Refuel() { jetpackFuel = movement.jetpackFuelRechargeAmount; }

	//HEALTH
	public float GetHealthPercentage() { return Mathf.Clamp01(stats.health / stats.maxHealth); }
	public void AddHP(float amount) { stats.health += amount; }
	public void FullHeal() {stats.health = stats.maxHealth; }
	public void Die(){
		CancelInvoke ();
		stats.dead = true;
		Instantiate (fx.deathParticles, transform.position, Quaternion.identity);
		SetSpriteVisibility (false);
	}

	public void Revive() {
		stats.dead = false;
		SetSpriteVisibility (true);
		FullHeal ();
	}

	public void SetGodMode(bool g) {stats.godMode = g;}
	public void SetGodMode() {stats.godMode = !stats.godMode;}

	public bool GetHurt(float amount) {

		if (stats.godMode || invincible) {
			return false;
		}

		SetVelocity (Vector3.up * 6.0f);

		AddHP (-1f * Mathf.Abs (amount));
		SetInvincibility (true);

		return true;

	}

	//PHYSX
	public Holster GetHolster() { return holster; }
	public Direction GetDirection() { return direction; }
	public void ApplyForce(Vector3 force) {velocity += force; forceApplied = true;}
	public void SetVelocity(Vector3 v) {velocity = v; forceApplied = true;}
	public void SetPosition(Vector2 position) {transform.position = position;}

	//ANIMATION
	public bool GetHeadBob() {
		if (Mathf.Abs(movementInput.x) > 0)
			return headBob;
		else {
			return false;
		}
	}
	public bool FacingRight() {return facingRight;}
	public float GetLookValue() {return Mathf.Clamp01 (Controller2D.Direction2Vector (direction).y);}
	public bool LookingDown() {return lookingDown;}
		
#endregion

	#region STATS

	private void ManageStats() {

		if (invincible) {

			invincibilityFrame -= Time.deltaTime;
			if (invincibilityFrame < 0) {
				SetInvincibility (false);
			}

		}

		if (stats.health <= 0) {
			Die ();
		}

	}

	private void SetInvincibility(bool i) {

		invincible = i;

		if (invincible) {
			invincibilityFrame = stats.secondsInvulnerableAfterDamage;
			InvokeRepeating ("ToggleSpriteVisibility", 0.0f, .04f);
		} else {
			CancelInvoke ();
			spriteRenderer.enabled = true;
		}

	}



#endregion

    #region ANIMATION

	private void SetHeadBobUp() {headBob = true;}
	private void SetHeadBobDown() {headBob = false;}

	private void SetSpriteVisibility(bool target) {
		spriteRenderer.enabled = target;
		holster.GetSpriteRenderer ().enabled = target;
	}

	private void ToggleSpriteVisibility() {
		bool target = !spriteRenderer.enabled;
		SetSpriteVisibility (target);
	}

    private void Animation()
    {
        spriteRenderer.flipX = facingRight;

        lookingDown = (direction == Direction.DOWN && !controller.collisions.below);

		anim.SetBool("LookingDown", LookingDown());
        anim.SetBool("Grounded", controller.collisions.below);
        anim.SetFloat("VelocityX", Mathf.Abs(movementInput.x));
        anim.SetFloat("VelocityY", Mathf.Sign(velocity.y));
		anim.SetFloat("Looking", GetLookValue());

    }

#endregion

    #region MOVEMENT

    private void Jetpack()
    {
        if (Input.GetButtonUp("Fire1")) { jetpack = true; }

        if (controller.collisions.below) { jetpack = false; jetpackFuel = movement.jetpackFuelRechargeAmount; }

        var emission = fx.jetpackParticles.emission;

        if(jetpack && Input.GetButton("Fire1") && (movementInput.y != 0 || movementInput.x != 0) && jetpackFuel > 0.0f)
        {

            bool sputters = (jetpackFuel < (movement.jetpackFuelRechargeAmount * .25f));

            Vector3 jetpackVelocity = Controller2D.Direction2Vector(direction);

            emission.enabled = true;

            emission.rateOverTime = (sputters ? 5 : 20);

            if(movementInput.x != 0)
            {
                jetpackVelocity = facingRight ? Vector2.right : Vector2.left;
            }

            jetpackVelocity *= (sputters ? .5f : 1.0f);

			if (!stats.godMode) {
				jetpackFuel -= Time.deltaTime;
			}
            velocity = jetpackVelocity * movement.jetpackSpeed;

        } else
        {
            emission.enabled = false;
        }
        

    }

    private void InitVerticalValues()
    {
        gravity = -(2 * movement.maxJumpHeight) / Mathf.Pow(movement.timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * movement.timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * movement.minJumpHeight);

        //print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);

    }

    private void GetInput()
    {

        direction = facingRight? Direction.RIGHT : Direction.LEFT;

        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //VERTICAL DIRECTION APPLY

        if(movementInput.x != 0)
        {
            direction = movementInput.x > 0 ? Direction.RIGHT : Direction.LEFT;
            facingRight = (movementInput.x < 0? false : (movementInput.x > 0 ? true : facingRight));
        }

        //HORIZONTAL DIRECTION APPLY

        float f = movementInput.y;
        if (controller.collisions.below) { f = Mathf.Clamp01(f); }
        if (f != 0)
        {
            direction = f > 0 ? Direction.UP : Direction.DOWN;

        }

        

    }

    private void Vertical()
    {
		if (forceApplied) {
			forceApplied = false;
		} else if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

		if(Input.GetButtonDown("Fire1") && controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }

		velocity.y += gravity * Time.deltaTime;   

    }

    private void Horizontal()
    {
        float targetVelocityX = movementInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            movement.accelerationTimeGrounded * (controller.collisions.below? 1.0f : movement.accelerationTimeAirborneMultiplier));
    }

    private void ApplyMovement()
    {
        controller.Move(velocity * Time.deltaTime);

    }

#endregion

}
