using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextureOverTime : MonoBehaviour 
{
	public float scrollSpeed = 0.5f;
	public float offset;
	public Renderer rend;
	public bool ignoreTimescale;

	public enum scrollMode 
	{
		X, Y, BothPositive, BothNegative, PosXNegY, NegXPosY
	}
	public scrollMode ScrollMode;

	void Start () 
	{
		if (rend == null) 
		{
			rend = GetComponent <Renderer> ();
		}
	}

	void Update () 
	{
		if (ignoreTimescale == false)
		{
			offset = Time.time * scrollSpeed;
		}

		if (ignoreTimescale == true)
		{
			offset = Time.unscaledTime * scrollSpeed;
		}
			
		if (ScrollMode == scrollMode.X)
		{
			rend.material.SetTextureOffset ("_MainTex", new Vector2 (offset, 0));
		}

		if (ScrollMode == scrollMode.Y)
		{
			rend.material.SetTextureOffset ("_MainTex", new Vector2 (0, offset));
		}

		if (ScrollMode == scrollMode.BothPositive)
		{
			rend.material.SetTextureOffset ("_MainTex", new Vector2 (offset, offset));
		}

		if (ScrollMode == scrollMode.BothNegative)
		{
			rend.material.SetTextureOffset ("_MainTex", new Vector2 (-offset, -offset));
		}

		if (ScrollMode == scrollMode.PosXNegY)
		{
			rend.material.SetTextureOffset ("_MainTex", new Vector2 (offset, -offset));
		}

		if (ScrollMode == scrollMode.NegXPosY)
		{
			rend.material.SetTextureOffset ("_MainTex", new Vector2 (-offset, offset));
		}
	}
}
