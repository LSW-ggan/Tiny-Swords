public static class EnhanceStatProvider {
	public static StatBonusData GetBonus(GearItem.GearItemType gearType, int enhanceLevel) {
		switch (gearType) {
			// 무기
			case GearItem.GearItemType.Sword:
				return EnhanceWeaponTable.GetBonus(enhanceLevel);
			// 방어구 (부위 여러 개)
			case GearItem.GearItemType.Helmet:
			case GearItem.GearItemType.Armor:
			case GearItem.GearItemType.Greaves:
			case GearItem.GearItemType.Gauntlet:
			case GearItem.GearItemType.Shoes:
			case GearItem.GearItemType.Shield:
				return EnhanceArmorTable.GetBonus(enhanceLevel);
			// 장신구
			case GearItem.GearItemType.Ring:
				return EnhanceAccessoriesTable.GetBonus(enhanceLevel);
			default:
				return null;
		}
	}
}