using Microsoft.Data.SqlClient;
using ProductionSheet.Components.Pages;

public class QCAuditService
{
    private readonly string _conn;

    public QCAuditService(IConfiguration config)
    {
        _conn = config.GetConnectionString("MyDB");
    }

    private static readonly string[] Columns = new[]
    {
        "Date", "Time", "LineNumber", "Shift", "Crew", "LoggedBy", "FormulaCode", "ProductType", "Width", "Charting", "Counter",
        "LSL1", "Target1", "USL1", "LSL2", "Target2", "USL2", "OrderNumber", "NumPallets", "Barcode", "Notify",
        "PalletNum1", "Position1", "Width1", "CoreProtection1", "GrossWeight1", "Weight1_1", "Weight1_2", "Weight1_3", "Weight1_4",
        "PalletNum2", "Position2", "Width2", "CoreProtection2", "GrossWeight2", "Weight2_1", "Weight2_2", "Weight2_3", "Weight2_4",
        "PalletNum3", "Position3", "Width3", "CoreProtection3", "GrossWeight3", "Weight3_1", "Weight3_2", "Weight3_3", "Weight3_4",
        "PalletNum4", "Position4", "Width4", "CoreProtection4", "GrossWeight4", "Weight4_1", "Weight4_2", "Weight4_3", "Weight4_4",
        "PalletNum5", "Position5", "Width5", "CoreProtection5", "GrossWeight5", "Weight5_1", "Weight5_2", "Weight5_3", "Weight5_4",
        "PalletNum6", "Position6", "Width6", "CoreProtection6", "GrossWeight6", "Weight6_1", "Weight6_2", "Weight6_3", "Weight6_4",
        "PalletNum7", "Position7", "Width7", "CoreProtection7", "GrossWeight7", "Weight7_1", "Weight7_2", "Weight7_3", "Weight7_4",
        "PalletNum8", "Position8", "Width8", "CoreProtection8", "GrossWeight8", "Weight8_1", "Weight8_2", "Weight8_3", "Weight8_4",
        "PalletNum9", "Position9", "Width9", "CoreProtection9", "GrossWeight9", "Weight9_1", "Weight9_2", "Weight9_3", "Weight9_4",
        "PalletNum10", "Position10", "Width10", "CoreProtection10", "GrossWeight10", "Weight10_1", "Weight10_2", "Weight10_3", "Weight10_4",
        "PalletNum11", "Position11", "Width11", "CoreProtection11", "GrossWeight11", "Weight11_1", "Weight11_2", "Weight11_3", "Weight11_4",
        "Web", "PackingCode", "Unit", "PalletType", "BoxNum", "Die", "LayersPallet", "Product", "BoxLabel", "QCTestRollPos",
        "RLS", "ExPalletTicket", "PW", "Size", "Condition", "Tail", "FlagComment"
    };

    private static void AddParameters(SqlCommand cmd, StretchFilmReelAuditLog.SheetData sheet)
    {
        cmd.Parameters.AddWithValue("@Date", sheet.Date);
        cmd.Parameters.AddWithValue("@Time", sheet.Time.ToTimeSpan());
        cmd.Parameters.AddWithValue("@LineNumber", sheet.LineNumber);
        cmd.Parameters.AddWithValue("@Shift", sheet.Shift);
        cmd.Parameters.AddWithValue("@Crew", sheet.Crew);
        cmd.Parameters.AddWithValue("@LoggedBy", sheet.LoggedBy);
        cmd.Parameters.AddWithValue("@FormulaCode", sheet.FormulaCode);
        cmd.Parameters.AddWithValue("@ProductType", sheet.ProductType);
        cmd.Parameters.AddWithValue("@Width", sheet.Width);
        cmd.Parameters.AddWithValue("@Charting", sheet.Charting);
        cmd.Parameters.AddWithValue("@Counter", sheet.Counter);
        cmd.Parameters.AddWithValue("@LSL1", sheet.LSL1);
        cmd.Parameters.AddWithValue("@Target1", sheet.Target1);
        cmd.Parameters.AddWithValue("@USL1", sheet.USL1);
        cmd.Parameters.AddWithValue("@LSL2", sheet.LSL2);
        cmd.Parameters.AddWithValue("@Target2", sheet.Target2);
        cmd.Parameters.AddWithValue("@USL2", sheet.USL2);
        cmd.Parameters.AddWithValue("@OrderNumber", sheet.OrderNumber);
        cmd.Parameters.AddWithValue("@NumPallets", sheet.NumPallets);
        cmd.Parameters.AddWithValue("@Barcode", sheet.Barcode);
        cmd.Parameters.AddWithValue("@Notify", sheet.Notify);
        cmd.Parameters.AddWithValue("@PalletNum1", sheet.PalletNum1); cmd.Parameters.AddWithValue("@Position1", sheet.Position1); cmd.Parameters.AddWithValue("@Width1", sheet.Width1); cmd.Parameters.AddWithValue("@CoreProtection1", sheet.CoreProtection1); cmd.Parameters.AddWithValue("@GrossWeight1", sheet.GrossWeight1); cmd.Parameters.AddWithValue("@Weight1_1", sheet.Weight1_1); cmd.Parameters.AddWithValue("@Weight1_2", sheet.Weight1_2); cmd.Parameters.AddWithValue("@Weight1_3", sheet.Weight1_3); cmd.Parameters.AddWithValue("@Weight1_4", sheet.Weight1_4);
        cmd.Parameters.AddWithValue("@PalletNum2", sheet.PalletNum2); cmd.Parameters.AddWithValue("@Position2", sheet.Position2); cmd.Parameters.AddWithValue("@Width2", sheet.Width2); cmd.Parameters.AddWithValue("@CoreProtection2", sheet.CoreProtection2); cmd.Parameters.AddWithValue("@GrossWeight2", sheet.GrossWeight2); cmd.Parameters.AddWithValue("@Weight2_1", sheet.Weight2_1); cmd.Parameters.AddWithValue("@Weight2_2", sheet.Weight2_2); cmd.Parameters.AddWithValue("@Weight2_3", sheet.Weight2_3); cmd.Parameters.AddWithValue("@Weight2_4", sheet.Weight2_4);
        cmd.Parameters.AddWithValue("@PalletNum3", sheet.PalletNum3); cmd.Parameters.AddWithValue("@Position3", sheet.Position3); cmd.Parameters.AddWithValue("@Width3", sheet.Width3); cmd.Parameters.AddWithValue("@CoreProtection3", sheet.CoreProtection3); cmd.Parameters.AddWithValue("@GrossWeight3", sheet.GrossWeight3); cmd.Parameters.AddWithValue("@Weight3_1", sheet.Weight3_1); cmd.Parameters.AddWithValue("@Weight3_2", sheet.Weight3_2); cmd.Parameters.AddWithValue("@Weight3_3", sheet.Weight3_3); cmd.Parameters.AddWithValue("@Weight3_4", sheet.Weight3_4);
        cmd.Parameters.AddWithValue("@PalletNum4", sheet.PalletNum4); cmd.Parameters.AddWithValue("@Position4", sheet.Position4); cmd.Parameters.AddWithValue("@Width4", sheet.Width4); cmd.Parameters.AddWithValue("@CoreProtection4", sheet.CoreProtection4); cmd.Parameters.AddWithValue("@GrossWeight4", sheet.GrossWeight4); cmd.Parameters.AddWithValue("@Weight4_1", sheet.Weight4_1); cmd.Parameters.AddWithValue("@Weight4_2", sheet.Weight4_2); cmd.Parameters.AddWithValue("@Weight4_3", sheet.Weight4_3); cmd.Parameters.AddWithValue("@Weight4_4", sheet.Weight4_4);
        cmd.Parameters.AddWithValue("@PalletNum5", sheet.PalletNum5); cmd.Parameters.AddWithValue("@Position5", sheet.Position5); cmd.Parameters.AddWithValue("@Width5", sheet.Width5); cmd.Parameters.AddWithValue("@CoreProtection5", sheet.CoreProtection5); cmd.Parameters.AddWithValue("@GrossWeight5", sheet.GrossWeight5); cmd.Parameters.AddWithValue("@Weight5_1", sheet.Weight5_1); cmd.Parameters.AddWithValue("@Weight5_2", sheet.Weight5_2); cmd.Parameters.AddWithValue("@Weight5_3", sheet.Weight5_3); cmd.Parameters.AddWithValue("@Weight5_4", sheet.Weight5_4);
        cmd.Parameters.AddWithValue("@PalletNum6", sheet.PalletNum6); cmd.Parameters.AddWithValue("@Position6", sheet.Position6); cmd.Parameters.AddWithValue("@Width6", sheet.Width6); cmd.Parameters.AddWithValue("@CoreProtection6", sheet.CoreProtection6); cmd.Parameters.AddWithValue("@GrossWeight6", sheet.GrossWeight6); cmd.Parameters.AddWithValue("@Weight6_1", sheet.Weight6_1); cmd.Parameters.AddWithValue("@Weight6_2", sheet.Weight6_2); cmd.Parameters.AddWithValue("@Weight6_3", sheet.Weight6_3); cmd.Parameters.AddWithValue("@Weight6_4", sheet.Weight6_4);
        cmd.Parameters.AddWithValue("@PalletNum7", sheet.PalletNum7); cmd.Parameters.AddWithValue("@Position7", sheet.Position7); cmd.Parameters.AddWithValue("@Width7", sheet.Width7); cmd.Parameters.AddWithValue("@CoreProtection7", sheet.CoreProtection7); cmd.Parameters.AddWithValue("@GrossWeight7", sheet.GrossWeight7); cmd.Parameters.AddWithValue("@Weight7_1", sheet.Weight7_1); cmd.Parameters.AddWithValue("@Weight7_2", sheet.Weight7_2); cmd.Parameters.AddWithValue("@Weight7_3", sheet.Weight7_3); cmd.Parameters.AddWithValue("@Weight7_4", sheet.Weight7_4);
        cmd.Parameters.AddWithValue("@PalletNum8", sheet.PalletNum8); cmd.Parameters.AddWithValue("@Position8", sheet.Position8); cmd.Parameters.AddWithValue("@Width8", sheet.Width8); cmd.Parameters.AddWithValue("@CoreProtection8", sheet.CoreProtection8); cmd.Parameters.AddWithValue("@GrossWeight8", sheet.GrossWeight8); cmd.Parameters.AddWithValue("@Weight8_1", sheet.Weight8_1); cmd.Parameters.AddWithValue("@Weight8_2", sheet.Weight8_2); cmd.Parameters.AddWithValue("@Weight8_3", sheet.Weight8_3); cmd.Parameters.AddWithValue("@Weight8_4", sheet.Weight8_4);
        cmd.Parameters.AddWithValue("@PalletNum9", sheet.PalletNum9); cmd.Parameters.AddWithValue("@Position9", sheet.Position9); cmd.Parameters.AddWithValue("@Width9", sheet.Width9); cmd.Parameters.AddWithValue("@CoreProtection9", sheet.CoreProtection9); cmd.Parameters.AddWithValue("@GrossWeight9", sheet.GrossWeight9); cmd.Parameters.AddWithValue("@Weight9_1", sheet.Weight9_1); cmd.Parameters.AddWithValue("@Weight9_2", sheet.Weight9_2); cmd.Parameters.AddWithValue("@Weight9_3", sheet.Weight9_3); cmd.Parameters.AddWithValue("@Weight9_4", sheet.Weight9_4);
        cmd.Parameters.AddWithValue("@PalletNum10", sheet.PalletNum10); cmd.Parameters.AddWithValue("@Position10", sheet.Position10); cmd.Parameters.AddWithValue("@Width10", sheet.Width10); cmd.Parameters.AddWithValue("@CoreProtection10", sheet.CoreProtection10); cmd.Parameters.AddWithValue("@GrossWeight10", sheet.GrossWeight10); cmd.Parameters.AddWithValue("@Weight10_1", sheet.Weight10_1); cmd.Parameters.AddWithValue("@Weight10_2", sheet.Weight10_2); cmd.Parameters.AddWithValue("@Weight10_3", sheet.Weight10_3); cmd.Parameters.AddWithValue("@Weight10_4", sheet.Weight10_4);
        cmd.Parameters.AddWithValue("@PalletNum11", sheet.PalletNum11); cmd.Parameters.AddWithValue("@Position11", sheet.Position11); cmd.Parameters.AddWithValue("@Width11", sheet.Width11); cmd.Parameters.AddWithValue("@CoreProtection11", sheet.CoreProtection11); cmd.Parameters.AddWithValue("@GrossWeight11", sheet.GrossWeight11); cmd.Parameters.AddWithValue("@Weight11_1", sheet.Weight11_1); cmd.Parameters.AddWithValue("@Weight11_2", sheet.Weight11_2); cmd.Parameters.AddWithValue("@Weight11_3", sheet.Weight11_3); cmd.Parameters.AddWithValue("@Weight11_4", sheet.Weight11_4);
        cmd.Parameters.AddWithValue("@Web", sheet.Web);
        cmd.Parameters.AddWithValue("@PackingCode", sheet.PackingCode);
        cmd.Parameters.AddWithValue("@Unit", sheet.Unit);
        cmd.Parameters.AddWithValue("@PalletType", sheet.PalletType);
        cmd.Parameters.AddWithValue("@BoxNum", sheet.BoxNum);
        cmd.Parameters.AddWithValue("@Die", sheet.Die);
        cmd.Parameters.AddWithValue("@LayersPallet", sheet.LayersPallet);
        cmd.Parameters.AddWithValue("@Product", sheet.Product);
        cmd.Parameters.AddWithValue("@BoxLabel", sheet.BoxLabel);
        cmd.Parameters.AddWithValue("@QCTestRollPos", sheet.QCTestRollPos);
        cmd.Parameters.AddWithValue("@RLS", sheet.RLS);
        cmd.Parameters.AddWithValue("@ExPalletTicket", sheet.ExPalletTicket);
        cmd.Parameters.AddWithValue("@PW", sheet.PW);
        cmd.Parameters.AddWithValue("@Size", sheet.Size);
        cmd.Parameters.AddWithValue("@Condition", sheet.Condition);
        cmd.Parameters.AddWithValue("@Tail", sheet.Tail);
        cmd.Parameters.AddWithValue("@FlagComment", sheet.FlagComment);
    }

    public async Task<int> SaveSheetAsync(StretchFilmReelAuditLog.SheetData sheet, int? createdByUserId = null)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        var sql = $@"
            INSERT INTO QCAuditLogs ({string.Join(", ", Columns)}, CreatedByUserId)
            VALUES ({string.Join(", ", Columns.Select(c => "@" + c))}, @CreatedByUserId);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var cmd = new SqlCommand(sql, conn);
        AddParameters(cmd, sheet);
        cmd.Parameters.AddWithValue("@CreatedByUserId", (object?)createdByUserId ?? DBNull.Value);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task UpdateSheetAsync(int id, StretchFilmReelAuditLog.SheetData sheet)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        var sql = $"UPDATE QCAuditLogs SET {string.Join(", ", Columns.Select(c => $"{c}=@{c}"))} WHERE Id = @Id";

        using var cmd = new SqlCommand(sql, conn);
        AddParameters(cmd, sheet);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<string?> GetFlagCommentForLineAndDateAsync(string line, DateTime date)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand(@"
            SELECT TOP 1 FlagComment FROM QCAuditLogs
            WHERE LineNumber = @Line AND CAST([Date] AS DATE) = @Date
            ORDER BY CreatedAt DESC", conn);
        cmd.Parameters.AddWithValue("@Line", line);
        cmd.Parameters.AddWithValue("@Date", date.Date);

        var result = await cmd.ExecuteScalarAsync();
        return result is null or DBNull ? null : result.ToString();
    }

    public async Task<List<(int Id, StretchFilmReelAuditLog.SheetData sheet)>> GetAllSheetsAsync()
    {
        var result = new List<(int, StretchFilmReelAuditLog.SheetData)>();

        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        var sql = @"
            SELECT h.*, u.Name AS CreatedByName
            FROM QCAuditLogs h
            LEFT JOIN Users u ON u.Id = h.CreatedByUserId
            ORDER BY h.CreatedAt DESC";
        using var cmd = new SqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var id = reader.GetInt32(reader.GetOrdinal("Id"));
            var sheet = new StretchFilmReelAuditLog.SheetData
            {
                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                Time = TimeOnly.FromTimeSpan((TimeSpan)reader["Time"]),
                LineNumber = reader["LineNumber"].ToString(),
                Shift = reader["Shift"].ToString(),
                Crew = reader["Crew"].ToString(),
                LoggedBy = reader["LoggedBy"].ToString(),
                FormulaCode = reader["FormulaCode"].ToString(),
                ProductType = reader["ProductType"].ToString(),
                Width = reader["Width"].ToString(),
                Charting = reader["Charting"].ToString(),
                Counter = reader["Counter"].ToString(),
                LSL1 = reader["LSL1"].ToString(),
                Target1 = reader["Target1"].ToString(),
                USL1 = reader["USL1"].ToString(),
                LSL2 = reader["LSL2"].ToString(),
                Target2 = reader["Target2"].ToString(),
                USL2 = reader["USL2"].ToString(),
                OrderNumber = reader["OrderNumber"].ToString(),
                NumPallets = reader["NumPallets"].ToString(),
                Barcode = reader["Barcode"].ToString(),
                Notify = reader["Notify"].ToString(),
                PalletNum1 = reader["PalletNum1"].ToString(),
                Position1 = reader["Position1"].ToString(),
                Width1 = reader["Width1"].ToString(),
                CoreProtection1 = reader["CoreProtection1"].ToString(),
                GrossWeight1 = reader["GrossWeight1"].ToString(),
                Weight1_1 = reader["Weight1_1"].ToString(),
                Weight1_2 = reader["Weight1_2"].ToString(),
                Weight1_3 = reader["Weight1_3"].ToString(),
                Weight1_4 = reader["Weight1_4"].ToString(),
                PalletNum2 = reader["PalletNum2"].ToString(),
                Position2 = reader["Position2"].ToString(),
                Width2 = reader["Width2"].ToString(),
                CoreProtection2 = reader["CoreProtection2"].ToString(),
                GrossWeight2 = reader["GrossWeight2"].ToString(),
                Weight2_1 = reader["Weight2_1"].ToString(),
                Weight2_2 = reader["Weight2_2"].ToString(),
                Weight2_3 = reader["Weight2_3"].ToString(),
                Weight2_4 = reader["Weight2_4"].ToString(),
                PalletNum3 = reader["PalletNum3"].ToString(),
                Position3 = reader["Position3"].ToString(),
                Width3 = reader["Width3"].ToString(),
                CoreProtection3 = reader["CoreProtection3"].ToString(),
                GrossWeight3 = reader["GrossWeight3"].ToString(),
                Weight3_1 = reader["Weight3_1"].ToString(),
                Weight3_2 = reader["Weight3_2"].ToString(),
                Weight3_3 = reader["Weight3_3"].ToString(),
                Weight3_4 = reader["Weight3_4"].ToString(),
                PalletNum4 = reader["PalletNum4"].ToString(),
                Position4 = reader["Position4"].ToString(),
                Width4 = reader["Width4"].ToString(),
                CoreProtection4 = reader["CoreProtection4"].ToString(),
                GrossWeight4 = reader["GrossWeight4"].ToString(),
                Weight4_1 = reader["Weight4_1"].ToString(),
                Weight4_2 = reader["Weight4_2"].ToString(),
                Weight4_3 = reader["Weight4_3"].ToString(),
                Weight4_4 = reader["Weight4_4"].ToString(),
                PalletNum5 = reader["PalletNum5"].ToString(),
                Position5 = reader["Position5"].ToString(),
                Width5 = reader["Width5"].ToString(),
                CoreProtection5 = reader["CoreProtection5"].ToString(),
                GrossWeight5 = reader["GrossWeight5"].ToString(),
                Weight5_1 = reader["Weight5_1"].ToString(),
                Weight5_2 = reader["Weight5_2"].ToString(),
                Weight5_3 = reader["Weight5_3"].ToString(),
                Weight5_4 = reader["Weight5_4"].ToString(),
                PalletNum6 = reader["PalletNum6"].ToString(),
                Position6 = reader["Position6"].ToString(),
                Width6 = reader["Width6"].ToString(),
                CoreProtection6 = reader["CoreProtection6"].ToString(),
                GrossWeight6 = reader["GrossWeight6"].ToString(),
                Weight6_1 = reader["Weight6_1"].ToString(),
                Weight6_2 = reader["Weight6_2"].ToString(),
                Weight6_3 = reader["Weight6_3"].ToString(),
                Weight6_4 = reader["Weight6_4"].ToString(),
                PalletNum7 = reader["PalletNum7"].ToString(),
                Position7 = reader["Position7"].ToString(),
                Width7 = reader["Width7"].ToString(),
                CoreProtection7 = reader["CoreProtection7"].ToString(),
                GrossWeight7 = reader["GrossWeight7"].ToString(),
                Weight7_1 = reader["Weight7_1"].ToString(),
                Weight7_2 = reader["Weight7_2"].ToString(),
                Weight7_3 = reader["Weight7_3"].ToString(),
                Weight7_4 = reader["Weight7_4"].ToString(),
                PalletNum8 = reader["PalletNum8"].ToString(),
                Position8 = reader["Position8"].ToString(),
                Width8 = reader["Width8"].ToString(),
                CoreProtection8 = reader["CoreProtection8"].ToString(),
                GrossWeight8 = reader["GrossWeight8"].ToString(),
                Weight8_1 = reader["Weight8_1"].ToString(),
                Weight8_2 = reader["Weight8_2"].ToString(),
                Weight8_3 = reader["Weight8_3"].ToString(),
                Weight8_4 = reader["Weight8_4"].ToString(),
                PalletNum9 = reader["PalletNum9"].ToString(),
                Position9 = reader["Position9"].ToString(),
                Width9 = reader["Width9"].ToString(),
                CoreProtection9 = reader["CoreProtection9"].ToString(),
                GrossWeight9 = reader["GrossWeight9"].ToString(),
                Weight9_1 = reader["Weight9_1"].ToString(),
                Weight9_2 = reader["Weight9_2"].ToString(),
                Weight9_3 = reader["Weight9_3"].ToString(),
                Weight9_4 = reader["Weight9_4"].ToString(),
                PalletNum10 = reader["PalletNum10"].ToString(),
                Position10 = reader["Position10"].ToString(),
                Width10 = reader["Width10"].ToString(),
                CoreProtection10 = reader["CoreProtection10"].ToString(),
                GrossWeight10 = reader["GrossWeight10"].ToString(),
                Weight10_1 = reader["Weight10_1"].ToString(),
                Weight10_2 = reader["Weight10_2"].ToString(),
                Weight10_3 = reader["Weight10_3"].ToString(),
                Weight10_4 = reader["Weight10_4"].ToString(),
                PalletNum11 = reader["PalletNum11"].ToString(),
                Position11 = reader["Position11"].ToString(),
                Width11 = reader["Width11"].ToString(),
                CoreProtection11 = reader["CoreProtection11"].ToString(),
                GrossWeight11 = reader["GrossWeight11"].ToString(),
                Weight11_1 = reader["Weight11_1"].ToString(),
                Weight11_2 = reader["Weight11_2"].ToString(),
                Weight11_3 = reader["Weight11_3"].ToString(),
                Weight11_4 = reader["Weight11_4"].ToString(),
                Web = reader["Web"].ToString(),
                PackingCode = reader["PackingCode"].ToString(),
                Unit = reader["Unit"].ToString(),
                PalletType = reader["PalletType"].ToString(),
                BoxNum = reader["BoxNum"].ToString(),
                Die = reader["Die"].ToString(),
                LayersPallet = reader["LayersPallet"].ToString(),
                Product = reader["Product"].ToString(),
                BoxLabel = reader["BoxLabel"].ToString(),
                QCTestRollPos = reader["QCTestRollPos"].ToString(),
                RLS = reader["RLS"].ToString(),
                ExPalletTicket = reader["ExPalletTicket"].ToString(),
                PW = reader["PW"].ToString(),
                Size = reader["Size"].ToString(),
                Condition = reader["Condition"].ToString(),
                Tail = reader["Tail"].ToString(),
                FlagComment = reader["FlagComment"].ToString(),
                CreatedByUserId = reader["CreatedByUserId"] is DBNull ? null : Convert.ToInt32(reader["CreatedByUserId"]),
                CreatedByName = reader["CreatedByName"] is DBNull ? null : reader["CreatedByName"].ToString(),
            };
            result.Add((id, sheet));
        }

        return result;
    }

    public async Task DeleteSheetAsync(int id)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand("DELETE FROM QCAuditLogs WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
