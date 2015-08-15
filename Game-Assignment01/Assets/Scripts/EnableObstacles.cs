using UnityEngine;
using System.Collections;


public class EnableObstacles : MonoBehaviour 
{
	public ObjectMover[] _Obstacles;

	private void OnPress(GameObject inObject)
	{
		if(_Obstacles == null  || _Obstacles.Length <= 0)
			return;

		for(int Idx =0; Idx < _Obstacles.Length; Idx++)
			_Obstacles[Idx].enabled = true;
	}

	private void OnClick(GameObject inObject)
	{
		if(_Obstacles == null  || _Obstacles.Length <= 0)
			return;

		for(int Idx =0; Idx < _Obstacles.Length; Idx++)
			_Obstacles[Idx].enabled = false;
	}	
}
