using System.Collections.Generic;
using System.Globalization;

public static class EnhanceAccessoriesTable {
    private static Dictionary<int, StatBonusData> _stats = new Dictionary<int, StatBonusData>();
    private static int _dataStartLine = 3;

    public static void LoadFromCSV(string csvText) {
        string[] lines = csvText.Split('\n');

        for (int i = _dataStartLine; i < lines.Length; i++) {
            if (string.IsNullOrWhiteSpace(lines[i])) {
                continue;
            }

            var cols = lines[i].Trim().Split(',');

            int level = int.Parse(cols[0]);

            _stats[level] = new StatBonusData {
                MaxHp = int.Parse(cols[1]),
                Attack = int.Parse(cols[2]),
                Speed = float.Parse(cols[3], CultureInfo.InvariantCulture),
                Defense = int.Parse(cols[4]),
                Critical = float.Parse(cols[5], CultureInfo.InvariantCulture),
                Balance = int.Parse(cols[6]),
            };
        }
    }

    public static StatBonusData GetBonus(int level) {
        return _stats[level];
    }
}