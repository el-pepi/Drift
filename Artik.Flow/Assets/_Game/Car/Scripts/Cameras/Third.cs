using UnityEngine;
using System.Collections;

public class Third : MonoBehaviour
{
	GameObject target;

	Vector3 start_offset;
	public Vector3 start_pos { get; protected set; }
	public Quaternion start_rot { get; protected set; }
	Rigidbody rbCar;
	public float offset;
	public float followTime;
	public float minSize;
	float prevMag;
	Vector3 prevVelocity = Vector3.zero;
	[Range(0f, 2f)]
	public float zoomTime;
	public Camera cam;

	public static Third instance;

	float nextOffset;
	float nexMinSize;
	float nexMaxSize;
	bool transitionCamera;
	Animator anim;

	public float fieldsOffViewMin;
	public float maxSize;

	public float lerpPos;

	void Awake()
	{
		instance = this;

		start_pos = transform.position;
		start_rot = transform.rotation;
	}

	void Start()
	{
		//cam = GetComponent<Camera> ();
		cam.orthographicSize = minSize;
		anim = transform.GetChild(0).GetComponent<Animator>();
	}

	public void SetTarget(GameObject t){
		target = t;
		if(target == null)
			return;

		rbCar = target.GetComponent<Rigidbody> ();
		if(start_offset == Vector3.zero) {
			start_offset = transform.position - target.transform.position;
		}

		Vector3 targetpos = target.transform.position+start_offset;

		//transform.position =Vector3.Lerp (transform.position,targetpos,lerpPos *Time.deltaTime);
		transform.position = targetpos;
	}

	public void SetOffsetAndMinSize(float fOffset,float fMinSize,float fMaxSize,bool transition)
	{
		if (transition)
		{
			nextOffset = fOffset;
			nexMinSize = fMinSize;
			nexMaxSize = fMaxSize;
			transitionCamera = transition;
		} else
		{
			offset = fOffset;
			minSize = fMinSize;
			maxSize = fMaxSize;
			transitionCamera = transition;
		}
	}

	void TranstionCamera()
	{
		if (transitionCamera) 
		{
			offset = Mathf.Lerp (offset,nextOffset,Time.deltaTime*0.5f);
			minSize = Mathf.Lerp (minSize,nexMinSize,Time.deltaTime*0.5f);
			maxSize = Mathf.Lerp (maxSize,nexMaxSize,Time.deltaTime*0.5f);
			if (offset == nextOffset && minSize == nexMinSize&& maxSize == nexMaxSize)
				transitionCamera = false;
		}
	}

	void Update()
	{
		if(target == null) {
			return;
		}

		TranstionCamera ();

		Vector3 velocity = rbCar.velocity.normalized;
		velocity.y = 0f;
		velocity = Vector3.Lerp (prevVelocity, velocity, followTime * Time.deltaTime);
		Vector3 targetpos = target.transform.position + (velocity * offset) + start_offset;

		//targetpos.y = transform.position.y;
		// transform.position = Vector3.Lerp (transform.position, targetpos, 0.5f * Time.deltaTime);
		transform.position = targetpos;
	
		prevVelocity = velocity;

		float mag = new Vector3 (rbCar.velocity.x,0f,rbCar.velocity.z).magnitude;
		mag = Mathf.Lerp (prevMag,mag,zoomTime*Time.deltaTime);
		//if(mag>30f)
		cam.fieldOfView = mag+fieldsOffViewMin;
		prevMag = mag;

		if (cam.fieldOfView < minSize)
			cam.fieldOfView = minSize;
		
		if (cam.fieldOfView > maxSize)
			cam.fieldOfView = maxSize;

		// Rotation
		if(start_rot != transform.rotation)
		{
			Quaternion targetrot = Quaternion.Slerp(transform.rotation, start_rot, Time.deltaTime);
			transform.rotation = targetrot;
		}
		
	}


	public void Hit(){
		anim.SetTrigger("Hit");
	}

	public void BossShake(){
		anim.SetTrigger("BossIntro");
	}
	
}
