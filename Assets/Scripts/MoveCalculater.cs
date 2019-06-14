using System.Collections.Generic;
using UnityEngine;

public class CalcMoveRange
{
	// 移動計算結果のデータ格納用
	List<Cell> resultAttackRangeList;

	// マップ上のx,z位置
	private int positionX;
	private int positionZ;

	// マップの大きさ
	private int xLength;
	private int zLength;

	//補正数値
	private const float FIXED_OFFSET = 1.1f;

	/// <summary>
	/// 攻撃範囲を算出
	/// ※0.5f単位で範囲を変更する
	/// </summary>
	/// <param name="list">マップ配列</param>
	/// <param name="currentX">現在のX値</param>
	/// <param name="currentZ">現在のZ値</param>
	/// <param name="range">攻撃範囲</param>
	/// <returns>攻撃範囲のマスオブジェクトのTransform配列</returns>
	public List<Cell> StartSearch(List<List<Cell>> list, CharacterBase character, int currentX, int currentZ, float range)
	{
		// _originalMapListのコピー作成
		resultAttackRangeList = new List<Cell>();

		positionX = currentX;
		positionZ = currentZ;
		//現在位置
		var currentPosition = list[positionZ][positionX].transform.localPosition;
		character.transform.localPosition = new Vector3(currentPosition.x, character.Offset, currentPosition.z);
		//レンジを計算
		var attackRange = Mathf.Abs(BoardManager.fixedOffset * range) * FIXED_OFFSET;

		for (var i = 0; i < list.Count; ++i)
		{
			for (var n = 0; n < list[i].Count; ++n)
			{
				var target = list[i][n];
				var posX = target.transform.localPosition.x;
				var posZ = target.transform.localPosition.z;

				var diff = getDistance(currentPosition.x, currentPosition.z, posX, posZ);

				if (diff <= attackRange)
				{
					resultAttackRangeList.Add(target);
				}
			}
		}

		return resultAttackRangeList;
	}

	protected float getDistance(float x, float y, float x2, float y2)
	{
		float distance = Mathf.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y));

		return distance;
	}
}