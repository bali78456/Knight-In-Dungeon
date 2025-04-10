using System.Collections.Generic;
using UnityEngine;

public class SkillSelector : MonoBehaviour
{
    [Header("스킬 선택 UI의 애니메이터")]
    [SerializeField] private Animator m_selector_ui_animator;

    [Header("스킬 선택 슬롯 UI의 부모 트랜스폼")]
    [SerializeField] private Transform m_slot_parent;

    [Header("공격 스킬 슬롯 UI의 부모 트랜스폼")]
    [SerializeField] private Transform m_active_skill_slot_parent;
    private SkillSlot[] m_active_skill_slots;
    public SkillSlot[] ActiveSkillSlots
    {
        get { return m_active_skill_slots; }
    }

    [Header("버프 스킬 슬롯 UI의 부모 트랜스폼")]
    [SerializeField] private Transform m_passive_skill_slot_parent;
    private SkillSlot[] m_passive_skill_slots;
    public SkillSlot[] PassiveSkillSlots
    {
        get { return m_passive_skill_slots; }
    }

    private SkillSelectSlot[] m_skill_select_slots;

    private List<int> m_selected_slots;

    private void Awake()
    {
        m_skill_select_slots = m_slot_parent.GetComponentsInChildren<SkillSelectSlot>();
        m_active_skill_slots = m_active_skill_slot_parent.GetComponentsInChildren<SkillSlot>();
        m_passive_skill_slots = m_passive_skill_slot_parent.GetComponentsInChildren<SkillSlot>();
        
        m_selected_slots = new List<int>();
    }

    private void Start()
    {
        Initialize();
    }

    public void OpenUI()
    {
        GameEventBus.Publish(GameEventType.Selecting);

        m_selector_ui_animator.SetBool("Open", true);

        ActivateSlots();
    }

    public void CloseUI()
    {
        GameEventBus.Publish(GameEventType.Playing);
        
        m_selector_ui_animator.SetBool("Open", false);

        Initialize();
    }

    private void ActivateSlots()
    {
        Initialize();

        if(CheckAllSlotsAreFull() is false)
        {
            Debug.Log("진입1");
            if(CheckActiveSlotsAreFull())
            {
                SetRandomSlotsExceptNonSelectActive();
            }
            else if(CheckPassiveSlotsAreFull())
            {
                SetRandomSlotsExceptNonSelectPassive();
            }
            else
            {
                SetRandomSkills();
            }
        }
        else
        {
            Debug.Log("진입2");
            if(CheckAllSlotsAreMax() is false)
            {
                if(CheckAnySkillsAreMax() is false)
                {
                    SetNarrowRandomSkills();
                }
                else
                {
                    if(CheckCantSelectThree() is false)
                    {
                        SetMoreNarrowRandomSlots();
                    }
                    else
                    {
                        SetNarrowSlots();
                    }
                }
            }
            else
            {
                GameManager.Instance.Player.UpdateHP(GameManager.Instance.Player.OriginStat.HP);
                CloseUI();
            }
        }
    }

    private void SetRandomSlotsExceptNonSelectActive()
    {
        List<SkillSelectSlot> skill_select_list = new List<SkillSelectSlot>();

        foreach(SkillSelectSlot select_slot in m_skill_select_slots)
        {
            if(select_slot.Skill.Type is SkillType.Active)
            {
                foreach(SkillSlot slot in ActiveSkillSlots)
                {
                    if(select_slot.Skill.ID == slot.Skill.m_id)
                    {
                        skill_select_list.Add(select_slot);
                    }
                }
            }
            else
            {
                skill_select_list.Add(select_slot);
            }
        }

        while(m_selected_slots.Count < 3)
        {
            int idx = UnityEngine.Random.Range(0,skill_select_list.Count);

            if(m_selected_slots.Contains(idx))
            {
                continue;
            }

            if(skill_select_list[idx].Base is not null)
            {
                if(skill_select_list[idx].Skill.Type is SkillType.Active)
                {
                    if(skill_select_list[idx].Base.Level >= 6)
                    {
                        continue;
                    }
                }
                else
                {
                    if(skill_select_list[idx].Base.Level >= 5)
                    {
                        continue;
                    }
                }
            }

            m_selected_slots.Add(idx);
        }

        foreach(int idx in m_selected_slots)
        {
            skill_select_list[idx].Initialize();

            if(skill_select_list[idx].Base is not null)
            {
                if(skill_select_list[idx].Skill.Type is SkillType.Active)
                {
                    if(skill_select_list[idx].Base.Level == 5)
                    {
                        foreach(SkillSlot slot in PassiveSkillSlots)
                        {
                            if(slot.Skill is null)
                            {
                                break;
                            }

                            if(skill_select_list[idx].Skill.Combination.ID == slot.Skill.m_id)
                            {
                                skill_select_list[idx].ChangeToEvolution();
                            }
                        }
                    }
                }
            }

            skill_select_list[idx].gameObject.SetActive(true);
        }
    }

    private void SetRandomSlotsExceptNonSelectPassive()
    {
        List<SkillSelectSlot> skill_select_list = new List<SkillSelectSlot>();

        foreach(SkillSelectSlot select_slot in m_skill_select_slots)
        {
            if(select_slot.Skill.Type is SkillType.Active)
            {
                skill_select_list.Add(select_slot);
            }
            else
            {
                foreach(SkillSlot slot in PassiveSkillSlots)
                {
                    if(select_slot.Skill.ID == slot.Skill.m_id)
                    {
                        skill_select_list.Add(select_slot);
                    }
                }                
            }
        }

        while(m_selected_slots.Count < 3)
        {
            int idx = UnityEngine.Random.Range(0,skill_select_list.Count);

            if(m_selected_slots.Contains(idx))
            {
                continue;
            }

            if(skill_select_list[idx].Base is not null)
            {
                if(skill_select_list[idx].Skill.Type is SkillType.Active)
                {
                    if(skill_select_list[idx].Base.Level >= 6)
                    {
                        continue;
                    }
                }
                else
                {
                    if(skill_select_list[idx].Base.Level >= 5)
                    {
                        continue;
                    }
                }
            }

            m_selected_slots.Add(idx);
        }

        foreach(int idx in m_selected_slots)
        {
            skill_select_list[idx].Initialize();

            if(skill_select_list[idx].Base is not null)
            {
                if(skill_select_list[idx].Skill.Type is SkillType.Active)
                {
                    if(skill_select_list[idx].Base.Level == 5)
                    {
                        foreach(SkillSlot slot in PassiveSkillSlots)
                        {
                            if(slot.Skill is null)
                            {
                                break;
                            }

                            if(skill_select_list[idx].Skill.Combination.ID == slot.Skill.m_id)
                            {
                                skill_select_list[idx].ChangeToEvolution();
                            }
                        }
                    }
                }
            }

            skill_select_list[idx].gameObject.SetActive(true);
        }
    }

    private bool CheckAllSlotsAreFull()
    {
        if(!CheckActiveSlotsAreFull())
        {
            return false;
        }

        if(!CheckPassiveSlotsAreFull())
        {
            return false;
        }

        return true;
    }

    private bool CheckActiveSlotsAreFull()
    {
        foreach(SkillSlot slot in ActiveSkillSlots)
        {
            if(slot.Skill is null)
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckPassiveSlotsAreFull()
    {
        foreach(SkillSlot slot in PassiveSkillSlots)
        {
            if(slot.Skill is null)
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckAllSlotsAreMax()
    {
        foreach(SkillSlot slot in ActiveSkillSlots)
        {
            foreach(SkillSelectSlot select_slot in m_skill_select_slots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    if(select_slot.Base.Level < 6)
                    {
                        return false;
                    }
                }
            }
        }

        foreach(SkillSlot slot in PassiveSkillSlots)
        {
            foreach(SkillSelectSlot select_slot in m_skill_select_slots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    if(select_slot.Base.Level < 5)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool CheckAnySkillsAreMax()
    {
        List<SkillSelectSlot> m_skill_select_list = new List<SkillSelectSlot>();

        foreach(SkillSelectSlot select_slot in m_skill_select_slots)
        {
            foreach(SkillSlot slot in ActiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }

            foreach(SkillSlot slot in PassiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }
        }

        foreach(SkillSelectSlot select_slot in m_skill_select_list)
        {
            if(select_slot.Skill.Type == SkillType.Active)
            {
                if(select_slot.Base.Level >= 6)
                {
                    return true;
                }
            }
            else
            {
                if (select_slot.Base == null) Debug.LogError("select_slot.Base 가 널임");
                if(select_slot.Base.Level >= 5)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SetNarrowRandomSkills()
    {
        List<SkillSelectSlot> m_skill_select_list = new List<SkillSelectSlot>();

        foreach(SkillSelectSlot select_slot in m_skill_select_slots)
        {
            foreach(SkillSlot slot in ActiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }

            foreach(SkillSlot slot in PassiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }
        }

        while(m_selected_slots.Count < 3)
        {
            int idx = UnityEngine.Random.Range(0, m_skill_select_list.Count);

            if(m_selected_slots.Contains(idx))
            {
                continue;
            }

            m_selected_slots.Add(idx);
        }

        foreach(int idx in m_selected_slots)
        {
            m_skill_select_list[idx].Initialize();

            if(m_skill_select_slots[idx].Skill.Type is SkillType.Active)
            {
                if(m_skill_select_list[idx].Base.Level == 5)
                {
                    foreach(SkillSlot slot in PassiveSkillSlots)
                    {
                        if(slot.Skill is null)
                        {
                            break;
                        }

                        if(m_skill_select_list[idx].Skill.Combination.ID == slot.Skill.m_id)
                        {
                            m_skill_select_list[idx].ChangeToEvolution();
                        }
                    }
                }
            }

            m_skill_select_list[idx].gameObject.SetActive(true);
        }
    }

    private bool CheckCantSelectThree()
    {
        List<SkillSelectSlot> m_skill_select_list = new List<SkillSelectSlot>();

        foreach(SkillSelectSlot select_slot in m_skill_select_slots)
        {
            foreach(SkillSlot slot in ActiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }

            foreach(SkillSlot slot in PassiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }
        }

        int max_level_count = 0;
        foreach(SkillSelectSlot select_slot in m_skill_select_list)
        {
            if(select_slot.Skill.Type is SkillType.Active)
            {
                if(select_slot.Base.Level >= 6)
                {
                    max_level_count++;
                }
            }
            else
            {
                if(select_slot.Base.Level >= 5)
                {
                    max_level_count++;
                }
            }
        }

        if(max_level_count > 5)
        {
            return true;
        }

        return false;
    }

    private void SetRandomSkills()
    {
        while(m_selected_slots.Count < 3)
        {
            int idx = UnityEngine.Random.Range(0, m_skill_select_slots.Length);

            if(m_selected_slots.Contains(idx))
            {
                continue;
            }

            if(m_skill_select_slots[idx].Base != null)
            {
                if(m_skill_select_slots[idx].Skill.Type is SkillType.Active)
                {
                    if(m_skill_select_slots[idx].Base.Level >= 6)
                    {
                        continue;
                    }
                }
                else
                {
                    if(m_skill_select_slots[idx].Base.Level >= 5)
                    {
                        continue;
                    }
                }
            }

            m_selected_slots.Add(idx);
        }

        foreach(int idx in m_selected_slots)
        {
            m_skill_select_slots[idx].Initialize();

            if(m_skill_select_slots[idx].Skill.Type is SkillType.Active)
            {
                if(m_skill_select_slots[idx].Base != null)
                {
                    if(m_skill_select_slots[idx].Base.Level == 5)
                    {
                        foreach(SkillSlot slot in PassiveSkillSlots)
                        {
                            if(slot.Skill is null)
                            {
                                break;
                            }

                            if(slot.Skill.m_id == m_skill_select_slots[idx].Skill.Combination.ID)
                            {
                                m_skill_select_slots[idx].ChangeToEvolution();
                            }
                        }
                    }
                }
            }

            m_skill_select_slots[idx].gameObject.SetActive(true);
        }
    }

    private void SetNarrowSlots()
    {
        List<SkillSelectSlot> m_skill_select_list = new List<SkillSelectSlot>();

        foreach(SkillSelectSlot select_slot in m_skill_select_slots)
        {
            foreach(SkillSlot slot in ActiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }

            foreach(SkillSlot slot in PassiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }
        }

        List<SkillSelectSlot> m_final_select_list = new List<SkillSelectSlot>();
        foreach(SkillSelectSlot select_slot in m_skill_select_list)
        {
            if(select_slot.Skill.Type is SkillType.Active)
            {
                if(select_slot.Base.Level < 6)
                {
                    m_final_select_list.Add(select_slot);
                }
            }
            else
            {
                if(select_slot.Base.Level < 5)
                {
                    m_final_select_list.Add(select_slot);
                }
            }
        }

        foreach(SkillSelectSlot select_slot in m_final_select_list)
        {
            select_slot.Initialize();


            if(select_slot.Skill.Type is SkillType.Active)
            {
                if(select_slot.Base.Level == 5)
                {
                    foreach(SkillSlot slot in PassiveSkillSlots)
                    {
                        if(slot.Skill is null)
                        {
                            break;
                        }

                        if(select_slot.Skill.Combination.ID == slot.Skill.m_id)
                        {
                            select_slot.ChangeToEvolution();
                        }
                    }
                }
            }

            select_slot.gameObject.SetActive(true);
        }
    }

    private void SetMoreNarrowRandomSlots()
    {
        List<SkillSelectSlot> m_skill_select_list = new List<SkillSelectSlot>();

        foreach(SkillSelectSlot select_slot in m_skill_select_slots)
        {
            foreach(SkillSlot slot in ActiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }

            foreach(SkillSlot slot in PassiveSkillSlots)
            {
                if(slot.Skill.m_id == select_slot.Skill.ID)
                {
                    m_skill_select_list.Add(select_slot);
                }
            }
        }

        List<SkillSelectSlot> m_final_select_list = new List<SkillSelectSlot>();
        foreach(SkillSelectSlot select_slot in m_skill_select_list)
        {
            if(select_slot.Skill.Type is SkillType.Active)
            {
                if(select_slot.Base.Level < 6)
                {
                    m_final_select_list.Add(select_slot);
                }
            }
            else
            {
                if(select_slot.Base.Level < 5)
                {
                    m_final_select_list.Add(select_slot);
                }
            }
        }

        while(m_selected_slots.Count < 3)
        {
            int idx = UnityEngine.Random.Range(0, m_final_select_list.Count);

            if(m_selected_slots.Contains(idx))
            {
                continue;
            }

            m_selected_slots.Add(idx);
        }

        foreach(int idx in m_selected_slots)
        {
            m_final_select_list[idx].Initialize();


            if(m_final_select_list[idx].Skill.Type is SkillType.Active)
            {
                if(m_final_select_list[idx].Base.Level == 5)
                {
                    foreach(SkillSlot slot in PassiveSkillSlots)
                    {
                        if(slot.Skill is null)
                        {
                            break;
                        }

                        if(m_final_select_list[idx].Skill.Combination.ID == slot.Skill.m_id)
                        {
                            m_final_select_list[idx].ChangeToEvolution();
                        }
                    }
                }
            }

            m_final_select_list[idx].gameObject.SetActive(true);
        }
    }

    private void DeactiveSlots()
    {
        foreach(SkillSelectSlot slot in m_skill_select_slots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    private void Initialize()
    {
        DeactiveSlots();
        m_selected_slots.Clear();
    }
}