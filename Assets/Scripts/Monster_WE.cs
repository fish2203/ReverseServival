using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_WE : MonoBehaviour
{
    public GameObject[] satellites;  // 위성 오브젝트들

    private void Start()
    {
        // 360도를 위성의 개수로 나누어 각 위성의 초기 각도 계산
        float angleStep = 360f / satellites.Length;

        // 위성들을 원형으로 배치
        for (int i = 0; i < satellites.Length; i++)
        {
            // 각도 계산
            float angle = i * angleStep;

            // 원형 배치 (위성들이 골렘 주변을 원으로 공전)
            Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0) * 2f; // 3f는 원의 반지름
            satellites[i].transform.position = transform.position + position;  // 골렘을 기준으로 위치 설정
        }
    }

    private void Update()
    {
        // 위성들이 골렘을 중심으로 회전
        foreach (GameObject satellite in satellites)
        {
            satellite.transform.RotateAround(transform.position, Vector3.forward, 100f * Time.deltaTime);  // Vector3.forward는 Z축 기준 회전
        }

        // 용사와 위성이 충돌하였다면, 데미지
        foreach (GameObject satellite in satellites)
        {
            Collider2D collider = satellite.GetComponent<Collider2D>();

            // 용사 콜라이더와 위성 콜라이더가 닿으면, 데미지를 주는 로직
        }
    }
}
