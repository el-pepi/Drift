using UnityEngine;
using System.Collections;

public class DamagePlayer : MonoBehaviour
{
	public enum DamageType{normal, laser, bomb};
	public int damageToPlayer = 1;
	public DamageType damageType = DamageType.normal;
}
