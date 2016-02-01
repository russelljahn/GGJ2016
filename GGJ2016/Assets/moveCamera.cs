using UnityEngine;
using System.Collections;

public class moveCamera : MonoBehaviour 
{

	public Transform Catty;
	public float Speed;


	void FixedUpdate () 
	{
		Vector3 TargetPos = new Vector3 (Catty.position.x,Catty.position.y,transform.position.z);
		transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * Speed);
	}
}
