using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターのベースクラス
/// </summary>
public class CharacterBase : MonoBehaviour
{
	/// <summary>
	/// 現在位置Z
	/// </summary>
	public int FieldPositionZ { get; protected set; }

	/// <summary>
	/// 現在位置X
	/// </summary>
	public int FieldPositionX { get; protected set; }

	/// <summary>
	/// キャラクターのステート
	/// </summary>
	public enum POSITION_STATE
	{
		NONE,
		SHOP,   //ショップ(購入対象に存在する)
		DECK,   //デッキ(デッキに存在する)
		FIELD   //フィールド(フィールドに召喚されている)
	}

	/// <summary>
	/// キャラクターの現在のステート
	/// </summary>
	public POSITION_STATE State = POSITION_STATE.NONE;

	/// <summary>
	/// キャラ用オフセットY値
	/// </summary>
	public float Offset = 2.0f;

	/// <summary>
	/// サイズ
	/// </summary>
	public Vector3 Size = new Vector3(4f, 4f, 4f);

	/// <summary>
	/// 攻撃範囲
	/// </summary>
	public virtual float AttackRange { get; }

	/// <summary>
	/// 元の位置
	/// </summary>
	public int Index { get; protected set; }

	/// <summary>
	/// DOTween用コンポーネント
	/// </summary>
	public RectTransform rectTransform = null;

	/// <summary>
	/// デフォルトのレイヤー
	/// </summary>
	private const int DEFAULT_LAYER = 0;

	/// <summary>
	/// UIのレイヤー
	/// </summary>
	private const int UI_LAYER = 5;

	/// <summary>
	/// 通常Rotation値
	/// </summary>
	private readonly Quaternion NORMAL_ROTATION = Quaternion.Euler(0f, 0f, 0f);

	/// <summary>
	/// UIレイヤー時のRotation値
	/// </summary>
	private readonly Quaternion UI_ROTATION = Quaternion.Euler(-90f, 180f, 0f);

	/// <summary>
	/// 敵Rotation値
	/// </summary>
	public readonly Quaternion ENEMY_ROTATION = Quaternion.Euler(0f, 180f, 0f);

	/// <summary>
	/// アニメーター
	/// </summary>
	public Animator animator;

	/// <summary>
	/// 全子要素のキャッシュ
	/// </summary>
	private List<GameObject> cacheChildObjects = null;

	/// <summary>
	/// アニメーションタイプ
	/// </summary>
	public enum ANIMATION_TYPE
	{
		IDLE,
		ATTACK,
		DAMAGE,
		DIE
	}

	/// <summary>
	/// ステートの更新
	/// </summary>
	/// <param name="state"></param>
	public void ChangeState(POSITION_STATE state)
	{
		State = state;
	}

	/// <summary>
	/// キャラクターのPosition・Scale・Rotationをデフォルトに戻す
	/// </summary>
	public void ResetCharacterView()
	{
		ChangeLayer(DEFAULT_LAYER);
		transform.localScale = Size;
		transform.rotation = NORMAL_ROTATION;
	}

	/// <summary>
	/// ドラッグ中のキャラクターのPosition・Scale・Rotationを更新する
	/// </summary>
	public void UpdateCharacterView(Vector3 position)
	{
		ChangeLayer(UI_LAYER);
		transform.localScale = new Vector3(Size.x * 2, Size.y * 2, Size.z * 2);
		transform.rotation = UI_ROTATION;
		transform.localPosition = new Vector3(position.x, Offset, position.z);
	}

	/// <summary>
	/// レイヤー変更
	/// </summary>
	/// <param name="layer"></param>
	public void ChangeLayer(int layer)
	{
		var allObjects = GetChildren();
		for (var i = 0; i < allObjects.Count; ++i)
		{
			allObjects[i].layer = layer;
		}
	}

	/// <summary>
	/// 全子要素を取得
	/// </summary>
	/// <param name="parentName"></param>
	/// <returns></returns>
	private List<GameObject> GetChildren()
	{
		if(cacheChildObjects != null)
		{
			return cacheChildObjects;
		}

		var transforms = gameObject.GetComponentsInChildren<Transform>();

		cacheChildObjects = new List<GameObject>();
		for(var i = 0; i < transforms.Length; ++i)
		{
			cacheChildObjects.Add(transforms[i].gameObject);
		}
		return cacheChildObjects;
	}

	/// <summary>
	/// ポジションを変更
	/// </summary>
	/// <param name="position"></param>
	/// <param name="duration"></param>
	public void UpdatePositon(Vector3 position, float duration = 0.2f)
	{
		rectTransform.DOMove(new Vector3(position.x, Offset, position.z), duration);
	}

	/// <summary>
	/// インデックスを設定
	/// </summary>
	/// <param name="index"></param>
	public void SetIndex(int index)
	{
		Index = index;
	}

	/// <summary>
	/// フィールドポジションを設定
	/// </summary>
	/// <param name="index"></param>
	public void SetFieldPosition(int x, int z)
	{
		FieldPositionX = x;
		FieldPositionZ = z;
	}

	/// <summary>
	/// フィールドポジションをリセット
	/// </summary>
	public void ResetFieldPosition()
	{
		FieldPositionX = 0;
		FieldPositionZ = 0;
	}

	/// <summary>
	/// アニメーション切り替え
	/// </summary>
	/// <param name="type"></param>
	public void PlayAnimation(ANIMATION_TYPE type)
	{
		switch(type)
		{
			case ANIMATION_TYPE.IDLE:
				animator.Play("idle");
				break;
			case ANIMATION_TYPE.ATTACK:
				animator.Play("attack");
				break;
			case ANIMATION_TYPE.DAMAGE:
				animator.Play("damage");
				break;
			case ANIMATION_TYPE.DIE:
				animator.Play("die");
				break;
		}
	}
}