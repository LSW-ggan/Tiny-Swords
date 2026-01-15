using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class EnhanceUI : MonoBehaviour {
    public static EnhanceUI Instance;

    // 타겟 아이템
    private InventoryItem _item;
    private GearItem _targetItem;

    // 타켓 아이템 강화 관련 정보
    public Image TargetIcon;
    public Image MaterialIcon;
    public TMP_Text StatIncreaseText;
    public TMP_Text MaterialText;
    public TMP_Text GoldText;
    public TMP_Text EnhanceLevelText;
    public TMP_Text PercentageText;
    private float _successRate;
    private bool _isEnhancing = false;

    // 강화 재료 정보
    public GameObject MaterialSlot1;
    public GameObject MaterialSlot2;
    private EnhanceMaterialItem _materialData = null;
    private int _requiredMaterialCount = 0;
    private int _requiredGold;

    // 강화 진행 UI
    public GameObject ProgressUI;
    public Image ProgressBar;
    public Image ProgressItemIcon;
    public TMP_Text ProgressItemText;
    public Button ProgressButton;
    public TMP_Text ProgressButtonText;
    public Image ProgressEffectImage;
    public Button EnhanceButton;

    // 컴포넌트 변수
    private Animator _animator;

    // 강화 사운드
    public AudioClip SuccessSoundClip;
    public AudioClip FailureSoundClip;

    private void Awake() {
        Instance = this;
        ClearTarget();
    }

    private void Start() {
        _animator = ProgressEffectImage.GetComponent<Animator>();
        ProgressButton.onClick.AddListener(OnClickCancelButton);
    }

    public void SetTarget(InventoryItem item) {
        _materialData = null;

        // 장비인지 확인
        if (item == null) {
            return;
        }
        if (item.Data is not GearItem) {
            return;
        }

        _item = item;
        _targetItem = (GearItem)item.Data;

        // 타겟 슬롯 UI 갱신
        TargetIcon.enabled = true;
        TargetIcon.sprite = item.Data.Icon;
        EnhanceLevelText.SetText($"+{item.EnhanceLevel} {item.Data.Name}");

        // 스탯 상승량 텍스트 UI 갱신
        string text = "";
        if (_item.EnhanceLevel == _targetItem.MaxEnhancementLevel) {
            text = "최대 강화 레벨입니다.";
            EnhanceButton.interactable = false;

            // 재료 슬롯 UI 갱신
            MaterialIcon.enabled = false;
            MaterialText.SetText("");

            // 성공 확률 텍스트 UI 갱신
            PercentageText.SetText($"");
        }
        // 강화 성공 시, 예상 스탯 상승량 표시
        else {
            EnhanceButton.interactable = true;
            StatBonusData stat = EnhanceStatProvider.GetBonus(_targetItem.GearType, _item.EnhanceLevel + 1);
            if (stat.MaxHp != 0) {
                text += $"최대 체력 {_targetItem.Hp + _item.EnhanceHp} -> {_targetItem.Hp + _item.EnhanceHp + stat.MaxHp}\n";
            }
            if (stat.Attack != 0) {
                text += $"공격력 {_targetItem.Attack + _item.EnhanceAttack} -> {_targetItem.Attack + _item.EnhanceAttack + stat.Attack}\n";
            }
            if (stat.Defense != 0) {
                text += $"방어력 {_targetItem.Defense + _item.EnhanceDefense} -> {_targetItem.Defense + _item.EnhanceDefense + stat.Defense}\n";
            }
            if (stat.Critical != 0) {
                text += $"크리티컬 {_targetItem.Critical + _item.EnhanceCritical} -> {_targetItem.Critical + _item.EnhanceCritical + stat.Critical}\n";
            }
            if (stat.Balance != 0) {
                text += $"밸런스 {_targetItem.Balance + _item.EnhanceBalance} -> {_targetItem.Balance + _item.EnhanceBalance + stat.Balance}\n";
            }
            if (stat.Speed != 0) {
                text += $"이동속도 {_targetItem.Speed + _item.EnhanceSpeed} -> {_targetItem.Speed + _item.EnhanceSpeed + stat.Speed}\n";
            }

            MaterialSlot1.SetActive(true);
            MaterialSlot2.SetActive(true);

            // 재료 슬롯 UI 갱신
            _materialData = _targetItem.EnhanceMaterial;
            _requiredMaterialCount = _item.EnhanceLevel + 1;
            MaterialIcon.enabled = true;
            MaterialIcon.sprite = _materialData.Icon;
            MaterialText.SetText(_materialData.Name + $"\n{_requiredMaterialCount}개");

            _requiredGold = EnhanceRateTable.GetRequiredGold(_item.EnhanceLevel + 1);
            GoldText.SetText($"{_requiredGold} 골드");

            // 재화가 충분한지 확인
            InventoryItem material = InventoryManager.Instance.FindItem((ItemData)_materialData);
            if (material == null || material.Amount < _requiredMaterialCount || InventoryManager.Instance.Gold < _requiredGold) {
                // 재화가 부족하면 버튼 상호작용 비활성화
                EnhanceButton.interactable = false;
            }
            else {
                // 재화가 충분하면 버튼 상호작용 활성화
                EnhanceButton.interactable = true;
            }

            // 성공 확률 텍스트 UI 갱신
            _successRate = EnhanceRateTable.GetRate(_item.EnhanceLevel + 1);
            PercentageText.SetText($"성공확률 {_successRate * 100}%");
        }
            
        StatIncreaseText.SetText(text);    
    }

    // 타겟 슬롯 비우기
    public void ClearTarget() {
        _targetItem = null;
        TargetIcon.enabled = false;
        EnhanceLevelText.SetText("");
    }

    // 강화하기 버튼 클릭
    public void OnClickEnhanceButton() {
        AudioManager.Instance.PlayButtonSound();
        if (_materialData == null) {
            return;
        }

        // 재화가 충분한지 확인
        InventoryItem material = InventoryManager.Instance.FindItem((ItemData)_materialData);

        if (material != null && material.Amount >= _requiredMaterialCount && InventoryManager.Instance.Gold >= _requiredGold) {
            EnhanceButton.interactable = false;
            ProgressUI.SetActive(true);
            ProgressItemIcon.sprite = TargetIcon.sprite;
            ProgressItemText.SetText($"+{_item.EnhanceLevel} {_item.Data.Name}");
            ProgressEffectImage.enabled = true;
            StartCoroutine(CoEnhanceProgress(material));
        }
    }

    // 강화 진행
    public IEnumerator CoEnhanceProgress(InventoryItem material) {
        _isEnhancing = true;
        ProgressBar.fillAmount = 0.0f;
        ProgressButtonText.SetText("취소");
        // 강화 진행 UI
        while (ProgressBar.fillAmount != 1) {
            if(!_isEnhancing) {
                yield break;
            }
            yield return new WaitForSeconds(0.03f);
            ProgressBar.fillAmount += 0.01f;
        }
        // 강화 결과 표시
        if(_isEnhancing) {
            ProgressButtonText.SetText("확인");
            ProgressButton.interactable = false;
            string resultText = "";
            // 강화 성공
            if (IsSuccess(EnhanceRateTable.GetRate(_item.EnhanceLevel + 1))) {
                _animator.SetTrigger("Success");
                AudioManager.Instance.PlayEffectSound(SuccessSoundClip);
                // 강화 성공 결과를 InventoryItem 및 PlayerDataManager에 반영
                ApplyEnhanceResult();
                resultText = "강화에 성공하였습니다.";
            }
            // 강화 실패
            else {
                _animator.SetTrigger("Fail");
                AudioManager.Instance.PlayEffectSound(FailureSoundClip);
                resultText = "강화에 실패하였습니다.";
            }
            // 강화 재료 소모
            InventoryManager.Instance.Gold -= _requiredGold;
            InventoryManager.Instance.RemoveItem(material, _requiredMaterialCount);

            // 강화 이펙트를 잠시 기다렸다가, 결과 텍스트 출력
            yield return new WaitForSeconds(1.0f);
            ProgressEffectImage.enabled = false;
            ProgressButton.interactable = true;
            ProgressItemText.SetText(resultText);

            InventoryManager.Instance.SaveData();
        }
    }

    // 강화 진행 중, 취소 버튼 클릭 시
    public void OnClickCancelButton() {
        AudioManager.Instance.PlayButtonSound();
        _isEnhancing = false;
        ProgressUI.SetActive(false);
        EnhanceButton.interactable = true;
        SetTarget(_item);
    }

    // 강화 결과 리턴
    private bool IsSuccess(float successRate) {
        return Random.value < successRate; 
    }

    // 강화에 성공한 경우, 결과 적용
    private void ApplyEnhanceResult() {
        // InventoryItem에 강화 스탯 반영
        StatBonusData stat = EnhanceStatProvider.GetBonus(_targetItem.GearType, _item.EnhanceLevel + 1);
        _item.EnhanceLevel++;
        _item.EnhanceHp += stat.MaxHp;
        _item.EnhanceAttack += stat.Attack;
        _item.EnhanceDefense += stat.Defense;
        _item.EnhanceCritical += stat.Critical;
        _item.EnhanceBalance += stat.Balance;
        _item.EnhanceSpeed += stat.Speed;

        // 장착중인 아이템을 강화한 경우, 보너스 스탯에 강화 수치 변화 반영
        if(_item.IsEquipped) {
            PlayerDataManager.Instance.BonusMaxHp += stat.MaxHp;
            PlayerDataManager.Instance.BonusAttack += stat.Attack;
            PlayerDataManager.Instance.BonusDefense += stat.Defense;
            PlayerDataManager.Instance.BonusCritical += stat.Critical;
            PlayerDataManager.Instance.BonusBalance += stat.Balance;
            PlayerDataManager.Instance.BonusSpeed += stat.Speed;
        }
    }
}