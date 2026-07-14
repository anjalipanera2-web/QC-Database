using Microsoft.Data.SqlClient;

public class MetricBracket
{
    public int Id { get; set; }
    public int MetricIndex { get; set; }
    public decimal? GaugeMin { get; set; }
    public decimal? GaugeMax { get; set; }
    public decimal? WidthMin { get; set; }
    public decimal? WidthMax { get; set; }
    public string Target { get; set; } = "";
    public string USL { get; set; } = "";
    public string LSL { get; set; } = "";
}

public class MetricFieldConfig
{
    public bool HasLSL { get; set; } = true;
    public bool HasTarget { get; set; } = true;
    public bool HasUSL { get; set; } = true;
}

public class SpecRule
{
    public int Id { get; set; }
    public List<string> FilmTypes { get; set; } = new();
    public string SetName { get; set; } = "";
    public string Spec1 { get; set; } = ""; public string Spec2 { get; set; } = "";
    public string Spec3 { get; set; } = ""; public string Spec4 { get; set; } = "";
    public string Spec5 { get; set; } = ""; public string Spec6 { get; set; } = "";
    public List<MetricBracket> MetricBrackets { get; set; } = new();
    public Dictionary<int, MetricFieldConfig> MetricFields { get; set; } = new();

    public static readonly string[] MetricLabels =
    {
        "Roll Weight", "Ultimate Stretch %", "Avg Stretch %", "Unwind", "Metric 5", "Metric 6", "Stretch Force", "Metric 8", "Puncture", "Gel Count"
    };

    public static readonly string[] SpecLabels =
    {
        "Avg Cling", "Puncture Notes", "Spec 3", "Avg Thickness Tolerance", "Haze Lt. Trans.", "Gel Count Notes"
    };

    // Metrics (1-based MetricIndex) whose spec grid depends on gauge only — no width axis.
    public static readonly HashSet<int> GaugeOnlyMetricIndexes = new() { 10 };

    // Metrics (1-based MetricIndex) that default to LSL-only when a rule doesn't configure fields for them yet
    // (a minimum the measurement must meet or exceed — e.g. Puncture).
    private static readonly HashSet<int> LslOnlyDefaultMetrics = new() { 9 };

    // Metrics (1-based MetricIndex) that default to USL-only — a maximum the measurement must not exceed (e.g. Gel Count, Unwind).
    private static readonly HashSet<int> UslOnlyDefaultMetrics = new() { 4, 10 };

    public IEnumerable<MetricBracket> BracketsFor(int metricIndex) =>
        MetricBrackets.Where(b => b.MetricIndex == metricIndex)
            .OrderBy(b => b.GaugeMin ?? decimal.MinValue)
            .ThenBy(b => b.WidthMin ?? decimal.MinValue);

    public MetricFieldConfig FieldConfigFor(int metricIndex) =>
        MetricFields.TryGetValue(metricIndex, out var cfg) ? cfg : DefaultFieldConfig(metricIndex);

    public static MetricFieldConfig DefaultFieldConfig(int metricIndex)
    {
        if (LslOnlyDefaultMetrics.Contains(metricIndex))
            return new MetricFieldConfig { HasLSL = true, HasTarget = false, HasUSL = false };
        if (UslOnlyDefaultMetrics.Contains(metricIndex))
            return new MetricFieldConfig { HasLSL = false, HasTarget = false, HasUSL = true };
        return new MetricFieldConfig();
    }
}

public class SpecRuleService
{
    private readonly string _conn;

    public SpecRuleService(IConfiguration config)
    {
        _conn = config.GetConnectionString("MyDB");
    }

    public async Task<List<SpecRule>> GetRulesForAsync(string filmType)
    {
        var result = new List<SpecRule>();
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using (var cmd = new SqlCommand(@"
            SELECT sr.* FROM SpecRules sr
            JOIN SpecRuleFilmTypes srft ON srft.SpecRuleId = sr.Id
            WHERE srft.FilmTypeName = @FilmType
            ORDER BY sr.SetName", conn))
        {
            cmd.Parameters.AddWithValue("@FilmType", filmType);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRule(reader));
        }

        await AttachFilmTypesAsync(conn, result);
        await AttachMetricBracketsAsync(conn, result);
        await AttachMetricFieldConfigAsync(conn, result);
        return result;
    }

    public async Task<List<SpecRule>> GetAllRulesAsync()
    {
        var result = new List<SpecRule>();
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using (var cmd = new SqlCommand("SELECT * FROM SpecRules ORDER BY SetName", conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                result.Add(ReadRule(reader));
        }

        await AttachFilmTypesAsync(conn, result);
        await AttachMetricBracketsAsync(conn, result);
        await AttachMetricFieldConfigAsync(conn, result);
        return result;
    }

    private static async Task AttachFilmTypesAsync(SqlConnection conn, List<SpecRule> rules)
    {
        var filmTypesByRuleId = new Dictionary<int, List<string>>();
        using var cmd = new SqlCommand("SELECT SpecRuleId, FilmTypeName FROM SpecRuleFilmTypes", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var ruleId = reader.GetInt32(0);
            var filmTypeName = reader.GetString(1);
            if (!filmTypesByRuleId.TryGetValue(ruleId, out var list))
                filmTypesByRuleId[ruleId] = list = new List<string>();
            list.Add(filmTypeName);
        }

        foreach (var rule in rules)
        {
            if (filmTypesByRuleId.TryGetValue(rule.Id, out var list))
            {
                list.Sort();
                rule.FilmTypes = list;
            }
        }
    }

    private static async Task AttachMetricBracketsAsync(SqlConnection conn, List<SpecRule> rules)
    {
        var bracketsByRuleId = new Dictionary<int, List<MetricBracket>>();
        using var cmd = new SqlCommand("SELECT * FROM SpecMetricBrackets", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var ruleId = reader.GetInt32(reader.GetOrdinal("SpecRuleId"));
            var bracket = new MetricBracket
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MetricIndex = reader.GetInt32(reader.GetOrdinal("MetricIndex")),
                GaugeMin = ReadNullableDecimal(reader, "GaugeMin"),
                GaugeMax = ReadNullableDecimal(reader, "GaugeMax"),
                WidthMin = ReadNullableDecimal(reader, "WidthMin"),
                WidthMax = ReadNullableDecimal(reader, "WidthMax"),
                Target = reader["Target"].ToString() ?? "",
                USL = reader["USL"].ToString() ?? "",
                LSL = reader["LSL"].ToString() ?? "",
            };
            if (!bracketsByRuleId.TryGetValue(ruleId, out var list))
                bracketsByRuleId[ruleId] = list = new List<MetricBracket>();
            list.Add(bracket);
        }

        foreach (var rule in rules)
        {
            if (bracketsByRuleId.TryGetValue(rule.Id, out var list))
                rule.MetricBrackets = list;
        }
    }

    private static async Task AttachMetricFieldConfigAsync(SqlConnection conn, List<SpecRule> rules)
    {
        var configByRuleId = new Dictionary<int, Dictionary<int, MetricFieldConfig>>();
        using var cmd = new SqlCommand("SELECT SpecRuleId, MetricIndex, HasLSL, HasTarget, HasUSL FROM SpecMetricFieldConfig", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var ruleId = reader.GetInt32(0);
            var metricIndex = reader.GetInt32(1);
            var cfg = new MetricFieldConfig
            {
                HasLSL = reader.GetBoolean(2),
                HasTarget = reader.GetBoolean(3),
                HasUSL = reader.GetBoolean(4),
            };
            if (!configByRuleId.TryGetValue(ruleId, out var byMetric))
                configByRuleId[ruleId] = byMetric = new Dictionary<int, MetricFieldConfig>();
            byMetric[metricIndex] = cfg;
        }

        foreach (var rule in rules)
        {
            if (configByRuleId.TryGetValue(rule.Id, out var byMetric))
                rule.MetricFields = byMetric;
        }
    }

    private static decimal? ReadNullableDecimal(SqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
    }

    private static SpecRule ReadRule(SqlDataReader reader) => new SpecRule
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        SetName = reader["SetName"].ToString() ?? "",
        Spec1 = reader["Spec1"].ToString() ?? "", Spec2 = reader["Spec2"].ToString() ?? "",
        Spec3 = reader["Spec3"].ToString() ?? "", Spec4 = reader["Spec4"].ToString() ?? "",
        Spec5 = reader["Spec5"].ToString() ?? "", Spec6 = reader["Spec6"].ToString() ?? "",
    };

    public async Task AddRuleAsync(SpecRule rule)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();
        using var transaction = conn.BeginTransaction();

        int ruleId;
        using (var cmd = new SqlCommand(@"
            INSERT INTO SpecRules (SetName, Spec1, Spec2, Spec3, Spec4, Spec5, Spec6)
            VALUES (@SetName, @Spec1, @Spec2, @Spec3, @Spec4, @Spec5, @Spec6);
            SELECT SCOPE_IDENTITY();", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@SetName", rule.SetName);
            cmd.Parameters.AddWithValue("@Spec1", rule.Spec1); cmd.Parameters.AddWithValue("@Spec2", rule.Spec2);
            cmd.Parameters.AddWithValue("@Spec3", rule.Spec3); cmd.Parameters.AddWithValue("@Spec4", rule.Spec4);
            cmd.Parameters.AddWithValue("@Spec5", rule.Spec5); cmd.Parameters.AddWithValue("@Spec6", rule.Spec6);

            ruleId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        await InsertFilmTypesAsync(conn, transaction, ruleId, rule.FilmTypes);
        await InsertBracketsAsync(conn, transaction, ruleId, rule.MetricBrackets);
        await InsertFieldConfigAsync(conn, transaction, ruleId, rule.MetricFields);

        transaction.Commit();
    }

    public async Task UpdateRuleAsync(int id, SpecRule rule)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();
        using var transaction = conn.BeginTransaction();

        using (var cmd = new SqlCommand(@"
            UPDATE SpecRules SET SetName = @SetName,
                Spec1 = @Spec1, Spec2 = @Spec2, Spec3 = @Spec3, Spec4 = @Spec4, Spec5 = @Spec5, Spec6 = @Spec6
            WHERE Id = @Id", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@SetName", rule.SetName);
            cmd.Parameters.AddWithValue("@Spec1", rule.Spec1); cmd.Parameters.AddWithValue("@Spec2", rule.Spec2);
            cmd.Parameters.AddWithValue("@Spec3", rule.Spec3); cmd.Parameters.AddWithValue("@Spec4", rule.Spec4);
            cmd.Parameters.AddWithValue("@Spec5", rule.Spec5); cmd.Parameters.AddWithValue("@Spec6", rule.Spec6);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = new SqlCommand("DELETE FROM SpecRuleFilmTypes WHERE SpecRuleId = @Id", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = new SqlCommand("DELETE FROM SpecMetricBrackets WHERE SpecRuleId = @Id", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = new SqlCommand("DELETE FROM SpecMetricFieldConfig WHERE SpecRuleId = @Id", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        await InsertFilmTypesAsync(conn, transaction, id, rule.FilmTypes);
        await InsertBracketsAsync(conn, transaction, id, rule.MetricBrackets);
        await InsertFieldConfigAsync(conn, transaction, id, rule.MetricFields);

        transaction.Commit();
    }

    private static async Task InsertFilmTypesAsync(SqlConnection conn, SqlTransaction transaction, int ruleId, IEnumerable<string> filmTypes)
    {
        foreach (var filmType in filmTypes)
        {
            using var cmd = new SqlCommand(
                "INSERT INTO SpecRuleFilmTypes (SpecRuleId, FilmTypeName) VALUES (@SpecRuleId, @FilmTypeName)",
                conn, transaction);
            cmd.Parameters.AddWithValue("@SpecRuleId", ruleId);
            cmd.Parameters.AddWithValue("@FilmTypeName", filmType);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task InsertBracketsAsync(SqlConnection conn, SqlTransaction transaction, int ruleId, IEnumerable<MetricBracket> brackets)
    {
        foreach (var bracket in brackets)
        {
            using var cmd = new SqlCommand(@"
                INSERT INTO SpecMetricBrackets (SpecRuleId, MetricIndex, GaugeMin, GaugeMax, WidthMin, WidthMax, Target, USL, LSL)
                VALUES (@SpecRuleId, @MetricIndex, @GaugeMin, @GaugeMax, @WidthMin, @WidthMax, @Target, @USL, @LSL)", conn, transaction);
            cmd.Parameters.AddWithValue("@SpecRuleId", ruleId);
            cmd.Parameters.AddWithValue("@MetricIndex", bracket.MetricIndex);
            cmd.Parameters.AddWithValue("@GaugeMin", (object?)bracket.GaugeMin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GaugeMax", (object?)bracket.GaugeMax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WidthMin", (object?)bracket.WidthMin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WidthMax", (object?)bracket.WidthMax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Target", bracket.Target);
            cmd.Parameters.AddWithValue("@USL", bracket.USL);
            cmd.Parameters.AddWithValue("@LSL", bracket.LSL);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task InsertFieldConfigAsync(SqlConnection conn, SqlTransaction transaction, int ruleId, Dictionary<int, MetricFieldConfig> fieldsByMetric)
    {
        foreach (var (metricIndex, cfg) in fieldsByMetric)
        {
            using var cmd = new SqlCommand(@"
                INSERT INTO SpecMetricFieldConfig (SpecRuleId, MetricIndex, HasLSL, HasTarget, HasUSL)
                VALUES (@SpecRuleId, @MetricIndex, @HasLSL, @HasTarget, @HasUSL)", conn, transaction);
            cmd.Parameters.AddWithValue("@SpecRuleId", ruleId);
            cmd.Parameters.AddWithValue("@MetricIndex", metricIndex);
            cmd.Parameters.AddWithValue("@HasLSL", cfg.HasLSL);
            cmd.Parameters.AddWithValue("@HasTarget", cfg.HasTarget);
            cmd.Parameters.AddWithValue("@HasUSL", cfg.HasUSL);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteRuleAsync(int id)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("DELETE FROM SpecRules WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public static bool TryParseNumber(string? text, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;

        var digits = new string(text.Where(c => char.IsDigit(c) || c == '.').ToArray());
        return decimal.TryParse(digits, out value);
    }

    // Gauge is sometimes typed without a decimal point (e.g. "045" meaning .45) — when that
    // happens, treat the digits as hundredths so it lands in the same scale as brackets typed correctly.
    public static bool TryParseGauge(string? text, out decimal value)
    {
        if (!TryParseNumber(text, out value)) return false;
        if (!text!.Contains('.'))
        {
            value /= 100m;
        }
        return true;
    }

    public static MetricBracket? ResolveMetric(IEnumerable<SpecRule> rules, int metricIndex, string? gaugeText, string? widthText)
    {
        if (!TryParseGauge(gaugeText, out var gauge)) return null;
        if (!TryParseNumber(widthText, out var width)) return null;
        return rules
            .SelectMany(r => r.BracketsFor(metricIndex))
            .FirstOrDefault(b =>
                (b.GaugeMin == null || gauge >= b.GaugeMin) && (b.GaugeMax == null || gauge <= b.GaugeMax) &&
                (b.WidthMin == null || width >= b.WidthMin) && (b.WidthMax == null || width <= b.WidthMax));
    }

    // Narrows a metric's bracket list down to only the rows that actually apply to the entered
    // gauge and the roll width(s) typed so far — instead of showing the whole gauge x width matrix.
    public static List<MetricBracket> FilterRelevant(IEnumerable<MetricBracket> brackets, string? gaugeText, IEnumerable<string?> widthTexts)
    {
        if (!TryParseGauge(gaugeText, out var gauge)) return new List<MetricBracket>();

        var widths = widthTexts
            .Select(w => TryParseNumber(w, out var v) ? (decimal?)v : null)
            .Where(v => v != null)
            .Select(v => v!.Value)
            .Distinct()
            .ToList();

        return brackets.Where(b =>
        {
            var gaugeOk = (b.GaugeMin == null || gauge >= b.GaugeMin) && (b.GaugeMax == null || gauge <= b.GaugeMax);
            if (!gaugeOk) return false;

            // No width axis on this bracket, or no roll width typed anywhere yet — don't filter by width,
            // just show every width bucket for the matched gauge. Once a width is typed, narrow to it.
            var noWidthAxis = b.WidthMin == null && b.WidthMax == null;
            if (noWidthAxis || widths.Count == 0) return true;

            return widths.Any(w => (b.WidthMin == null || w >= b.WidthMin) && (b.WidthMax == null || w <= b.WidthMax));
        }).ToList();
    }

    private static string Trim(decimal v) => v.ToString("0.###");

    public static string FormatRange(decimal? min, decimal? max, string unit = "")
    {
        if (min == null && max == null) return "any";
        if (min == null) return $"{Trim(max.Value)}{unit}";
        if (max == null) return $"{Trim(min.Value)}{unit} or above";
        return $"{Trim(min.Value)}–{Trim(max.Value)}{unit}";
    }
}
