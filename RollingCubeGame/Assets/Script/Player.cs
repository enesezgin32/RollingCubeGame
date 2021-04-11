using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	#region Variables

	[Header("**Roll Details**")]
	public float step = 9;
	public float speed = 0.02f;

	private bool canPress = true;
	private bool canAttach = true;

	[HideInInspector]
	public List<GameObject> objectToAttach = new List<GameObject>();

	// Roll Control
	private List<Transform> boxes = new List<Transform>();
	private bool canRollRU = true;
	private bool canRollLU = true;
	private bool canRollRD = true;
	private bool canRollLD = true;

	[HideInInspector]
	public List<Transform> currentPoints = new List<Transform>();

	Transform rightUpPoint;
	Transform leftUpPoint;
	Transform rightDownPoint;
	Transform leftDownPoint;

	GoalManager goal;
	#endregion

	void Start()
	{
		//Get List of boxes and Player
		foreach (GameObject box in GameObject.FindGameObjectsWithTag("Unattached"))
		{
			boxes.Add(box.transform);
		}
		foreach (GameObject box in GameObject.FindGameObjectsWithTag("Player"))
		{
			boxes.Add(box.transform);
		}

		//Find Goal Manager
		goal = GameObject.FindGameObjectWithTag("GoalManager").GetComponent<GoalManager>();
	}

	void Update()
	{
		NewPoints();
		if (canAttach)
		{
			StartCoroutine(Attach());
		}
		//Controls
		if (canPress)
		{
			RollControl();
			if (Input.GetKey("q") && canRollLU)
			{
				NewPoints();
				canPress = false;
				StartCoroutine(Roll(leftUpPoint.position, Vector3.right, -1));
			}
			else if (Input.GetKey("w") && canRollRU)
			{
				NewPoints();
				canPress = false;
				StartCoroutine(Roll(rightUpPoint.position, Vector3.forward, 1));
			}
			else if (Input.GetKey("s") && canRollRD)
			{
				NewPoints();
				canPress = false;
				StartCoroutine(Roll(rightDownPoint.position, Vector3.right, 1));

			}
			else if (Input.GetKey("a") && canRollLD)
			{
				NewPoints();
				canPress = false;
				StartCoroutine(Roll(leftDownPoint.position, Vector3.forward, -1));
			}
		}
	}

	IEnumerator Roll(Vector3 rotatePoint, Vector3 axis, int way)
	{
		canAttach = false;
		for (int i = 0; i < 90/step; i++)
		{
			transform.RotateAround(rotatePoint, axis, way * step);
			if (!(i + 1 < 90/step))
			{
				canAttach = true;
			}
			yield return new WaitForSeconds(speed);
		}

		// Wait for end of OnTriggerStay that is in RotatePointController
		yield return new WaitForFixedUpdate();
		goal.Check();
		canPress = true;
	}

	IEnumerator Attach()
	{
		//Attach Object info comes from Attacher 
		if (objectToAttach.Count != 0)
		{
			foreach (GameObject obj in objectToAttach)
			{
				//Tag Change
				obj.tag = "Attached";

				//Attach
				obj.transform.parent = transform;

				//Recalculate rotatepoint
				NewPoints();
			}
		}
		objectToAttach.Clear();
		yield return null;
	}

	void NewPoints()
	{
		Vector3 maxX;
		Vector3 minX;
		Vector3 maxZ;
		Vector3 minZ;
		maxX.x = float.MinValue;
		minX.x = float.MaxValue;
		maxZ.z = float.MinValue;
		minZ.z = float.MaxValue;

		//Find Bounds
		foreach (Transform point in currentPoints)
		{
			if (point.position.x > maxX.x)
			{
				maxX = point.position;
				leftDownPoint = point;
			}
			if (point.position.x < minX.x)
			{
				minX = point.position;
				rightUpPoint = point;
			}
			if (point.position.z > maxZ.z)
			{
				maxZ = point.position;
				rightDownPoint = point;
			}
			if (point.position.z < minZ.z)
			{
				minZ = point.position;
				leftUpPoint = point;
			}
		}
	}

	void RollControl()
	{
		// Limitation bools reset
		canRollRU = true;
		canRollLU = true;
		canRollRD = true;
		canRollLD = true;
		int _LUtemp = 0;
		int _RUtemp = 0;
		int _RDtemp = 0;
		int _LDtemp = 0;

		int checkingAngle;

		//Controlling player and every box
		foreach (Transform obj in boxes)
		{
			//Controlling whether that box attached or not
			if (obj.CompareTag("Player") || obj.CompareTag("Attached"))
			{
				//Positions relative to roll point
				Vector3 RUP = obj.position - rightUpPoint.position;
				Vector3 RDP = obj.position - rightDownPoint.position;
				Vector3 LUP = obj.position - leftUpPoint.position;
				Vector3 LDP = obj.position - leftDownPoint.position;
				Collider[] obstacles;
				if (canRollLU)
				{
					checkingAngle = 15;
					while (checkingAngle <= 90)
					{
						obstacles = Physics.OverlapSphere(Quaternion.AngleAxis(-checkingAngle, Vector3.right) * LUP + leftUpPoint.position, 0.45f);

						//Checking new rolling positions
						foreach (var obs in obstacles)
						{
							//does it collide
							if (obs.CompareTag("Plane") || obs.CompareTag("Unattached"))
							{
								canRollLU = false;
							}

							//Whether any of them in surface after rolling or not
							if(checkingAngle == 90 && obs.CompareTag("Surface"))
								_LUtemp++;
						}
						//New angle
						checkingAngle += 15;
					}
				}

				if (canRollRU)
				{
					checkingAngle = 15;
					while (checkingAngle <= 90)
					{
						obstacles = Physics.OverlapSphere(Quaternion.AngleAxis(checkingAngle, Vector3.forward) * RUP + rightUpPoint.position, 0.45f);

						//Checking new rolling positions
						foreach (var obs in obstacles)
						{
							//does it collide
							if (obs.CompareTag("Plane") || obs.CompareTag("Unattached"))
							{
								canRollRU = false;
							}

							//Whether any of them in surface after rolling or not
							if (checkingAngle == 90 && obs.CompareTag("Surface"))
								_RUtemp++;
						}
						//New angle
						checkingAngle += 15;
					}
				}

				if (canRollRD)
				{
					checkingAngle = 15;
					while (checkingAngle <= 90)
					{
						obstacles = Physics.OverlapSphere(Quaternion.AngleAxis(checkingAngle, Vector3.right) * RDP + rightDownPoint.position, 0.45f);

						//Checking new rolling positions
						foreach (var obs in obstacles)
						{
							//does it collide
							if (obs.CompareTag("Plane") || obs.CompareTag("Unattached"))
							{
								canRollRD = false;
							}

							//Whether any of them in surface after rolling or not
							if (checkingAngle == 90 && obs.CompareTag("Surface"))
								_RDtemp++;
						}
						//New angle
						checkingAngle += 15;
					}
				}

				if (canRollLD)
				{
					checkingAngle = 15;
					while (checkingAngle <= 90)
					{
						obstacles = Physics.OverlapSphere(Quaternion.AngleAxis(-checkingAngle, Vector3.forward) * LDP + leftDownPoint.position, 0.45f);

						//Checking new rolling positions
						foreach (var obs in obstacles)
						{
							//does it collide
							if (obs.CompareTag("Plane") || obs.CompareTag("Unattached"))
							{
								canRollLD = false;
							}

							//Whether any of them in surface after rolling or not
							if (checkingAngle == 90 && obs.CompareTag("Surface"))
								_LDtemp++;
						}
						//New angle
						checkingAngle += 15;
					}
				}
				/*
				Gizmos.DrawSphere(Quaternion.AngleAxis(-90, Vector3.right) * LUP + leftUpPoint.position, 0.3f);
				Gizmos.DrawSphere(Quaternion.AngleAxis(90, Vector3.forward) * RUP + rightUpPoint.position, 0.3f);
				Gizmos.DrawSphere(Quaternion.AngleAxis(90, Vector3.right) * RDP + rightDownPoint.position, 0.3f);
				Gizmos.DrawSphere(Quaternion.AngleAxis(-90, Vector3.forward) * LDP + leftDownPoint.position, 0.3f);*/
			}
		}
		canRollLU = _LUtemp > 0 ? canRollLU : false;
		canRollLD = _LDtemp > 0 ? canRollLD : false;
		canRollRD = _RDtemp > 0 ? canRollRD : false;
		canRollRU = _RUtemp > 0 ? canRollRU : false;
	}
}
