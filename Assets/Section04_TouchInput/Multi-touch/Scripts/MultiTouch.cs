using UnityEngine;
using System.Collections;

public class MultiTouch : MonoBehaviour
{

    private class TouchData
    {
        public readonly TouchPhase TouchPhase;
        public readonly int TapCount;
        public readonly Vector2 TouchPosition;
        public readonly int TouchId;

        public TouchData(Touch touch)
        {
            TouchPosition = touch.position;
            TouchPhase = touch.phase;
            TapCount = touch.tapCount;
            TouchId = touch.fingerId;
        }
    }

    private int _touchCountLimit = 5;       //How many fingers do we want to detect?
    private TouchData[] _touchData;

    void Start()
    {
        _touchData = new TouchData[_touchCountLimit];
    }

    void Update()
    {
        var count = Input.touchCount;

        if (count < 1)
            return;

        for (int i = 0; i < count; i++)
        {
            Touch touch = Input.GetTouch(i);
            int fingerId = touch.fingerId;

            if (fingerId >= _touchCountLimit)
            {
                fingerId = fingerId % _touchCountLimit;
            }

            // Cache the touch data.
            _touchData[fingerId] = new TouchData(touch);

        }

    }

    void OnGUI()
    {
        DisplayOptions();
        DisplayTouchData();
    }

    private void DisplayOptions()
    {
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height * 0.35f));
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Multitouch: " + Input.multiTouchEnabled, GUILayout.MinHeight(40)))
        {
            Input.multiTouchEnabled = !Input.multiTouchEnabled;
            ClearTouchData();
        }

        if (GUILayout.Button("Reset", GUILayout.MinHeight(40)))
        {
            ClearTouchData();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void ClearTouchData()
    {
        _touchData = new TouchData[_touchCountLimit];
    }

    private void DisplayTouchData()
    {
        for (int i = 0; i < _touchCountLimit; i++)
        {
            if (_touchData[i] == null)
                return;


            string labelText = "ID: " + _touchData[i].TouchId + "\n" + _touchData[i].TouchPhase;

            if (_touchData[i].TapCount > 1)
                labelText += "\nTaps: " + _touchData[i].TapCount;

            var tPos = _touchData[i].TouchPosition;
            tPos.y = Screen.height - _touchData[i].TouchPosition.y; // We flip y coordinates for correct positioning.

            var boxWidth = 30;
            var boxHeight = 30;
            var labelWidth = 75;
            var labelHeight = 50;
            var labelPosOffset = new Vector2(-80, -90);

            GUI.Label(new Rect(tPos.x + labelPosOffset.x, tPos.y + labelPosOffset.y, labelWidth, labelHeight), labelText);
            GUI.Box(new Rect(tPos.x - (boxWidth * 0.5f), tPos.y - (boxHeight * 0.5f), boxWidth, boxHeight), "");
        }

    }
}
