using UnityEngine;
using System.Collections;

namespace ArtikFlowExample
{
	public class Player : MonoBehaviour
	{
		public static Player instance;

		public Camera mainCamera;
		public float speed;

		float scoreTimer;

		void Awake()
		{
			instance = this;
		}

		public void reset()
		{
			// Delete previous renderer:
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform oldRenderer = transform.GetChild(i);
				Destroy(oldRenderer.gameObject);
			}

			// Other reseting
			transform.position = Vector3.zero;
			scoreTimer = 0;
		}

		public void setCharacter(AFArcade.Character character)
		{
			// Set new renderer
			GameObject newRenderer = Instantiate(((ExampleCharacter)character).characterPrefab);
			newRenderer.transform.parent = transform;
			newRenderer.transform.localPosition = Vector3.zero;
			newRenderer.name = "Renderer";
		}

		void Update()
		{
			if(!GameManager.instance) {
				return;
			}
			if (GameManager.instance.playing)
			{
				// Movement
				if (Input.GetMouseButton(0))
				{
					Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
					transform.position = new Vector3(pos.x, pos.y, 0);
				}

				// Powerup
				if (Input.GetMouseButtonDown(1))
				{
					GameManager.instance.usePowerup();
				}

				// Need Gems
				if (Input.GetKeyDown(KeyCode.N))
					GameManager.instance.eventNeedGems.Invoke();

				// Score
				scoreTimer += Time.deltaTime;
				if (scoreTimer > 1f)
				{
					scoreTimer -= 1f;
					GameManager.instance.eventAddScore.Invoke(1);
					if (Random.Range(0, 5) == 0)
						GameManager.instance.eventAddCoins.Invoke(1);
				}				
			}
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "DeathTrigger")
				GameManager.instance.die();
		}

	}

}