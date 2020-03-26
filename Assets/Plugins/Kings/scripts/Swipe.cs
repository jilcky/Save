using UnityEngine;
using System.Collections;
using UnityEngine.Events;



public class Swipe : MonoBehaviour
{

    private Vector3 startPosition;
    private Vector3 stopPosition;
    private Vector3 swipe;
    private Vector3 lastPosition;

    private void Start()
    {
        initScreenPositions();
    }

    [System.Serializable] public class mEvent : UnityEvent { }

    [System.Serializable]
    public class normSwipe
    {
        [Tooltip("After which swipe up/down distance is an event executed?")]
        [Range(0f, 1f)] public float swipeDetectionLimit_UD = 0.3f;

        [Tooltip("After which swipe left/right distance is an event executed?")]
        [Range(0f, 1f)] public float swipeDetectionLimit_LR = 0.3f;
        public mEvent swipeUp;                          //execute an Event on Swipe up
        public mEvent swipeDown;                        //...down
        public mEvent swipeLeft;                        //...left
        public mEvent swipeRight;						//...right
        public mEvent swipeUpPreview;
        public mEvent swipeDownPreview;
        public mEvent swipeLeftPreview;
        public mEvent swipeRightPreview;
        public mEvent swipePreviewReset;
    }

    public normSwipe usualSwipes;

    public enum SwipeDirections
    {
        none,
        up,
        down,
        left,
        right
    }
    [ReadOnlyInspector] public SwipeDirections direction;
    SwipeDirections old_direction; //memorize last swipe direction to detect a change

    Vector3 swipeVector = Vector3.zero;
    Vector3 mouseSwipeVector = Vector3.zero;

    public Vector2 getSwipeVector()
    {
        return swipeVector;
    }

    [Tooltip("Scale the swipe for getting it with 'getScaledSwipeVector()'.\nThis is used for linking it with the blend tree of the card movement.")]
    public Vector2 swipeScale = Vector3.zero;

    [System.Serializable]
    public class C_IdleConfig
    {
        [Tooltip("Swipe movement fake until user interact with the screen.")]
        public AnimationCurve idleCurve;
        public bool idleActive = false;
        public bool startIdle = false;
        /*[HideInInspector] */public int touchCount = 0;
        [HideInInspector] public float idleTime = 0f;
        [ReadOnlyInspector] public float evaluation = 0f;
    }
    public C_IdleConfig idleConfig;
    public void setIdleSwipeActive(bool enable)
    {
        idleConfig.idleActive = enable;
    }
    public bool getIdleSwipeActive()
    {
        return idleConfig.idleActive;
    }

    [System.Serializable]
    public class C_ScreenPosition
    {
        public float timeToMoveToCenter = 3f;
        [Range(0.1f, 10f)] public float lerpSpeed = 5f;
        [ReadOnlyInspector] public Vector3 center;
        [ReadOnlyInspector] public Vector3 left;
        [ReadOnlyInspector] public Vector3 right;
        [ReadOnlyInspector] public Vector3 up;
        [ReadOnlyInspector] public Vector3 down;

        [HideInInspector] public Vector3 lerpedPosition;
        [HideInInspector] public Vector3 targetPosition;

        [HideInInspector] public float movementActiveTime = 0f;
        [HideInInspector] public Vector3 lerpStart = Vector3.zero;
        [HideInInspector] public SwipeDirections lastSetDirection;
        public bool GetPositioningActive()
        {
            if(movementActiveTime <= timeToMoveToCenter)
            {
                return true;
            }
            return false;
        }
        public void Reset()
        {
            lerpedPosition = Vector3.zero;
            targetPosition = Vector3.zero;
            lerpStart = Vector3.zero;
            lastSetDirection = SwipeDirections.none;
            movementActiveTime = timeToMoveToCenter + 1f;
        }
    }
    public C_ScreenPosition screenPositions;
    void initScreenPositions()
    {
        screenPositions.Reset();

        screenPositions.center = Vector3.zero;
        screenPositions.left = new Vector3(-usualSwipes.swipeDetectionLimit_LR - 0.01f, 0f, 0f);
        screenPositions.right = new Vector3(usualSwipes.swipeDetectionLimit_LR + 0.01f, 0f, 0f);
        screenPositions.up = new Vector3(0f, usualSwipes.swipeDetectionLimit_UD + 0.01f,  0f);
        screenPositions.down = new Vector3(0f, -usualSwipes.swipeDetectionLimit_UD - 0.01f,  0f);
    }
    public void SetScreenTargetPosition(SwipeDirections direction)
    {
        if (direction != screenPositions.lastSetDirection)
        {
            screenPositions.targetPosition = directionToTargetPosition(direction);

            //memorize actual position and reset lerp time
            screenPositions.lerpStart = screenPositions.lerpedPosition;
            screenPositions.movementActiveTime = 0f;
            screenPositions.lastSetDirection = direction;
        }
        else {
            //the same is pressed again. Test for execution.
            //Debug.Log("Second press: " + direction.ToString());

            //set the card instantly to the target position for correct onRelease evalution
            screenPositions.lerpedPosition = screenPositions.targetPosition;
            swipeVector = screenPositions.targetPosition;
            onRelease();
            //reset position and timings for next card
            screenPositions.Reset();
            swipeVector = Vector3.zero;
        }
    }
    public Vector3 directionToTargetPosition(SwipeDirections direction)
    {
        Vector3 retVector3 = Vector3.zero;
        //select new target position
        switch (direction)
        {
            case SwipeDirections.left:
                retVector3 = screenPositions.left;
                break;
            case SwipeDirections.right:
                retVector3 = screenPositions.right;
                break;
            case SwipeDirections.up:
                retVector3 = screenPositions.up;
                break;
            case SwipeDirections.down:
                retVector3 = screenPositions.down;
                break;
            case SwipeDirections.none:
            default:
                retVector3 = screenPositions.center;
                break;
        }

        return retVector3;
    }

    private void screenTargetPositionUpdate()
    {
        screenPositions.lerpedPosition = Vector3.Lerp(screenPositions.lerpStart, screenPositions.targetPosition, screenPositions.movementActiveTime * screenPositions.lerpSpeed);
        screenPositions.movementActiveTime += Time.deltaTime;
        if(screenPositions.GetPositioningActive() == false && screenPositions.lastSetDirection != SwipeDirections.none)
        {
            SetScreenTargetPosition(SwipeDirections.none);
        }
    }

    [System.Serializable]
    public class C_KeyConfiguration
    {
        public C_KeyConfiguration(KeyCode keyCode)
        {
            key = keyCode;
        }
        public KeyCode key;
        public string keyCode;
        public bool GetKey()
        {
            if (!string.IsNullOrEmpty(keyCode))
            {
                return (Input.GetKey(key) || Input.GetKey(keyCode));
            }
            return Input.GetKey(key);
        }
        public bool GetKeyDown()
        {
            if (!string.IsNullOrEmpty(keyCode))
            {
                return (Input.GetKeyDown(key) || Input.GetKeyDown(keyCode));
            }
            return Input.GetKeyDown(key);
        }
        public bool GetKeyUp()
        {
            if (!string.IsNullOrEmpty(keyCode))
            {
                return (Input.GetKeyUp(key) || Input.GetKeyUp(keyCode));
            }
            return Input.GetKeyUp(key);
        }
    }
    [System.Serializable]
    public class C_KeyMovements
    {
        public C_KeyConfiguration left = new C_KeyConfiguration(KeyCode.LeftArrow);
        public C_KeyConfiguration right = new C_KeyConfiguration(KeyCode.RightArrow);
        public C_KeyConfiguration up = new C_KeyConfiguration(KeyCode.UpArrow);
        public C_KeyConfiguration down = new C_KeyConfiguration(KeyCode.DownArrow);
        public C_KeyConfiguration escape = new C_KeyConfiguration(KeyCode.Space);
    }
    public C_KeyMovements keyMovements;
    private void checkKeyActions()
    {
        if (keyMovements.left.GetKeyDown())
        {
            SetScreenTargetPosition(SwipeDirections.left);
        }
        if (keyMovements.right.GetKeyDown())
        {
            SetScreenTargetPosition(SwipeDirections.right);
        }
        if (keyMovements.up.GetKeyDown())
        {
            SetScreenTargetPosition(SwipeDirections.up);
        }
        if (keyMovements.down.GetKeyDown())
        {
            SetScreenTargetPosition(SwipeDirections.down);
        }
        if (keyMovements.escape.GetKeyDown())
        {
            SetScreenTargetPosition(SwipeDirections.none);
        }
    }

    [ReadOnlyInspector] public bool pressed = false;
    [HideInInspector] public bool mousePressed = false;
    [HideInInspector] public bool keyActive = false;
    private bool oldMousePressed = false;
    [ReadOnlyInspector] public float actualSwipeDistance = 0f;

    public Vector2 getScaledSwipeVector()
    {
        Vector2 retVal = Vector2.zero;
        retVal.x = swipeVector.x * swipeScale.x;
        retVal.y = swipeVector.y * swipeScale.y;
        return retVal;
    }

    void Update()
    {

        //get the actual mouse or finger press
        mousePressed = Input.GetMouseButton(0);

        //on an change of the press, compute the distance
        if (oldMousePressed == false && mousePressed == true)
        {
            onKlick();
        }
        if (oldMousePressed == true && mousePressed == false)
        {
            onRelease();
        }

        //added 27.03.2019, keyControl
        checkKeyActions();
        screenTargetPositionUpdate();


        //track the swipe distance and scale it to to the screen size
        if (mousePressed == true)
        {
            mouseSwipeVector = Input.mousePosition - startPosition;
            mouseSwipeVector.x = mouseSwipeVector.x / Screen.width;
            mouseSwipeVector.y = mouseSwipeVector.y / Screen.height;
            actualSwipeDistance = mouseSwipeVector.magnitude;
            idleConfig.idleActive = false;
            idleConfig.touchCount++;

            swipeVector = mouseSwipeVector;
            //swipeVector += screenPositions.lerpedPosition;
        }else if (screenPositions.GetPositioningActive())
        {
            swipeVector = screenPositions.lerpedPosition;
            idleConfig.idleActive = false;
            idleConfig.touchCount++;
        }
        else
        {
            actualSwipeDistance = 0f;
            swipeVector = Vector3.zero;
            mouseSwipeVector = Vector3.zero;
            swipeVector.x = idleConfig.evaluation;
        }

        pressed = mousePressed || screenPositions.GetPositioningActive();

        //generate events for preview of value changes
        processSwipePreview();
        oldMousePressed = mousePressed;
    }

    private void LateUpdate()
    {
        swipeIdleMovement();
    }

    void swipeIdleMovement()
    {
        if (idleConfig.startIdle == true && idleConfig.touchCount < 1)
        {
            idleConfig.idleActive = true;
        }

        if (idleConfig.idleActive == true)
        {
            idleConfig.evaluation = idleConfig.idleCurve.Evaluate(idleConfig.idleTime);
            //idleConfig.evaluation = Mathf.Sin(idleConfig.idleTime);
            idleConfig.idleTime += Time.deltaTime;
            actualSwipeDistance = swipeVector.magnitude;
        }
        else
        {
            idleConfig.evaluation = 0f;
            idleConfig.idleTime = 0f;
        }

    }


    void onKlick()
    {

        startPosition = Input.mousePosition;
        lastPosition = startPosition;
    }

    void onRelease()
    {

        stopPosition = Input.mousePosition;
        processSwipe();
    }

    //Swipe preview is tested each frame.
    //If it exceeds the limits, it generates the preview events.
    void processSwipePreview()
    {

        direction = SwipeDirections.none;

        //process the usual swipes

        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {

            //decide: left/right swipe or up/down
            if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_LR)
            {
                // left or right
                if (swipeVector.x > 0f)
                {
                    direction = SwipeDirections.right;
                }
                else
                {
                    direction = SwipeDirections.left;
                }
            }
        }
        else
        {
            if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_UD)
            {
                // up or down
                if (swipeVector.y > 0f)
                {
                    direction = SwipeDirections.up;
                }
                else
                {
                    direction = SwipeDirections.down;
                }
            }
        }

        if (old_direction != direction)
        {
            switch (direction)
            {
                case SwipeDirections.left:
                    usualSwipes.swipeLeftPreview.Invoke();
                    break;
                case SwipeDirections.right:
                    usualSwipes.swipeRightPreview.Invoke();
                    break;
                case SwipeDirections.up:
                    usualSwipes.swipeUpPreview.Invoke();
                    break;
                case SwipeDirections.down:
                    usualSwipes.swipeDownPreview.Invoke();
                    break;
                default:
                    //on (up, down) or none: reset the preview
                    usualSwipes.swipePreviewReset.Invoke();
                    break;
            }

            old_direction = direction;
        }
    }

    void processSwipe()
    {

        //process the usual swipes

        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {

            //decide: left/right swipe or up/down
            if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_LR)
            {
                // left or right
                if (swipeVector.x > 0f)
                {
                    usualSwipes.swipeRight.Invoke();
                }
                else
                {
                    usualSwipes.swipeLeft.Invoke();
                }
            }
        }
        else
        {
            if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_UD)
            {
                // up or down
                if (swipeVector.y > 0f)
                {
                    usualSwipes.swipeUp.Invoke();
                }
                else
                {
                    usualSwipes.swipeDown.Invoke();
                }
            }
        }

        //after swipe: reset the positions to zeros
        startPosition = stopPosition = Vector3.zero;
    }
}
