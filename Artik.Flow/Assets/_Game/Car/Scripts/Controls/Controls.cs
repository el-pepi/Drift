using UnityEngine;
using System.Collections;

public abstract class Controls : MonoBehaviour
{
	Car car;

	void Awake()
	{
		car = GetComponentInParent<Car>();
    }

	void OnEnable()
	{
		car.setControls(this);
	}


	public abstract float getTurnFactor();

}
