using UnityEngine;
using System.Collections;



[CreateAssetMenu()]
public class DriftCharacter : AFArcade.Character {
	public string carPrefabName;

	public string shopCarResource;
	public Texture styleTexture;
	public Texture defenseTexture;
	public Texture weaponTexture;
	public enum WeaponsActive
	{
		Laser,
		Ray,
		Spinner,
		Rocket
	}
	[Header("Bars")]
	[Range(0, 12)]
	public int damageShopBar = 0;

	[Range(0, 12)]
	public int healthShopBar = 0;


	public WeaponsActive[] weaponsIconsActive;
	[Header("Car Info")]
	public string carName;
	public string descriptionKey;
	public string tierString;
	public AFBase.ArtikProduct iapProduct;		// null if can't be bought with IAP
}
