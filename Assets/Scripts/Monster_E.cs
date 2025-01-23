using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_E : MonoBehaviour
{
    // �� ������Ʈ�� Monster
    public Monster monster;

    // ���డ �߻��ϴ� ����ü ������
    public GameObject bulletPref;


    float fireTimer = 0f;
    readonly float fireDuration = 4f;

    bool positionFix = false; // �÷��̾�� ���� �Ÿ� ���ϰ� �� �Ŀ� �̵����� ����.


    private void Update()
    {
        // ���� �ð����� �߻�
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireDuration)
        {
            StartCoroutine(Fire());
            fireTimer -= fireDuration;
        }

        // �̵�
        if (!positionFix)
        {
            // ���� ����
            Vector3 dirToHero = GameManager.instance.heroObject.transform.position - transform.position;
            // ��������(ũ�� 1)��
            dirToHero = dirToHero.normalized;

            // �ٶ󺸴� ���� ����
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

            // �������� �̵�
            transform.position += dirToHero * monster.speed * Time.deltaTime;

            // �÷��̾�� ���� �Ÿ� �̳���� �ڸ� ����
            if (Vector2.Distance(transform.position, GameManager.instance.heroObject.transform.position) < 2f)
            {
                positionFix = true;
            }
        }
        // ī�޶󿡼� ������ �ʰ� �Ǿ��ٸ� ����
        if (GameManager.instance.hero.transform.position.y - transform.position.y > 6f)
        {
            monster.Obliterate();
        }
    }

    IEnumerator Fire()
    {
        // ȿ���� ���
        AudioManager.instance.PlaySfx(AudioManager.Sfx.E_shot);

        // ���� ����
        Vector3 dirToHero = GameManager.instance.heroObject.transform.position - transform.position;
        // ��������(ũ�� 1)��
        dirToHero = dirToHero.normalized;

        GameObject bulletObj = Instantiate(bulletPref, transform.position, Quaternion.identity);
        WitchBullet bullet = bulletObj.GetComponent<WitchBullet>();
        bullet.power = monster.power;

        float timer = 0f;
        float duration = 3f;
        while (timer < duration && bulletObj != null)
        {
            // �������� �̵�
            bulletObj.transform.position += dirToHero * 6.0f * Time.deltaTime;

            // ��Ҵ��� Ȯ��
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
