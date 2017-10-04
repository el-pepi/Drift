using UnityEngine;
using System.Collections;

public class ScoreText : MonoBehaviour 
{
	public GameObject scoreText;
	public Vector3 offSet;
	Enemy enemy;
	Transform canvas;
	UILabel text;
	public GameObject hPanel;

	void Awake () 
	{
		enemy = GetComponent<Enemy> ();
		hPanel = Instantiate (scoreText)as GameObject;
		canvas = GameObject.Find ("ScoreManager").transform;
		hPanel.transform.SetParent (canvas);
		text = hPanel.GetComponentInChildren<UILabel> ();
		text.text = enemy.score.ToString();
	}

	void OnEnable()
	{
		if (hPanel != null) {
			Vector3 pos = transform.position + offSet;

			Vector3 wordPos = Camera.main.WorldToScreenPoint (pos);

			hPanel.transform.position = wordPos;
			hPanel.gameObject.SetActive (true);
		}
	}

	void OnDisable()
	{
		if (hPanel != null) 
		{
			hPanel.gameObject.SetActive (false);
			hPanel.transform.position = new Vector3 (1000f, 1000f, 1000f);
		}
	}



}
