using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class SkillManager : MonoBehaviour {
    public static SkillManager Instance;

    // 플레이어가 습득 가능한 스킬 셋 SO
    public SkillSet PlayerSkillSet;

    // 스킬 슬롯 UI
    public SkillData[] SkillSlots = new SkillData[4];

    // 스킬 슬롯 변화 시, 이벤트
    public event Action OnSkillSlotsChanged;

    // 각 스킬의 레벨 데이터
    private int[] _skillLevels;

    // 활성화된 스킬 상태 관리 리스트
    private Skill[] _runtimeSkillStatus;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        OnSkillSlotsChanged += RefreshSkillSlots;
    }

    private void OnEnable() {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy() {
        if (Instance == this) Instance = null;
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    // 스킬 슬롯에 SkillData에 해당하는 스킬 등록
    public void SetSkillSlot(int index, SkillData data) {
        SkillSlots[index] = data;

        OnSkillSlotsChanged?.Invoke();
    }

    // 스킬 슬롯 갱신
    private void RefreshSkillSlots() {
        GameObject player = GameObject.FindWithTag("Player");
        for(int i  = 0; i < SkillSlots.Length; i++) {
            if (SkillSlots[i] == null) {
                continue;
            }
        }
    }

    // 스킬 레벨 Getter
    public int GetLevel(SkillData data) => _skillLevels[data.Id];
    public int GetLevel(int id) => _skillLevels[id];

    // 스킬 레벨 Setter
    public void SetLevel(SkillData data, int level) {
        _skillLevels[data.Id] = Mathf.Clamp(level, 0, data.MaxLevel);
    }

    // 스킬 사용
    public void UseSkill(int slotNum, GameObject player) {
        if (SkillSlots[slotNum] == null) return;

        SkillData data = SkillSlots[slotNum];
        int id = data.Id;

        // _runtimeSkill 리스트에 등록되어 있지 않은(시전한 적 없는) 스킬이라면 인스턴스를 생성하여 리스트에 넣어줌
        if (_runtimeSkillStatus[id] == null) {
            _runtimeSkillStatus[id] = CreateSkillInstance(data);
        }
        // 해당 인스턴스를 참조하여 스킬 시전
        Skill skill = _runtimeSkillStatus[id];

        if (skill.SkillCanUse(player)) {
            skill.SkillUse(player);
        }
    }

    // 스킬 인스턴스 생성
    private Skill CreateSkillInstance(SkillData data) {
        SkillSet.SkillType type = data.type;
        switch(type) {
            case SkillSet.SkillType.Heal:
                return new Heal(data);
            case SkillSet.SkillType.Aura:
                return new Aura(data);
            case SkillSet.SkillType.DashAttack:
                return new DashAttack(data);
            case SkillSet.SkillType.Sanctuary:
                return new Sanctuary(data);
            case SkillSet.SkillType.DoubleAttack:
                return new DoubleAttack(data);
            default:
                return null;
        }
    }

    // 스킬 쿨타임 계산
    public float GetCoolTimeRemaining(SkillData data) {
        Skill skill = _runtimeSkillStatus[data.Id];

        if (skill == null) return 0;

        float remainingTime = (skill.LastUsedTime + data.CoolTime) - Time.time;
        return Mathf.Max(0, remainingTime);
    }

    // 씬 전환 시, 스킬 정보 자동 저장
    private void OnSceneChanged(Scene prevScene, Scene nextScene) {
        if (nextScene.buildIndex == (int)Scenes.BuildNumber.Loading) return;
        SaveData();
    }

    // 서버로부터 받은 스킬 데이터 초기화
    public void InitializeFromServer(SkillDataBundle data) {
        // 초기 레벨 초기화
        _skillLevels = new int[PlayerSkillSet.Skills.Length];
        for (int i = 0; i < _skillLevels.Length; i++) {
            _skillLevels[i] = 0;
        }

        _runtimeSkillStatus = new Skill[PlayerSkillSet.Skills.Length];
        for (int i = 0; i < _runtimeSkillStatus.Length; i++) {
            _runtimeSkillStatus[i] = null;
        }

        int totalLevel = 0;

        // 스킬 데이터 복원
        foreach(var skill in data.skills) {
            _skillLevels[skill.skillDataId] = skill.level;
            totalLevel += skill.level;
        }

        // 남은 스킬 포인트 계산
        PlayerDataManager.Instance.SkillPoint = (PlayerDataManager.Instance.Level - 1) * PlayerDataManager.Instance.SkillPointIncrease - totalLevel;

        // 스킬 슬롯 데이터 복원
        foreach(var slot in data.slots) {
            SkillSlots[slot.slotIndex] = PlayerSkillSet.Skills[slot.skillDataId];
        }
    }

    // 스킬 정보 저장
    public void SaveData(Action<bool> callback = null) {
        SkillDataBundle bundle = new SkillDataBundle { skills = new List<PlayerSkillData>(), slots = new List<SkillSlotData>()};

        // 플레이어 스킬 데이터
        for (int i = 0; i < _skillLevels.Length; i++) {
            if (_skillLevels[i] <= 0) continue;

            bundle.skills.Add(new PlayerSkillData {
                skillDataId = i,
                level = _skillLevels[i]
            });
        }

        // 스킬 슬롯 데이터
        for (int i = 0; i < SkillSlots.Length; i++) {
            if (SkillSlots[i] == null) continue;

            bundle.slots.Add(new SkillSlotData {
                slotIndex = i,
                skillDataId = SkillSlots[i].Id
            });
        }

        StartCoroutine(SaveManager.SaveSkillData(bundle, callback));
    }
}
