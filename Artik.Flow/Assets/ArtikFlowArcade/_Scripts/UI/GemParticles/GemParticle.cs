using UnityEngine;
using System.Collections;

namespace AFArcade {

public class GemParticle : MonoBehaviour
{
	static float DIRECTION_CHANGE_FACTOR = 0.13f;    // Larger is smoother, but slower
	static float DIRECTION_TRAVEL_FORCE = 1500f;	// Larger is faster, speed moving towards coins
	
	public GameObject blinkPrefab;
	public float minStartSpeed = 0.3f;
	public float maxStartSpeed = 2f;
	public float scaleMultiplier = 1f;

	Vector3 target;
	GameObject blink;
	bool travelling;
	Vector2 velocity;

	void Awake()
	{
		blink = Instantiate(blinkPrefab, transform.position, transform.rotation) as GameObject;
		blink.transform.parent = transform.parent;
		blink.SetActive(false);
	}

	void Update()
	{
		transform.localPosition = transform.localPosition + (Vector3) (velocity * Time.deltaTime);
		transform.localRotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f), velocity);

		float distance = Vector3.Distance(transform.position, target);
		float scale = distance * 3f;
		if (scale < 1f)
		{
			scale -= 0.23f;
			if (scale < 0)
				scale = 0f;
			scale *= scaleMultiplier;

			transform.localScale = new Vector3(scale, scale, scale);
		}

		if (travelling)
		{
			velocity = (Vector3)velocity + ((target - transform.position).normalized * DIRECTION_TRAVEL_FORCE * Time.deltaTime);

			/*
			if ((transform.position - target).sqrMagnitude < 0.03f)
				disappear();
			*/
			if (transform.localPosition.y > 520 || transform.localPosition.x > 400)
				disappear();
		}
		else
			velocity = velocity - (velocity * 0.5f * Time.deltaTime);
	}

	void disappear()
	{
		blink.transform.position = transform.position;
		blink.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		blink.SetActive(true);
		gameObject.SetActive(false);
		Invoke("killBlink", 0.2f);
	}

	void killBlink()
	{
		blink.SetActive(false);
	}

	// --- Public ---

	public void reset()
	{
		travelling = false;
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one * scaleMultiplier;

		velocity = Random.insideUnitCircle * Random.Range(minStartSpeed, maxStartSpeed);
		transform.localRotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f), velocity);
	}

	public void setTarget(Vector3 new_target)
	{
		target = new_target;
	}

	public void travel()
	{
		travelling = true;
		velocity *= DIRECTION_CHANGE_FACTOR;
	}

}

}