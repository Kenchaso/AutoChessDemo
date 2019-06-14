using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボード用のマネージャy－クラス
/// </summary>
public class BattleManager : MonoBehaviour
{
	/// <summary>
	/// 攻撃範囲計算クラス
	/// </summary>
	public CalcMoveRange CalcurateMoveRange = null;

	/// <summary>
	/// キャラ配置用クラス
	/// </summary>
	public SpawnManager SpawnManager = null;

	/// <summary>
	///	ボード用クラス
	/// </summary>
	public BoardManager BoardManager = null;

	/// <summary>
	/// インプット系クラス
	/// </summary>
	public InputManager InputManager = null;

	public Camera Camera2D = null;

	/// <summary>
	/// バトルの開始処理
	/// </summary>
	void Start()
	{
		Initialize();
	}

	/// <summary>
	/// 各種クラス破棄処理
	/// </summary>
	private void OnDestroy()
	{
		CalcurateMoveRange = null;
		SpawnManager = null;
		BoardManager = null;
		InputManager = null;
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	private void Initialize()
	{
		CalcurateMoveRange = new CalcMoveRange();
		SpawnManager = new SpawnManager();

		//ボード生成開始
		BoardManager = Instantiate(BoardManager.gameObject).GetComponent<BoardManager>();
		BoardManager.Initialize(this);

		//インプット処理の初期化処理開始
		InputManager = new InputManager();
		InputManager.Initialize(this);
	}

	void Update()
	{
		if(InputManager == null)
		{
			return;
		}

		InputManager.Update();
	}

	public void OnClickBattleStart()
	{
		SpawnManager.SpawnObjectOnField();
	}
}
