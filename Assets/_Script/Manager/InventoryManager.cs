using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour {
    public enum InventoryMode {
        Normal,      // 일반
        Trading,     // 상점 이용
        Enhance,     // 강화 이용
    }

    public static InventoryManager Instance;

    private InventoryMode _mode;

    // Prefab 변수
    public ItemLibrary ItemLibrary;
    public GameObject InventoryUIPrefab;
    public GameObject AlertUIPrefab;
    public GameObject ConfirmUIPrefab;
    public GameObject QuantityUIPrefab;

    // 장착 아이템(장비칸) 리스트
    public List<InventoryItem> EquipGears;

    // 인벤토리 아이템 리스트
    public List<InventoryItem> Items;

    // 인벤토리 슬롯 개수
    public int SlotCount = 25;

    // 인벤토리 변화 이벤트
    public event Action OnInventoryChanged;
    public event Action OnGoldChanged;

    // 사운드 클립
    [Header("SoundClip")]
    public AudioClip GearEquipSoundClip;
    public AudioClip UsePotionSoundClip;
    public AudioClip ItemPickUpAudioClip;
    public AudioClip ItemPurchaseAudioClip;

    // 소지금(골드)
    private int _gold = 0;

    public int Gold {
        get => _gold;
        set {
            _gold = value;
            OnGoldChanged?.Invoke();
        }
    }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start() {
        // 착용 가능한 장비 종류 개수만큼의 크기를 가진 EquipGears 리스트 생성 및 초기화
        EquipGears = new List<InventoryItem>();
        for (int i = 0; i < System.Enum.GetValues(typeof(GearItem.GearItemType)).Length; i++) {
            EquipGears.Add(null);
        }

        _mode = InventoryMode.Normal;

        // 인벤토리 최대 슬롯 개수만큼의 크기를 가진 Items 리스트 생성 및 초기화 
        Items = new List<InventoryItem>();

        for (int i = 0; i < SlotCount; i++) {
            Items.Add(null);
        }
    }

    private void OnEnable() {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy() {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    // 인벤토리 모드 Getter
    public InventoryMode GetMode() => _mode;

    // 인벤토리 모드 Setter
    public void SetMode(InventoryMode mode) {
        _mode = mode;
    }

    // 아이템을 인벤토리에 추가
    public void AddItem(ItemData item, int amount) {
        // 슬롯에 겹쳐서 저장할 수 있는 아이템의 경우
        if (item.IsStackable) {
            // 기존 인벤토리에 해당 아이템이 이미 존재하는지 확인
            InventoryItem targetItem = Items.Find(x => x != null && x.Data == item);

            // 기존에 존재하는 경우 개수만 더해줌
            if (targetItem != null) {
                targetItem.Amount += amount;
            }
            // 그렇지 않은 경우 빈 슬롯을 찾아서 저장
            else {
                int index = 0;
                while (Items[index] != null) {
                    index++;
                }
                Items[index] = (new InventoryItem(item, amount));
            }
        }
        // 하나의 슬롯에 겹쳐서 소지할 수 없는 경우(ex : 장비)
        else {
            int index = 0;
            while (Items[index] != null) {
                index++;
            }
            Items[index] = (new InventoryItem(item, amount));
        }

        // 인벤토리 변화 이벤트 델리게이트 Invoke
        OnInventoryChanged?.Invoke();
    }

    // 인벤토리가 가득 찼는지 확인
    public bool IsFull(ItemData item) {
        // 겹칠 수 있는 아이템인 경우
        if (item.IsStackable) {
            int index = 0;
            // 해당 아이템이 인벤토리에 이미 존재하거나, 빈 슬롯을 찾은 경우에는 return false;
            while (Items[index] != null && Items[index].Data.Id != item.Id) {
                index++;
                if (index >= SlotCount) {
                    return true;
                }
            }
        }
        // 겹칠 수 없는 아이템인 경우
        else {
            int index = 0;
            // 빈 슬롯이 없는 경우에는 return false;
            while (Items[index] != null) {
                index++;
                if (index >= SlotCount) {
                    return true;
                }
            }
        }
        return false;
    }

    // 아이템을 재료로 소모하여 개수가 감소하는 경우
    public void RemoveItem(InventoryItem item, int amount) {
        if (item == null || item.Amount < amount) {
            return;
        }
        // 소지 아이템의 개수 감소
        item.Amount -= amount;

        OnInventoryChanged?.Invoke();
    }

    // 바닥에 버리는 경우
    public IEnumerator DropItem(InventoryItem item, int amount) {
        bool isCancel = false;
        int removeAmount = 1;

        // 아이템이 장비중인 경우
        if (item.IsEquipped) {
            GlobalUIManager.Instance.CreateAlertUI("장비 중인 아이템은 버릴 수 없습니다.");
            isCancel = true;
        }
        // 해당 아이템이 Stackable이고, 개수가 여러 개인 경우
        else if (amount != 1) {
            yield return StartCoroutine(GlobalUIManager.Instance.CreateQuantityUI("삭제할 아이템의 수량을 입력해주세요.", item.Amount, (answer, amount) => {
                isCancel = !answer;
                removeAmount = amount;
            }));
        }
        // 버릴 수 없는 경우가 아니면서, UI에서 중간에 취소를 누르지 않았다면
        if (!isCancel) {
            // 확인창 출력
            yield return StartCoroutine(GlobalUIManager.Instance.CreateConfirmUI("정말 버리시겠습니까?\n아이템은 즉시 삭제되며\n복구할 수 없습니다.", (answer) => {
                // 아이템 삭제 처리
                if (answer) {
                    item.Amount -= removeAmount;

                    if (item.Amount <= 0) {
                        int index = Items.IndexOf(item);
                        Items[index] = null;
                    }

                    OnInventoryChanged?.Invoke();
                }
            }));
        }
    }

    // 아이템 검색               
    public InventoryItem FindItem(ItemData item) {
        for (int i = 0; i < SlotCount; i++) {
            if (Items[i] == null) {
                continue;
            }
            if (Items[i].Data == item) {
                return Items[i];
            }
        }
        return null;
    }

    // 아이템 사용
    public void UseItem(InventoryItem item) {
        if (item == null || item.Amount < 1) {
            return;
        }
        // ItemData의 Use메서드 호출
        item.Data.Use(item);
        // 만약 사용 후에 아이템 개수가 줄어서 0이 된 경우 인벤토리에서 지워줌
        if (item.Amount <= 0) {
            int index = Items.IndexOf(item);
            Items[index] = null;
        }

        OnInventoryChanged?.Invoke();
    }

    // 인벤토리의 index번째 슬롯에 들어있는 InventoryItem객체 리턴
    public InventoryItem GetItem(int index) => Items[index];

    public void SlotItemSwap(int currentSlotIndex, int targetSlotIndex) {
        InventoryItem temp = Items[targetSlotIndex];
        Items[targetSlotIndex] = Items[currentSlotIndex];
        Items[currentSlotIndex] = temp;

        OnInventoryChanged?.Invoke();
    }

    // 아이템 판매
    public IEnumerator SellItem(InventoryItem item) {
        bool isCancel = false;
        int sellAmount = 1;
        // 장착 중인 아이템인 경우
        if (item.IsEquipped || item.IsQuickEquipped) {
            GlobalUIManager.Instance.CreateAlertUI("장비 중인 아이템은\n판매할 수 없습니다.");
            isCancel = true;
        }
        // Stackable 이면서 여러개를 소지하고 있는 경우
        else if (item.Amount != 1) {
            yield return StartCoroutine(GlobalUIManager.Instance.CreateQuantityUI("판매할 아이템의 수량을\n입력해주세요.", item.Amount, (answer, amount) => {
                isCancel = !answer;
                sellAmount = amount;
            }));
        }
        // 아이템 판매 확인UI 출력
        else {
            yield return StartCoroutine(GlobalUIManager.Instance.CreateConfirmUI("아이템을 판매하시겠습니까?", (Answer) => {
                isCancel = !Answer;
            }));
        }
        // 위 과정에서 중간에 취소하지 않았다면, 아이템 판매 처리
        if (!isCancel) {
            item.Amount -= sellAmount;
            Gold += (sellAmount * item.Data.SellPrice);
            if (item.Amount <= 0) {
                int index = Items.IndexOf(item);
                Items[index] = null;
            }
            OnInventoryChanged?.Invoke();
        }
    }

    // 장비 장착
    public void RequestEquip(InventoryItem item, int gearType) {
        if (item.IsEquipped) {
            return;
        }

        AudioManager.Instance.PlayEffectSound(GearEquipSoundClip);

        // 기존 장비 해제
        InventoryItem prev = EquipGears[gearType];
        if (prev != null) {
            if (prev.Data is GearItem prevGear) {
                prevGear.Unequip(prev);
            }
            prev.IsEquipped = false;
            EquipGears[gearType] = null;
        }

        // 새로운 장비 장착
        EquipGears[gearType] = item;
        item.IsEquipped = true;

        if (item.Data is GearItem gear) {
            gear.Equip(item);
        }

        OnInventoryChanged?.Invoke();
    }

    // 장비 장착 해제 요청
    public void RequestUnequip(int gearType) {
        InventoryItem item = EquipGears[gearType];
        if (item == null) return;

        // 효과 제거
        if (item.Data is GearItem gear) {
            gear.Unequip(item);
        }

        // 해당 InventoryItem 정보 갱신
        item.IsEquipped = false;

        // 장비칸 비워주기
        EquipGears[gearType] = null;

        OnInventoryChanged?.Invoke();
    }

    // 서버로부터 받아온 데이터로 변수 초기화
    public void InitializeFromServer(List<InventoryItemData> inventoryItems) {
        // 인벤토리 메모리 초기화
        for (int i = 0; i < SlotCount; i++) {
            Items[i] = null;
        }

        // 아이템 슬롯 데이터 복원
        foreach (var data in inventoryItems) {
            ItemData itemData = ItemLibrary.GetItemData(data.itemDataId);

            // 해당 Id를 갖는 SO Item이 없는 경우
            if (itemData == null) {
                Debug.LogError($"{data.itemDataId} 아이템 찾을 수 없음");
                continue;
            }
            // 해당 데이터가 가지고 있는 슬롯 번호가 존재하지 않는 슬롯일 경우
            if (data.slotIndex < 0 || data.slotIndex >= SlotCount) {
                Debug.LogError($"{data.slotIndex}번 슬롯은 존재하지 않음");
                continue;
            }

            // InventoryItem 객체 인스턴스 생성
            InventoryItem item = new InventoryItem(itemData, data.amount) {
                EnhanceLevel = data.enhanceLevel,
                IsEquipped = data.isEquipped,
                QuickSlotIndex = data.quickSlotIndex
            };

            // 원래 슬롯에 넣어주기
            Items[data.slotIndex] = item;

            // 퀵슬롯 복원
            if (item.QuickSlotIndex != -1) {
                QuickSlotManager.Instance.SetItem(item.QuickSlotIndex, item);
            }
        }

        // 장비칸 메모리 초기화
        for (int i = 0; i < EquipGears.Count; i++) {
            EquipGears[i] = null;
        }

        // 장착 상태 복원
        for (int i = 0; i < Items.Count; i++) {
            InventoryItem item = Items[i];
            if (item == null || !item.IsEquipped || item.Data is not GearItem gear) {
                continue;
            }

            int gearType = (int)gear.GearType;

            // 헬멧, 바지, 신발 등 GearType에 맞는 슬롯에 장비 데이터 저장
            EquipGears[gearType] = item;

            // 강화 수치 적용 (강화 레벨에 따른 강화 보너스 스탯을 계산하여 반영)
            item.CalculateEnhanceBonus();

            // 장착 처리하여 플레이어 데이터에 반영
            gear.Equip(item);
        }

        OnInventoryChanged?.Invoke();
    }

    // 씬 전환 시, 자동 저장
    private void OnSceneChanged(Scene prevScene, Scene nextScene) {
        if (nextScene.buildIndex == (int)Scenes.BuildNumber.Loading) return;
        SaveData();
    }

    // 인벤토리 정보 저장
    public void SaveData(Action<bool> callback = null) {
        // 해당 정보에 맞는 DTO 객체에 정보를 담아줌
        InventoryItemBundle bundle = new InventoryItemBundle { inventory = new List<InventoryItemData>() };

        for (int i = 0; i < SlotCount; i++) {
            InventoryItem item = Items[i];
            if (item == null)
                continue;

            bundle.inventory.Add(new InventoryItemData {
                itemDataId = item.Data.Id,
                slotIndex = i,
                amount = item.Amount,
                enhanceLevel = item.EnhanceLevel,
                isEquipped = item.IsEquipped,
                quickSlotIndex = item.QuickSlotIndex
            });
        }

        StartCoroutine(SaveManager.SaveInventoryData(bundle, callback));
    }
}