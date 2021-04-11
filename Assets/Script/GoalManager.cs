using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
	private List<Transform> goalBoxes = new List<Transform>();
	void Start()
	{
		foreach (GameObject goal in GameObject.FindGameObjectsWithTag("Goal"))
		{
			goalBoxes.Add(goal.transform);
		}
	}

	public void Check()
	{
		int number = 0;
		foreach (Transform goal in goalBoxes)
		{
			foreach (Collider obj in Physics.OverlapSphere(goal.position, 0.1f))
			{
				if (obj.CompareTag("Attached") || obj.CompareTag("Player"))
				{
					number++;
				}
			}
		}
		if (number == goalBoxes.Count)
			print("Win!");
	}
}
