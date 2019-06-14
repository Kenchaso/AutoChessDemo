using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
	[HideInInspector]
	public bool IsSelectable = false;
	[HideInInspector]
	public int PositionX;
	[HideInInspector]
	public int PositionZ;

	[HideInInspector]
	public MeshRenderer Renderer;

	public int Index;

	public Collider Collider;
}
