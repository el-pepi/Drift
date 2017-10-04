using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
	public int renderQueue = 3000;

	Material mMat;

	void Start ()
	{
		Renderer ren = GetComponent<Renderer>();
		ParticleSystem sys = null;

		if (ren == null)
		{
			sys = GetComponent<ParticleSystem>();
			if (sys != null) ren = sys.GetComponent<Renderer>();
		}

		if (ren != null)
		{
			mMat = new Material(ren.sharedMaterial);
			mMat.renderQueue = renderQueue;
			ren.material = mMat;
		}

		if (sys != null && sys.playOnAwake)
			sys.Play();
	}

	void OnDestroy () { if (mMat != null) Destroy(mMat); }
}