using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public enum Type
    {
        Q, // �̴� ��
        W, // ����
        E, // ����
        QW, // ��ť����  shift q
        QE, // �ҵ���    shift w
        WE, // ��      shift e
        QWE // ����      r
    }
    public Type type;

    // ���� ����
    public float health; // ü��
    public float power; // ���ݷ�
    public float speed; // �̵��ӵ�
    public float xp; // ����ġ

    public bool isDirRight; // �⺻ ��������Ʈ�� �������� �ٶ󺸰� �ִ°�?

    public Collider2D monsterCollider;


    private void Update()
    {
        // E(����)�� QE(�ҵ���)�� ��ü���� �̵� ���� ����
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        if (type != Type.E && type != Type.QE)
        {
            // ���� ����
            Vector3 dirToHero = GameManager.instance.heroObject.transform.position - transform.position;
            // ��������(ũ�� 1)��
            dirToHero = dirToHero.normalized;

            // �ٶ󺸴� ���� ����
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

            // �������� �̵�
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
        // Q / W / E ���� ��� �� ���� ȹ��
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

        // ��� ȿ���� ���
        if (type == Type.W) AudioManager.instance.PlaySfx(AudioManager.Sfx.W_die);
        else if (type == Type.E) AudioManager.instance.PlaySfx(AudioManager.Sfx.E_die);
        else if (type == Type.QW) AudioManager.instance.PlaySfx(AudioManager.Sfx.QW_die);
        else if (type == Type.QE) AudioManager.instance.PlaySfx(AudioManager.Sfx.QE_die);
        else if (type == Type.WE) AudioManager.instance.PlaySfx(AudioManager.Sfx.WE_die);

        // 5% Ȯ���� ��� ����
        float random = UnityEngine.Random.Range(0f, 100f);
        if (random <= GameManager.instance.rubyDropPer)
        {
            GameObject ruby = Instantiate(GameManager.instance.rubyPref);
            ruby.transform.position = transform.position;
        }

        GameManager.instance.ReturnMonster(this);
    }
    // ������Ʈ�� �ı���
    public void Obliterate()
    {
        // Q / W / E ���� ��� �� ���� ȹ��
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
    // ������Ʈ�� �Ҹ��Ŵ. ��� ������� ����.
}
