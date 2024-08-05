using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node : MonoBehaviour
{
	private LevelManager levelManager;
	private float rotationSpeed;
	private bool mainNode = false;
	private int nodeType = 0;

	[Header("Node Default Active Settings")]
	[SerializeField] public bool top = false;
	[SerializeField] public bool right = false;
	[SerializeField] public bool bottom = false;
	[SerializeField] public bool left = false;

	[SerializeField] public int[] activeSides;
	[HideInInspector] public float rotationDiff;

	[SerializeField] private Block block;

	// Public Methods
	public int GetNodeType
	{
		get { return nodeType; }
		set { nodeType = value; }
	}

	public bool MainNode
	{
		get { return mainNode; }
	}

	public bool SetMainNode(bool value)
	{
		bool r = false;

		mainNode = value;
		/*
		if (mainNode)
		{
			GetComponent<SpriteRenderer>().sprite = levelManager.GetMainNodeSprite();
		}
		else
		{
			GetComponent<SpriteRenderer>().sprite = levelManager.GetDefaultNodeSprite();
		}*/

		return r;
	}

	public int[] ActiveSides()
	{
		return activeSides;
	}

	// Use this for initialization
	void Awake()
	{
		// Get Globals
		levelManager = LevelManager.Instance;

		// Settings
		rotationSpeed = levelManager.RotationSpeed();

		// Active Sides
		activeSides = new int[4];
		activeSides[0] = top ? 1 : 0;
		activeSides[1] = right ? 1 : 0;
		activeSides[2] = bottom ? 1 : 0;
		activeSides[3] = left ? 1 : 0;

		GetComponent<BoxCollider2D>().enabled = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (transform.root.eulerAngles.z != rotationDiff)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rotationDiff), rotationSpeed);
		}
	}

	void OnMouseDown()
	{
		int difference = -levelManager.CheckNode((int)transform.position.x, (int)transform.position.y);

		RotateNode();

		difference += levelManager.CheckNode((int)transform.position.x, (int)transform.position.y);

		levelManager.GetCurrentLevel().curLinkCount += difference;

		if (levelManager.GetCurrentLevel().curLinkCount == levelManager.GetCurrentLevel().totalLinks)
			StartCoroutine(AllNodesAreGoodToGo());

		block.Check();
	}

	private IEnumerator AllNodesAreGoodToGo()
	{
		foreach (Node n in levelManager.LevelSettings.nodes)
		{
			n.GetComponent<BoxCollider2D>().enabled = false;
		}

		yield return new WaitForSeconds(0.5f);
	}

	public void RotateNode()
	{
		rotationDiff += 90;

		if (rotationDiff == 360)
			rotationDiff = 0;

		RotateValues();
	}

	public void RotateValues()
	{
		int aux = activeSides[0];

		for (int i = 0; i < activeSides.Length - 1; i++)
		{
			activeSides[i] = activeSides[i + 1];
		}
		activeSides[3] = aux;
	}


}
