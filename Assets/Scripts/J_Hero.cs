using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UIElements;

public class J_Hero : MonoBehaviour
{
    public enum State
    {
        Normal, 
        Attack,
        Hit,
        Avoid,
        Return,
        Die
    }

    public GameObject normalAttackPref;
    public GameObject axeAttackPref;
    public GameObject iceAttackPref;

    // stat
    public float hp = 10.0f;
    public int level = 1;
    public float lv_exp = 0.0f;
    public bool hasShield = true;
    public float walkSpeed = 1.0f;
    public int[] attackPoint = new int[3] { 1, 1, 1 };
    public bool[] attackPointFlag = new bool[3] { true, false, false };

    // for test
    public float normalAttackTime = 1.0f;
    public float normalDestroyTime = 0.2f;
    public float axeAttactTime = 3.0f;
    public float axeSpeed = 10.0f;
    public float axeDestroyTime = 2.0f;
    public float iceAttactTime = 5.0f;
    public float iceShootSpeed = 1.0f;
    public float noHitTime = 3.0f;

    // for AI
    private Vector3 originPosition;
    private Vector3 avoidPosition;
    private Vector3 runPosition; 


    // hero state
    public State state = State.Normal;

    private SpriteRenderer renderer;

    // Animation
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetBool("IsPlaying", true);
        originPosition = transform.position;
        StartCoroutine(CoNormalAttack());
    }

    // Update is called once per frame
    void Update()
    {
        // z�� ��ġ ���̷� ������ ���� �ʵ��� �׻� 0���� ����
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        // ��簡 ���ư��� �� ���� �׻� ���
        if(Vector3.Magnitude(transform.position - originPosition) < 2.0f && state != State.Return)
            originPosition += Vector3.up * walkSpeed * Time.deltaTime;
        else if(state == State.Return) // ���ߴٰ� originPosition���� ��ȯ���� ���
            originPosition += Vector3.up * walkSpeed * 0.5f * Time.deltaTime;

        
        if (state == State.Normal) // �Ϲ����� ��� ������ ���� ��
        {
            transform.position = originPosition;
        }
        else if(state == State.Avoid) // ���͸� ���� ��� 
        {
            transform.position += avoidPosition;
            
            // ���⿡ ���� �ִϸ��̼� ������
            if (avoidPosition.x > 0)
                renderer.flipX = true;
            else 
                renderer.flipX = false;
        }
        else if(state == State.Return) // ������Ȳ�� �Ǿ� ��ȯ�ϴ� ���
        {
            transform.position += (originPosition - transform.position)/*.normalized*/ * walkSpeed * Time.deltaTime;
            if (Vector3.Magnitude(transform.position - originPosition) < 0.2f)
                state = State.Normal;
        }
        else if(state == State.Hit) // ���Ϳ��� ���� ���
        {
            transform.position += runPosition;

            // ���⿡ ���� �ִϸ��̼� ������
            if (runPosition.x > 0)
                renderer.flipX = true;
            else
                renderer.flipX = false;
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "S_Monster" && state != State.Hit)
        {
            // hit by monster
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster)
            {
                // ȿ���� ���
                if (monster.type == Monster.Type.Q) AudioManager.instance.PlaySfx(AudioManager.Sfx.Q_hit);
                else if (monster.type == Monster.Type.W) AudioManager.instance.PlaySfx(AudioManager.Sfx.W_hit);
                else if (monster.type == Monster.Type.E) AudioManager.instance.PlaySfx(AudioManager.Sfx.E_hit);
                else if (monster.type == Monster.Type.QW) AudioManager.instance.PlaySfx(AudioManager.Sfx.QW_hit);
                else if (monster.type == Monster.Type.QE) AudioManager.instance.PlaySfx(AudioManager.Sfx.QE_hit);
                else if (monster.type == Monster.Type.WE) AudioManager.instance.PlaySfx(AudioManager.Sfx.WE_hit);

                hp -= monster.power;
            }
            else // ������ ����ü�� ���� ���
            {
                hp -= collision.gameObject.GetComponent<WitchBullet>().power;
/*                Destroy(collision.gameObject);
*/            }

            if (hp <= 0)
            {
                Death();
                return;
            }

            // change state
            ChangeToHit(collision.gameObject);
            StartCoroutine("CoHitEffect");
        }
    }

    public void Death()
    {
        // �÷��̾ �¸� (��� ����)
        //Time.timeScale = 0.0f;
        StartCoroutine(GameManager.instance.GameOver(true));
    }

    // if hit, blink effect
    private IEnumerator CoHitEffect()
    {
        int countTime = 0;

        while(countTime < noHitTime * 10)
        {
            if (countTime % 2 == 0)
                renderer.color = new Color32(255, 255, 255, 90);
            else
                renderer.color = new Color32(255, 255, 255, 180);

            transform.position += runPosition;

            yield return new WaitForSeconds(0.1f);

            countTime++;
        }

        state = State.Avoid;
        renderer.color = new Color32(255, 255, 255, 255);

        yield return null;
    }

    private IEnumerator CoNormalAttack()
    {
        while (true)
        {
            Debug.Log("NormalAttack");
            GameObject normalAttack = Instantiate(normalAttackPref);
            normalAttack.transform.position = transform.position;
            normalAttack.SetActive(true);
            //Destroy(normalAttack, 0.5f);

            yield return new WaitForSeconds(normalAttackTime);
        }
    }

    private IEnumerator CoAxeAttack()
    {
        while (true)
        {
            Debug.Log("AxeAttack");
            GameObject axeAttack = Instantiate(axeAttackPref);
            axeAttack.transform.position = transform.position;
            axeAttack.SetActive(true);

            Destroy(axeAttack, axeDestroyTime);
            yield return new WaitForSeconds(axeAttactTime);
        }
    }

    private IEnumerator CoIceAttack()
    {
        while (true)
        {
            Debug.Log("IceAttack");

            for(int i = 0; i < 6; i++)
            {
                GameObject iceAttack = Instantiate(iceAttackPref);
                iceAttack.transform.position = transform.position;
                iceAttack.transform.rotation = Quaternion.Euler(0, 0, (60.0f * i)-45.0f);
                iceAttack.SetActive(true);
            }

            yield return new WaitForSeconds(iceAttactTime);
        }
    }

    public void GetExp(float exp)
    {
        lv_exp += exp;

        if(level * 10 <= lv_exp)
        {
            lv_exp -= level * 10;
            level++;
            LevelUp();
        }
    }

     void LevelUp()
    {
        // Choose Random Skill Index
        var randomSkillNumber = new List<int>() { 0, 1, 2 };
        
        // �⺻ ��ų�� �ִ�ġ�� ��� ���� �Ұ�
        if (attackPoint[0] == 5)
            randomSkillNumber.Remove(0);

        // �ٸ� ��ų�� �ִ�ġ�� ��� ���� �Ұ�
        for (int i = 1; i < 3; i++)
        {
            if (attackPoint[i] == 6)
                randomSkillNumber.Remove(i);
        }

        // ������ �ø� �� �ִ� ��ų�� �ִ� ��� ��ų ��ȭ
        if (randomSkillNumber.Count > 0)
        {
            int randomNumber = randomSkillNumber.OrderBy(_ => Guid.NewGuid()).First();
            ++attackPoint[randomNumber];

            // ������� ���� ����� ��� ��� ����
            if(attackPointFlag[randomNumber] == false)
            {
                attackPointFlag[randomNumber] = true;

                switch (randomNumber)
                {
                    case 1:
                        StartCoroutine(CoAxeAttack());
                        break;
                    case 2:
                        StartCoroutine(CoIceAttack());
                        break;
                }
            }
        }
    }

    // ���� ���� ��� ��� ��ų ����
    public void OpenAllSkill()
    {
        hasShield = false;
        for(int i = 0; i < 3; i++)
        {
            if (attackPointFlag[i]) continue;

            attackPointFlag[i] = true;

            switch (i)
            {
                case 1:
                    StartCoroutine(CoAxeAttack());
                    break;
                case 2:
                    StartCoroutine(CoIceAttack());
                    break;
            }
        }
    }

    public void ChangeToAvoid(GameObject monster)
    {
        // ���Ͱ� ���ϴ� ���⿡�� 90�� ����
        Vector3 avoidDir = Quaternion.Euler(new Vector3(90, 90, 90)) * (monster.transform.position - transform.position);
        //var dir = new Vector3(avoidDir.z, avoidDir.y, avoidDir.x);
        avoidPosition = avoidDir/*.normalized */* walkSpeed * Time.deltaTime;
        state = State.Avoid;
        animator.SetBool("IsBlocked", true);
    }

    public void ChangeToReturn()
    {
        state = State.Return;
        animator.SetBool("IsBlocked", false);
    }

    public void ChangeToHit(GameObject monster)
    {
        state = State.Hit;
        animator.SetBool("IsBlocked", true);

        // ���Ͱ� ���ϴ� ���⿡�� 90�� ����
        Vector3 runDir = Quaternion.Euler(new Vector3(90, 90, 90)) * (monster.transform.position - transform.position);
        //var dir = new Vector3(runDir.z, runDir.y, runDir.x);
        runPosition = runDir.normalized * walkSpeed * 3 * Time.deltaTime;
    }
}
