using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AFArcade {

public class FontOverrides : MonoBehaviour 
	{
	void Start()
	{
		Color nullColor = new Color32(0,0,0,0);

		foreach(ArtikFlowArcadeConfiguration.FontOverrideProperties p in ArtikFlowArcade.instance.configuration.fontOverrideProps)
		{
			if(!p.overrideFont && !p.overrideColor && !p.overrideEffect && !p.overrideFontSize && !p.overrideSpacingX)		// Skip conditions
				continue;
				
			try
			{
				string[] path = p.path.Split('.');
				Transform t = null;
				foreach(GameObject g in SceneManager.GetSceneByName("ArtikFlowArcade").GetRootGameObjects())
				{
					if(g.name == path[0])
						t = g.transform;
				}
				bool first = false;

				foreach(string s in path)
				{
					if(!first)
						first = true;
					else
						t = t.Find(s);
				}

				UILabel label = t.GetComponent<UILabel>();
				
				if(p.overrideFont)
					label.bitmapFont = p.newFont;
				if(p.overrideColor)
					label.color = p.newColor;
				if(p.overrideEffect)
					label.effectStyle = p.newEffect;
				if(p.overrideFontSize)
					label.fontSize = p.newFontSize;
				if(p.overrideSpacingX)
					label.spacingX = p.newSpacingX;
			}
			catch (System.Exception e)
			{
				Debug.LogWarning("[ERROR] Error processing font override for path: '" + p.path + "'\nException:\n" + e.ToString());
			}
		}

	}

}

}