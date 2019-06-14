using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボード用のマネージャy－クラス
/// </summary>
public class BoardManager : MonoBehaviour
{
	/// <summary>
	/// セルオブジェクト
	/// </summary>
	[SerializeField]
	public Cell CellObject;

	/// <summary>
	/// デッキオブジェクト
	/// </summary>
	[SerializeField]
	public List<Deck> DeckCells;

	/// <summary>
	/// マスの大きさ
	/// </summary>
	private const int MAX_COLLUM_NUM = 8;
	private const int MAX_RAW_NUM = 8;

	/// <summary>
	/// マスの数
	/// </summary>
	private const int MAX_COLLIDER_COUNT = MAX_COLLUM_NUM * MAX_RAW_NUM;

	/// <summary>
	/// マスのベースポジション
	/// </summary>
	private readonly Vector3 BasePosition = new Vector3(-27f, 1.3f, 26.5f);

	/// <summary>
	/// マスのScale
	/// </summary>
	private readonly Vector3 BaseScale = new Vector3(7.64f, 7.64f, 0.5f);

	/// <summary>
	/// マス用のオフセットX値
	/// </summary>
	public const float fixedOffset = 7.64f;

	/// <summary>
	/// マップ配列
	/// </summary>
	private List<List<Cell>> MapList = null;

	/// <summary>
	/// 攻撃範囲計算クラス
	/// </summary>
	private CalcMoveRange calcurateMoveRange = null;

	/// <summary>
	/// キャラ配置用クラス
	/// </summary>
	private SpawnManager spawnManager = null;

	/// <summary>
	///	初期化処理が完了しているか否か
	/// </summary>
	public bool IsReady = false;

	/// <summary>
	/// 初期化処理
	/// </summary>
	/// <param name="spawnManager"></param>
	/// <param name="calcurateMoveRange"></param>
	public void Initialize(BattleManager battleManager)
	{
		MapList = new List<List<Cell>>();
		calcurateMoveRange = battleManager.CalcurateMoveRange;
		spawnManager = battleManager.SpawnManager;

		int colCount = 0;
		int rawCount = 0;
		List<Cell> moveXList = new List<Cell>();

		for (var i = 0; i < MAX_COLLIDER_COUNT; ++i)
		{
			if (colCount >= MAX_COLLUM_NUM)
			{
				colCount = 0;
				rawCount++;
			}

			float offsetX = fixedOffset * colCount;
			float offsetZ = fixedOffset * rawCount;

			var panel = Instantiate(CellObject.gameObject);
			var cell = panel.GetComponent<Cell>();
			cell.Index = i;
			cell.PositionX = colCount;
			cell.PositionZ = rawCount;
			var renderer = panel.GetComponent<MeshRenderer>();
			cell.Renderer = renderer;
			if(rawCount > 3)
			{
				cell.IsSelectable = true;
			}
			else
			{
				cell.Collider.enabled = false;
			}

			panel.name = i.ToString();
			panel.transform.localPosition = new Vector3(BasePosition.x + offsetX, BasePosition.y, BasePosition.z - offsetZ);
			panel.transform.localScale = new Vector3(BaseScale.x, BaseScale.y, BaseScale.z);
			moveXList.Add(cell);

			if (moveXList.Count == MAX_COLLUM_NUM)
			{
				MapList.Add(moveXList);
				moveXList = new List<Cell>();
			}
			colCount++;
		}
		spawnManager.SetUp(DeckCells, MapList);

		IsReady = true;
	}

	/// <summary>
	/// 攻撃範囲を表示
	/// </summary>
	/// <param name="cellObj"></param>
	public void ShowRange(GameObject cellObj, CharacterBase target)
	{
		var cell = cellObj.GetComponent<Cell>();
		if(cell == null)
		{
			return;
		}

		var posX = cell.PositionX;
		var posZ = cell.PositionZ;

		if (cell.IsSelectable == false)
		{
			posZ = 4;
		}

		List<Cell> result = calcurateMoveRange.StartSearch(MapList, target, posX, posZ, target.AttackRange);

		ClearCellColor(target);
		for (var i = 0; i < result.Count; ++i)
		{
			result[i].Renderer.enabled = true;
		}
	}

	/// <summary>
	/// 攻撃範囲表示リセット
	/// </summary>
	public void ClearCellColor(CharacterBase character)
	{
		if(character != null)
		{
			character.ResetCharacterView();
		}
		for (var i = 0; i < MapList.Count; ++i)
		{
			for (var n = 0; n < MapList[i].Count; ++n)
			{
				MapList[i][n].Renderer.enabled = false;
			}
		}
	}

	/// <summary>
	/// Deckオブジェクトの表示切り替え
	/// </summary>
	/// <param name="target"></param>
	/// <param name="baseDeck"></param>
	/// <param name="toDeck"></param>
	public void OperateDeckBehaviour(CharacterBase target, Deck baseDeck, Deck toDeck)
	{
		switch (target.State)
		{
			case CharacterBase.POSITION_STATE.DECK:
				ShowDeckEffect(baseDeck.Index, toDeck.GetComponent<Deck>().Index);
				break;
			case CharacterBase.POSITION_STATE.FIELD:
				ShowDeckEffect(toDeck.Index);
				break;
		}
	}

	/// <summary>
	/// デッキ選択エフェクト表示
	/// </summary>
	/// <param name="fromDeck"></param>
	/// <param name="ToDeck"></param>
	private void ShowDeckEffect(int fromIndex, int ToIndex)
	{
		HideAllDeckEffect();
		DeckCells[fromIndex].SelectEffect.SetActive(true);
		DeckCells[ToIndex].SelectEffect.SetActive(true);
	}

	/// <summary>
	/// インデックス指定したDeckエフェクトを表示
	/// </summary>
	/// <param name="index"></param>
	public void ShowDeckEffect(int index)
	{
		HideAllDeckEffect();
		DeckCells[index].SelectEffect.SetActive(true);
	}

	/// <summary>
	/// インデックス指定したDeckエフェクト以外を非表示
	/// </summary>
	/// <param name="index"></param>
	public void HideDeckEffectException(int exceptIndex)
	{
		for (var i = 0; i < DeckCells.Count; ++i)
		{
			if(i == exceptIndex)
			{
				continue;
			}
			DeckCells[i].SelectEffect.SetActive(false);
		}
	}

	/// <summary>
	/// デッキエフェクト非表示
	/// </summary>
	public void HideAllDeckEffect()
	{
		for (var i = 0; i < DeckCells.Count; ++i)
		{
			DeckCells[i].SelectEffect.SetActive(false);
		}
	}
}
