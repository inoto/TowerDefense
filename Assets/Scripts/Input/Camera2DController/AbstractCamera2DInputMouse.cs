using TowerDefense;
using UnityEngine;

public abstract class AbstractCamera2DInputMouse : AbstractCamera2DInput
{
	protected Vector2 LastClickPosition = Vector2.zero;
	protected Vector2 InitialClick = Vector2.zero;
	protected float LastClickDeltaTime = 0;
	protected float deltaX, deltaY;

	private void Awake()
	{
		Attach();
	}

#if UNITY_EDITOR
	protected virtual void Update()
	{
		if (Attached)
		{
			if (Input.GetMouseButtonDown(0))
			{
				OnMouseBtnDown();
			}
			if (Input.GetMouseButton(0))
			{
				OnMouseBtnHold();
			}
			if (Input.GetMouseButtonUp(0))
			{
				OnMouseBtnUp();
			}
			if (ZoomEnabled && Input.mouseScrollDelta.y != 0)
			{
				OnMouseScroll();
			}
		}
	}
#endif
	
	private void Move(float tDeltaX, float tDeltaY)
	{
//		Debug.Log("move");
		Vector3 direction = new Vector3(tDeltaX, tDeltaY, 0f).normalized;
		float distance = 75 * Time.deltaTime;

		Vector3 newPosition = transform.localPosition;
		newPosition -= direction * distance;
		newPosition.x = Mathf.Clamp(newPosition.x, -15, 15);
		newPosition.y = Mathf.Clamp(newPosition.y, -15, 15);
		transform.localPosition = newPosition;
	}

	protected virtual void OnMouseBtnDown()
	{
		InitialClick = LastClickPosition = Input.mousePosition;
		if (DoubleClickEnabled)
		{
			if ((Time.time - LastClickDeltaTime) < DoubleClickCatchTime)
			{
				RaiseDoubleClickTap(InitialClick);
			}
			else
			{
				IsInteractionStatic = true;
			}
			LastClickDeltaTime = Time.time;
		}
		else
		{
			IsInteractionStatic = true;
		}
	}

	protected virtual void OnMouseBtnUp()
	{
		LastClickPosition = Vector2.zero;
		DragStarted = false;
		
		if (IsInteractionStatic && ClickEnabled)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaiseClickTap(ray.origin + ray.direction);

			RaycastHit2D hit = TheCamera.Raycast2DScreen(Input.mousePosition);
			if (hit)
			{
				// ISelectable selectable = hit.transform.gameObject.GetComponent<ISelectable>();
				// if (selectable != null)
				// {
				// 	   selectable.onClick(ray.origin + ray.direction);
				// }
			}
		}
	}

	protected virtual void OnMouseBtnHold()
	{
		Vector2 clickPosition = Input.mousePosition;
		

		// Длинный жест пальцем - перетаскивание
		if ((clickPosition - InitialClick).magnitude > DragTreshold)
		{
			if (DragScreenEnabled)
			{
//				if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
//				{
//					TheCamera.TranslateScreen(LastClickPosition, clickPosition);
//				}
				deltaX = Input.GetAxis("Mouse X");
				deltaY = Input.GetAxis("Mouse Y");
				if (deltaX != 0f || deltaY != 0f)
				{
					Move(deltaX, deltaY);
				}
			}
			IsInteractionStatic = false;
		}
	}
	
	protected virtual void OnMouseScroll()
	{
		TheCamera.ZoomScreen(Input.mousePosition, ZoomSpeed * -Input.mouseScrollDelta.y);
	}
}
