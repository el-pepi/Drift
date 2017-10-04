using UnityEngine;
using System.Collections;

public abstract class ExampleScreen : MonoBehaviour
{
	private ExamplesMenu menu;

	void Awake()
	{
		menu = GetComponent<ExamplesMenu>();
	}

	public abstract void Draw();

	protected void SetStatus(string status)
	{
		menu.Status = status;
	}
}
