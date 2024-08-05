using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	[SerializeField] private LevelManager levelManager;

	public GameObject ivyLeafsObject;
	public GameObject ivyStickObject;

	public List<GameObject> neighborsBlocks = new List<GameObject>();


	void Start()
	{
		levelManager = LevelManager.Instance;

		CheckNode((int)transform.parent.transform.position.x, (int)transform.parent.transform.position.y);

		PlayIvysSequence();
	}

	public void StartIvyIncrease()
	{
		ivyLeafsObject.GetComponent<IvyController>().PlayIvyAnim(true);
		ivyStickObject.GetComponent<IvyController>().PlayIvyAnim(true);
	}

	public void Check()
	{

		DestroyIvy();

		CheckNode((int)transform.parent.transform.position.x, (int)transform.parent.transform.position.y);

		CheckExNode((int)transform.parent.transform.position.x, (int)transform.parent.transform.position.y);


		PlayIvysSequence();


	}


	public void CheckNeighbors()
	{
		for (int i = 0; i < neighborsBlocks.Count; i++)
		{
			neighborsBlocks[i].transform.GetChild(0).GetComponent<Block>().Check();

		}
	}

	public void CheckExNode(int w, int h)
	{

		if (h != levelManager.LevelSettings.height - 1)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[0] == 0 && levelManager.LevelSettings.nodes[w, h + 1].ActiveSides()[2] == 1)
			{

				neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h + 1].gameObject);

				levelManager.LevelSettings.nodes[w, h + 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h].gameObject);
			}
		}

		if (w != levelManager.LevelSettings.width - 1)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[1] == 0 && levelManager.LevelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
			{

				neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w + 1, h].gameObject);

				levelManager.LevelSettings.nodes[w + 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h].gameObject);

			}
		}

		if (w != 0)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[3] == 0 && levelManager.LevelSettings.nodes[w - 1, h].ActiveSides()[1] == 1)
			{

				neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w - 1, h].gameObject);

				levelManager.LevelSettings.nodes[w - 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h].gameObject);

			}
		}

		if (h != 0)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[2] == 0 && levelManager.LevelSettings.nodes[w, h - 1].ActiveSides()[0] == 1)
			{

				neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h - 1].gameObject);

				levelManager.LevelSettings.nodes[w, h - 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Remove(levelManager.LevelSettings.nodes[w, h].gameObject);

			}
		}

	}


	void PlayIvysSequence()
	{

		levelManager.PlaySqIvyAnim();
		/*
		for (int i = 0; i < neighborsBlocks.Count; i++)
        {
			neighborsBlocks[i].transform.GetChild(0).GetComponent<Block>().StartIvyIncrease();
		
		}*/
	}

	void PlayIvyAnim()
	{
		ivyLeafsObject.GetComponent<IvyController>().PlayIvyAnim(true);
	}

	public void CheckNode(int w, int h)
	{
		// Check Sides Clock Wise
		if (h != levelManager.LevelSettings.height - 1)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[0] == 1 && levelManager.LevelSettings.nodes[w, h + 1].ActiveSides()[2] == 1)
			{

				if (levelManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
				{

					levelManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

				}
				if (levelManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h + 1].gameObject) == false)
				{

					levelManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h + 1].gameObject);

				}


				if (neighborsBlocks.Contains((levelManager.LevelSettings.nodes[w, h + 1].gameObject)) == false)
					neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h + 1].gameObject);

				if (levelManager.LevelSettings.nodes[w, h + 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
					levelManager.LevelSettings.nodes[w, h + 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

			}

		}

		if (w != levelManager.LevelSettings.width - 1)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[1] == 1 && levelManager.LevelSettings.nodes[w + 1, h].ActiveSides()[3] == 1)
			{

				if (levelManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
				{
					levelManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h].gameObject);
				}
				if (levelManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w + 1, h].gameObject) == false)
				{
					levelManager.blockSequence.Add(levelManager.LevelSettings.nodes[w + 1, h].gameObject);
				}

				if (neighborsBlocks.Contains((levelManager.LevelSettings.nodes[w + 1, h].gameObject)) == false)
					neighborsBlocks.Add(levelManager.LevelSettings.nodes[w + 1, h].gameObject);

				if (levelManager.LevelSettings.nodes[w + 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
					levelManager.LevelSettings.nodes[w + 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

			}

		}

		if (w != 0)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[3] == 1 && levelManager.LevelSettings.nodes[w - 1, h].ActiveSides()[1] == 1)
			{

				if (levelManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
				{
					levelManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

				}
				if (levelManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w - 1, h].gameObject) == false)
				{
					levelManager.blockSequence.Add(levelManager.LevelSettings.nodes[w - 1, h].gameObject);

				}


				if (neighborsBlocks.Contains((levelManager.LevelSettings.nodes[w - 1, h].gameObject)) == false)
					neighborsBlocks.Add(levelManager.LevelSettings.nodes[w - 1, h].gameObject);

				if (levelManager.LevelSettings.nodes[w - 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
					levelManager.LevelSettings.nodes[w - 1, h].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

			}

		}

		if (h != 0)
		{
			if (levelManager.LevelSettings.nodes[w, h].ActiveSides()[2] == 1 && levelManager.LevelSettings.nodes[w, h - 1].ActiveSides()[0] == 1)
			{
				if (levelManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
				{
					levelManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h].gameObject);
				}
				if (levelManager.blockSequence.Contains(levelManager.LevelSettings.nodes[w, h - 1].gameObject) == false)
				{
					levelManager.blockSequence.Add(levelManager.LevelSettings.nodes[w, h - 1].gameObject);

				}


				if (neighborsBlocks.Contains((levelManager.LevelSettings.nodes[w, h - 1].gameObject)) == false)
					neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h - 1].gameObject);

				if (levelManager.LevelSettings.nodes[w, h - 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Contains(levelManager.LevelSettings.nodes[w, h].gameObject) == false)
					levelManager.LevelSettings.nodes[w, h - 1].transform.GetChild(0).GetComponent<Block>().neighborsBlocks.Add(levelManager.LevelSettings.nodes[w, h].gameObject);

			}


		}

	}

	private void DestroyIvy()
	{
		ivyLeafsObject.GetComponent<IvyController>().PlayParticles();

		ivyLeafsObject.GetComponent<IvyController>().ResetVars();

		//ivyStickObject.GetComponent<IvyController>().PlayParticles();
		ivyStickObject.GetComponent<IvyController>().ResetVars();

		levelManager.blockSequence.Remove(transform.parent.gameObject);

	}

	public void ResetVars()
	{
		for (int i = 0; i < neighborsBlocks.Count; i++)
		{
			neighborsBlocks.RemoveAt(i);

		}
	}
}
