using UnityEngine;

public class Swipe : MonoBehaviour {

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

    private float _swipeSpeedThreshold = .50f;       //Speed measured in screen size per second 
    private int _diagonalScreenSize;
    private GameObject _swipeTrail;      

    void Start()
    {
        _swipeTrail = Instantiate(Resources.Load("Trail") as GameObject);
        if (_swipeTrail != null) _swipeTrail.SetActive(false);

        _diagonalScreenSize = (int) Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
        Input.multiTouchEnabled = false;        
    }

    void Update()
    {
        Debug.Log(_swipeTrail.transform.position);
        if (Input.touchCount < 1)
            return;

        Touch touch = Input.GetTouch(0);
      
        //Visial Debugging
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
        Color circleColor = Color.white;        

        // Cache the touch data (for GUI).
        _touchData = new TouchData(touch);      

        if (touch.phase == TouchPhase.Moved)
        {      
            
            if (TouchSpeedIsSignificant(touch))
            {
                //Swipe gesture detected
                if (!_swipeTrail.activeInHierarchy)
                    _swipeTrail.SetActive(true);
                _swipeTrail.transform.position = touchPos;

                circleColor = Color.red;

            }
        }
        GLDebug.DrawCircle(touchPos, .3f, circleColor, 2f);
    }

    private bool TouchSpeedIsSignificant(Touch touch)
    {
        if (touch.deltaTime == 0.0) return false;  //To avoid division by zero on the logic below

        Vector2 distanceMeasuredInScreenSize = touch.deltaPosition/_diagonalScreenSize;        
        Vector2 speedMeasuredInScreenSizePerSecond = distanceMeasuredInScreenSize/touch.deltaTime;

        return speedMeasuredInScreenSizePerSecond.magnitude > _swipeSpeedThreshold;
    }

    void OnGUI()
    {
        DisplayTouchData();
        DisplayOptions();
    }

    private void DisplayOptions()
    {
        GUILayout.BeginArea(new Rect(10, 10, (Screen.width * 0.5f) - 20, Screen.height * 0.35f));
        GUILayout.BeginVertical();

        _swipeSpeedThreshold = GUILayout.HorizontalSlider(_swipeSpeedThreshold, 0f, 3f);
        GUILayout.Label("SwipeSpeedThreshold: " + _swipeSpeedThreshold.ToString("F2"));
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DisplayTouchData()
    {
        if (_touchData == null)
            return;

        string labelText = "\n" + _touchData.TouchPhase;

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
