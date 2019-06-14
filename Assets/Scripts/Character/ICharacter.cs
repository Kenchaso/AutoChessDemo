using UnityEngine;

/// <summary>
/// キャラクターのベースクラス
/// </summary>
public interface ICharacter
{
	void Setup(float range, float offset, Vector3 size);
}