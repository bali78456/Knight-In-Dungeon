using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    public string m_user_id;
    public int m_user_level;
    public float m_user_exp;
    public int m_user_money;
    public List<SlotData> m_item_inventory;
    public List<SlotData> m_equipment_inventory;
    public int m_evolution_level;
    public int m_current_stage;

    public UserData(string user_id)
    {
        m_user_id = user_id;
        m_user_level = 1;
        m_user_exp = 0f;
        m_user_money = 0;
        m_item_inventory = new List<SlotData>();
        m_equipment_inventory = new List<SlotData> {new SlotData(70, ItemType.WEAPON, 0)};
        m_evolution_level = 1;
        m_current_stage = 1;
    }
}