using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public float maxJumpHeight = 3f;
    public float minJumpHeight = 1.5f;
    public float timeToJumpApex = .4f;
    public float accelerationTimeGrounded = .1f;
    public float accelerationTimeAirborneMultiplier = 2f;
    public float jetpackSpeed = 5.0f;
    public float jetpackFuelRechargeAmount = 10.0f;

    public Transform camTarget;
    public float range = 5;

    public ParticleSystem jetpackParticles;

    float moveSpeed = 6f;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    Vector2 movementInput;
    float velocityXSmoothing;
    bool jetpack;
    float jetpackFuel;

    Controller2D controller;
    Animator anim;
    Direction direction;
    bool facingRight;
	bool lookingDown;
	bool headBob = false;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        InitVerticalValues();
        direction = Direction.RIGHT;
        facingRight = true;
    }

    private void Update()
    {
        //MOVEMENT
        GetInput();
        Horizontal();
        Vertical();
        Jetpack();
        ApplyMovement();

        //ANIMATION
        Animation();
        UpdateCamTarget();
    }

    #region PUBLIC

	public float GetFuelPercentage() { return Mathf.Clamp01(jetpackFuel / jetpackFuelRechargeAmount); }
	public void ApplyForce(Vector3 force) {velocity += force;}
    public void Die(){transform.position = Vector3.zero;}
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

    #region ANIMATION

	private void SetHeadBobUp() {headBob = true;}
	private void SetHeadBobDown() {headBob = false;}

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

    private void UpdateCamTarget()
    {

        Vector3 targetPosition = transform.position +   new Vector3(
            facingRight ? 1 : -1,
            Controller2D.Direction2Vector(direction).y) * range;

        camTarget.position = targetPosition;

    }

#endregion

    #region MOVEMENT

    private void Jetpack()
    {
        if (Input.GetButtonUp("Fire1")) { jetpack = true; }

        if (controller.collisions.below) { jetpack = false; jetpackFuel = jetpackFuelRechargeAmount; }

        var emission = jetpackParticles.emission;

        if(jetpack && Input.GetButton("Fire1") && (movementInput.y != 0 || movementInput.x != 0) && jetpackFuel > 0.0f)
        {

            bool sputters = (jetpackFuel < (jetpackFuelRechargeAmount * .25f));

            Vector3 jetpackVelocity = Controller2D.Direction2Vector(direction);

            emission.enabled = true;

            emission.rateOverTime = (sputters ? 5 : 20);

            if(movementInput.x != 0)
            {
                jetpackVelocity = facingRight ? Vector2.right : Vector2.left;
            }

            jetpackVelocity *= (sputters ? .5f : 1.0f);

            jetpackFuel -= Time.deltaTime;

            velocity = jetpackVelocity * jetpackSpeed;

        } else
        {
            emission.enabled = false;
        }
        

    }

    private void InitVerticalValues()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

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
        if (controller.collisions.above || controller.collisions.below)
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
            accelerationTimeGrounded * (controller.collisions.below? 1.0f : accelerationTimeAirborneMultiplier));
    }

    private void ApplyMovement()
    {
        controller.Move(velocity * Time.deltaTime);

    }

#endregion

}
