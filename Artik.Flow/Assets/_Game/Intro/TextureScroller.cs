using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroller : MonoBehaviour 
{
	public float speedX = 2f;
	public float speedY = 0f;

	Material tunnelMaterial;
	
	void Awake()
	{
		MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
		if(renderer != null)
			tunnelMaterial = GetComponentInChildren<MeshRenderer>().sharedMaterial;
		else
			tunnelMaterial = GetComponent<Projector>().material;
	}

	void Update()
	{
		tunnelMaterial.mainTextureOffset = new Vector2(
			tunnelMaterial.mainTextureOffset.x + (Time.deltaTime * speedX),
			tunnelMaterial.mainTextureOffset.y + (Time.deltaTime * speedY));
	}

}
