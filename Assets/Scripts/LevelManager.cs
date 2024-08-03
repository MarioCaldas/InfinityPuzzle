using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager>
{
	[Header("Objects Settings")]
	[SerializeField] private float rotationSpeed;
	[SerializeField] private GameObject[] nodesPrefabs;

	[Header("Setup Level")]
	[SerializeField] private bool BuildRandomLevel;
	[SerializeField] private Level levelSettings;
	public Level LevelSettings
	{
		get { return levelSettings; }
		set { levelSettings = value; }
	}
	[SerializeField] private int maxEndLineNodes;
	[HideInInspector] public bool playAgain = false;

	private bool hasMainNode;

	// Use this for initialization
	protected override void Awake()
	{
		base.Awake();

		BuildLevel();
		levelSettings.totalLinks = GetLinksRequired();
		RotateNodes();
		levelSettings.curLinkCount = CheckNodes();
		CameraFocus();
	}

	public void CameraFocus()
	{
		Camera.main.transform.position = new Vector3((levelSettings.width / 2) - 0.2f, (levelSettings.height / 2) - 0.5f, -3);
	}

	private void BuildLevel()
	{
		levelSettings.nodes = new Node[levelSettings.width, levelSettings.height];

		int[] auxSides = { 0, 0, 0, 0 };
		hasMainNode = false;

		for (int h = 0; h < levelSettings.height; h++)
		{
			for (int w = 0; w < levelSettings.width; w++)
			{
				// Set Limits
				auxSides = new int[4];

				if (w == 0)
					auxSides[3] = 0;
				else
					auxSides[3] = levelSettings.nodes[w - 1, h].ActiveSides()[1];

				if (w == levelSettings.width - 1)
					auxSides[1] = 0;
				else
					auxSides[1] = UnityEngine.Random.Range(0, 2);

				if (h == 0)
					auxSides[2] = 0;
				else
					auxSides[2] = levelSettings.nodes[w, h - 1].ActiveSides()[0];

				if (h == levelSettings.height - 1)
					auxSides[0] = 0;
				else
					auxSides[0] = UnityEngine.Random.Range(0, 2);

				// Return the Node Type
				int nodeType = auxSides[0] + auxSides[1] + auxSides[2] + auxSides[3];
				if (nodeType == 2 && auxSides[0] != auxSides[2])
					nodeType = 5;


				// Instantiate Node and set rotation
				GameObject newNode = Instantiate(nodesPrefabs[nodeType], new Vector3(w, h, 0), Quaternion.identity);
				newNode.GetComponent<Node>().GetNodeType = nodeType;

				newNode.GetComponent<Node>().newNodeType = nodeType;

				while (newNode.GetComponent<Node>().ActiveSides()[0] != auxSides[0] || newNode.GetComponent<Node>().ActiveSides()[1] != auxSides[1]
					|| newNode.GetComponent<Node>().ActiveSides()[2] != auxSides[2] || newNode.GetComponent<Node>().ActiveSides()[3] != auxSides[3])
				{
					newNode.GetComponent<Node>().RotateNode();
				}

				// Set Main Node
				int newNodeType = newNode.GetComponent<Node>().ActiveSides()[0]
						+ newNode.GetComponent<Node>().ActiveSides()[1]
						+ newNode.GetComponent<Node>().ActiveSides()[2]
						+ newNode.GetComponent<Node>().ActiveSides()[3];

				if (!hasMainNode && newNodeType == 1)
				{
					newNode.GetComponent<Node>().SetMainNode(true);
					hasMainNode = true;
				}

				levelSettings.nodes[w, h] = newNode.GetComponent<Node>();
			}
		}

		CameraFocus();
	}


	private int GetLinksRequired()
	{
		int linkCount = 0;
		foreach (var piece in levelSettings.nodes)
		{
			foreach (var j in piece.ActiveSides())
			{
				linkCount += j;
			}
		}

		linkCount /= 2;

		return linkCount;
	}

	private void RotateNodes()
	{
		foreach (var piece in levelSettings.nodes)
		{
			int s = UnityEngine.Random.Range(0, 4);

			for (int i = 0; i < s; i++)
			{
				piece.RotateNode();
			}
		}
	}

	private Vector2 CheckDimensions()
	{
		Vector2 aux = Vector2.zero;

		GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");

		foreach (var p in nodes)
		{
			if (p.transform.position.x > aux.x)
				aux.x = p.transform.position.x;

			if (p.transform.position.y > aux.y)
				aux.y = p.transform.position.y;
		}

		aux.x++;
		aux.y++;

		return aux;
	}

	public float RotationSpeed()
	{
		return rotationSpeed;
	}

	public Level GetCurrentLevel()
	{
		return levelSettings;
	}


	public int CheckNodes()
	{
		int value = 0;

		for (int h = 0; h < levelSettings.height; h++)
		{
			for (int w = 0; w < levelSettings.width; w++)
			{
				if (h != levelSettings.height - 1)
				{
					if (levelSettings.nodes[w, h].ActiveSides()[0] == 1 && levelSettings.nodes[w, h + 1].ActiveSides()[2] == 1)
					{
						value++;
					}
				}

				{
					if (w != levelSettings.width - 1)
						if (levelSettings.nodes[w, h].ActiveSides()[1] == 1 && levelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
						{
							value++;
						}
				}
			}
		}

		return value;
	}

	public int CheckNode(int w, int h)
	{
		int value = 0;

		// Check Sides Clock Wise
		if (h != levelSettings.height - 1)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[0] == 1 && levelSettings.nodes[w, h + 1].ActiveSides()[2] == 1)
			{
				value++;
			}
		}

		if (w != levelSettings.width - 1)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[1] == 1 && levelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
			{
				value++;
			}
		}

		if (w != 0)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[3] == 1 && levelSettings.nodes[w - 1, h].ActiveSides()[1] == 1)
			{
				value++;
			}
		}

		if (h != 0)
		{
			if (levelSettings.nodes[w, h].ActiveSides()[2] == 1 && levelSettings.nodes[w, h - 1].ActiveSides()[0] == 1)
			{
				value++;
			}
		}

		return value;
	}


}

[Serializable]
public class Level
{
	public int totalLinks;
	public int curLinkCount;

	public int width;
	public int height;
	public Node[,] nodes;
}
