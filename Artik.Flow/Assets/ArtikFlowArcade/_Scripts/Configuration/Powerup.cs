using UnityEngine;
using System.Collections;

namespace AFArcade {

[CreateAssetMenu()]
public class Powerup : ScriptableObject
{
	public string internalId;

	public Texture texture;
	[Tooltip("Tag used in Smart Localization")]
	public string titleTag;
	[Tooltip("Tag used in Smart Localization")]
	public string descriptionTag;

	[Space(20)]
	[Tooltip("How many of this powerup to give in the first option")]
	public int option1Count;
	[Tooltip("How much the first buy option costs")]
	public int option1Price;

	[Space(10)]
	[Tooltip("How many of this powerup to give in the second option")]
	public int option2Count;
	[Tooltip("How much the second buy option costs")]
	public int option2Price;

	[Space(10)]
	[Tooltip("How many of this powerup to give for watching a video")]
	public int videoCount;
}

}