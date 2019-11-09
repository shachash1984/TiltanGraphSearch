using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using DG.Tweening;

public class CharacterMovement : MonoBehaviour
{
    private Vector3 _startPos, _endPos;
    private SNode _startNode, _goalNode;
    public static Stopwatch stopwatch;

    private List<SNode> _path;
    public Transform goal;
    public int BezierPointsNum = 4;
    public float lerpParam = 0.25f;
    [Min(1)]
    public float speed = 2f;
    List<Vector3> actualPath = new List<Vector3>();

    private void Start()
    {
        stopwatch = new Stopwatch();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            stopwatch.Reset();
            stopwatch.Start();
            _startPos = transform.position;
            _endPos = goal.position;
            FindPath();
            float currentY = transform.position.y;
            actualPath.Clear();
            if(_path != null && _path.Count > 0)
            {
                for (int i = 0; i < _path.Count; i++)
                {
                    Vector3 nextPos = _path[i].position;

                    List<Vector3> bezierPts = new List<Vector3>();
                    for (int j = i; j < i + BezierPointsNum; j++)
                    {
                        if (j < _path.Count)
                            bezierPts.Add(_path[j].position);
                    }
                    nextPos = Utils.Bezier(lerpParam, bezierPts);
                    nextPos.y = currentY;
                    actualPath.Add(nextPos);
                }
                stopwatch.Stop();
                Debug.Log("Calculation time: " + stopwatch.ElapsedMilliseconds);
                transform.DOPath(actualPath.ToArray(), _path.Count * 1 / speed).SetEase(Ease.Linear);
            }
            if (stopwatch.IsRunning)
                stopwatch.Stop();
        }
        
    }

    private void FindPath()
    {

        _startNode = new SNode(GridHandler.S.GetGridCellCenter(GridHandler.S.GetGridIndex(_startPos)));
        _goalNode = new SNode(GridHandler.S.GetGridCellCenter(GridHandler.S.GetGridIndex(_endPos)));
        //stopwatch.Reset();
        //stopwatch.Start();
        _path = AStarUtil.FindPath(_startNode, _goalNode);
        //stopwatch.Stop();
        //Debug.Log("Calculation time: " + stopwatch.ElapsedMilliseconds);
    }

    void OnDrawGizmos()
    {
        if (_path != null && _path.Count > 0)
        {
            int index = 1;
            foreach (Node node in _path)
            {
                if (index < _path.Count)
                {
                    SNode nextNode = _path[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
            };
            int vecIndex = 1;
            if(actualPath.Count > 0)
            {
                foreach (Vector3 vec in actualPath)
                {
                    if(vecIndex < actualPath.Count)
                    {
                        Vector3 nextVec = actualPath[vecIndex];
                        Debug.DrawLine(vec, nextVec, Color.red);
                        vecIndex++;
                    }
                    
                }
            }
            
        }
    }
}
