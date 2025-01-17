using UnityEngine;

public class E : MonoBehaviour
{
    public static E Instance;
    public bool IsUsable = true; //��ų ��� ���� ���� (��Ÿ�ӿ� ����)
    public float CoolTime = 8f; //��ų ��Ÿ��
    public float SkillDamage = 0; //��ų ���� ������
    public float CalculatingDamage; //���ݷ� ��� �ջ�� ������
    public float UseMp = 50f; //��ų �Ҹ� Mp

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        //���� ���� ��, ��Ÿ�� �ʱ�ȭ
        CoolTime = 0;
    }


    private void Update()
    {
        //��ų ��� �Ұ����� ����
        if (!IsUsable)
        {
            //�ð� ���� ��Ÿ�� ��
            CoolTime -= Time.deltaTime;

            //��Ÿ�� ���� ���� ó��
            CoolTime = Mathf.Clamp(CoolTime, 0, Mathf.Infinity);

            //��Ÿ�� 0 ���ϸ�
            if (CoolTime <= 0)
            {
                //��ų ��� ����
                IsUsable = true;
            }
        }
    }

    public void UseSkill() //��ų ���
    {
        //��ų �Ҹ� Mp��ŭ �÷��̾� Mp �Ҹ�
        Player.Instance.CurrentMp -= UseMp;
        Player.Instance.MpBar.fillAmount = Player.Instance.CurrentMp / Player.Instance.MaxMp;
        Player.Instance.MpText.text = Player.Instance.CurrentMp + "/" + Player.Instance.MaxMp;

        //��Ÿ�� ����
        StartCoolDown();
    }

    private void StartCoolDown() //��Ÿ�� ����
    {
        //��ų ��� �Ұ��� ó��
        IsUsable = false;

        //��Ÿ�� 8�� �ο�
        CoolTime = 8f;
    }
}
