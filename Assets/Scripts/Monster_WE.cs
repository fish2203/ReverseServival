using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_WE : MonoBehaviour
{
    public GameObject[] satellites;  // ���� ������Ʈ��

    private void Start()
    {
        // 360���� ������ ������ ������ �� ������ �ʱ� ���� ���
        float angleStep = 360f / satellites.Length;

        // �������� �������� ��ġ
        for (int i = 0; i < satellites.Length; i++)
        {
            // ���� ���
            float angle = i * angleStep;

            // ���� ��ġ (�������� �� �ֺ��� ������ ����)
            Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0) * 2f; // 3f�� ���� ������
            satellites[i].transform.position = transform.position + position;  // ���� �������� ��ġ ����
        }
    }

    private void Update()
    {
        // �������� ���� �߽����� ȸ��
        foreach (GameObject satellite in satellites)
        {
            satellite.transform.RotateAround(transform.position, Vector3.forward, 100f * Time.deltaTime);  // Vector3.forward�� Z�� ���� ȸ��
        }

        // ���� ������ �浹�Ͽ��ٸ�, ������
        foreach (GameObject satellite in satellites)
        {
            Collider2D collider = satellite.GetComponent<Collider2D>();

            // ��� �ݶ��̴��� ���� �ݶ��̴��� ������, �������� �ִ� ����
        }
    }
}
