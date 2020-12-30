using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
	[SerializeField] float delay = 100;
	[SerializeField] float movement = 0.1f;
	[SerializeField] float tilt = 30;

	public void Move(Vector2 value)
	{
		transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * -value.y * movement + Vector3.right * value.x * movement, Time.deltaTime * delay);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(value.y * tilt, 0, value.x * -tilt), delay);
	}
}
