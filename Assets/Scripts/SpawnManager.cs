using DG.Tweening;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class SpawnManager
{
	/// <summary>
	/// デッキにいるキャラクターリスト
	/// </summary>
	public List<CharacterBase> DeckCharactersList { private set; get; }

	/// <summary>
	/// 召喚されたフィールド上のキャラクターリスト
	/// </summary>
	public List<CharacterBase> SummonCharacterList { private set; get; }

	/// <summary>
	/// 召喚されたフィールド上の敵キャラクターリスト
	/// </summary>
	public List<CharacterBase> EnemyCharacterList { private set; get; }

	/// <summary>
	/// デッキセルのリスト
	/// </summary>
	private List<Deck> DeckCellList;

	/// <summary>
	/// フィールドセルのリスト
	/// </summary>
	private List<List<Cell>> FieldCellList;

	private List<string> pathList;

	/// <summary>
	/// キャラの配置処理
	/// </summary>
	public void SetUp(List<Deck> deckCells, List<List<Cell>> mapList)
	{
		SummonCharacterList = new List<CharacterBase>();
		DeckCharactersList = new List<CharacterBase>();

		DeckCellList = deckCells;
		FieldCellList = mapList;

		pathList = new List<string>();
		pathList.Add("Prefab/Character/DragonA");
		pathList.Add("Prefab/Character/DragonB");
		pathList.Add("Prefab/Character/DragonC");
		pathList.Add("Prefab/Character/DragonD");

		for (var i = 0; i < pathList.Count; ++i)
		{
			var character = GameObject.Instantiate(Resources.Load(pathList[i])) as GameObject;
			var component = character.GetComponent<CharacterBase>();
			component.rectTransform = character.AddComponent<RectTransform>();
			component.SetIndex(i);
			component.State = CharacterBase.POSITION_STATE.DECK;
			character.transform.localScale = component.Size;
			character.transform.position = new Vector3(deckCells[i].transform.position.x, component.Offset, deckCells[i].transform.position.z);
			DeckCharactersList.Add(component);
		}
	}

	public void SpawnObjectOnField()
	{
		EnemyCharacterList = new List<CharacterBase>();

		System.Random rnd = new System.Random();

		for (var i = 0; i < pathList.Count; ++i)
		{
			int randomX = rnd.Next(8);
			int randomZ = rnd.Next(4);

			var character = GameObject.Instantiate(Resources.Load(pathList[i])) as GameObject;
			var component = character.GetComponent<CharacterBase>();
			component.rectTransform = character.AddComponent<RectTransform>();
			component.SetIndex(i);
			component.State = CharacterBase.POSITION_STATE.FIELD;
			component.SetFieldPosition(randomX, randomZ);
			character.transform.localScale = component.Size;
			character.transform.rotation = component.ENEMY_ROTATION;
			character.transform.position = new Vector3(FieldCellList[randomZ][randomX].transform.position.x, component.Offset, FieldCellList[randomZ][randomX].transform.position.z);
			EnemyCharacterList.Add(component);
		}
	}

	/// <summary>
	/// フィールドへ召喚
	/// </summary>
	/// <param name="position"></param>
	public void Summon(CharacterBase character, Deck fromDeck, Cell fromCell, Cell toCell)
	{
		bool isExist = SummonCharacterList.Exists(c => c.Index == toCell.Index);

		//既にそこにキャラクターがいた場合もとのCellに戻す
		if(isExist)
		{
			if(fromCell != null)
			{
				character.UpdatePositon(fromCell.transform.position);
				character.SetIndex(fromCell.Index);
				character.SetFieldPosition(fromCell.PositionX, fromCell.PositionZ);
			
			}
			else
			{
				character.UpdatePositon(fromDeck.transform.position, 0f);
				character.SetIndex(fromDeck.Index);
				character.ResetFieldPosition();
			}
			return;
		}

		character.UpdatePositon(toCell.transform.position);
		character.SetIndex(toCell.Index);
		character.SetFieldPosition(toCell.PositionX, toCell.PositionZ);

		//既にフィールドにいたらポジションだけ変更する
		if (character.State == CharacterBase.POSITION_STATE.FIELD)
		{
			return;
		}

		DeckCharactersList.Remove(character);
		SummonCharacterList.Add(character);
		character.ChangeState(CharacterBase.POSITION_STATE.FIELD);
	}

	/// <summary>
	/// フィールドへ召喚
	/// </summary>
	/// <param name="position"></param>
	public void ReturnToDeck(CharacterBase character, Cell fromCell, Deck toDeck)
	{
		bool isExist = DeckCharactersList.Exists(c => c.Index == toDeck.Index);

		//既にそこにキャラクターがいた場合もとのCellに戻す
		if (isExist)
		{
			character.UpdatePositon(fromCell.transform.position);
			character.SetIndex(fromCell.Index);
			character.SetFieldPosition(fromCell.PositionX, fromCell.PositionZ);
			return;
		}

		character.UpdatePositon(toDeck.transform.position, 0f);
		character.SetIndex(toDeck.Index);
		character.ResetFieldPosition();

		SummonCharacterList.Remove(character);
		DeckCharactersList.Add(character);
		character.ChangeState(CharacterBase.POSITION_STATE.DECK);
	}

	/// <summary>
	/// 立ち位置交換
	/// </summary>
	/// <param name="original"></param>
	/// <param name="target"></param>
	public void SwapPosition(CharacterBase original, Deck target)
	{
		int originIndex = original.Index;
		int targetIndex = target.Index;

		original.UpdatePositon(DeckCellList[targetIndex].transform.position);

		CharacterBase swapTarget = DeckCharactersList.FirstOrDefault(chara => chara.Index == targetIndex);
		if(swapTarget != null)
		{
			swapTarget.UpdatePositon(DeckCellList[originIndex].transform.position, 0.2f);
			swapTarget.SetIndex(originIndex);
		}
		original.SetIndex(targetIndex);
	}

	/// <summary>
	/// ポジションのリセット
	/// </summary>
	/// <param name="character"></param>
	public void ResetPosition(CharacterBase character)
	{
		switch(character.State)
		{
			case CharacterBase.POSITION_STATE.FIELD:
				character.UpdatePositon(FieldCellList[character.FieldPositionZ][character.FieldPositionX].transform.position);
				break;
			case CharacterBase.POSITION_STATE.DECK:
				character.UpdatePositon(DeckCellList[character.Index].transform.position);
				break;
		}
	}
}
