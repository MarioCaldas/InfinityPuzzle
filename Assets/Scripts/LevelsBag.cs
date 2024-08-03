using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Prototype/LevelsBag ", order = 1)]
public class LevelsBag : ScriptableObject
{
	[Header("Select Difficulty")]
	public BagDifficulty bagDifficulty;

	[Header("Levels Saved")]
	public List<PseudoLevel> savedLevels;
}

[Serializable]
public enum BagDifficulty
{
	easy = 0,
	medium = 1,
	hard = 2,
	hardcore = 3,
	insane = 4
}

[Serializable]
public struct PseudoLevel
{
	public int totalLinks;
	public int curLinkCount;

	public int width;
	public int height;
	public List<PseudoNode> nodes;
}

[Serializable]
public struct PseudoNode
{
	public int w;
	public int h;
	public int nodeType;

	public bool mainNode;

	public bool top;
	public bool right;
	public bool bottom;
	public bool left;

	public int[] activeSides;
	public float rotationDiff;
}