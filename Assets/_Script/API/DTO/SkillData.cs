using System;
using System.Collections.Generic;

// 스킬 데이터 DTO
[Serializable]
public class PlayerSkillData {
    public int skillDataId;
    public int level;
}

[Serializable]
public class SkillSlotData {
    public int slotIndex;
    public int skillDataId;     
}

[Serializable]
public class SkillDataBundle {
    public List<PlayerSkillData> skills;
    public List<SkillSlotData> slots;
}