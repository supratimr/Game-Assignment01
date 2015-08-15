using UnityEngine;
using System.Collections;

public class FollowPath : MonoBehaviour
{
	public EaseType moveEase;
	public EaseType stopEase;
	public float moveDuration;
	public float stopTweenDuration;
	public float stopDuration;
	public Vector3 stopScale = Vector3.one;
	public Vector3[] points;
	public int startIndex;
	
	void Start()
	{
		StartCoroutine(MoveOnPath());
	}
	
	IEnumerator MoveOnPath()
	{
		int index = startIndex;
		transform.localPosition = points[index];
		while (true)
		{
			index = (index + 1) % points.Length;
			Vector3 localPoint = new Vector3 (points[index].x, transform.localPosition.y, points[index].z);
			transform.LookAt(localPoint);
			yield return StartCoroutine(transform.MoveTo(localPoint, moveDuration, moveEase));
			if (stopTweenDuration > 0)
				StartCoroutine(transform.ScaleFrom(stopScale, stopTweenDuration, stopEase));
			if (stopDuration > 0)
				yield return StartCoroutine(MoverCommon.Wait(stopDuration));
		}
	}
}
