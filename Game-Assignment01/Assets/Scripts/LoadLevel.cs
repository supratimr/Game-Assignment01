using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour 
{
	public void LoadTargetLevel(string inLevelName)
	{
		Application.LoadLevel(inLevelName);
	}
}
