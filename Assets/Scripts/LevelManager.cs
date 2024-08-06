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
	[SerializeField] private bool buildRandomLevel;
	[SerializeField] private Level levelSettings;
	public Level LevelSettings
	{
		get { return levelSettings; }
		set { levelSettings = value; }
	}
	[SerializeField] private int maxEndLineNodes;
	[HideInInspector] public bool playAgain = false;

	private bool hasMainNode;

	public List<GameObject> blockSequence = new List<GameObject>();
	public List<GameObject> allBlocks = new List<GameObject>();

	private GameManager gameManager;

	private bool playingRandom = false;

	 void Awake()
	{

		BuildLevel();
		levelSettings.totalLinks = GetLinksRequired();
		RotateNodes();
		levelSettings.curLinkCount = CheckNodes();

		gameManager = GameManager.Instance;

		UIManager.Instance.ShowEndLevel(false);
	}

	
	private void BuildLevel()
	{
		blockSequence.Clear();
		allBlocks.Clear();

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

				allBlocks.Add(newNode);
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

	public void PlaySqIvyAnim()
	{
		for (int i = 0; i < blockSequence.Count; i++)
		{
			blockSequence[i].transform.GetChild(0).GetComponent<Block>().StartIvyIncrease();
		}
	}

	// Save and Load Level Methods
	public void BuildRandomLevel()
	{
		playingRandom = true;

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
		{
			Destroy(obj);
		}

		levelSettings.nodes = new Node[levelSettings.width, levelSettings.height];
		playAgain = true;

		SetNewLevelSize();
		BuildLevel();

		levelSettings.totalLinks = GetLinksRequired();
		RotateNodes();
		levelSettings.curLinkCount = CheckNodes();

		UIManager.Instance.ShowEndLevel(false);

	}

	public void SetNewLevelSize()
	{
		levelSettings.width = UnityEngine.Random.Range(2, 7);
		levelSettings.height = UnityEngine.Random.Range(3, 13);
	}

	public void SaveLevelToObject()
	{
		List<PseudoNode> nodes = new List<PseudoNode>();

		foreach (Node n in levelSettings.nodes)
		{
			PseudoNode p = new PseudoNode();
			p.w = (int)n.transform.position.x;
			p.h = (int)n.transform.position.y;
			p.nodeType = n.GetNodeType;
			p.mainNode = n.MainNode;
			p.activeSides = n.ActiveSides();
			p.top = n.top;
			p.right = n.right;
			p.bottom = n.bottom;
			p.left = n.left;
			p.activeSides = n.activeSides;
			p.rotationDiff = n.rotationDiff;

			nodes.Add(p);
		}

		gameManager.SavedLevels.savedLevels.Add(CreateNewLevel(levelSettings.totalLinks, levelSettings.width, levelSettings.height, nodes));
		EditorUtility.SetDirty(gameManager.SavedLevels);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

	}
	
	public void LoadThisLevel(int index)
	{
		blockSequence.Clear();
		allBlocks.Clear();
		gameManager.PlayingLevel = index;
		playingRandom = false;

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
		{
			Destroy(obj);
		}

		Level newLevel = new Level();
		newLevel.totalLinks = gameManager.SavedLevels.savedLevels[index].totalLinks;
		newLevel.curLinkCount = gameManager.SavedLevels.savedLevels[index].curLinkCount;
		newLevel.width = gameManager.SavedLevels.savedLevels[index].width;
		newLevel.height = gameManager.SavedLevels.savedLevels[index].height;
		newLevel.nodes = new Node[newLevel.width, newLevel.height];


		foreach (PseudoNode p in gameManager.SavedLevels.savedLevels[index].nodes)
		{
			GameObject newNode = Instantiate(nodesPrefabs[p.nodeType], new Vector3(p.w, p.h, 0), Quaternion.identity);
			Node n = newNode.GetComponent<Node>();

			n.SetMainNode(p.mainNode);
			n.top = p.top;
			n.right = p.right;
			n.bottom = p.bottom;
			n.left = p.left;

			// Return the Node Type
			int nodeType = n.ActiveSides()[0] + n.ActiveSides()[1] + n.ActiveSides()[2] + n.ActiveSides()[3];
			if (nodeType == 2 && n.ActiveSides()[0] != n.ActiveSides()[2])
				nodeType = 5;


			newNode.GetComponent<SpriteRenderer>().sprite = nodesPrefabs[nodeType].GetComponent<SpriteRenderer>().sprite;
			newLevel.nodes[p.w, p.h] = n;

			allBlocks.Add(newNode);

		}


		playAgain = true;
		levelSettings = newLevel;

		levelSettings.totalLinks = GetLinksRequired();
		RotateNodes();
		CameraFocus();
		levelSettings.curLinkCount = CheckNodes();

	}

	public void RemoveLastSavedLevel()
	{
		gameManager.SavedLevels.savedLevels.RemoveAt(gameManager.SavedLevels.savedLevels.Count - 1);

		EditorUtility.SetDirty(gameManager.SavedLevels);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	public void CleanSavedLevelList()
	{

		gameManager.SavedLevels.savedLevels.Clear();
		EditorUtility.SetDirty(gameManager.SavedLevels);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

	}

	public PseudoLevel CreateNewLevel(int t, int w, int h, List<PseudoNode> n)
	{
		PseudoLevel newLvl = new PseudoLevel();

		newLvl.totalLinks = t;
		newLvl.curLinkCount = 0;
		newLvl.width = w;
		newLvl.height = h;
		newLvl.nodes = n;

		return newLvl;
	}

	//Camera adjustments
	public void CameraFocus()
	{
		Vector3 levelCenterPos = CalculateCenter(allBlocks);
		Camera.main.transform.position = levelCenterPos;
		AdjustCameraPosition(levelCenterPos, allBlocks);

	}
	Vector3 CalculateCenter(List<GameObject> objects)
	{
		if (objects.Count <= 0)
		{
			return Vector3.zero;
		}
		Vector3 sum = Vector3.zero;

		foreach (GameObject obj in objects)
		{
			sum += obj.transform.position;
		}

		Vector3 center = sum / objects.Count;
		return center;
	}
	void AdjustCameraPosition(Vector3 center, List<GameObject> objects)
	{
		if (objects.Count <= 0)
		{
			return;
		}

		Bounds bounds = new Bounds(objects[0].transform.position, Vector3.zero);
		foreach (GameObject obj in objects)
		{
			bounds.Encapsulate(obj.transform.position);
		}

		// Adjust the camera's z position based on the bounds
		float levelWidth = bounds.size.x;
		float levelHeight = bounds.size.y;

		// Adjust the camera distance
		float aspectRatio = (float)Screen.width / Screen.height;
		float cameraSize = Mathf.Max(levelWidth / aspectRatio, levelHeight) / 2;


		float fov = Camera.main.fieldOfView;
		float distance = cameraSize / Mathf.Tan(fov * 0.3f * Mathf.Deg2Rad);
		Camera.main.transform.position = new Vector3(center.x, center.y, -distance);

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
