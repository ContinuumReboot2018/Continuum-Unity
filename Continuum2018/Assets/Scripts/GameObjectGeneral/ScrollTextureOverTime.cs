using UnityEngine;

public class ScrollTextureOverTime : MonoBehaviour 
{
	[Tooltip ("Speed to scroll texture offset.")]
	public Vector2 ScrollSpeed = new Vector2 (0.5f, 0.5f);
	[Tooltip ("Current offset.")]
	public Vector2 CurrentOffset; // Current offset.

	private Renderer rend; // Current renderer.
	[Tooltip ("Horizontal scroll mode.")]
	public scrollType HorizontalScrollMode;
	[Tooltip ("Vertical scroll mode.")]
	public scrollType VerticalScrollMode;
	public enum scrollType 
	{
		None = 0, 
		Positive = 1, 
		Negative = 2
	}

	[Tooltip ("Should texture scrolling be scaled by time scale or not.")]
	public bool ignoreTimescale;

	void Start () 
	{
		rend = GetComponent <Renderer> ();
	}

	void Update () 
	{
		if (ignoreTimescale == false)
		{
			// Update scroll amount scaled.
			CurrentOffset = new Vector2 (
				Time.deltaTime * ScrollSpeed.x,
				Time.deltaTime * ScrollSpeed.y
			);
		}
			
		if (ignoreTimescale == true)
		{
			// Update scroll amount unscaled.
			CurrentOffset = new Vector2 (
				Time.unscaledDeltaTime * ScrollSpeed.x,
				Time.unscaledDeltaTime * ScrollSpeed.y
			);
		}

		// Set new horizontal scroll amount.
		switch (HorizontalScrollMode) 
		{
		case scrollType.None:
			rend.material.SetTextureOffset (
				"_MainTex", 
				new Vector2 (
					rend.material.GetTextureOffset ("_MainTex").x, 
					rend.material.GetTextureOffset ("_MainTex").y
				)
			);
			break;
		case scrollType.Positive:
			rend.material.SetTextureOffset (
				"_MainTex", 
				new Vector2 (
					CurrentOffset.x, 
					rend.material.GetTextureOffset ("_MainTex").y
				)
			);
			break;
		case scrollType.Negative:
			rend.material.SetTextureOffset (
				"_MainTex", 
				new Vector2 (
					-CurrentOffset.x, 
					rend.material.GetTextureOffset ("_MainTex").y
				)
			);
			break;
		}

		// Set new vertical scroll amount.
		switch (VerticalScrollMode) 
		{
		case scrollType.None:
			rend.material.SetTextureOffset (
				"_MainTex", 
				new Vector2 (
					rend.material.GetTextureOffset ("_MainTex").x, 
					rend.material.GetTextureOffset ("_MainTex").y
				)
			);
			break;
		case scrollType.Positive:
			rend.material.SetTextureOffset (
				"_MainTex", 
				new Vector2 (
					rend.material.GetTextureOffset ("_MainTex").x, 
					CurrentOffset.y
				)
			);
			break;
		case scrollType.Negative:
			rend.material.SetTextureOffset (
				"_MainTex", 
				new Vector2 (
					rend.material.GetTextureOffset ("_MainTex").x, 
					-CurrentOffset.y
				)
			);
			break;
		}
	}
}