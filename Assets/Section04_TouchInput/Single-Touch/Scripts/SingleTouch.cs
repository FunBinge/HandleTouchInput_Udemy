using UnityEngine;

public class SingleTouch : MonoBehaviour
{

    private class TouchData
    {
        public readonly TouchPhase TouchPhase;
        public readonly int TapCount;
        public readonly Vector2 TouchPosition;

        public TouchData(Touch touch)
        {
            TouchPosition = touch.position;
            TouchPhase = touch.phase;
            TapCount = touch.tapCount;
        }
    }

    private TouchData _touchData;

    void Update()
    {
        if (Input.touchCount < 1)
            return;

        Touch touch = Input.GetTouch(0);

        // Cache the touch data.
        _touchData = new TouchData(touch);

    }

    void OnGUI()
    {
        DisplayTouchData();
    }

    private void DisplayTouchData()
    {
        if (_touchData == null)
            return;

        string labelText = _touchData.TouchPhase.ToString();

        if (_touchData.TapCount > 1)
            labelText += "\nTaps: " + _touchData.TapCount;


        var tPos = _touchData.TouchPosition;
        tPos.y = Screen.height - _touchData.TouchPosition.y; // We flip y coordinates for correct positioning.

        var boxWidth = 30;
        var boxHeight = 30;
        var labelWidth = 75;
        var labelHeight = 50;
        var labelPosOffset = new Vector2(-80, -90);

        GUI.Label(new Rect(tPos.x + labelPosOffset.x, tPos.y + labelPosOffset.y, labelWidth, labelHeight), labelText);
        GUI.Box(new Rect(tPos.x - (boxWidth * 0.5f), tPos.y - (boxHeight * 0.5f), boxWidth, boxHeight), "");

    }

}
