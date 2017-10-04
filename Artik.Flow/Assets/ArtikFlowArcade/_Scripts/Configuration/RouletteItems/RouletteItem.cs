using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public abstract class RouletteItem : ScriptableObject 
{
    [Tooltip("Texture used when displaying the item in the roulette. Can be null.")]
	public Texture itemTexture;
    [Tooltip("Amount of gems.")]
	public int itemCount;

    // Set this to a string so that text will be displayed below the texture.
    [HideInInspector]
    public string itemText = null;

    /// The item can override the text shown in this method.
    public virtual void setText() { itemText = itemCount.ToString(); }
    /// Method called when winning a RouletteItem.
    public abstract void onEarn();
}

}