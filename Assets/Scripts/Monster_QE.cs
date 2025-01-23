using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_QE : MonoBehaviour
{
    // �� ������Ʈ�� Monster
    public Monster monster;



    private void Update()
    {
        if (waitAndRush == null)
        {
            waitAndRush = StartCoroutine(WaitAndRush());
        }
    }

    Coroutine waitAndRush;
    IEnumerator WaitAndRush()
    {
        yield return new WaitForSeconds(2f);

        // ȿ����
        AudioManager.instance.PlaySfx(AudioManager.Sfx.QE_shot);

        // ���� ���� (2D)
        Vector2 dirToHero = GameManager.instance.heroObject.transform.position - transform.position;
        // ��������(ũ�� 1)��
        dirToHero = dirToHero.normalized;

        
        float speed = 0f;
        float maxX = 8.0f;
        while (true)
        {
            // �������� �̵�
            transform.position += (Vector3)dirToHero * speed * Time.deltaTime;

            // ���� ��Ҵٸ�
            if (dirToHero.x > 0 && transform.position.x > maxX)
            {
                break;
            }
            else if (dirToHero.x < 0 && transform.position.x < -maxX)
            {
                break;
            }

            speed += 15f * Time.deltaTime;
            yield return null;
        }
        
        
        waitAndRush = null;
    }
    // ��� ����ϴ�, ��� �������� ������ ����
}
