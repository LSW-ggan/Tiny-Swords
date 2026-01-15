using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour {
    public static PlayerDataManager Instance;

    private GameObject _player;

    // 플레이어 상태 변화 이벤트 델리게이트
    public event Action OnPlayerSpawned;
    public event Action OnPlayerDead;

    // 스탯 변화 이벤트 델리게이트
    public event Action OnHpChanged;
    public event Action OnMpChanged;
    public event Action OnStatChanged;
    public event Action OnLevelChanged;
    public event Action OnExpChanged;

    // 플레이어 레벨
    public int PlayerLevel = 1;
    public int AttackLevel = 1;
    public int DefenseLevel = 1;
    public int SpeedLevel = 1;
    public int CriticalLevel = 1;
    public int BalanceLevel = 1;

    // 플레이어 스탯 기본 수치
    private int _baseAttack;
    private int _baseDefense;
    private float _baseSpeed;
    private float _baseCritical;
    private int _baseBalance;
    private int _baseMaxHp;
    private int _baseMaxMp;

    // 플레이어 스택 보너스 수치(장비 착용, 버프 등)
    private int _bonusAttack;
    private int _bonusDefense;
    private float _bonusSpeed;
    private float _bonusCritical;
    private int _bonusBalance;
    private int _bonusMaxHp;
    private int _bonusMaxMp;

    // 체력 및 마나
    private int _currentHp;
    private int _currentMp;

    // 스킬 및 스탯 포인트
    private int _statPoint = 0;
    private int _skillPoint = 0;

    private bool _isDead = false;

    // 플레이어 대시 참조 수치
    private float _dashSpeed = 8f;
    private float _dashDuration = 0.2f;
    private float _dashCoolTime = 1.0f;

    // 레벨업 스탯 증가량
    public int HpIncrease { get; private set; } = 10;
    public int MpIncrease { get; private set; } = 5;
    public int StatPointIncrease { get; private set; } = 3;
    public int SkillPointIncrease { get; private set; } = 1;

    // 경험치 및 레벨별 요구 경험치 테이블
    public List<int> RequireExprienceTable { get; private set; } = new List<int>();
    private int _currentExprience = 0;

    // 오디오 및 이펙트
    public GameObject LevelUpEffect;
    public AudioClip LevelUpSoundClip;

    public GameObject Player {
        get => _player;
        set {
            _player = value;
            OnPlayerSpawned?.Invoke();
        }
    }

    public bool IsDead {
        get => _isDead;
        set {
            _isDead = value;
            if(_isDead) {
                OnPlayerDead?.Invoke();
            }
        }
    }

    public int Level {
        get => PlayerLevel;
        set {
            PlayerLevel = value;
            OnLevelChanged?.Invoke();
        }
    }

    public int Exprience {
        get => _currentExprience;
        set {
            int exp = value;
            if (exp > RequireExprienceTable[Level]) {
                // 한 번에 2레벨 이상 증가하는 경우 처리
                while (exp >= RequireExprienceTable[Level]) {
                    exp -= RequireExprienceTable[Level];
                    Level += 1;
                }
            }
            _currentExprience = exp;
            OnExpChanged?.Invoke();
        }
    }

    public int StatPoint {
        get => _statPoint;
        set {
            _statPoint = value >= 0 ? value : 0;
        }
    }

    public int SkillPoint {
        get => _skillPoint;
        set {
            _skillPoint = value >= 0 ? value : 0;
        }
    }

    public int BaseAttack {
        get => _baseAttack;
        set {
            _baseAttack = value;
            OnStatChanged?.Invoke();
        }
    }

    public int BaseDefense {
        get => _baseDefense;
        set {
            _baseDefense = value;
            OnStatChanged?.Invoke();
        }
    }

    public float BaseSpeed {
        get => _baseSpeed;
        set {
            _baseSpeed = value;
            OnStatChanged?.Invoke();
        }
    }

    public float BaseCritical {
        get => _baseCritical;
        set {
            _baseCritical = value;
            OnStatChanged?.Invoke();
        }
    }

    public int BaseBalance {
        get => _baseBalance;
        set {
            _baseBalance = value;
            OnStatChanged?.Invoke();
        }
    }

    public int BaseMaxHp {
        get => _baseMaxHp;
        set {
            _baseMaxHp = value;
            OnHpChanged?.Invoke();
        }
    }

    public int BaseMaxMp {
        get => _baseMaxMp;
        set {
            _baseMaxMp = value;
            OnMpChanged?.Invoke();
        }
    }

    public int BonusAttack {
        get => _bonusAttack;
        set {
            _bonusAttack = value;
            OnStatChanged?.Invoke();
        }
    }

    public int BonusDefense {
        get => _bonusDefense;
        set {
            _bonusDefense = value;
            OnStatChanged?.Invoke();
        }
    }

    public float BonusSpeed {
        get => _bonusSpeed;
        set {
            _bonusSpeed = value;
            OnStatChanged?.Invoke();
        }
    }

    public float BonusCritical {
        get => _bonusCritical;
        set {
            _bonusCritical = value;
            OnStatChanged?.Invoke();
        }
    }

    public int BonusBalance {
        get => _bonusBalance;
        set {
            _bonusBalance = value;
            OnStatChanged?.Invoke();
        }
    }

    public int BonusMaxHp {
        get => _bonusMaxHp;
        set {
            _bonusMaxHp = value;
            if(_currentHp > MaxHp) {
                _currentHp = MaxHp;
            }
            OnHpChanged?.Invoke();
        }
    }

    public int BonusMaxMp {
        get => _bonusMaxMp;
        set {
            _bonusMaxMp = value;
            if (_currentMp > MaxMp) {
                _currentMp = MaxMp;
            }
            OnMpChanged?.Invoke();
        }
    }

    // 기본 + 보너스 스탯 합산 능력치 Getter
    public int Attack => _baseAttack + _bonusAttack;
    public int Defense => _baseDefense + _bonusDefense;
    public float Speed => _baseSpeed + _bonusSpeed;
    public float Critical => _baseCritical + _bonusCritical;
    public int Balance => _baseBalance + _bonusBalance;
    public int MaxHp => _baseMaxHp + _bonusMaxHp;
    public int MaxMp => _baseMaxMp + _bonusMaxMp;


    public int Hp {
        get => _currentHp;
        set {
            if(value > MaxHp) {
                _currentHp = MaxHp;
            }
            else {
                _currentHp = value;
            }
            OnHpChanged?.Invoke();
        } 
    }

    public int Mp {
        get => _currentMp;
        set {
            if(value > MaxMp) {
                _currentMp = MaxMp;
            }
            else {
                _currentMp = value;
            }
            OnMpChanged?.Invoke();
        }
    }

    public float DashSpeed {
        get => _dashSpeed;
        set {
            _dashSpeed = value;
        }
    }

    public float DashDuration {
        get => _dashDuration;
        set {
            _dashDuration = value;
        }
    }

    public float DashCoolTime {
        get => _dashCoolTime;
        set {
            _dashCoolTime = value;
        }
    }

    private void Awake() {
        _currentHp = _baseMaxHp;
        _currentMp = _baseMaxMp;
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        OnLevelChanged += LevelUp;
        StartCoroutine(CoRecoverMp());
        RequireExprienceTable.Add(0);
        RequireExprienceTable.Add(100);
        for (int i = 2; i < 200; i++) {
            RequireExprienceTable.Add((int)(RequireExprienceTable[i - 1] * 1.5f));
        }
    }

    private void OnEnable() {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy() {
        OnLevelChanged -= LevelUp;
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    // 플레이어 레벨업 처리
    private void LevelUp() {
        _statPoint += StatPointIncrease;
        _skillPoint += SkillPointIncrease;
        _baseMaxHp += HpIncrease;
        _currentHp += HpIncrease;
        _baseMaxMp += MpIncrease;
        _currentMp += MpIncrease;
        _currentExprience = 0;

        Instantiate(LevelUpEffect);
        Vector3 _spawnPos = Player.GetComponent<Transform>().position;
        StatusEffectManager.Instance.CreateFloatingText(_spawnPos + Vector3.up * 1.5f, "Level Up!", TextType.LevelUp, Player);
        AudioManager.Instance.PlayEffectSound(LevelUpSoundClip);
    }

    // 플레이어 마나 자동 회복
    private IEnumerator CoRecoverMp() {
        while (true) {
            yield return new WaitForSeconds(1.0f);
            Mp += 1;
        }
    }

    // 플레이어 데이터 로딩
    public void InitializeFromServer(PlayerData data) {

        PlayerLevel = data.level;
        AttackLevel = data.levels.attack;
        DefenseLevel = data.levels.defense;
        SpeedLevel = data.levels.speed;
        CriticalLevel = data.levels.critical;
        BalanceLevel = data.levels.balance;

        _statPoint = data.statPoint;

        _baseAttack = data.stats.attack;
        _baseDefense = data.stats.defense;
        _baseSpeed = data.stats.speed;
        _baseCritical = data.stats.critical;
        _baseBalance = data.stats.balance;

        _baseMaxHp = data.stats.maxHp;
        _baseMaxMp = data.stats.maxMp;

        _currentHp = _baseMaxHp;
        _currentMp = _baseMaxMp;

        _currentExprience = data.experience;

        InventoryManager.Instance.Gold = data.gold;
    }

    // 씬 전환 시, 플레이어 데이터 자동 저장
    private void OnSceneChanged(Scene prevScene, Scene nextScene) {
        // 게임 진입 로딩창에 들어갈 때에는 저장하지 않도록 함
        if (nextScene.buildIndex == (int)Scenes.BuildNumber.Loading) return;
        SaveData();
    }

    // 플레이어 데이터 저장
    public void SaveData(Action<bool> callback = null) {
        // DTO/PlayerData.cs 참조
        PlayerData data = new PlayerData {
            level = PlayerLevel,
            experience = _currentExprience,
            gold = InventoryManager.Instance.Gold,
            statPoint = _statPoint,

            levels = new LevelData {
                attack = AttackLevel,
                defense = DefenseLevel,
                speed = SpeedLevel,
                critical = CriticalLevel,
                balance = BalanceLevel
            },

            stats = new StatData {
                attack = _baseAttack,
                defense = _baseDefense,
                speed = _baseSpeed,
                critical = _baseCritical,
                balance = _baseBalance,
                maxHp = _baseMaxHp,
                maxMp = _baseMaxMp
            }
        };

        StartCoroutine(SaveManager.SavePlayerData(data, callback));
    }
}
