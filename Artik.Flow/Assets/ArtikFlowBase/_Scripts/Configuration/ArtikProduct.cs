using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;

namespace AFBase {

[CreateAssetMenu()]
public class ArtikProduct : ScriptableObject
{
	public ProductType type;
	public string productID;
	public string appleID;
	public string googleID;
	public string playphoneID;
}

}