using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public enum Type
    {
        Q, // 미니 골렘
        W, // 박쥐
        E, // 마녀
        QW, // 서큐버스  shift q
        QE, // 불덩이    shift w
        WE, // 골렘      shift e
        QWE // 마왕      r
    }
    public Type type;

    // 공통 정보
    public float health; // 체력
    public float power; // 공격력
    public float speed; // 이동속도
    public float xp; // 경험치

    public bool isDirRight; // 기본 스프라이트가 오른쪽을 바라보고 있는가?

    public Collider2D monsterCollider;


    private void Update()
    {
        // E(마녀)와 QE(불덩이)는 자체적인 이동 로직 보유
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        if (type != Type.E && type != Type.QE)
        {
            // 영웅 방향
            Vector3 dirToHero = GameManager.instance.heroObject.transform.position - transform.position;
            // 단위벡터(크기 1)로
            dirToHero = dirToHero.normalized;

            // 바라보는 방향 결정
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (dirToHero.x < 0)
            {
                spriteRenderer.flipX = (isDirRight)
                    ? true : false;
            }
            else
            {
                spriteRenderer.flipX = (isDirRight)
                    ? false : true;
            }

            // 영웅에게 이동
            transform.position += dirToHero * speed * ((GameManager.instance.isRightBuff) ? 1.3f : 1.0f) * Time.deltaTime;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("S_Ruby"))
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Ruby);

            GameManager.instance.haveRuby += 1;
            Destroy(collision.gameObject);
        }
    }

    public void Death()
    {
        // Q / W / E 몬스터 사망 시 코인 획득
        if (type == Type.Q)
        {
            GameManager.instance.wCoin += GameManager.instance.qDeathrattle;
        }
        else if (type == Type.W)
        {
            GameManager.instance.eCoin += GameManager.instance.wDeathrattle;
        }
        else if (type == Type.E)
        {
            GameManager.instance.qCoin += GameManager.instance.eDeathrattle;
        }

        // 사망 효과음 재생
        if (type == Type.W) AudioManager.instance.PlaySfx(AudioManager.Sfx.W_die);
        else if (type == Type.E) AudioManager.instance.PlaySfx(AudioManager.Sfx.E_die);
        else if (type == Type.QW) AudioManager.instance.PlaySfx(AudioManager.Sfx.QW_die);
        else if (type == Type.QE) AudioManager.instance.PlaySfx(AudioManager.Sfx.QE_die);
        else if (type == Type.WE) AudioManager.instance.PlaySfx(AudioManager.Sfx.WE_die);

        // 5% 확률로 루비를 떨굼
        float random = UnityEngine.Random.Range(0f, 100f);
        if (random <= GameManager.instance.rubyDropPer)
        {
            GameObject ruby = Instantiate(GameManager.instance.rubyPref);
            ruby.transform.position = transform.position;
        }

        GameManager.instance.ReturnMonster(this);
    }
    // 오브젝트를 파괴함
    public void Obliterate()
    {
        // Q / W / E 몬스터 사망 시 코인 획득
        if (type == Type.Q)
        {
            GameManager.instance.wCoin += GameManager.instance.qDeathrattle;
        }
        else if (type == Type.W)
        {
            GameManager.instance.eCoin += GameManager.instance.wDeathrattle;
        }
        else if (type == Type.E)
        {
            GameManager.instance.qCoin += GameManager.instance.eDeathrattle;
        }

        GameManager.instance.ReturnMonster(this);
    }
    // 오브젝트를 소멸시킴. 루비가 드랍되지 않음.
}
