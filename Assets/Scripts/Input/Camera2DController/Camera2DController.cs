using System;
using UnityEngine;

public class Camera2DController : MonoBehaviour
{
	public LayerMask RaycastLayers;

	private Camera _camera;
	public Camera Camera
	{
		get { return _camera; }
	}

	private Transform trans;
	public Transform Transform
	{
		get { return trans; }
	}

	// Шорткат для текущего размера камеры
	public float Size
	{
		get { return _camera.orthographicSize; }
		set { _camera.orthographicSize = value; }
	}

	private Vector3 defaultPosition = Vector3.zero;
	public Vector3 DefaultPosition
	{
		get { return defaultPosition; }
		set { defaultPosition = value; }
	}
	private float defaultSize = 1f;
	public float DefaultSize
	{
		get { return defaultSize; }
		set { defaultSize = value; }
	}

	public float MinSize = 0.5f;
	public float MaxSize = 10f;

	public bool TranslationRestrained = true;
	public Vector2 TranslationBounds;

	private Vector2 lastZoomCenterScreen = Vector2.zero;
	private Vector3 lastZoomCenterWorld = Vector2.zero;

	// Events
	public event Action OnChange;
	public event Action OnTranslate;
	public event Action OnZoom;
	private bool queuedTranslate = false;
	private bool queuedZoom = false;

	void Awake()
	{
		_camera = GetComponent<Camera>();
		trans = GetComponent<Transform> ();
		TranslationBounds = new Vector2(MaxSize - MinSize/2f, MaxSize - MinSize/2f);

		defaultSize = Size;
		defaultPosition = trans.position;
	}

	void LateUpdate()
	{
		// Оповещение внешних подписчиков событий
		if (queuedZoom)
		{
			if (OnZoom != null)
				OnZoom();
		}
		if (queuedTranslate)
		{
			if (OnTranslate != null)
				OnTranslate();
		}
		if (queuedTranslate || queuedZoom)
		{
			queuedZoom = false;
			queuedTranslate = false;
			if (OnChange != null)
				OnChange();
		}
	}

	public void Translate(Vector3 direction)
	{
		Vector3 targetPosition;
		if (TranslationRestrained)
		{
			targetPosition = direction + trans.position;
			targetPosition = new Vector3 (
				Mathf.Min (Mathf.Abs (targetPosition.x), TranslationBounds.x) * Mathf.Sign (targetPosition.x),
				Mathf.Min (Mathf.Abs (targetPosition.y), TranslationBounds.y) * Mathf.Sign (targetPosition.y),
				targetPosition.z
			) - trans.position;
		}
		else
		{
			targetPosition = direction;
		}
		trans.Translate(targetPosition);
		queuedTranslate = true;
	}

	public void TranslateScreen(Vector3 from, Vector3 to)
	{
//		trans.Translate(camera.ScreenToWorldPoint (from) - camera.ScreenToWorldPoint (to));
		Translate (_camera.ScreenToWorldPoint (from) - _camera.ScreenToWorldPoint (to));
	}

	public void Zoom(Vector3 center, float deltaScale)
	{
		if (_camera.orthographic)
		{
			// Лимит на изменение увеличения камеры
			float targetDelta = Mathf.Clamp(
				// Умножение на размер камеры для сохранения постоянного углового увеличения
				deltaScale * Size,
				MinSize - _camera.orthographicSize,
				MaxSize - _camera.orthographicSize
			);
			Size += targetDelta;

			// Смещение точки увеличения из-за измения размера камеры
			Vector3 deltaSize = new Vector3(targetDelta*_camera.aspect, targetDelta, 0f);

			// Направление смещение центра камеры
			Vector3 centerTranslation = (trans.position - center);
			// Вектор направления не может быть длиннее 1
			float translation = Mathf.Min(centerTranslation.magnitude, 1f);
			centerTranslation = centerTranslation.normalized * translation;

			centerTranslation = new Vector3(centerTranslation.x * deltaSize.x, centerTranslation.y * deltaSize.y, 0f);

			Translate(centerTranslation);
		}
		queuedZoom = true;
	}

	public void ZoomScreen(Vector2 center, float deltaScale)
	{
		if (lastZoomCenterScreen != center)
		{
			lastZoomCenterScreen = center;
			lastZoomCenterWorld = _camera.ScreenToWorldPoint(center);
		}
		Zoom (
			lastZoomCenterWorld,
			deltaScale / _camera.ScreenToWorldPoint (Vector3.one).magnitude
		);
	}

	public RaycastHit2D[] Raycast2DWorld(Vector3 worldPosition)
	{
		return Physics2D.RaycastAll(worldPosition, trans.forward, Mathf.Infinity, RaycastLayers);
	}

	public RaycastHit2D Raycast2DScreen(Vector2 screenPosition)
	{
		Vector3 worldTouch = _camera.ScreenToWorldPoint(screenPosition);
//		RaycastHit hitsUI = UICamera.lastHit;
		RaycastHit2D[] hits = Raycast2DWorld(new Vector3 (worldTouch.x, worldTouch.y, 0f));
		return hits != null && hits.Length > 0 ? hits[0] : new RaycastHit2D();
	}

	public void Reset()
	{
		ResetSize();
		ResetPosition();
	}

	public void ResetSize()
	{
		Zoom(defaultPosition, defaultSize / Size);
	}

	public void ResetPosition()
	{
		Translate(defaultPosition - trans.position);
	}
}
