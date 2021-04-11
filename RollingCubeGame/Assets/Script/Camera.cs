using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
	[Header("**Move Details**")]
	public int step = 9;
	public float speed = 0.02f;
	public float waitBetweenMoves = 0.02f;

	private bool canPress = true;

	void Update()
	{
		if (canPress)
		{
			if (Input.GetKey(KeyCode.RightArrow))
			{
				StartCoroutine(Roll(Vector3.zero, Vector3.up, -1));
				canPress = false;
			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				StartCoroutine(Roll(Vector3.zero, Vector3.up, 1));
				canPress = false;
			}
		}
	}

	IEnumerator Roll(Vector3 rotatePoint, Vector3 axis, int way)
	{
		for (int i = 0; i < 90 / step; i++)
		{
			transform.RotateAround(rotatePoint, axis, way * step);
			yield return new WaitForSeconds(speed);
		}

		yield return new WaitForSeconds(waitBetweenMoves);
		canPress = true;
	}
}
