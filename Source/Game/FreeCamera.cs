using FlaxEngine;

namespace Game
{
    public class FreeCamera : Script
    {
        [Limit(0, 100), Tooltip("Camera movement speed factor")]
        public float MoveSpeed { get; set; } = 20;

        [Limit(0, 10), Tooltip("Camera movement speed boost on shift")]
        public float ShiftSpeedBoost { get; set; } = 2;

        [Tooltip("Camera rotation smoothing factor")]
        public float CameraSmoothing { get; set; } = 20.0f;

        private float pitch;
        private float yaw;

        public override void OnUpdate()
        {
            Screen.CursorVisible = false;
            Screen.CursorLock = CursorLockMode.Locked;

            var mouseDelta = new Float2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            pitch = Mathf.Clamp(pitch + mouseDelta.Y, -88, 88);
            yaw += mouseDelta.X;
        }

        public override void OnFixedUpdate()
        {
            var camTrans = Actor.Transform;
            var camFactor = Mathf.Saturate(CameraSmoothing * Time.DeltaTime);
            camTrans.Orientation = Quaternion.Lerp(camTrans.Orientation, Quaternion.Euler(pitch, yaw, 0), camFactor);

            var inputH = Input.GetAxis("Horizontal");
            var inputV = Input.GetAxis("Vertical");
            var move = new Vector3(inputH, 0.0f, inputV);
            move.Normalize();
            move = camTrans.TransformDirection(move);

            var gamepads = Input.Gamepads;
            if (Input.GetKey(KeyboardKeys.E) || (gamepads.Length > 0 && gamepads[0].GetButton(GamepadButton.RightShoulder)))
                move += Vector3.Up;
            if (Input.GetKey(KeyboardKeys.Q) || (gamepads.Length > 0 && gamepads[0].GetButton(GamepadButton.LeftShoulder)))
                move += Vector3.Down;

            var speed = MoveSpeed;
            if (Input.GetKey(KeyboardKeys.Shift))
                speed *= ShiftSpeedBoost;
            camTrans.Translation += move * speed;

            Actor.Transform = camTrans;
        }
    }
}
