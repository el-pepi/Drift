using UnityEngine;
using System.Collections;

namespace AFArcade {

public class ShopItem : MonoBehaviour
{
	public enum State
	{
		LOCKED,
		UNLOCKED,
		SELECTED,
	}
	State state;

	UITexture texture;
	GameObject selectedTick;
	GameObject priceObject;

	ParticleSystem selectedParticles;
	ParticleSystem boughtParticles;

	UISprite button;

	int tier;	// 1, 2 or 3

	void Awake()
	{
		button = GetComponentInChildren<UISprite>();
		texture = button.transform.Find("Texture").GetComponent<UITexture>();
		selectedTick = transform.Find("Sprite_Selected").gameObject;
		priceObject = transform.Find("Price").gameObject;
		selectedParticles = transform.Find("Selected_Particles").GetComponent<ParticleSystem>();
		boughtParticles = transform.Find("Bought_Particles").GetComponent<ParticleSystem>();
	}

	// --- Public methods ---

	public void setState(State newState)
	{
		if (newState == State.LOCKED)
		{
			selectedTick.SetActive(false);
			priceObject.SetActive(true);
			Color col = button.color;
			col.a = 0.3f;
			button.color = col;
			texture.color = new Color(0.4f, 0.4f, 0.4f);
		}
		else if(newState == State.UNLOCKED)
		{
			selectedTick.SetActive(false);
			priceObject.SetActive(false);
			Color col =button.color;
			col.a = 1f;
			button.color = col;
			texture.color = Color.white;
		}	
		else if(newState == State.SELECTED)
		{
			selectedTick.SetActive(true);
			priceObject.SetActive(false);
			Color col = button.color;
			col.a = 1f;
			button.color = col;
			texture.color = Color.white;
			selectedTick.GetComponent<TweenScale>().enabled = true;
            selectedTick.GetComponent<TweenScale>().ResetToBeginning();
			selectedTick.GetComponent<TweenPosition>().enabled = true;
			selectedTick.GetComponent<TweenPosition>().ResetToBeginning();
			GetComponent<TweenScale>().enabled = true;
			GetComponent<TweenScale>().ResetToBeginning();
			selectedParticles.time = 0f;
			selectedParticles.Play();
		}

		if(state == State.LOCKED && (newState == State.UNLOCKED || newState == State.SELECTED))
		{
			boughtParticles.time = 0;
			boughtParticles.Play();
        }

		state = newState;
	}

	public void setTier(int t)
	{
		tier = t;

		if (tier == 1)
		{
			button.color = new Color32(112, 238, 248, 255);
            selectedTick.GetComponent<UISprite>().color = new Color32(255, 77, 121, 255);
			selectedParticles.startColor = new Color32(255, 77, 121, 255);
		}
		else if (tier == 2)
		{
			button.color = new Color32(189, 13, 169, 255);
			selectedTick.GetComponent<UISprite>().color = new Color32(77, 254, 204, 255);
			selectedParticles.startColor = new Color32(77, 254, 204, 255);
		}
		else if (tier == 3)
		{
			button.color = new Color32(226, 16, 30, 255);
			selectedTick.GetComponent<UISprite>().color = new Color32(77, 110, 254, 255);
			selectedParticles.startColor = new Color32(77, 110, 254, 255);
		}
	}

}

}