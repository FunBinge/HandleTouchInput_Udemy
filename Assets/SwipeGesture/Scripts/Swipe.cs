using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Swipe : MonoBehaviour {

    private class TouchData
    {
        public readonly TouchPhase TouchPhase;
        public readonly int TapCount;
        public readonly Vector2 TouchPosition;
        public string gestureType;

        public TouchData(Touch touch)
        {
            TouchPosition = touch.position;
            TouchPhase = touch.phase;
            TapCount = touch.tapCount;
        }
    }

    private TouchData _touchData;

    private float swipeMaxTime = 0.05f;
    private float minSwipeLength = 1f;
    private float startTime;
    private Vector2 touchMovement;

    private int diagonalScreenSize;

    private enum GestureState
    {
        Began, Active, Ended, None
    }

    private GestureState SwipeState = GestureState.None;

    private Vector2 startPos;
    private Vector2 swipeStartPos;

    void Start()
    {
        diagonalScreenSize = (int) Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
        Input.multiTouchEnabled = false;
        DebugConsole.Log("Screen width: " + Screen.width + " Screen Height: " + Screen.height + "Diagonal Screen Size = " + diagonalScreenSize);        
    }

    void Update()
    {
        if (Input.touchCount < 1)
            return;

        Touch touch = Input.GetTouch(0);

        // Cache the touch data (for GUI).
        _touchData = new TouchData(touch);

        if (touch.phase == TouchPhase.Began)
        {
            startPos = touch.position;

            touchMovement = Vector2.zero;
            startTime = Time.time;
            DebugConsole.Clear();
        }

        //DebugConsole.Log(touch.phase.ToString());

        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {            
            var distanceMeasuredInScreenSize = touch.deltaPosition/diagonalScreenSize;
            if (touch.deltaTime == 0.0) return;
            var speedMeasuredInScreenWidthsPerSecond = distanceMeasuredInScreenSize / touch.deltaTime;

            //touchMovement += touch.deltaPosition;

            if (speedMeasuredInScreenWidthsPerSecond.magnitude > .25f)
            {
                if (SwipeState == GestureState.None)
                {
                    swipeStartPos = Camera.main.ScreenToWorldPoint(touch.position);
                    SwipeState = GestureState.Began;
                }
                else if (SwipeState == GestureState.Began)
                    SwipeState = GestureState.Active;

                DebugConsole.Log(SwipeState.ToString());

            }
            else
            {
                if (SwipeState == GestureState.Active || SwipeState == GestureState.Began)
                {
                    SwipeState = GestureState.Ended;
                    GLDebug.DrawLine(swipeStartPos, Camera.main.ScreenToWorldPoint(touch.position), Color.white, 3f);
                }
                else if (SwipeState == GestureState.Ended)
                    SwipeState = GestureState.None;
            }
        }       

        if (touch.phase == TouchPhase.Ended)
        {              
            SwipeState = GestureState.None;                                    

            float distance = Vector2.Distance(Camera.main.ScreenToViewportPoint(startPos), Camera.main.ScreenToViewportPoint(touch.position)) * 10;
            DebugConsole.Log(SwipeState.ToString());
            //DebugConsole.Log("The Touch moved: " + distance.ToString("F2") + " pixels in " + (Time.time - startTime).ToString("F2") + " seconds");
        }
        _touchData.gestureType = SwipeState == GestureState.None ? "" : "Swipe";
    }

    void OnGUI()
    {
        DisplayTouchData();
    }

    private void DisplayTouchData()
    {
        if (_touchData == null)
            return;

        string labelText = "Gesture: " + _touchData.gestureType + "\n" + _touchData.TouchPhase.ToString();

        if (_touchData.TapCount > 1)
            labelText += "\nTaps: " + _touchData.TapCount;

        var tPos = _touchData.TouchPosition;
        tPos.y = Screen.height - _touchData.TouchPosition.y; // We flip y coordinates for correct positioning.

        var boxWidth = 30;
        var boxHeight = 30;
        var labelWidth = 175;
        var labelHeight = 50;
        var labelPosOffset = new Vector2(-80, -90);

        GUI.Label(new Rect(tPos.x + labelPosOffset.x, tPos.y + labelPosOffset.y, labelWidth, labelHeight), labelText);
        GUI.Box(new Rect(tPos.x - (boxWidth * 0.5f), tPos.y - (boxHeight * 0.5f), boxWidth, boxHeight), "");

    }
}
