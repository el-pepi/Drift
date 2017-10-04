using UnityEngine;
using System.Collections;

public class Particles : MonoBehaviour
{
	public ParticleSystem p1;
	public ParticleSystem p2;
	public Tails tail1;
	public Tails tail2;

	public LayerMask floorLayer;


	Rigidbody rb;
	bool driftingL = false;
	bool driftingR = false;

	ParticleSystem.EmissionModule em1;
	ParticleSystem.EmissionModule em2;

	//public WheelCar car;

	Car car;


	float driftVolume=0;

	void Start()
	{
		rb = GetComponent<Rigidbody>();

		em1 = p1.emission;
		em2 = p2.emission;

		car = GetComponent<Car>();
	}

	void FixedUpdate ()
	{
		
		/*if (!car.isGrounded ()) {
			
			return;
		}*/


		float angle = Vector3.Angle(rb.velocity, transform.forward);

		if (rb.velocity.magnitude > 0.1f && angle > 25)
		{
			if(!driftingL) {
				if(car.leftGrounded) {
					StartDrift(true);
				}
			} else {
				if(car.leftGrounded==false){//Physics.Raycast(tail1.transform.position, Vector3.down, 1f, floorLayer) == false) {
					StopDrift(true);
				}
			}
			if(!driftingR) {
				if(car.rightGrounded){//Physics.Raycast(tail2.transform.position, Vector3.down, 1f, floorLayer)) {
					StartDrift(false);
				}
			} else {
				if(car.rightGrounded==false){//Physics.Raycast(tail2.transform.position, Vector3.down, 1f, floorLayer) == false) {
					StopDrift(false);
				}
			}

			if(car.leftGrounded && car.rightGrounded) {
				driftVolume = Mathf.Lerp(driftVolume, 0.6f, Time.deltaTime * 4);
			} else {
				driftVolume = Mathf.Lerp(driftVolume,0,Time.deltaTime*8);
			}
		}
		else
		{
			StopDrift(false);
			StopDrift(true);
			driftVolume = Mathf.Lerp(driftVolume,0,Time.deltaTime*8);
		}

		SoundManager.DriftVolume(driftVolume);
	}

	void StartDrift(bool left){
		if(left) {
			if(driftingL) {
				return;
			}
			driftingL = true;

			tail1.StartDrift();
			em1.enabled = true;
		} else {
			if(driftingR) {
				return;
			}
			driftingR = true;

			tail2.StartDrift();
			em2.enabled = true;
		}
	}
	void StopDrift(bool left){
		if(left) {
			if(driftingL == false) {
				return;
			}
			driftingL = false;

			tail1.StopDrift();
			em1.enabled = false;
		} else {
			if(driftingR == false) {
				return;
			}
			driftingR = false;

			tail2.StopDrift();
			em2.enabled = false;
		}
	}
}
