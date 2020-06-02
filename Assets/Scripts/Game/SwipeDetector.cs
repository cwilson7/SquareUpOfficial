using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 fingerDownPos, fingerUpPos;
    public bool detectSwipeOnlyAfterRelease = true;

    [SerializeField] private float minDistanceForSwipe = 20f;

    public static event Action<SwipeData> OnSwipe = delegate { };

    void FixedUpdate()
    {
        if (IsOnJoystick())
        {
            StartCoroutine(JoyStickDelay());
            return;
        }

        if (GameInfo.GI.TimeStopped) return;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPos = touch.position;
                fingerDownPos = touch.position;
            }

            if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPos = touch.position;
                DetectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPos = touch.position;
                DetectSwipe();
            }

            
        }
    }

    IEnumerator JoyStickDelay()
    {
        yield return 0;
    }

    private void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPos.y - fingerUpPos.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                SendSwipe(direction);
            }
            else
            {
                var direction = fingerDownPos.x - fingerUpPos.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                SendSwipe(direction);
            }
            fingerUpPos = fingerDownPos;
        }
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private bool IsOnJoystick()
    {
        FloatingJoystick joystick = JoyStickReference.joyStick.gameObject.GetComponent<FloatingJoystick>();
        return joystick.background.gameObject.activeSelf;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPos.y - fingerUpPos.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPos.x - fingerUpPos.x);
    }

    private void SendSwipe(SwipeDirection direction)
    {
        SwipeData swipeData = new SwipeData()
        {
            Direction = direction,
            StartPos = fingerDownPos,
            EndPos = fingerUpPos
        };
        OnSwipe(swipeData);
    }

}

public struct SwipeData
{
    public Vector2 StartPos, EndPos;
    public SwipeDirection Direction;
}

public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}
