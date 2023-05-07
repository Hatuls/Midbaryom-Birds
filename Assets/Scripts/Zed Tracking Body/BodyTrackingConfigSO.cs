using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Body Tracking/New Body Tracking")]
public class BodyTrackingConfigSO :ScriptableObject
{
    [SerializeField]
    private InputMode _inputMode;
    [SerializeField]
    private TurningConfig _leftTurn;
    [SerializeField]
    private TurningConfig _rightTurn;
    [SerializeField,Range(0,1f)]
    private float _distance = 0.4f;


    public float Distance => _distance;

    public TurningConfig LeftTurn { get => _leftTurn; }
    public TurningConfig RightTurn { get => _rightTurn; }
    public InputMode InputModeType { get => _inputMode; }

    [Serializable]
    public class TurningConfig
    {
        public float LeftArmAngle;
        public float RightArmAngle;
    }




    public void SetInput(InputMode inputMode)
    => _inputMode = inputMode;
    public enum InputMode
    {
        Camera,
        Keyboard
    }
}
