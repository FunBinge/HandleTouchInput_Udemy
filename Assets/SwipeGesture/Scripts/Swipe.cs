using UnityEngine;
using System.Collections;

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

    private float swipeMaxTime = 0.1f;
    private float minSwipeLenght = 25f;
    private float startTime;
    private Vector2 touchMovement;

    void Start()
    {
        Input.multiTouchEnabled = false;
        DebugConsole.Log("Screen width: " + Screen.width + " Screen Height: " + Screen.height);
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
            touchMovement = Vector2.zero;
            startTime = Time.time;
            DebugConsole.Clear();
        }

        DebugConsole.Log(touch.phase.ToString());

        if (touch.phase == TouchPhase.Moved)
        {
            touchMovement += touch.deltaPosition;
            if (Time.time - startTime < swipeMaxTime && touchMovement.magnitude > minSwipeLenght)
            {                                         //Is the gesture short enough (in time and length) to be a swipe?               
                _touchData.gestureType = "Swipe";                
            }
        }

        if (touch.phase == TouchPhase.Ended)
        {
            DebugConsole.Log("The Touch moved: " + touchMovement.magnitude + " pixels in " + (Time.time - startTime).ToString("F2") + " seconds");
        }    

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
