using ClubPenguin.BlobShadows;
using ClubPenguin.Core;
using ClubPenguin.LOD;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using Tweaker.Core;
using Tweaker.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin
{
    public class FreeCameraController : MonoBehaviour
    {
        private const string LeftHorizontal = "LeftHorizontal";
        private const string LeftVertical = "LeftVertical";
        private const string RightHorizontal = "RightHorizontal";
        private const string RightVertical = "RightVertical";
        private const string LeftTrigger = "LeftTrigger";
        private const string RightTrigger = "RightTrigger";
        private const string LeftBumper = "LeftBumper";
        private const string RightBumper = "RightBumper";
        private const string StartButton = "StartButton";
        private const string BButton = "BButton";
        private const string XButton = "XButton";
        private const string YButton = "YButton";
        private const string DPadLeft = "DPadLeft";
        private const string DPadRight = "DPadRight";
        private const string DPadUp = "DPadUp";
        private const float defaultFOV = 60f;

        public Transform Target;
        public Camera Camera;
        public float XSensitivity = 4f;
        public float YSensitivity = 2f;
        public float ZSensitivity = 2f;
        public float FOVSensitivity = 1f;
        public float XSpeed = 1f;
        public float YSpeed = 0.2f;
        public float ZSpeed = 1f;
        public bool WorldRelativeZMotion;
        public float RotationModifierFOV = 0.8f;
        public float XSpeedModifierFOV = 0.8f;
        public float YSpeedModifierFOV = 0.8f;
        public float ZSpeedModifierFOV = 0.8f;

        private bool wasLeftTriggerPressed;
        private bool wasRightTriggerPressed;
        private StateMachineContext context;
        private BlobShadowCaster blobShadowCaster;
        private GroupCulling groupCulling;
        private Canvas worldChatCanvas;
        private Camera mainCamera;
        private int localPlayerMask;
        private bool isLocalPlayerActive;

        private float yValue
        {
            get
            {
                float leftTrigger = GetAxisOrButton(LeftTrigger);
                float rightTrigger = GetAxisOrButton(RightTrigger);

                if (wasLeftTriggerPressed && wasRightTriggerPressed)
                {
                    return rightTrigger - leftTrigger;
                }
                float num = 0f;
                if (Math.Abs(leftTrigger) > float.Epsilon)
                {
                    num -= leftTrigger + 1f;
                    wasLeftTriggerPressed = true;
                }
                if (Math.Abs(rightTrigger) > float.Epsilon)
                {
                    num += rightTrigger + 1f;
                    wasRightTriggerPressed = true;
                }
                return num;
            }
        }

        private void Start()
        {
            Camera = gameObject.AddComponent<Camera>();
            localPlayerMask = LayerMask.NameToLayer("LocalPlayer");
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Camera.cullingMask = mainCamera.cullingMask;
                mainCamera.enabled = false;
            }
            gameObject.tag = "MainCamera";

            GameObject trayRootObj = GameObject.FindGameObjectWithTag(UIConstants.Tags.UI_Tray_Root);
            if (trayRootObj != null)
            {
                context = trayRootObj.GetComponent<StateMachineContext>();
                context.SendEvent(new ExternalEvent("Root", "noUI"));
            }
            Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.HidePlayerNames));
            DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
            GameObjectReferenceData component;
            if (!localPlayerHandle.IsNull && Service.Get<CPDataEntityCollection>().TryGetComponent(localPlayerHandle, out component))
            {
                blobShadowCaster = component.GameObject.GetComponent<BlobShadowCaster>();
            }
            groupCulling = UnityEngine.Object.FindObjectOfType<GroupCulling>();
            if (groupCulling != null)
            {
                groupCulling.enabled = false;
            }
            GameObject worldChatObj = GameObject.Find("WorldChatCanvas");
            if (worldChatObj != null)
            {
                worldChatCanvas = worldChatObj.GetComponent<Canvas>();
                if (worldChatCanvas != null)
                {
                    worldChatCanvas.worldCamera = Camera;
                }
            }
            setLocalPlayerActive(false);
        }

        private void Update()
        {
            var gamepad = Gamepad.current;

            if (gamepad == null)
            {
#if UNITY_STANDALONE_LINUX
                Debug.LogWarning("No gamepad detected on Linux. Ensure your device is supported and configured.");
#endif
                return;
            }

            Target.position += Target.right * GetAxisOrButton(LeftHorizontal) * XSpeed * getFOVModification(XSpeedModifierFOV);

            if (WorldRelativeZMotion)
            {
                Vector3 forward = Target.forward;
                forward.y = 0f;
                // Changed -= to += to fix reversed controls
                Target.position += forward.normalized * GetAxisOrButton(LeftVertical) * ZSpeed * getFOVModification(ZSpeedModifierFOV);
            }
            else
            {
                // Changed -= to += to fix reversed controls
                Target.position += Target.forward * GetAxisOrButton(LeftVertical) * ZSpeed * getFOVModification(ZSpeedModifierFOV);
            }

            Target.position += Vector3.up * yValue * YSpeed * getFOVModification(YSpeedModifierFOV);

            Quaternion lhs = Quaternion.AngleAxis(GetAxisOrButton(RightHorizontal) * XSensitivity * getFOVModification(RotationModifierFOV), Vector3.up);
            Target.transform.rotation = lhs * Target.transform.rotation;

            // Changed Vector3.right to Vector3.left to invert the vertical rotation direction
            // If this feels wrong, switch it back to Vector3.right
            Quaternion rhs = Quaternion.AngleAxis(GetAxisOrButton(RightVertical) * YSensitivity * getFOVModification(RotationModifierFOV), Vector3.left);
            Target.transform.rotation = Target.transform.rotation * rhs;

            if (GetButton(LeftBumper))
            {
                Camera.fieldOfView += GetAxisOrButton(LeftBumper) * FOVSensitivity;
            }
            if (GetButton(RightBumper))
            {
                Camera.fieldOfView -= GetAxisOrButton(RightBumper) * FOVSensitivity;
                if (Camera.fieldOfView < 1f)
                {
                    Camera.fieldOfView = 1f;
                }
            }
            if (GetButton(DPadLeft))
            {
                Quaternion lhs2 = Quaternion.AngleAxis(ZSensitivity, Target.forward);
                Target.transform.rotation = lhs2 * Target.transform.rotation;
            }
            if (GetButton(DPadRight))
            {
                Quaternion lhs2 = Quaternion.AngleAxis(0f - ZSensitivity, Target.forward);
                Target.transform.rotation = lhs2 * Target.transform.rotation;
            }
            if (GetButtonDown(DPadUp))
            {
                Vector3 eulerAngles = Target.transform.rotation.eulerAngles;
                eulerAngles.z = 0f;
                Quaternion lhs2 = Quaternion.Euler(eulerAngles);
                Target.transform.rotation = lhs2;
            }
            if (GetButtonDown(StartButton))
            {
                Service.Get<TweakerConsoleController>().gameObject.SetActive(true);
            }
            if (GetButtonDown(BButton))
            {
                Service.Get<TweakerConsoleController>().gameObject.SetActive(false);
            }
            if (GetButtonDown(XButton))
            {
                WorldRelativeZMotion = !WorldRelativeZMotion;
            }
            if (GetButtonDown(YButton))
            {
                setLocalPlayerActive(!isLocalPlayerActive);
            }
        }

        private float getFOVModification(float modifier)
        {
            float num = ((Camera.fieldOfView - 60f) / 60f + 1f) * modifier;
            if (Math.Abs(num) > float.Epsilon)
            {
                return num;
            }
            return 1f;
        }

        // --- Input System Wrappers ---

        private float GetAxisOrButton(string control)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return 0f;

            switch (control)
            {
                case LeftHorizontal:
                    return gamepad.leftStick.x.ReadValue();
                case LeftVertical:
                    return gamepad.leftStick.y.ReadValue();
                case RightHorizontal:
                    return gamepad.rightStick.x.ReadValue();
                case RightVertical:
                    return gamepad.rightStick.y.ReadValue();
                case LeftTrigger:
                    return gamepad.leftTrigger.ReadValue();
                case RightTrigger:
                    return gamepad.rightTrigger.ReadValue();
                case LeftBumper:
                    return gamepad.leftShoulder.isPressed ? 1f : 0f;
                case RightBumper:
                    return gamepad.rightShoulder.isPressed ? 1f : 0f;
                case DPadLeft:
                    return gamepad.dpad.left.isPressed ? 1f : 0f;
                case DPadRight:
                    return gamepad.dpad.right.isPressed ? 1f : 0f;
                default:
                    return 0f;
            }
        }

        private bool GetButton(string control)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return false;

            switch (control)
            {
                case LeftBumper:
                    return gamepad.leftShoulder.isPressed;
                case RightBumper:
                    return gamepad.rightShoulder.isPressed;
                case DPadLeft:
                    return gamepad.dpad.left.isPressed;
                case DPadRight:
                    return gamepad.dpad.right.isPressed;
                default:
                    return false;
            }
        }

        private bool GetButtonDown(string control)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return false;

            switch (control)
            {
                case DPadUp:
                    return gamepad.dpad.up.wasPressedThisFrame;
                case StartButton:
                    return gamepad.startButton.wasPressedThisFrame;
                case BButton:
                    return gamepad.buttonEast.wasPressedThisFrame; // B
                case XButton:
                    return gamepad.buttonWest.wasPressedThisFrame; // X
                case YButton:
                    return gamepad.buttonNorth.wasPressedThisFrame; // Y
                default:
                    return false;
            }
        }

        [Invokable("FreeCamera.StartCamera", Description = "Sets camera to free camera mode. Try plugging in a game controller. * This was used for in-game video capture")]
        [PublicTweak]
        public static void StartCamera()
        {
            Transform transform = Service.Get<GameObject>().transform;
            if (transform.Find("FreeCameraTarget") == null)
            {
                GameObject freeCameraTargetObj = new GameObject("FreeCameraTarget");
                freeCameraTargetObj.transform.SetParent(transform, false);
                freeCameraTargetObj.transform.position = Camera.main.transform.position;
                freeCameraTargetObj.transform.rotation = Camera.main.transform.rotation;
                FreeCameraController freeCameraController = freeCameraTargetObj.AddComponent<FreeCameraController>();
                freeCameraController.Target = freeCameraTargetObj.transform;
            }
        }

        [Invokable("FreeCamera.StopCamera", Description = "Stops free camera mode.")]
        [PublicTweak]
        public static void StopCamera()
        {
            Transform transform = Service.Get<GameObject>().transform.Find("FreeCameraTarget");
            if (transform != null)
            {
                UnityEngine.Object.Destroy(transform.gameObject);
            }
        }

        private void setLocalPlayerActive(bool isActive)
        {
            isLocalPlayerActive = isActive;
            if (localPlayerMask != 0 && localPlayerMask != -1)
            {
                if (isActive)
                {
                    Camera.cullingMask |= 1 << localPlayerMask;
                }
                else
                {
                    Camera.cullingMask &= ~(1 << localPlayerMask);
                }
            }
            if (blobShadowCaster != null)
            {
                blobShadowCaster.SetIsActive(isActive);
            }
        }

        private void OnDestroy()
        {
            if (context != null)
            {
                context.SendEvent(new ExternalEvent("Root", "restoreUI"));
            }
            Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.ShowPlayerNames));
            if (worldChatCanvas != null)
            {
                worldChatCanvas.worldCamera = mainCamera;
            }
            if (groupCulling != null)
            {
                groupCulling.enabled = true;
            }
            if(mainCamera != null)
                mainCamera.enabled = true;
            setLocalPlayerActive(true);
        }
    }
}
