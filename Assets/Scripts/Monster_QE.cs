using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_QE : MonoBehaviour
{
    // 이 오브젝트의 Monster
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

        // 효과음
        AudioManager.instance.PlaySfx(AudioManager.Sfx.QE_shot);

        // 영웅 방향 (2D)
        Vector2 dirToHero = GameManager.instance.heroObject.transform.position - transform.position;
        // 단위벡터(크기 1)로
        dirToHero = dirToHero.normalized;

        
        float speed = 0f;
        float maxX = 8.0f;
        while (true)
        {
            // 영웅에게 이동
            transform.position += (Vector3)dirToHero * speed * Time.deltaTime;

            // 벽에 닿았다면
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
    // 잠시 대기하다, 용사 방향으로 빠르게 돌진
}
