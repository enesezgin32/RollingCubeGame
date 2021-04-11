using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacher : MonoBehaviour
{
	Player player;

	public float radius = 0.55f;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	private void Update()
	{
		foreach (Collider collider in Physics.OverlapSphere(transform.position, radius))
		{
			if (collider.CompareTag("Attached") || collider.CompareTag("Player"))
			{
				player.objectToAttach.Add(gameObject);
				//Destroy(GetComponent<Attacher>());
			}
		}
	}
}
