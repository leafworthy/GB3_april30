using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicators : MonoBehaviour
{


   [SerializeField] private HideRevealObjects p1indicator;
   [SerializeField] private HideRevealObjects p2indicator;
   [SerializeField] private HideRevealObjects p3indicator;
   [SerializeField] private HideRevealObjects p4indicator;

   public void SetColors(Color c1, Color c2, Color c3, Color c4)
   {
	   foreach (var go in p1indicator.objectsToReveal)
	   {
		   var sr = go.GetComponent<SpriteRenderer>();
		   if (sr != null)
		   {
			   c1.a = sr.color.a;
			   sr.color = c1;
		   }
	   }

	   foreach (var go in p2indicator.objectsToReveal)
	   {
		   var sr = go.GetComponent<SpriteRenderer>();
		   if (sr != null)
		   {
			   c2.a = sr.color.a;
			   sr.color = c2;
		   }
	   }

	   foreach (var go in p3indicator.objectsToReveal)
	   {
		   var sr = go.GetComponent<SpriteRenderer>();
		   if (sr != null)
		   {
			   c3.a = sr.color.a;
			   sr.color = c3;
		   }
	   }

	   foreach (var go in p4indicator.objectsToReveal)
	   {
		   var sr = go.GetComponent<SpriteRenderer>();
		   if (sr != null)
		   {
			   c4.a = sr.color.a;
			   sr.color = c4;
		   }
	   }
   }
    public void HighlightTheTrueOnes(bool p1, bool p2, bool p3, bool p4)
    {
	    if (p1)
	    {
		    p1indicator.SetActiveObject(1);
	    }
	    else
	    {
		    p1indicator.SetActiveObject(0);
	    }

	    if (p2)
	    {
		    p2indicator.SetActiveObject(1);
	    }
	    else
	    {
		    p2indicator.SetActiveObject(0);
	    }

	    if (p3)
	    {
		    p3indicator.SetActiveObject(1);
	    }
	    else
	    {
		    p3indicator.SetActiveObject(0);
	    }

	    if (p4)
	    {
		    p4indicator.SetActiveObject(1);
	    }
	    else
	    {
		    p4indicator.SetActiveObject(0);
	    }

    }

    public void Deselect(PlayerIndex index)
    {
	    switch (index)
	    {
		    case PlayerIndex.One:
			    p1indicator.SetActiveObject(0);
			    break;
		    case PlayerIndex.Two:
			    p2indicator.SetActiveObject(0);
			    break;
		    case PlayerIndex.Three:
			    p3indicator.SetActiveObject(0);
			    break;
		    case PlayerIndex.Four:
			    p4indicator.SetActiveObject(0);
			    break;
		    default:
			    throw new ArgumentOutOfRangeException(nameof(index), index, null);
	    }
    }

    public void Select(PlayerIndex index)
    {
	    switch (index)
	    {
		    case PlayerIndex.One:
			    p1indicator.SetActiveObject(2);
			    break;
		    case PlayerIndex.Two:
			    p2indicator.SetActiveObject(2);
			    break;
		    case PlayerIndex.Three:
			    p3indicator.SetActiveObject(2);
			    break;
		    case PlayerIndex.Four:
			    p4indicator.SetActiveObject(2);
			    break;
		    case PlayerIndex.Five:
			    p4indicator.SetActiveObject(2);
			    break;
	    }
    }
}
