using UnityEngine;

[CreateAssetMenu(fileName = "New Time Scale Preset", menuName = "Modifiers/Time Scale preset", order = 2)]
public class TimescalePreset : ScriptableObject
{
	[Multiline]
	public string description = "Time scale preset description";
	public float TimeScaleMult = 0.07f;
	public float TimeScaleAdd = 0.3f;
	public float TimeScaleIncreaseRate = 0.001f;
	public float TimeScaleSmoothing = 6;
}