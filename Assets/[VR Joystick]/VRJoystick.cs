using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace codepa
{
	[RequireComponent(typeof(Interactable))]
	public class VRJoystick : MonoBehaviour
	{
		[Serializable] class ValueEvent : UnityEvent<Vector2> { }

		[SerializeField] Transform pivotPoint;
		[Space]
		[SerializeField, Range(0, 90)] float maxXAngles;
		[SerializeField, Range(0, 90)] float maxYAngles;
		[Space]
		[SerializeField] ValueEvent OnValueChage;

		private Interactable interactable;
		private Vector3 originalPosition;
		private Vector2 joystickValue;

		public Vector2 Value
		{
			get
			{
				return joystickValue;
			}

			private set
			{
				joystickValue = value;
				OnValueChage?.Invoke(joystickValue);
			}
		}

		void Awake()
		{
			interactable = this.GetComponent<Interactable>();

			// Save our position/rotation so that we can restore it when we detach
			originalPosition = pivotPoint.transform.localPosition;
		}

		protected virtual void HandHoverUpdate(Hand hand)
		{
			GrabTypes startingGrabType = hand.GetGrabStarting();

			if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
			{
				hand.AttachObject(gameObject, startingGrabType, Hand.AttachmentFlags.DetachOthers);
			}
		}

		protected virtual void HandAttachedUpdate(Hand hand)
		{
			// Ensure hand stay in place.
			pivotPoint.transform.localPosition = originalPosition;
			pivotPoint.transform.rotation = hand.transform.rotation;

			// Correct hand rotation
			pivotPoint.transform.Rotate(Vector3.right, 90);

			// Errase vertical rotation and set it to the correct orientation.
			pivotPoint.transform.localEulerAngles = new Vector3(
				ClampDegrees(pivotPoint.transform.localEulerAngles.x, maxXAngles),
				0,
				ClampDegrees(pivotPoint.transform.localEulerAngles.z, maxYAngles));

			// Update Joystick value
			Value = new Vector2(DegreePrecentage(pivotPoint.transform.localEulerAngles.z, -maxYAngles), DegreePrecentage(pivotPoint.transform.localEulerAngles.x, maxXAngles));

			// Apply local offset to the pivot point
			pivotPoint.transform.localEulerAngles += -transform.localEulerAngles;

			// Check if trigger is released
			if (hand.IsGrabEnding(this.gameObject))
			{
				hand.DetachObject(gameObject);
			}
		}

		/// <summary>
		/// Utilite method to clamp the rotation of the stick.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="degree"></param>
		/// <returns></returns>
		private float ClampDegrees(float value, float degree)
		{
			if (value > 180) return Mathf.Clamp(value, 360 - degree, 360);
			else return Mathf.Clamp(value, 0, degree);
		}

		/// <summary>
		/// Utilitie method to check the joystick movement percentage.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="degree"></param>
		/// <returns></returns>
		private float DegreePrecentage(float value, float degree)
		{
			if (value > 180)
				value = value - 360;
			return value / degree;
		}

		/// <summary>
		/// Helpful Gizmos
		/// </summary>
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(pivotPoint.position, 0.01f);
			DrawGizmosArrow(transform.position, transform.forward * 0.1f, 0.025f, 30);

			void DrawGizmosArrow(in Vector3 pos, in Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
			{
				Gizmos.DrawRay(pos, direction);
				Gizmos.DrawRay(pos + direction, Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back * arrowHeadLength);
				Gizmos.DrawRay(pos + direction, Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back * arrowHeadLength);
			}
		}
	}
}