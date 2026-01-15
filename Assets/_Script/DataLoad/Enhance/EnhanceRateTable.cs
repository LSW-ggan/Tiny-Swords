using System.Collections.Generic;
using System.Globalization;

public static class EnhanceRateTable {
    private static readonly Dictionary<int, EnhanceRateData> _rateTable = new Dictionary<int, EnhanceRateData>();
    private static int _dataStartLine = 3;

    public static void LoadFromCSV(string csvText) {
        _rateTable.Clear();

        if (string.IsNullOrEmpty(csvText)) {
            return;
        }

        string[] lines = csvText.Split('\n');

        for (int i = _dataStartLine; i < lines.Length; i++) {

            if (string.IsNullOrWhiteSpace(lines[i])) {
                continue;
            }

            string[] cols = lines[i].Trim().Split(',');

            int level = int.Parse(cols[0]);
            float rate = float.Parse(cols[1], CultureInfo.InvariantCulture);
            int gold = int.Parse(cols[2]);

            _rateTable[level] = new EnhanceRateData {
                Level = level,
                SuccessRate = rate,
                RequiredGold = gold
            };
        }
    }

    public static float GetRate(int level) {
        if (_rateTable == null) {
            return 0f;
        }
        else {
            return _rateTable[level].SuccessRate;
        }
    }

    public static int GetRequiredGold(int level) {
        if (_rateTable == null) {
            return 0;
        }
        else {
            return _rateTable[level].RequiredGold;
        }
    }
}