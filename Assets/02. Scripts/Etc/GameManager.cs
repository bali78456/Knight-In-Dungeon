using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public BulletPoolManager BulletPool { get; set; }

    private GameEventType m_game_state;
    public GameEventType GameState
    {
        get { return m_game_state; }
        private set { m_game_state = value; }
    }

    private ItemInventory m_item_inventory;
    public ItemInventory Inventory
    {
        get { return m_item_inventory; }
        private set { m_item_inventory = value; }
    }

    private EquipmentInventory m_equipment_inventory;
    public EquipmentInventory Equipment
    {
        get { return m_equipment_inventory; }
        private set { m_equipment_inventory = value; }
    }

    private CalculatedStat m_calculated_stat;
    public CalculatedStat CalculatedStat
    {
        get { return m_calculated_stat; }
        set { m_calculated_stat = value; }
    }

    private PlayerCtrl m_player_ctrl;
    public PlayerCtrl Player
    {
        get { return m_player_ctrl; }
        private set { m_player_ctrl = value; }
    }

    private bool m_can_init = false;

    private new void Awake()
    {
        base.Awake();

        GameEventBus.Subscribe(GameEventType.None, None);
        GameEventBus.Subscribe(GameEventType.Loading, Loading);
        GameEventBus.Subscribe(GameEventType.Waiting, Waiting);

        GameEventBus.Publish(GameEventType.None);
    }

    public void None()
    {
        GameState = GameEventType.None;

        SoundManager.Instance.PlayBGM("Login Background");
    }
    
    public void Loading()
    {
        GameState = GameEventType.Loading;
    }

    public void Waiting()
    {
        GameState = GameEventType.Waiting;

        SoundManager.Instance.PlayBGM("Title Background");

        m_can_init = true;

        Inventory = GameObject.Find("Inventory Manager").GetComponent<ItemInventory>();
        Inventory.Initialize();

        Equipment = m_item_inventory.GetComponent<EquipmentInventory>();
        Equipment.Initialize();
    }

    public void Playing()
    {
        GameState = GameEventType.Playing;

        if(m_can_init)
        {
            m_can_init = false;

            SoundManager.Instance.PlayBGM("Game Background");
            
            Player = GameObject.Find("Player").GetComponent<PlayerCtrl>();
            BulletPool = GameObject.Find("Bullet Pool Manager").GetComponent<BulletPoolManager>();
        }
        else
        {
            if(!SettingManager.Instance.Data.BGM)
            {
                SoundManager.Instance.BGM.UnPause();
            }
        }
    }

    public void Setting()
    {
        GameState = GameEventType.Setting;
    }

    public void Selecting()
    {
        GameState = GameEventType.Selecting;
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            if(GameState == GameEventType.Waiting && DataManager.Instance.Data is not null)
            {
                Inventory?.SaveSlotData();
                Equipment?.SaveSlotData();
                DataManager.Instance.SaveUserData(DataManager.Instance.Data);
            }
        }
    }

    private void OnApplicationQuit()
    {
        if(GameState == GameEventType.Waiting && DataManager.Instance.Data is not null)
        {

            Inventory?.SaveSlotData();
            Equipment?.SaveSlotData();
            DataManager.Instance.SaveUserData(DataManager.Instance.Data);
        }
    }
}
