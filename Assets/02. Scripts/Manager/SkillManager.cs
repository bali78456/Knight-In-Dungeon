using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public List<PlayerSkillBase> UsingSKills { get; set; } = new List<PlayerSkillBase>();

    private int m_max_using_skill = 4;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddSkill<T>() where T : PlayerSkillBase, new() // �������� ��ų �߰�
    {
        if (UsingSKills.Count < m_max_using_skill)
        {
            PlayerSkillBase new_skill = new T();
            UsingSKills.Add(new_skill);
            Debug.Log($"{typeof(T).Name} ��ų �߰�");
        }
    }

    public void UseSkills()
    {
        if (UsingSKills == null) return;

        foreach(var skill in UsingSKills)
        {
            skill.UseSKill();
        }
    }

}
