using System.Collections;
using UnityEngine;

public class Q : MonoBehaviour
{
    public static Q Instance;
    public bool IsUsable = true; //��ų ��� ���� ���� (��Ÿ�ӿ� ����)
    public float CoolTime = 3f; //��ų ��Ÿ��
    public float SkillDamage = 100f; //��ų ���� ������
    public float CalculatingDamage; //���ݷ� ��� �ջ�� ������
    public float UseMp = 30f; //��ų �Ҹ� Mp

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        //(�ջ� ������) = (��ų ���� ������) + (�÷��̾� ���ݷ�) + (�÷��̾� �߰� ���ݷ�)
        CalculatingDamage = SkillDamage + Player.Instance.Power + Player.Instance.AddedPower;
        
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

        //��Ÿ�� ����
        StartCoolDown();
    }

    private void StartCoolDown() //��Ÿ�� ����
    {
        //��ų ��� �Ұ��� ó��
        IsUsable = false; 

        //��Ÿ�� 3�� �ο�
        CoolTime = 3f;
    }
}