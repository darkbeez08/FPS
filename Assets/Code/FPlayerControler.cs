using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPlayerControler : MonoBehaviour
{
    float m_Yaw;
    float m_Pitch;
    public float m_YawRotationalSpeed;
    public float m_PitchRotationalSpeed;

    public float m_minPitch;
    public float m_maxPitch;

    public Transform m_PitchController;
    public bool m_UseYawInverted;
    public bool m_UsePitchInverted;

    public CharacterController m_CharacterController;
    public float m_Speed;
    public float m_FastSpeedMultiplier = 1.5f;
    public KeyCode m_LeftKeyCode;
    public KeyCode m_RightKeyCode;
    public KeyCode m_UpKeyCode;
    public KeyCode m_DownKeyCode;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;

    public Camera m_Camera;
    public float m_NormalMovementFOV = 60.0f;
    public float m_RunMovementFOV = 75.0f; 
     

    float m_VerticalSpeed = 0.0f;
    public bool m_OnGround = true;

    public float m_JumpSpeed = 10.0f;

    void Start()
    {
        m_Yaw = transform.rotation.y;
        m_Pitch = m_PitchController.localRotation.x;
    }

    void Update()
    {
        //Movement

        Vector3 l_RightDirection = transform.right;
        Vector3 l_ForwardDirection = transform.forward;
        Vector3 l_Direction = Vector3.zero;
        float l_Speed = m_Speed;

        if (Input.GetKey(m_UpKeyCode))
            l_Direction = l_ForwardDirection;
        if (Input.GetKey(m_DownKeyCode))
            l_Direction = -l_ForwardDirection;
        if (Input.GetKey(m_RightKeyCode))
            l_Direction = l_RightDirection;
        if (Input.GetKey(m_LeftKeyCode))
            l_Direction = -l_RightDirection;
        if (Input.GetKey(m_JumpKeyCode) && m_OnGround)
        {
            Debug.Log(m_OnGround);
            m_VerticalSpeed = m_JumpSpeed;
        }         
        if (Input.GetKey(m_RunKeyCode) && m_OnGround)
            l_Speed = m_Speed * m_FastSpeedMultiplier;

        l_Direction.Normalize();
        Vector3 l_Movement = l_Direction * l_Speed * Time.deltaTime;

        //camera

        float l_FOV = m_NormalMovementFOV;
        if (Input.GetKey(m_RunKeyCode))
        {
            l_Speed = m_Speed * m_FastSpeedMultiplier;
            l_FOV = m_RunMovementFOV;
        }
        //m_Camera.fieldOfView = l_FOV;

        m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, l_FOV, 0.5f);


        //Rotation
        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");
        m_Yaw = m_Yaw + m_YawRotationalSpeed * l_MouseX * Time.deltaTime*(m_UseYawInverted ? -1.0f : 1.0f);
        m_Pitch = m_Pitch + m_PitchRotationalSpeed * l_MouseY * Time.deltaTime * (m_UsePitchInverted ? -1.0f : 1.0f);
        m_Pitch = Mathf.Clamp(m_Pitch, m_minPitch, m_maxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        //gravity man v=vo+a*delta t
        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed>0.0f)
            m_VerticalSpeed = 0.0f;
        
        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = 0.0f;
            m_OnGround = true;
        }
        else
            m_OnGround = false;

    }


}
