using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor;

public class RotatePointController : MonoBehaviour
{
	Player player;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

	void OnTriggerStay(Collider collider)
	{
		//RotatePointCollider parent control and is it already added?
		if (!collider.transform.parent.parent.CompareTag("Unattached") && !player.currentPoints.Contains(collider.transform))
		{
			//Parent must be on surface
			foreach (Collider obj in Physics.OverlapSphere(collider.transform.parent.parent.transform.position, 0.3f))
			{
				if (obj.CompareTag("Surface"))
					player.currentPoints.Add(collider.transform);
			}
		}
	}

	void OnTriggerExit(Collider collider)
	{
		player.currentPoints.Remove(collider.transform);
	}

	#region PrintPoints
	public void PrintPoints()
	{
		foreach (Transform point in player.currentPoints)
		{
			print(point.name);
		}
	}
	#endregion
}