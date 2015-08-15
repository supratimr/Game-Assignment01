// -------------------------------------------------------------------------------
// Attach this script to an object in the scene to have it clickable
// -------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class ClickableObject : MonoBehaviour
{
    public delegate void ActivatedEventHandler(GameObject go);

    public bool _Draw = false;
	public bool _Active = true;
	public bool _UseGlobalActive = true;

	public string _StartMarker = "";

	public GameObject _ActivateObject = null;
	public GameObject _MessageObject = null;
	public const string _HighlightShader = "";
	public Material _HighlightMaterial = null;

	public float _Range = 0.0f;
	public float _RangeAngle = 20.0f;
	public Vector3 _RangeOffset = new Vector3(0, 0, 0);
	public Vector3 _Offset = new Vector3(0, 0, 0);


	public bool _RolloverStopOnMouseExit = true;
	public float _RolloverReplayDelay;
	public float _RolloverTime = 1.0f;
	public bool _FirstOnly = false;

	protected bool mMouseOver = false;
	public bool pMouseOver { get { return mMouseOver; } }

	private bool mMousePressed = false;
	public bool pMousePressed { get { return mMousePressed; } }

	protected Shader[][] mShaders = null;
	private static bool mGlobalActive = true;
	private float mRolloverTime = -1.0f;
	private float mRolloverReplayDelayTimer = 0;
	protected bool mHighlighted = false;
#if UNITY_IOS || UNITY_ANDROID
	protected bool mLineOfSightHighlight = false;
#endif
	private static event ActivatedEventHandler OnActivated;

	private static GameObject mMouseOverObject = null;
	public static GameObject pMouseOverObject
	{
		get { return mMouseOverObject; }
		set { mMouseOverObject = value; }
	}

	public static bool pGlobalActive
	{
		get { return mGlobalActive; }
		set { mGlobalActive = value; }
	}

	public virtual bool IsActive()
	{
		return !(_UseGlobalActive && !pGlobalActive || !_Active);
	}

	public virtual void ProcessMouseUp()
	{
		if (_MessageObject)
			_MessageObject.SendMessage("OnClick", gameObject, SendMessageOptions.DontRequireReceiver);

		if (!WithinRange())
			return;
		SendMessage("OnActivate", null, SendMessageOptions.DontRequireReceiver);
	}

	public virtual void OnMouseUp()
	{	
		mMousePressed = false;

		if (!mMouseOver)
			return;

		if (!IsActive())
			return;


		ProcessMouseUp();
	}

	public virtual void OnMouseDrag()
	{
		if (!mMousePressed)
			return;

		if (!mMouseOver)
			return;

		if (!IsActive())
			return;

		if (_MessageObject)
			_MessageObject.SendMessage("OnDrag", gameObject, SendMessageOptions.DontRequireReceiver);
	}

	public virtual void OnMouseDown()
	{
#if UNITY_IOS || UNITY_ANDROID
		OnMouseEnter();
#endif //UNITY_IOS || UNITY_ANDROID
		
		if (!mMouseOver)
			return;

		if (!IsActive())
			return;

		mMousePressed = true;

		if (_MessageObject)
			_MessageObject.SendMessage("OnPress", gameObject, SendMessageOptions.DontRequireReceiver);
	}

	public virtual void OnActivate()
	{
		if (OnActivated != null)
			OnActivated(gameObject);
		
		mRolloverTime = -1.0f;

		if (_ActivateObject != null)
			_ActivateObject.SetActive(true);
	}

	float mLastUpdateTime = 0;
	public virtual void Update()
	{
		// Only update 4 times per second
		float curTime = Time.realtimeSinceStartup;
		float interval = 1/4f;
		if (curTime - mLastUpdateTime < interval)
			return;
		mLastUpdateTime = curTime+UnityEngine.Random.value*0.1f*interval;
		
		
		bool isWithinRange = WithinRange();

#if UNITY_IOS || UNITY_ANDROID	
		bool isWithinSight = (isWithinRange && _Range != 0) && WithinSight();
		if(isWithinSight)
		{
			if(!mHighlighted)
			{
				mLineOfSightHighlight = true;
				Highlight();
			}
		}
		else if(mHighlighted && !mMouseOver)
		{
			if(mLineOfSightHighlight)
			{
				mLineOfSightHighlight = false;
				UnHighlight();
			}
		}
#endif

		if (mMouseOver && (!IsActive() || (!isWithinRange)))
		{
			mMouseOver = false;
			mMouseOverObject = null;
			ResetObject();
			mRolloverTime = -1.0f;
		}

		if (mRolloverTime > 0.0f)
		{
			mRolloverTime -= Time.deltaTime;
			if (mRolloverTime < 0.0f)
			{
				mRolloverTime = -1.0f;
				mRolloverReplayDelayTimer = _RolloverReplayDelay;
			}
		}

		if (mRolloverReplayDelayTimer > 0.0f)
			mRolloverReplayDelayTimer -= Time.deltaTime;
	}
	
#if UNITY_IOS || UNITY_ANDROID	
	public virtual bool WithinSight()
	{
        return true;
	}
#endif
	
	public virtual bool WithinRange()
	{
		return true;
	}

	public void GeometryUpdated()
	{
		if (mMouseOver)
			return;

		mShaders = null;
	}

	public virtual void Highlight()
	{
		if (_HighlightMaterial != null && mHighlighted == false)
		{
			mHighlighted = true;

			Component[] renderers = GetComponentsInChildren(typeof(Renderer));

			if (mShaders == null || mShaders.Length != renderers.Length)
				mShaders = new Shader[renderers.Length][];

			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer r = (Renderer)renderers[i];
				if (_FirstOnly == false || i == 0)
				{
					if (mShaders[i] == null || mShaders[i].Length != r.materials.Length)
						mShaders[i] = new Shader[r.materials.Length];

					for (int j = 0; j < r.materials.Length; j++)
					{
						mShaders[i][j] = r.materials[j].shader;
						r.materials[j].shader = Shader.Find(_HighlightShader);
						if (_HighlightMaterial.HasProperty("_RimColor"))
							r.materials[j].SetColor("_RimColor", _HighlightMaterial.GetColor("_RimColor"));
						if (_HighlightMaterial.HasProperty("_RimPower"))
							r.materials[j].SetFloat("_RimPower", _HighlightMaterial.GetFloat("_RimPower"));
					}
				}
			}
		}
	}

	public virtual void ProcessMouseEnter()
	{
		mMouseOver = true;
		mMouseOverObject = gameObject;

		Highlight();
	}

	public virtual void OnMouseEnter()
	{

#if !UNITY_IOS && !UNITY_ANDROID
      
#else
        if (!Application.isPlaying || !enabled)
#endif
            return;
		
		if (!IsActive())
			return;


#if UNITY_IOS || UNITY_ANDROID
		if (mMouseOver)
#else
        if (mMouseOver)
#endif
			return;

		ProcessMouseEnter();
	}

	public virtual void OnMouseOver()
	{
		OnMouseEnter();
	}

	public virtual void ProcessMouseExit()
	{
		mMouseOver = false;
		mMouseOverObject = null;
		ResetObject();
	}

	public virtual void OnMouseExit()
	{
		if (!Application.isPlaying || !enabled)
			return;

		if (!IsActive())
			return;

		if (!mMouseOver)
			return;
		ProcessMouseExit();
	}

	public virtual void UnHighlight()
	{
		if (mHighlighted == false)
			return;

		mHighlighted = false;

		if (_HighlightMaterial != null && mShaders != null)
		{
			Component[] renderers = GetComponentsInChildren(typeof(Renderer));

			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer r = (Renderer)renderers[i];
				if (_FirstOnly == false || i == 0)
				{
					for (int j = 0; j < r.materials.Length; j++)
					{
						if (i < mShaders.Length && j < mShaders[i].Length)
							r.materials[j].shader = mShaders[i][j];
					}
				}
			}
		}
	}

	public virtual void ResetObject()
	{
		mRolloverTime = -1.0f;
		UnHighlight();
	}

	void OnDrawGizmos()
	{
		if (_Draw)
		{
			if (_Range != 0.0f)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(_RangeOffset), _Range);
			}

			if (_Offset.magnitude != 0)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(transform.position + transform.TransformDirection(_Offset), new Vector3(0.2f, 0.2f, 0.2f));
			}
		}
	}

	public virtual Component[] GetRenderers()
	{
		return GetComponentsInChildren(typeof(Renderer));
	}

	public static void AddActivatedEventHandler(ActivatedEventHandler objectclicked)
	{
		OnActivated += objectclicked;
	}

	public static void RemoveActivatedEventHandler(ActivatedEventHandler objectclicked)
	{
		OnActivated -= objectclicked;
	}
}
