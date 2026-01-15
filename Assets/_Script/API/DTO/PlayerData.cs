// ¿Ø¥÷ µ•¿Ã≈Õ DTO
[System.Serializable]
public class PlayerData {
    public int level;
    public int experience;
    public int gold;
    public int statPoint;
    public LevelData levels;
    public StatData stats;
}

[System.Serializable]
public class LevelData {
    public int attack;
    public int defense;
    public int speed;
    public int critical;
    public int balance;
}

[System.Serializable]
public class StatData {
    public int attack;
    public int defense;
    public float speed;
    public float critical;
    public int balance;
    public int maxHp;
    public int maxMp;
}
