using UnityEngine;
using System.Collections;

namespace AFArcade {

public class RewardNotification : MonoBehaviour
{
	public static RewardNotification instance;

	UISprite background;
	UILabel coinsLabel;
	GameObject clickDetector;
	GemParticleManager gemParticleManager;
	int coinsToGive;

	void Awake()
	{
		instance = this;
		gemParticleManager = transform.Find("TopPanel").Find("NGUI_GemParticles").GetComponent<GemParticleManager>();
		clickDetector = transform.Find("TopPanel").Find("GemsClickDetector").gameObject;
		background = transform.Find("Background").GetComponent<UISprite>();

		coinsLabel = transform.Find("TopPanel").Find("Label_Gems").GetComponent<UILabel>();
		coinsLabel.supportEncoding = true;
        transform.Find("TopPanel").Find("Label_Earned").GetComponent<UILabel>().text = Language.get("Reward.Earned");
	}

	void Start()
	{
		gemParticleManager.setTarget(IGameplayScreen.instance.getCoinsPosition() - new Vector3(0f, 0.1f, 0f));
		gameObject.SetActive(false);
	}

	void activateClickDetector()
	{
		clickDetector.SetActive(true);
	}

	void giveCoins()
	{
		IGameplayScreen.instance.freezeCoins = false;
	}

	// --- Public ---

	public void playSound(string sound)
	{
		Audio.instance.playName(sound);
	}

	public void give(int coins)
	{
		print("[INFO] Giving " + coins + " gems");

		// to-do: Show x2?

		gameObject.SetActive(false);
		background.color = new Color(0f, 0f, 0f, 0.92f);     // Anim fix
		background.gameObject.SetActive(true);
		foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>(true))
			p.gameObject.SetActive(true);

		clickDetector.SetActive(false);
		Invoke("activateClickDetector", 0.4f);
        gemParticleManager.deactivate();

		coinsLabel.text = "[b]" + Language.get("Reward.Coins").Replace("%", "[c][64d3fd]" + coins + "[-][/c]") + "[/b]";
		Audio.instance.playName("video_reward");

		coinsToGive = coins;
        gameObject.SetActive(true);

		IGameplayScreen.instance.freezeCoins = true;
		SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() + coinsToGive);
	}

	// --- Callbacks ---

	public void onAnimationFinish()
	{
		gameObject.SetActive(false);
	}

	public void onGemsClick()
	{
		gemParticleManager.explode();
		GetComponent<Animator>().SetTrigger("end");

		clickDetector.SetActive(false);

		foreach(ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
			p.gameObject.SetActive(false);

		Invoke("giveCoins", 0.8f);
	}

}

}