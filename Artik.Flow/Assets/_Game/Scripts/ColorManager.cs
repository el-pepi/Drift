using UnityEngine;
using System.Collections;


[System.Serializable]
public class ChangeColorMaterial
{
	public Material mat;
	public Color[] col;

}
	
public class ColorManager : MonoBehaviour 
{

	public static ColorManager instace;
	public ChangeColorMaterial[] colorMats;
	public int index;

	public Color[] fogColors;
	//Camera cam;

	//public BossBar bossBar;

	public Material shapes;
	public Texture2D[] shapeColors;

	void Start () 
	{
		instace = this;
		//bossBar = GameObject.FindObjectOfType<BossBar> ();
	}

	public void SetColors()
	{
		if (index < 5)
			index++;
		
		SetColors(index);
	}

	public void SetColors(int ind)
	{
		if (ind < 5) {
			index = ind;
		}
		else {
			index = 4;
		}
		foreach (var item in colorMats)
		{
			item.mat.color = item.col [index];
		}

		Third.instance.cam.backgroundColor = fogColors[index];
		RenderSettings.fogColor = fogColors[index];

		shapes.mainTexture = shapeColors[index];
	}



}
