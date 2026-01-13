using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SmoothieSmash
{
	public class SmoothieSmashInputObserver : MonoBehaviour
	{
		public Vector2 CurrentSteering;

		public event Action<Vector2, Vector2> SteeringChangedEvent;

		private void Update()
		{
			Vector2 vector = Vector2.zero;

			// Prefer Gamepad input if present, otherwise use Keyboard.
			if (Gamepad.current != null)
			{
				vector = Gamepad.current.leftStick.ReadValue();
			}
			else if (Keyboard.current != null)
			{
				float x = 0f, y = 0f;
				if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
					x -= 1f;
				if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
					x += 1f;
				if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
					y += 1f;
				if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
					y -= 1f;
				vector = new Vector2(x, y);
				vector = vector.normalized;
			}

			if (vector != CurrentSteering && this.SteeringChangedEvent != null)
			{
				this.SteeringChangedEvent(CurrentSteering, vector);
			}
			CurrentSteering = vector;
		}

		private void OnDestroy()
		{
			this.SteeringChangedEvent = null;
		}
	}
}