using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour
{
	[Header("Config")]
	public LayerMask groundLayer;
	public float acceleration = 45f;

	public float turnForceDrift = 70f;

	[Range(0.7f, 1f)]
	public float driftinessFactorDrift = 0.99f;
	public float maxDriftAngle = 90f;
	//public float maxRotationPerSecond = 90f;
	public float maxAngularVelocity = 2f;
	[Tooltip("0: No deceleration when turning. 1: Brake when turning.")]

	[Range(0f, 1f)]
	public float turnDecelerationFactorDrift = 0.1f;
	public Vector3 debugRay;
	public Controls controls;
	Rigidbody rb;
	ConstantForce engine;
	//bool reactivateConstraints = true;
	//RigidbodyConstraints contraints;
	Vector3 startPos;
	float checkTime = 0;
	public float maxSpeed;

	public bool debugMode = false;

	public float vel;
	//public Vector3 velVec;

	PlayerHealth pHealth;

	public GameObject geometry;
	Animator anim;
	float dir=0;

	public Transform reviveTransform;
	ChaserGO chaser;

	bool isGrounded = true;


	Vector3[] wheelPos;

	void Awake()
	{
		wheelPos = new Vector3[4];
		wheelPos[0] = new Vector3(1f,0,2f);//fr
		wheelPos[1] = new Vector3(1f,0,-2f);//br
		wheelPos[2] = new Vector3(-1f,0,2f);//fl
		wheelPos[3] = new Vector3(-1f,0,-2f);//bl

		rb = GetComponent<Rigidbody>();
		pHealth = GetComponent<PlayerHealth> ();
		rb.constraints = RigidbodyConstraints.FreezePositionY;
		engine = GetComponent<ConstantForce>();
		startPos = transform.position;
		rb.isKinematic = true;
		anim = geometry.GetComponent<Animator>();

	}

	// --- Controller ---
	
	void FixedUpdate()
	{
		if (!GameManager.instance.playing)
			return;
		else {
			rb.isKinematic = false;
		}

		if(isGrounded) {
			if(CheckGrounded() == false) {
				disableConstraints();
				isGrounded = false;
			}
		} else {
			if(CheckGrounded() == true) {
				enableConstraints ();
				rb.MovePosition(new Vector3(transform.position.x,-3.2f,transform.position.z));
				isGrounded = true;
			}
		}

		if(isGrounded) {
			GroundedFixedUpdate();
		} else {
			UnGroundedFixedUpdate();
		}	
	}

	public Animator GetAnimator(){
		return anim;
	}

	void GroundedFixedUpdate(){
		//velVec = rb.velocity;
		Vector3 force = Vector3.zero;

		// --- Main engines

		force.z = acceleration * (1f - (Mathf.Abs (controls.getTurnFactor ()) * turnDecelerationFactorDrift));
		engine.relativeForce = force;


		force = Vector3.zero;

		force.y = controls.getTurnFactor () * turnForceDrift;

		engine.relativeTorque = force;

		if (rb.angularVelocity.magnitude > maxAngularVelocity) {
			rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
		}
		vel = rb.velocity.magnitude;
		updateForwardVelocity ();
		if (rb.velocity.magnitude > maxSpeed)
			rb.velocity = rb.velocity.normalized * maxSpeed;
	}

	void UnGroundedFixedUpdate(){
		checkTime = (checkTime + Time.deltaTime);

		if (checkTime > 2f) 
		{
			rb.isKinematic = true;
			GameManager.instance.die ();
		}
		//engine.relativeForce = Vector3.zero;
		//engine.relativeTorque = Vector3.zero;
		rb.angularVelocity = new Vector3 (0f, 0f, 0f);
		//rb.drag = 0f;
		//rb.constraints = RigidbodyConstraints.None;
		rb.AddForceAtPosition(Vector3.down * 5000 * Time.deltaTime , transform.TransformPoint(Vector3.forward*0.5f),ForceMode.Acceleration);
	}


	void Update(){
		if(pHealth.currentHealth <= 0 || !GameManager.instance.playing)
		{
			return;
		}

		dir = Mathf.Lerp(dir,controls.getTurnFactor (),8f*Time.deltaTime);
		anim.SetFloat("Dir",dir);

		SoundManager.DriveVolume(rb.velocity.magnitude / 130f);
	}

	public void Reset()
	{
		enableConstraints ();
		chaser = GameObject.FindObjectOfType<ChaserGO> ();
		pHealth.Reset ();
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
		engine.relativeForce = Vector3.zero;
		engine.relativeTorque = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.position = startPos;
		Camera.main.orthographicSize = 40f;
		geometry.SetActive(true);
		dir = 0;
	}

	public void Revive()
	{
		Reset ();

		Transform newRotationT =transform;
		Vector3 forwardVectorToMatch = -reviveTransform.right;
		float corrRotation = Azimuth (forwardVectorToMatch) - Azimuth(transform.forward);

		//reviveTransform.position

		Vector3 newRevPos = reviveTransform.position;
		newRevPos.y = -3.2f;
		transform.position = newRevPos;


		if (GameManager.instance.onBoss)
		{
			Third.instance.SetOffsetAndMinSize (22f, 85f, 110f, true);
		} 
		else
		{
			Third.instance.SetOffsetAndMinSize (14f,65f,70f,true);
		}

		LevelManager.instance.UpdateDistance ();
		pHealth.Inmune ();
		StartCoroutine (SpawnChaser(reviveTransform));

		transform.rotation = Quaternion.identity;
		newRotationT.RotateAround (transform.position,Vector3.up,corrRotation);

		enableConstraints ();
		ReviveTime.instance.Invoke("StartCounter",0.02f);
	}



	public void TutorialDead(Transform point)
	{
		Reset ();
		geometry.SetActive(true);
		Transform newRotationT = transform;
		Vector3 forwardVectorToMatch = -point.right;
		float corrRotation = Azimuth (forwardVectorToMatch) - Azimuth(transform.forward);
		newRotationT.RotateAround (transform.position,Vector3.up,corrRotation);
		transform.position = point.position + new Vector3 (0f,2f,0f);

		Third.instance.SetOffsetAndMinSize (14f,65f,70f,true);

	//	LevelManager.instance.UpdateDistance ();
		pHealth.Inmune ();
		StartCoroutine (SpawnChaser(point));
	}

	IEnumerator SpawnChaser(Transform trans)
	{
		chaser.gameObject.SetActive (false);
		yield return new WaitForSeconds (2f);
		chaser.gameObject.SetActive (true);
		chaser.Revive(trans);
	}

	float Azimuth(Vector3 vector)
	{
		return Vector3.Angle (Vector3.forward,vector)*Mathf.Sign(vector.x);
	}

	private void enableConstraints()
	{
		rb.constraints = RigidbodyConstraints.FreezePositionY;
		checkTime = 0;
		//rb.drag = 0.93f;
		//rb.drag = 0.93f;
		//rb.angularDrag = 2f;
	}

	private void disableConstraints()
	{
		rb.constraints = RigidbodyConstraints.None;
		//rb.drag = 0.2f;
		/*rb.drag = 0;
		rb.angularDrag = 0f;*/
		engine.relativeForce = Vector3.zero;
		//engine.relativeTorque = Vector3.zero;
		rb.angularVelocity = new Vector3 (0f, 0f, 0f);
	}

	/*
	void accelerate()
	{
		if (!isGrounded())
			return;

		rb.AddRelativeForce(Vector3.forward * acceleration);

		if (rb.velocity.magnitude > maxSpeed)
			rb.velocity = rb.velocity.normalized * maxSpeed;
	}*/
	/*
	void turn()
	{
		if (!isGrounded())
			return;

		if(physicsMode == PhysicsMode.DRIFTY)
		{
			float torque = controls.getTurnFactor() * driftTorque;

			print(torque);
			rb.AddRelativeTorque(new Vector3(0f, torque, 0f));

		}
	}
	*/

	void updateForwardVelocity()
	{

		if (!CheckGrounded())
			return;

		if (rb.velocity.magnitude > maxSpeed)
		{
			return;
		}
		
		float factor;       // 1f more drifty, 0f instant-turn
		factor = driftinessFactorDrift;

		float prev_magnitude = rb.velocity.magnitude;
		rb.velocity = rb.velocity * factor;
		float new_magnitude = rb.velocity.magnitude;
		rb.velocity = rb.velocity + (transform.forward * (prev_magnitude - new_magnitude));
	}

	public bool leftGrounded = false;
	public bool rightGrounded = false;

	public Module GetCurrentModule()
	{
		RaycastHit hit;
		if (Physics.Raycast (transform.position + transform.TransformDirection( debugRay),transform.TransformDirection(-Vector3.up),out hit, 4f, groundLayer, QueryTriggerInteraction.Ignore)) 
		{
			return hit.transform.parent.gameObject.GetComponent<Module>();
		}
		return null;
	}

	public bool CheckGrounded()
	{
		int hits = 0;
		for(int i = 0; i < wheelPos.Length; i++) {
			Ray r = new Ray(transform.TransformPoint(wheelPos[i]), Vector3.down);
			RaycastHit hit;
			if(Physics.Raycast(r, out hit, 5f, groundLayer, QueryTriggerInteraction.Ignore)) {
				//print(hit.transform.name);
				hits++;

				if(i == 3) {
					leftGrounded = true;
				} else if(i == 1) {
						rightGrounded = true;
					}
			} else {
				if(i == 3) {
					leftGrounded = false;
				} else if(i == 1) {
					rightGrounded = false;
				}
			}
			Debug.DrawRay(r.origin,r.direction,Color.blue);
		}
		if(hits >= 2) {
			return true;
		} else {
			return false;
		}
		//return Physics.Raycast (transform.position + transform.TransformDirection (debugRay+Vector3.left), transform.TransformDirection (-Vector3.up), 2f, groundLayer, QueryTriggerInteraction.Ignore)||Physics.Raycast (transform.position + transform.TransformDirection (debugRay-Vector3.left), transform.TransformDirection (-Vector3.up), 2f, groundLayer, QueryTriggerInteraction.Ignore);
		//return Physics.Raycast (transform.position + transform.TransformDirection (debugRay + Vector3.left), transform.TransformDirection (-Vector3.up), 1f, groundLayer, QueryTriggerInteraction.Ignore);
		//return Physics.Raycast (transform.position + transform.TransformDirection (debugRay), Vector3.down * 30, 1f, groundLayer, QueryTriggerInteraction.Ignore);
	}



	public void setControls(Controls newControls)
	{
		controls = newControls;
	}

	public void OnDeath(){
		pHealth.currentHealth = 0;
		geometry.SetActive(false);
	}

	public void CleanUp(){
		GetComponent<Particles>().tail1.Reset();
 	    GetComponent<Particles>().tail2.Reset();
		ParticleManager.ReturnParticle("LaserHitLight");
		ParticleManager.ReturnParticle("LaserHitSparks");
	}
}