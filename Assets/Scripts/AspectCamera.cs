using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AspectCamera : MonoBehaviour
{

	public Vector2 aspect = new Vector2(4, 3);
	public Color32 backgroundColor = Color.black;
	private float aspectRate;
	private Camera _camera;
	private static Camera _backgroundCamera;
	int sizeVal = 1;

	public const float LOWER_LIMIT_ASPECT_RATIO = 0.5625f;      //このアスペクト比を超えるとセーフエリアを設定する(16:9のアスペクト比）
	public const float UPPER_LIMIT_ASPECT_RATIO = 0.4618f;      //このアスペクト比になったとき、SAFE_AREA_RATEを適用する（LOSER_LIMIT時は1→UPPER_LIMIT時はSAFE_AREA_RATEで中間は線形補完）
	public const float LOWER_LIMIT_ASPECT_RATIO_RECIPROCAL = 1 / LOWER_LIMIT_ASPECT_RATIO;
	private const float SAFE_AREA_RATE = 0.892f;    //セーフエリアを画面サイズのどの割合に設定するか（UPPER_LIMIT_ASPECT_RATIO(iPhoneX)の時にこの値になる）
	private const float SAFE_AREA_NONE_RATE = 1f;   //セーフエリアが必要ない時は画面端になるので１

	public static float SafeAreaRate;   //セーフエリアを設定するために画面幅に掛ける割合

	void Start()
	{
		aspectRate = (float)aspect.x / aspect.y;
		_camera = GetComponent<Camera>();

		//アスペクト比からセーフエリアを計算する
		SafeAreaRate = 1f;
		float safeAreaAspectRate = (float)Screen.height / Screen.width;

		//LOWER_LIMIT_ASPECTよりも横長の端末ではセーフエリアを設定する
		if (safeAreaAspectRate < LOWER_LIMIT_ASPECT_RATIO)
		{
			safeAreaAspectRate = Mathf.Max(safeAreaAspectRate, UPPER_LIMIT_ASPECT_RATIO);

			float rate = (LOWER_LIMIT_ASPECT_RATIO - safeAreaAspectRate) / (LOWER_LIMIT_ASPECT_RATIO - UPPER_LIMIT_ASPECT_RATIO);
			SafeAreaRate = Mathf.Lerp(SAFE_AREA_NONE_RATE, SAFE_AREA_RATE, rate);
		}

		//CreateBackgroundCamera ();
		//UpdateScreenRate ();

		//enabled = false;
	}

	//	void CreateBackgroundCamera ()
	//	{
	//		#if UNITY_EDITOR
	//		if(! UnityEditor.EditorApplication.isPlaying )
	//			return;
	//		#endif
	//		
	//		if (_backgroundCamera != null)
	//			return;
	//		
	//		var backGroundCameraObject = new GameObject ("Background Color Camera");
	//		_backgroundCamera = backGroundCameraObject.AddComponent<Camera> ();
	//		_backgroundCamera.depth = -99;
	//		_backgroundCamera.fieldOfView = 1;
	//		_backgroundCamera.farClipPlane = 1.1f;
	//		_backgroundCamera.nearClipPlane = 1; 
	//		_backgroundCamera.cullingMask = 0;
	//		_backgroundCamera.depthTextureMode = DepthTextureMode.None;
	//		_backgroundCamera.backgroundColor = backgroundColor;
	//		_backgroundCamera.renderingPath = RenderingPath.VertexLit;
	//		_backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
	//		_backgroundCamera.useOcclusionCulling = false;
	//		backGroundCameraObject.hideFlags = HideFlags.NotEditable;
	//	}

	void UpdateScreenRate()
	{
		float baseAspect = aspect.y / aspect.x;
		float nowAspect = (float)Screen.height / Screen.width;

		//最大アスペクト比を超える場合は設定値に抑える
		if (nowAspect < LOWER_LIMIT_ASPECT_RATIO)
		{
			nowAspect = LOWER_LIMIT_ASPECT_RATIO;
		}

		if (baseAspect > nowAspect)
		{
			var changeAspect = nowAspect / baseAspect;
			_camera.rect = new Rect(0, 0, 1, 1);
			_camera.orthographicSize = sizeVal * changeAspect;
		}
		else
		{
			var changeAspect = baseAspect / nowAspect;
			_camera.rect = new Rect(0, 0, 1, 1);
			_camera.orthographicSize = sizeVal / changeAspect;
		}
	}

	bool IsChangeAspect()
	{
		return _camera.aspect == aspectRate;
	}

	void Update()
	{
		//		if (IsChangeAspect ())
		//		{
		//			return;
		//		}
		UpdateScreenRate();
		_camera.ResetAspect();
	}
}