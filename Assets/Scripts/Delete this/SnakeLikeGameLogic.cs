
using System.Linq;
using UnityEngine;
using ZED.Tracking;

public class SnakeLikeGameLogic : MonoBehaviour
{
    private const int FORWARD_MOTION_ID = 1;
    private const int LEFT_MOTION_ID = 2;
    private const int RIGHT_MOTION_ID = 3;

    [SerializeField]
    private Transform _transform;

    public Vector3[] Lanes;
    public float Speed =5f;

    private int _counter;
    private int _currentPositionIndex;

    [SerializeField]
    private TrackingMotionManagerMB _trackingMotionManagerMB;


    [SerializeField]
    private ZedsMotionPacks[] _snakeMotionsPack;
    private void Awake()
    {
        _currentPositionIndex = 1;
        _counter = -1;
    }
    private void Start()
    {

        _trackingMotionManagerMB.TryGetOrCreateEmptyObserver(FORWARD_MOTION_ID, out MotionObserver<SkeletonHandler> observer);
        observer.OnGoing += MoveForward;

        _trackingMotionManagerMB.TryGetOrCreateEmptyObserver(LEFT_MOTION_ID, out  observer);
        observer.OnComplete += MoveOneLeft;

        _trackingMotionManagerMB.TryGetOrCreateEmptyObserver(RIGHT_MOTION_ID, out observer);
        observer.OnComplete += MoveOneRight;

        _counter++;
        _trackingMotionManagerMB.SetNewMotionsPack(_snakeMotionsPack[_counter % _snakeMotionsPack.Length]);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _counter++;
            _trackingMotionManagerMB.SetNewMotionsPack( _snakeMotionsPack[_counter % _snakeMotionsPack.Length]);
        }
    }
    private void OnDestroy()
    {
        _trackingMotionManagerMB.TryGetObserver(FORWARD_MOTION_ID, out var observer);
        observer.OnGoing -= MoveForward;
        _trackingMotionManagerMB.TryGetObserver(LEFT_MOTION_ID, out observer);
        observer.OnComplete -= MoveOneLeft;
        _trackingMotionManagerMB.TryGetObserver(RIGHT_MOTION_ID, out observer);
        observer.OnComplete -= MoveOneRight;
    }

    public void MoveOneLeft()
    {
        if (_currentPositionIndex > 0)
            _currentPositionIndex--;
        else
            return;

        Vector3 pos = _transform.position;
        pos.x = Lanes[_currentPositionIndex].x;
        _transform.position = pos;
    }

    public void MoveOneRight()
    {
        if (_currentPositionIndex < Lanes.Length-1)
            _currentPositionIndex++;

        Vector3 pos = _transform.position;
        pos.x = Lanes[_currentPositionIndex].x;
        _transform.position = pos;
    }
    private void MoveForward()
    {
        _transform.position += transform.forward * Time.deltaTime * Speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < Lanes.Length; i++)
        {
            Gizmos.DrawSphere(_transform.position+Lanes[i], .1f);
        }
       
    }
}
