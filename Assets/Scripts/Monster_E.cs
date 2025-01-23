using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_E : MonoBehaviour
{
    // 이 오브젝트의 Monster
    public Monster monster;

    // 마녀가 발사하는 투사체 프리팹
    public GameObject bulletPref;


    float fireTimer = 0f;
    readonly float fireDuration = 4f;

    bool positionFix = false; // 플레이어와 일정 거리 이하가 된 후엔 이동하지 않음.


    private void Update()
    {
        // 일정 시간마다 발사
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireDuration)
        {
            StartCoroutine(Fire());
            fireTimer -= fireDuration;
        }

        // 이동
        if (!positionFix)
        {
            // 영웅 방향
            Vector3 dirToHero = GameManager.instance.heroObject.transform.position - transform.position;
            // 단위벡터(크기 1)로
            dirToHero = dirToHero.normalized;

            // 바라보는 방향 결정
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (dirToHero.x < 0)
            {
                spriteRenderer.flipX = (monster.isDirRight)
                    ? true : false;
            }
            else
            {
                spriteRenderer.flipX = (monster.isDirRight)
                    ? false : true;
            }

            // 영웅에게 이동
            transform.position += dirToHero * monster.speed * Time.deltaTime;

            // 플레이어와 일정 거리 이내라면 자리 고정
            if (Vector2.Distance(transform.position, GameManager.instance.heroObject.transform.position) < 2f)
            {
                positionFix = true;
            }
        }
        // 카메라에서 보이지 않게 되었다면 삭제
        if (GameManager.instance.hero.transform.position.y - transform.position.y > 6f)
        {
            monster.Obliterate();
        }
    }

    IEnumerator Fire()
    {
        // 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.E_shot);

        // 영웅 방향
        Vector3 dirToHero = GameManager.instance.heroObject.transform.position - transform.position;
        // 단위벡터(크기 1)로
        dirToHero = dirToHero.normalized;

        GameObject bulletObj = Instantiate(bulletPref, transform.position, Quaternion.identity);
        WitchBullet bullet = bulletObj.GetComponent<WitchBullet>();
        bullet.power = monster.power;

        float timer = 0f;
        float duration = 3f;
        while (timer < duration && bulletObj != null)
        {
            // 영웅에게 이동
            bulletObj.transform.position += dirToHero * 6.0f * Time.deltaTime;

            // 닿았는지 확인
            if (bullet.hitToHero)
            {
                //GameManager.instance.hero.hp -= monster.power;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        //Destroy(bulletObj);
        yield return null;
    }
}
