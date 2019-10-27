using System;
using UnityEngine;

public abstract class AbstractCamera2DInput : MonoBehaviour
{

	public Camera2DController TheCamera;

	public bool ClickEnabled = true;
	public bool DoubleClickEnabled = false;
	public float DoubleClickCatchTime = 0.3f;
//	public bool LongClickEnabled = false; // not implemented
	public bool ZoomEnabled = false;
	public float ZoomSpeed = 1f;
	public bool DragScreenEnabled = false;
	public float DragTreshold = 10f;

	protected bool DragStarted = false;
	protected bool ZoomStarted = false;

	protected bool IsInteractionStatic = true;

	protected Vector2 LastZoomCenter = Vector2.zero;

	// Подписка на события ввода и старт/остановка контроллера
	protected bool Attached = false;

	protected virtual void Attach()
	{
		Detach ();
		if (TheCamera == null)
		{
			TheCamera = GetComponent<Camera2DController>();
		}
		Attached = true;
	}

	protected virtual void Detach()
	{
		Attached = false;
	}

	// Подписка для внешних слушателей
	public virtual event Action<Vector2> OnClickTap;
	public virtual event Action<Vector2> OnDoubleClickTap;
	public virtual event Action<Vector2> OnLongClickTap;

	protected void RaiseDoubleClickTap(Vector2 position)
	{
		if (OnDoubleClickTap != null)
			OnDoubleClickTap(position);
	}

    protected void RaiseClickTap(Vector2 position)
    {
	    if (OnClickTap != null)
		    OnClickTap(position);
    }
	
	protected void RaiseLongClickTap(Vector2 position)
	{
		if (OnLongClickTap != null)
			OnLongClickTap(position);
	}
}
