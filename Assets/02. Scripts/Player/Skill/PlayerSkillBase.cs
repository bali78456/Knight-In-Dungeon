using UnityEngine;

public abstract class PlayerSkillBase //�������̽� ���� �߻� Ŭ������ ����
{
    public int Level { get; protected set; } = 1;

    public void LevelUP()
    {
        Level++;
        ApplyLevelUpEffect(Level);
    }

    public abstract void UseSKill();
    protected abstract void ApplyLevelUpEffect(int level);
}
