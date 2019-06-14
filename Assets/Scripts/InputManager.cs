using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タッチ・マウス入力系マネージャー
/// </summary>
public class InputManager
{
	/// <summary>
	/// ボード用マネージャー
	/// </summary>
	private BoardManager boardManager = null;

	/// <summary>
	/// スポーン用マネージャー
	/// </summary>
	private SpawnManager spawnManager = null;

	/// <summary>
	/// タッチ対象のキャラクター
	/// </summary>
	private CharacterBase target = null;

	/// <summary>
	/// タッチ対象のデッキ
	/// </summary>
	private Deck targetDeck = null;

	/// <summary>
	/// タッチ対象のセル
	/// </summary>
	private Cell targetCell = null;

	/// <summary>
	/// 2D用カメラ
	/// </summary>
	private Camera camera2D = null;

	/// <summary>
	/// 2Dカメラ描画用のワールド座標
	/// </summary>
	private Vector3 screenToWorldPointPosition;

	/// <summary>
	/// タッチ判定したキャラクターオブジェクト
	/// </summary>
	private Collider CharaObject = null;

	/// <summary>
	/// タッチ判定したデッキオブジェクト
	/// </summary>
	private Collider DeckObject = null;

	/// <summary>
	/// タッチ判定したセルオブジェクト
	/// </summary>
	private Collider CellObject = null;

	/// <summary>
	/// タッチ判定したその他のオブジェクト
	/// </summary>
	private Collider OtherObject = null;

	public void Initialize(BattleManager battleManager)
	{
		boardManager = battleManager.BoardManager;
		spawnManager = battleManager.SpawnManager;
		camera2D = battleManager.Camera2D;
	}

	/// <summary>
	/// タッチ判定用Update処理
	/// </summary>
	public void Update()
	{
		if (boardManager == null || !boardManager.IsReady)
		{
			return;
		}

		var position = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay(position);
		List<Collider> cellColliders = RaycastCell(ray);
		CharaObject = null;
		DeckObject = null;
		CellObject = null;
		OtherObject = null;

		for (var i = 0; i < cellColliders.Count; ++i)
		{
			var collider = cellColliders[i];
			string tagName = collider.tag;
			switch (tagName)
			{
				case "Character":
					CharaObject = collider;
					break;
				case "Deck":
					DeckObject = collider;
					break;
				case "Cell":
					CellObject = collider;
					break;
				case "Background":
					break;
				default:
					OtherObject = collider;
					break;
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			if(CharaObject != null)
			{
				target = CharaObject.GetComponent<CharacterBase>();
				target.animator.enabled = false;
			}
			else
			{
				target = null;
			}

			if(DeckObject != null)
			{
				targetDeck = DeckObject.GetComponent<Deck>();
			}
			else
			{
				targetDeck = null;
			}

			if(CellObject != null)
			{
				targetCell = CellObject.GetComponent<Cell>();
			}
			else
			{
				targetCell = null;
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (target != null)
			{
				if(CellObject != null)
				{
					//フィールド召喚
					spawnManager.Summon(target, targetDeck, targetCell, CellObject.GetComponent<Cell>());
				}
				else
				{
					if (DeckObject != null)
					{
						var deck = DeckObject.GetComponent<Deck>();

						switch (target.State)
						{
							case CharacterBase.POSITION_STATE.DECK:
								//ポジションを変える
								spawnManager.SwapPosition(target, deck);
								break;
							case CharacterBase.POSITION_STATE.FIELD:
								//デッキへ戻す
								spawnManager.ReturnToDeck(target, targetCell, deck);
								break;
						}
						boardManager.HideAllDeckEffect();
					}
					else
					{
						spawnManager.ResetPosition(target);
					}
				}
				boardManager.ClearCellColor(target);
				target.animator.enabled = true;
				target = null;
			}

			if(targetDeck != null)
			{
				boardManager.HideAllDeckEffect();
				targetDeck = null;
			}
		}

		if (Input.GetMouseButton(0))
		{
			if (target == null)
			{
				boardManager.ClearCellColor(target);
				return;
			}

			if (CellObject != null)
			{
				target.ResetCharacterView();
				boardManager.ShowRange(CellObject.gameObject, target);
				
				if(targetDeck != null)
				{
					boardManager.HideDeckEffectException(targetDeck.Index);
				}
				return;
			}
			else if(DeckObject != null)
			{
				boardManager.OperateDeckBehaviour(target, targetDeck, DeckObject.GetComponent<Deck>());
			}
			else
			{
				if (OtherObject != null)
				{
					return;
				}
			}
			MoveOutBoard(position, target);
		}
	}

	/// <summary>
	/// ボード外移動処理
	/// </summary>
	private void MoveOutBoard(Vector3 position, CharacterBase target)
	{
		boardManager.ClearCellColor(null);
		screenToWorldPointPosition = camera2D.ScreenToWorldPoint(position);
		target.UpdateCharacterView(screenToWorldPointPosition);
	}

	/// <summary>
	/// Raycastの結果のColliderを返却
	/// </summary>
	/// <param name="ray"></param>
	/// <returns></returns>
	public List<Collider> RaycastCell(Ray ray)
	{
		List<Collider> colliders = new List<Collider>();	

		RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
		for(var i = 0; i < hits.Length; ++i)
		{
			colliders.Add(hits[i].collider);
		}
		return colliders;
	}
}
