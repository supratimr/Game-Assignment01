using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DiscBehaviour : MonoBehaviour
{
	public ObjectMover[] _Obstacles;
	public bool _DragAllowed = true;
	public Button _GameEndBtn;
	
	public float JumpValue = 2.0f;
	public float JumpDuration = 2.0f;

	public float _MoveSpeedMultiplier = 10.0f;

	private float mCurrentY;
	private float mTargetY;
	private float mJumpInterval;

	private bool mStartJump;
	private bool mMoveDown;

	private Vector3 mCachedMousePos;

	private void OnClick(GameObject inObject)
	{
		if(!mStartJump && !mMoveDown)
		{
			mCurrentY = transform.position.y;
			mTargetY = mCurrentY + JumpValue;
			mJumpInterval =  (JumpValue/(JumpDuration/2.0f)) * Time.deltaTime;

			mStartJump = true;
		}
	}

	private void OnDrag(GameObject inObject)
	{
		if(!_DragAllowed)
			return;

		if(Input.mousePosition != mCachedMousePos)
		{
			Vector3 mouseMoved = Input.mousePosition - mCachedMousePos;
			MoveObject(mouseMoved);
			mCachedMousePos = Input.mousePosition;
		}
	}

	private void OnPress(GameObject inObject)
	{
		if(!_DragAllowed)
			return;

		mCachedMousePos = Input.mousePosition;
	}

	void Update()
	{
		if(mStartJump && !mMoveDown)
		{
			float currentY = transform.position.y + mJumpInterval;
			transform.position = new Vector3(transform.position.x, currentY, transform.position.z);

			if(transform.position.y >= mTargetY)
			{
				mStartJump = false;
				mMoveDown = true;
			}
		}

		if(!mStartJump && mMoveDown)
		{
			float currentY = transform.position.y - mJumpInterval;
			transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
			
			if(transform.position.y <= mCurrentY)
			{
				transform.position = new Vector3(transform.position.x, mCurrentY, transform.position.z);
				mMoveDown = false;
			}
		}
	}

	private void MoveObject(Vector3 inDelta)
	{
		float xValue = transform.position.x + (inDelta.x * Time.deltaTime * _MoveSpeedMultiplier);
		float zValue = transform.position.z + (inDelta.y* Time.deltaTime * _MoveSpeedMultiplier);
		transform.position = new Vector3(xValue, transform.position.y, zValue);
	}

	void OnTriggerEnter(Collider other)
	{
		for(int Idx = 0; Idx < _Obstacles.Length; Idx++)
		{
			if(other.name == _Obstacles[Idx].name && !_GameEndBtn.gameObject.activeSelf )
			{
				//Time.timeScale = 0.0f;
				_GameEndBtn.gameObject.SetActive(true);
			}
		}
	}
}
