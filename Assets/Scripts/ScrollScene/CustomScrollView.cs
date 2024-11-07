using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollView : MonoBehaviour
{
    public enum TouchEventType
    {
        None, Left, Right
    }

    [Header("Grids")]
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private CustomScrollViewGrid _prevGrid;
    [SerializeField]
    private CustomScrollViewGrid _currGrid;
    [SerializeField]
    private CustomScrollViewGrid _nextGrid;

    private TouchEventType _touchEventType;
    private bool _isMobile;
    private bool _isTouchBegan;
    private float _beganPositionX;
    private float _movedPositionX;
    private float _eventDistanceX;
    private int _currPage;
    private int _lastPage;

    void Start()
    {
        _isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        _eventDistanceX = Screen.width * 0.05f; // 20분의 1

        int currStage = 1;
        int lastStage = 45;

        for (int i = 15, page = 1; i < lastStage; i += 15, page++)
        {
            if (currStage <= i)
            {
                _currPage = page;
                break;
            }
        }

        _lastPage = lastStage / 15;

        if (lastStage % 15 != 0)
        {
            _lastPage += 1;
        }

        _prevGrid.Init(null);
        _currGrid.Init(null);
        _nextGrid.Init(null);

        UpdateGrid();
    }

    void Update()
    {
        float positionX;

        if (_touchEventType == TouchEventType.None)
        {
            OnInputEvent();

            if (_isTouchBegan)
            {
                float distanceX = _movedPositionX -_beganPositionX;

                if ((_beganPositionX < _movedPositionX && _currPage == 1) || (_beganPositionX > _movedPositionX && _currPage == _lastPage))
                {
                    distanceX = 0f;
                    _beganPositionX = _movedPositionX;
                }
                else if (Mathf.Abs(distanceX) > _eventDistanceX)
                {
                    _touchEventType = distanceX > 0 ? TouchEventType.Left : TouchEventType.Right;
                }

                positionX = distanceX;
            }
            else
            {
                positionX = Mathf.Lerp(_rectTransform.anchoredPosition.x, 0f, 10f * Time.deltaTime);
            }
        }
        else
        {
            if (_touchEventType == TouchEventType.Left)
            {
                positionX = Mathf.Lerp(_rectTransform.anchoredPosition.x, 2560f, 10f * Time.deltaTime);
            }
            else
            {
                positionX = Mathf.Lerp(_rectTransform.anchoredPosition.x, -2560f, 10f * Time.deltaTime);
            }

            if (positionX < -2544f || positionX > 2544f)
            {
                positionX = 0f;

                // Page
                _currPage = _touchEventType == TouchEventType.Left ? _currPage -= 1 : _currPage += 1;
                UpdateGrid();

                _isTouchBegan = false;
                _touchEventType = TouchEventType.None;
            }
        }

        _rectTransform.anchoredPosition = new Vector2(positionX, _rectTransform.anchoredPosition.y);
    }

    void OnInputEvent()
    {
        Vector2 touchPosition = Vector2.zero;
        TouchPhase touchPhase = TouchPhase.Ended;

        if (_isMobile)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                touchPosition = touch.position;
                touchPhase = touch.phase;
            }
        }
        else
        {
            touchPosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                touchPhase = TouchPhase.Began;
            }
            else if (Input.GetMouseButton(0))
            {
                touchPhase = TouchPhase.Moved;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                touchPhase = TouchPhase.Ended;
            }
        }

        if (touchPhase == TouchPhase.Began)
        {
            _isTouchBegan = true;
            _beganPositionX = touchPosition.x;
            _movedPositionX = touchPosition.x;
        }
        else if (touchPhase == TouchPhase.Moved && _isTouchBegan)
        {
            _movedPositionX = touchPosition.x;
        }
        else if (touchPhase == TouchPhase.Ended && _isTouchBegan)
        {
            _isTouchBegan = false;
        }
    }

    void UpdateGrid()
    {
        if (_currPage == 1)
        {
            _prevGrid.gameObject.SetActive(false);
        }

        if (_currPage == _lastPage)
        {
            _nextGrid.gameObject.SetActive(false);
        }
        
        if (!_prevGrid.gameObject.activeSelf && _currPage > 1)
        {
            _prevGrid.gameObject.SetActive(true);
        }

        if (!_nextGrid.gameObject.activeSelf &&_currPage < _lastPage)
        {
            _nextGrid.gameObject.SetActive(true);
        }

        _currGrid.SetGrid((_currPage - 1) * 15 + 1, _currPage * 15, 100);

        if (_prevGrid.gameObject.activeSelf)
        {
            _prevGrid.SetGrid((_currPage - 2) * 15 + 1, (_currPage - 1) * 15, 100);
        }

        if (_nextGrid.gameObject.activeSelf)
        {
            _nextGrid.SetGrid(_currPage * 15 + 1, (_currPage + 1) * 15, 100);
        }
    }

}
