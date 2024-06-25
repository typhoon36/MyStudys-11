using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointTrack : MonoBehaviour
{
    public Color lineColor = Color.yellow;
    private Transform[] points;

    private void OnDrawGizmos()
    {
        //라인의 색상 지정
        Gizmos.color = lineColor;
        //WayPointGroup 게임오브젝트 아래에 있는 모든 Point 게임오브젝트 추출
        points = GetComponentsInChildren<Transform>();

        int nextIdx = 1;

        Vector3 currPos = points[nextIdx].position;
        Vector3 nextPos;

        //Point 게임오브젝트를 순회하면서 라인을 그림
        for (int i = 0; i <= points.Length; i++)
        {
            //마지막 Point일 경우 첫 번째 point로 지정
            nextPos = (++nextIdx >= points.Length) ? points[1].position :
                points[nextIdx].position;
            //시작 위치에서 종료 위치까지 라인을 그림
            Gizmos.DrawLine(currPos, nextPos);
            currPos = nextPos;
        }

    }


    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
