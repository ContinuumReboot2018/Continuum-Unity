﻿using UnityEngine;

[CreateAssetMenu(fileName = "New Block Type", menuName = "Block Type")]
public class BlockManager : ScriptableObject 
{
	[Tooltip ("Default speed for this block type.")]
	public float Speed;
	[Tooltip ("Default material for this block type.")]
	public Material Material;
	[Tooltip ("Default base point value for this block type.")]
	public int BasePointValue;
	[Tooltip ("Default explosion for this block type.")]
	public GameObject Explosion;
	[Tooltip ("Default text color for this block type.")]
	public Color TextColor;
}