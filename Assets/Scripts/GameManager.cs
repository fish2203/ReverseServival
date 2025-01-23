using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;




    public Image whitete;
    public Image princesessessee;


    public int qDeathrattle;
    public int wDeathrattle;
    public int eDeathrattle;

    public int qCoin; // q���͸� ��ȯ�ϱ� ���� ����
    public int wCoin; // w���͸� ��ȯ�ϱ� ���� ����
    public int eCoin; // e���͸� ��ȯ�ϱ� ���� ����

    public float rubyDropPer; // �����Ȯ��, 0 ~ 100



    // ������ (�ν����Ϳ��� �Ҵ�)
    public GameObject monsterQpref;
    public GameObject monsterWpref;
    public GameObject monsterEpref;
    public GameObject monsterQWpref;
    public GameObject monsterQEpref;
    public GameObject monsterWEpref;
    public GameObject monsterQWEpref;

    public GameObject rubyPref;



    public GameObject heroObject; // ����
    public J_Hero hero;

    public Image DefeatImage;
    public Image WinImage;

    readonly float startY = 0f; // ���� ���� �� y��ǥ
    readonly float endY = 30f; // ������ ����Ǵ�(���ְ� �ִ�) y��ǥ

    public Camera mainCamera; // ���� ī�޶�

    public Text haveRubyText;
    public Text qCoinText;
    public Text wCoinText;
    public Text eCoinText;
    public RectTransform progressBarTransform;
    public Image[] princesses;
    public GameObject background;

    private Monster.Type activeMonster; // Ȱ��ȭ�� ���� (Ŭ�� �� �ش� ���� ��ȯ)
    public MonsterIcon[] monsterIcons; // �� 7��, Q/W/E/QW/QE/WE/QWE ����

    public Image luciferImage;

    public Text[] needRuby; // QW, QE, WE 3������ �䱸 ���

    public GameObject hpBarMask; // �÷��̾� ü�¹� ����ũ

    public Texture2D defaultCursor;
    public Texture2D clickCursor;
    public Texture2D errorCursor;



    
    [HideInInspector] public int haveRuby; // �������� ���
    [HideInInspector] public float progress; // ���� ���൵ (0 ~ 1)
    private bool is75 = true;

    [HideInInspector] public bool isRightBuff; // ��Ŭ�� �� ����Ǵ� ���� ����



    private List<Monster> monsterPool = new List<Monster>();
    public Monster GetMonster(Monster.Type monsterType)
    {
        // Ǯ�� ���Ͱ� ���ٸ� ���� �ν��Ͻ�ȭ
/*        if (!monsterPool.Any(m => m.type == monsterType))
        {*/
            GameObject newMonsterObj = null;
            switch (monsterType)
            {
                case Monster.Type.Q:
                    newMonsterObj = Instantiate(monsterQpref, transform);
                    break;

                case Monster.Type.W:
                    newMonsterObj = Instantiate(monsterWpref, transform);
                    break;

                case Monster.Type.E:
                    newMonsterObj = Instantiate(monsterEpref, transform);
                    break;

                case Monster.Type.QW:
                    newMonsterObj = Instantiate(monsterQWpref, transform);
                    break;

                case Monster.Type.QE:
                    newMonsterObj = Instantiate(monsterQEpref, transform);
                    break;

                case Monster.Type.WE:
                    newMonsterObj = Instantiate(monsterWEpref, transform);
                    break;

                case Monster.Type.QWE:
                    newMonsterObj = Instantiate(monsterQWEpref, transform);
                    break;

                default:
                    Debug.Log("��");
                    break;
            }
        /*
                    Monster newMonster = newMonsterObj.GetComponent<Monster>();
                    monsterPool.Add(newMonster);
                }*/
        /*
                // ����Ʈ�� ù��° ���͸� ���� �� ��ȯ
                Monster monster = monsterPool.First(m => m.type == monsterType);
                monsterPool.Remove(monster);*/
        /*monster.gameObject.SetActive(true);
        return monster;*/

        newMonsterObj.gameObject.SetActive(true);
        return newMonsterObj.gameObject.transform.GetComponent<Monster>();
    }
    public void ReturnMonster(Monster monster)
    {
        // ���Ͱ� ����Ʈ�� ������ Ȯ�� �� �߰�
        if (!monsterPool.Contains(monster))
        {
            monsterPool.Add(monster);

            monster.gameObject.SetActive(false);
        }
    }



    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        activeMonster = Monster.Type.Q;

        // �ʱ� ��ȭ ����
        haveRuby = 0;

        StartCoroutine(Princess());
    }
    private void Update()
    {
        

        // ���콺 Ŀ�� �ؽ���
        if (showError == null)
        {
            if (Input.GetMouseButton(0))
            {
                Cursor.SetCursor(clickCursor, new Vector2(errorCursor.width / 2, errorCursor.height / 2), CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(defaultCursor, new Vector2(errorCursor.width / 2, errorCursor.height / 2), CursorMode.Auto);
            }
        }

        // ���콺 Ŭ�� ��, ���� Ȱ��ȭ�� ���� ��ȯ
        if (Input.GetMouseButtonDown(0))
        {
            if (activeMonster == Monster.Type.QWE)
            {
                SummonMonster(activeMonster);
            }
            else
            {
                ContinuousSummon();
            }
        }

        // �������� ���, ���� ȭ�鿡 ǥ��
        haveRubyText.text = haveRuby.ToString();
        qCoinText.text = qCoin.ToString();
        wCoinText.text = wCoin.ToString();
        eCoinText.text = eCoin.ToString();
        needRuby[0].color = (haveRuby >= 1)
            ? Color.white
            : Color.red;
        needRuby[1].color = (haveRuby >= 3)
            ? Color.white
            : Color.red;
        needRuby[2].color = (haveRuby >= 5)
            ? Color.white
            : Color.red;

        // ���൵ ���
        float progress = (heroObject.transform.position.y - startY) / (endY - startY);
        // ���൵ ��
        progressBarTransform.sizeDelta = new Vector2(1770.205f * progress, 70f);
        
        // ���൵�� ó������ 75%�� �Ѿ�ٸ�, ���� ���;����� ǥ��
        if (progress > 0.75f)
        {
            luciferImage.color = Color.white;          
        }
        // ���൵�� 100%�� �����ߴٸ�, �й� (��� �¸�)
        if (progress >= 1.0f)
        {
            StartCoroutine(GameOver(false));
        }

        // �÷��̾� ü�¹�
        float hpRatio = hero.hp / 30f; // ���� �÷��̾� ü�°� �����ؾ� ��
        hpBarMask.transform.localScale = new Vector3(0.711f * hpRatio, 0.16f, 1f);
        hpBarMask.transform.localPosition = new Vector3(-0.3555f * (1f - hpRatio), 0f, 0f);

        // Ȱ��ȭ ���� ����
        // �� Ű�� ������ ��, ���ؽð� �� �ٸ� Ű�� ������ QW/QE/WE Ȱ��ȭ
        // �׷��� �ʴٸ� Q/W/E Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.Q))
        {
            activeMonster = (Input.GetKey(KeyCode.LeftShift))
                ? Monster.Type.QW
                : Monster.Type.Q;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            activeMonster = (Input.GetKey(KeyCode.LeftShift))
                ? Monster.Type.QE
                : Monster.Type.W;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            activeMonster = (Input.GetKey(KeyCode.LeftShift))
                ? Monster.Type.WE
                : Monster.Type.E;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (progress > 0.75f)
            {
                activeMonster = Monster.Type.QWE;
            }
        }
        foreach (MonsterIcon icon in monsterIcons)
        {
            // Ȱ��ȭ�� �����ܸ� ������� ����
            icon.SetSprite(icon.myMonsterType == activeMonster);
        }
    }


    public IEnumerator GameOver(bool isWin)
    {
        if (isWin)
        {
            SceneManager.LoadScene("Win");
        }
        else
        {
            whitete.gameObject.SetActive(true);
            float timer = 0f;
            float duration = 1f;
            while (timer < duration)
            {
                whitete.color = new Color(1f, 1f, 1f, timer / duration);

                timer += Time.deltaTime;
                yield return null;
            }

            princesessessee.gameObject.SetActive(true);
            float timerr = 0f;
            float durationr = 1f;
            while (timerr < durationr)
            {
                princesessessee.color = new Color(1f, 1f, 1f, timerr / durationr);

                timerr += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(1.0f);
            while (timerr > 0f)
            {
                princesessessee.color = new Color(1f, 1f, 1f, timerr / durationr);

                timerr -= Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);

            SceneManager.LoadScene("Defeat");
        }
    }

    Coroutine continuousSummon;
    private void ContinuousSummon()
    {
        if (continuousSummon != null)
        {
            StopCoroutine(continuousSummon);
        }

        StartCoroutine(_ContinuousSummon());
    }
    IEnumerator _ContinuousSummon()
    {
        float firstDelay = 0.0f; // ó�� ��ٸ� �ð�
        float interval = 0.1f; // ��ȯ �ð�����

        yield return new WaitForSeconds(firstDelay);

        float timer = interval;
        while (Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                SummonMonster(activeMonster);
                timer -= interval;
            }

            yield return null;
        }

        
    }
    private void SummonMonster(Monster.Type type)
    {
        // ���� ������ ��ȯ �õ� �� ���
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        if (Vector2.Distance(mousePos, heroObject.transform.position) < 2.2f) // <<< �� ���� �ٲ� �Ÿ� ����
        {
            return;
        }

        // ��� ����, ����� ������ �� ���ٸ� ��ȯ ��� �� �������
        bool charged = false;
        if (type == Monster.Type.QWE)
        {
            if (is75 && haveRuby >= 50)
            {
                is75 = false;
                haveRuby -= 50;
                charged = true;
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.Q)
        {
            if (qCoin > 0)
            {
                qCoin--;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Q_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.W)
        {
            if (wCoin > 0)
            {
                wCoin--;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.W_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.E)
        {
            if (eCoin > 0)
            {
                eCoin--;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.E_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.QW)
        {
            if (haveRuby >= 1)
            {
                haveRuby -= 1;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.QW_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.QE)
        {
            if (haveRuby >= 3)
            {
                haveRuby -= 3;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.QE_spawn);
            }
            else
            {
                ShowError();
            }
        }
        else if (type == Monster.Type.WE)
        {
            if (haveRuby >= 5)
            {
                haveRuby -= 5;
                charged = true;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.WE_spawn);
            }
            else
            {
                ShowError();
            }
        }

        if (charged)
        {
            // ��ȯ
            Monster summoningMonster = GetMonster(type);
            summoningMonster.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }       
    }



    Coroutine showError;
    void ShowError()
    {
        Debug.Log("errrroorrror");

        if (showError != null)
        {
            StopCoroutine(showError);
        }

        showError = StartCoroutine(_ShowError());
    }
    IEnumerator _ShowError()
    {
        // ���콺 Ŀ�� ����
        Cursor.SetCursor(errorCursor, new Vector2(errorCursor.width / 2, errorCursor.height / 2), CursorMode.Auto);

        // ȿ���� ���
        AudioManager.instance.PlaySfx(AudioManager.Sfx.NoMoney);

        // ȭ�� ����
        yield return new WaitForSeconds(0.5f);

        showError = null;
    }

    IEnumerator Princess()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            princesses[0].gameObject.SetActive(true);
            princesses[1].gameObject.SetActive(false);

            yield return new WaitForSeconds(0.3f);
            princesses[0].gameObject.SetActive(false);
            princesses[1].gameObject.SetActive(true);
        }
    }


    public IEnumerator BuffMonsters()
    {
        isRightBuff = true;

        int duration = 5; // ���ӽð�(��)

        for (int i = 0; i < duration; i++)
        {
            yield return new WaitForSeconds(1f);

            // ��� ���� ü�� ȸ��
            List<Monster> monsterList = new List<Monster>();
            foreach (Transform child in transform)
            {
                // �ڽ� ������Ʈ���� Monster ������Ʈ�� �ִ��� Ȯ��
                Monster monsterScript = child.GetComponent<Monster>();
                if (monsterScript != null)
                {
                    // Monster ��ũ��Ʈ�� ���� ������Ʈ��� ����Ʈ�� �߰�
                    monsterList.Add(monsterScript);
                }
            }

            foreach (Monster monster in monsterList)
            {
                monster.health += 1;
            }
        }

        isRightBuff = false;
    }

    /*
    // q,w,e�� ������ ���ؽð� ���� �ٸ� Ű�� ������ QW/QE/WE Ȱ��ȭ
    // �� Ű�� ���� ���¿��� ������ �� Ű�� ������ QWE Ȱ��ȭ
    // �׷��� �ʰ� ���ؽð��� �������� Q/W/E Ȱ��ȭ
    private Coroutine inputWait;
    private void InputWait(bool isQ, bool isW, bool isE)
    {
        if (inputWait != null)
        {
            StopCoroutine(inputWait);
        }

        inputWait = StartCoroutine(_InputWait(isQ, isW, isE));
    }
    IEnumerator _InputWait(bool isQ, bool isW, bool isE)
    {
        float duration = 0.005f; // Ȯ���� ���ؽð�
        float timer = 0f;

        while (timer < duration)
        {
            // ���ο� Ű�� ���ȴ��� Ȯ��
            if (Input.GetKey(KeyCode.Q) && !isQ)
            {
                InputWait(true, isW, isE);
            }
            if (Input.GetKey(KeyCode.W) && !isW)
            {
                InputWait(isQ, true, isE);
            }
            if (Input.GetKey(KeyCode.E) && !isE)
            {
                InputWait(isQ, isW, true);
            }

            timer += Time.deltaTime; // ��� �ð� ����
            yield return null;       // ���� �����ӱ��� ���
        }

        // ���ؽð� ���� �� Ű�� ������ �ʾ����Ƿ�, Ȱ��ȭŰ ����
        // ������ Ȯ��
        if (isQ && isW && isE)
        {
            // 75 �������� Ȱ��ȭ �Ұ���
            if (is75) activeMonster = Monster.Type.QWE;
        }
        else if (isQ && isW)
        {
            activeMonster = Monster.Type.QW;
        }
        else if (isQ && isE)
        {
            activeMonster = Monster.Type.QE;
        }
        else if (isW && isE)
        {
            activeMonster = Monster.Type.WE;
        }
        else if (isQ)
        {
            activeMonster = Monster.Type.Q;
        }
        else if (isW)
        {
            activeMonster = Monster.Type.W;
        }
        else if (isE)
        {
            activeMonster = Monster.Type.E;
        }
        Debug.Log("now active : " + activeMonster);
        // ���;����� �ð��� ����
        foreach (MonsterIcon icon in monsterIcons)
        {
            // Ȱ��ȭ�� �����ܸ� ������� ����
            icon.SetSprite(icon.myMonsterType == activeMonster);
        }
    }
    */
}
