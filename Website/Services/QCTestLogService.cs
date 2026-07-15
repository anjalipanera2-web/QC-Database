using ProductionSheet.Components.Pages;
using Microsoft.Data.SqlClient;

public class QCTestLogService
{
    private readonly string _conn;

    public QCTestLogService(IConfiguration config)
    {
        _conn = config.GetConnectionString("MyDB");
    }

    private static readonly string[] HeaderColumns = new[]
    {
        "Date", "QCTech", "Shift", "Crew", "Line", "FilmType", "Formula", "RollWidth", "Gauge",
        "USL1", "USL2", "USL3", "USL4", "USL5", "USL6", "USL7", "USL8",
        "LSL1", "LSL2", "LSL3", "LSL4", "LSL5", "LSL6", "LSL7", "LSL8",
        "Target1", "Target2", "Target3", "Target4", "Target5", "Target6", "Target7", "Target8",
        "Spec1", "Spec2", "Spec3", "Spec4", "Spec5", "Spec6", "FlagComment"
    };

    private static void AddHeaderParameters(SqlCommand cmd, QCTestLogClassic.SheetHeader header)
    {
        cmd.Parameters.AddWithValue("@FlagComment", header.FlagComment);
        cmd.Parameters.AddWithValue("@Date", header.Date);
        cmd.Parameters.AddWithValue("@QCTech", header.QCTech);
        cmd.Parameters.AddWithValue("@Shift", header.Shift);
        cmd.Parameters.AddWithValue("@Crew", header.Crew);
        cmd.Parameters.AddWithValue("@Line", header.Line);
        cmd.Parameters.AddWithValue("@FilmType", header.FilmType);
        cmd.Parameters.AddWithValue("@Formula", header.Formula);
        cmd.Parameters.AddWithValue("@RollWidth", header.RollWidth);
        cmd.Parameters.AddWithValue("@Gauge", header.Gauge);
        cmd.Parameters.AddWithValue("@USL1", header.USL1);
        cmd.Parameters.AddWithValue("@USL2", header.USL2);
        cmd.Parameters.AddWithValue("@USL3", header.USL3);
        cmd.Parameters.AddWithValue("@USL4", header.USL4);
        cmd.Parameters.AddWithValue("@USL5", header.USL5);
        cmd.Parameters.AddWithValue("@USL6", header.USL6);
        cmd.Parameters.AddWithValue("@USL7", header.USL7);
        cmd.Parameters.AddWithValue("@USL8", header.USL8);
        cmd.Parameters.AddWithValue("@LSL1", header.LSL1);
        cmd.Parameters.AddWithValue("@LSL2", header.LSL2);
        cmd.Parameters.AddWithValue("@LSL3", header.LSL3);
        cmd.Parameters.AddWithValue("@LSL4", header.LSL4);
        cmd.Parameters.AddWithValue("@LSL5", header.LSL5);
        cmd.Parameters.AddWithValue("@LSL6", header.LSL6);
        cmd.Parameters.AddWithValue("@LSL7", header.LSL7);
        cmd.Parameters.AddWithValue("@LSL8", header.LSL8);
        cmd.Parameters.AddWithValue("@Target1", header.Target1);
        cmd.Parameters.AddWithValue("@Target2", header.Target2);
        cmd.Parameters.AddWithValue("@Target3", header.Target3);
        cmd.Parameters.AddWithValue("@Target4", header.Target4);
        cmd.Parameters.AddWithValue("@Target5", header.Target5);
        cmd.Parameters.AddWithValue("@Target6", header.Target6);
        cmd.Parameters.AddWithValue("@Target7", header.Target7);
        cmd.Parameters.AddWithValue("@Target8", header.Target8);
        cmd.Parameters.AddWithValue("@Spec1", header.Spec1);
        cmd.Parameters.AddWithValue("@Spec2", header.Spec2);
        cmd.Parameters.AddWithValue("@Spec3", header.Spec3);
        cmd.Parameters.AddWithValue("@Spec4", header.Spec4);
        cmd.Parameters.AddWithValue("@Spec5", header.Spec5);
        cmd.Parameters.AddWithValue("@Spec6", header.Spec6);
    }

    private static async Task InsertRowsAsync(SqlConnection conn, SqlTransaction? transaction, int headerId, List<QCTestLogClassic.SheetRow> rows)
    {
        var rowSql = @"
            INSERT INTO QCTestLogRows (HeaderId, SampleTime, TestTime, RollWidth, TestRollWidth, Position, RollWeight, StretchP, AvgStretch, Unwind, Puncture, PW, AvgThickness, ThicknessOuter1, ThicknessMiddle, ThicknessOuter2, LtTransm, GelCount, Remarks, RollStatus, AvgOutCling, OutClingGrade, AvgInCling, InClingGrade, PunStdv, StretchForce, NumPunct, Tail, HazeStdv, Barcode, PalletNum, SpecSet)
            VALUES (@HeaderId, @SampleTime, @TestTime, @RollWidth, @TestRollWidth, @Position, @RollWeight, @StretchP, @AvgStretch, @Unwind, @Puncture, @PW, @AvgThickness, @ThicknessOuter1, @ThicknessMiddle, @ThicknessOuter2, @LtTransm, @GelCount, @Remarks, @RollStatus, @AvgOutCling, @OutClingGrade, @AvgInCling, @InClingGrade, @PunStdv, @StretchForce, @NumPunct, @Tail, @HazeStdv, @Barcode, @PalletNum, @SpecSet)";

        foreach (var row in rows)
        {
            using var cmd = new SqlCommand(rowSql, conn, transaction);
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            cmd.Parameters.AddWithValue("@SampleTime", row.SampleTime.ToTimeSpan());
            cmd.Parameters.AddWithValue("@TestTime", row.TestTime.ToTimeSpan());
            cmd.Parameters.AddWithValue("@RollWidth", row.RollWidth);
            cmd.Parameters.AddWithValue("@TestRollWidth", row.TestRollWidth);
            cmd.Parameters.AddWithValue("@Position", row.Position);
            cmd.Parameters.AddWithValue("@RollWeight", row.RollWeight);
            cmd.Parameters.AddWithValue("@StretchP", row.StretchP);
            cmd.Parameters.AddWithValue("@AvgStretch", row.AvgStretch);
            cmd.Parameters.AddWithValue("@Unwind", row.Unwind);
            cmd.Parameters.AddWithValue("@Puncture", row.Puncture);
            cmd.Parameters.AddWithValue("@PW", row.PW);
            cmd.Parameters.AddWithValue("@AvgThickness", row.AvgThickness);
            cmd.Parameters.AddWithValue("@ThicknessOuter1", row.ThicknessOuter1);
            cmd.Parameters.AddWithValue("@ThicknessMiddle", row.ThicknessMiddle);
            cmd.Parameters.AddWithValue("@ThicknessOuter2", row.ThicknessOuter2);
            cmd.Parameters.AddWithValue("@LtTransm", row.LtTransm);
            cmd.Parameters.AddWithValue("@GelCount", row.GelCount);
            cmd.Parameters.AddWithValue("@Remarks", row.Remarks);
            cmd.Parameters.AddWithValue("@RollStatus", row.RollStatus);
            cmd.Parameters.AddWithValue("@AvgOutCling", row.AvgOutCling);
            cmd.Parameters.AddWithValue("@OutClingGrade", row.OutClingGrade);
            cmd.Parameters.AddWithValue("@AvgInCling", row.AvgInCling);
            cmd.Parameters.AddWithValue("@InClingGrade", row.InClingGrade);
            cmd.Parameters.AddWithValue("@PunStdv", row.PunStdv);
            cmd.Parameters.AddWithValue("@StretchForce", row.StretchForce);
            cmd.Parameters.AddWithValue("@NumPunct", row.NumPunct);
            cmd.Parameters.AddWithValue("@Tail", row.Tail);
            cmd.Parameters.AddWithValue("@HazeStdv", row.HazeStdv);
            cmd.Parameters.AddWithValue("@Barcode", row.Barcode);
            cmd.Parameters.AddWithValue("@PalletNum", row.PalletNum);
            cmd.Parameters.AddWithValue("@SpecSet", row.SpecSet);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<int> SaveSheetAsync(QCTestLogClassic.SheetHeader header, List<QCTestLogClassic.SheetRow> rows, int? createdByUserId = null)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        var headerSql = $@"
            INSERT INTO QCTestLogHeaders ({string.Join(", ", HeaderColumns)}, CreatedByUserId)
            VALUES ({string.Join(", ", HeaderColumns.Select(c => "@" + c))}, @CreatedByUserId);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        int headerId;
        using (var cmd = new SqlCommand(headerSql, conn))
        {
            AddHeaderParameters(cmd, header);
            cmd.Parameters.AddWithValue("@CreatedByUserId", (object?)createdByUserId ?? DBNull.Value);
            headerId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        await InsertRowsAsync(conn, null, headerId, rows);
        return headerId;
    }

    public async Task UpdateSheetAsync(int headerId, QCTestLogClassic.SheetHeader header, List<QCTestLogClassic.SheetRow> rows)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var transaction = conn.BeginTransaction();

        var headerSql = $"UPDATE QCTestLogHeaders SET {string.Join(", ", HeaderColumns.Select(c => $"{c}=@{c}"))} WHERE Id = @Id";
        using (var cmd = new SqlCommand(headerSql, conn, transaction))
        {
            AddHeaderParameters(cmd, header);
            cmd.Parameters.AddWithValue("@Id", headerId);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = new SqlCommand("DELETE FROM QCTestLogRows WHERE HeaderId = @HeaderId", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            await cmd.ExecuteNonQueryAsync();
        }

        await InsertRowsAsync(conn, transaction, headerId, rows);

        transaction.Commit();
    }

    public async Task<(string? Shift, string? Crew)> GetShiftAndCrewForDateAsync(DateTime date)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand(@"
            SELECT TOP 1 Shift, Crew FROM QCTestLogHeaders
            WHERE CAST([Date] AS DATE) = @Date
            ORDER BY CreatedAt DESC", conn);
        cmd.Parameters.AddWithValue("@Date", date.Date);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return (reader["Shift"].ToString(), reader["Crew"].ToString());
        }
        return (null, null);
    }

    public async Task<string?> GetFilmTypeForLineAndDateAsync(string line, DateTime date)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand(@"
            SELECT TOP 1 FilmType FROM QCTestLogHeaders
            WHERE Line = @Line AND CAST([Date] AS DATE) = @Date
            ORDER BY CreatedAt DESC", conn);
        cmd.Parameters.AddWithValue("@Line", line);
        cmd.Parameters.AddWithValue("@Date", date.Date);

        var result = await cmd.ExecuteScalarAsync();
        return result is null or DBNull ? null : result.ToString();
    }

    public async Task<string?> GetFlagCommentForLineAndDateAsync(string line, DateTime date)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand(@"
            SELECT TOP 1 FlagComment FROM QCTestLogHeaders
            WHERE Line = @Line AND CAST([Date] AS DATE) = @Date
            ORDER BY CreatedAt DESC", conn);
        cmd.Parameters.AddWithValue("@Line", line);
        cmd.Parameters.AddWithValue("@Date", date.Date);

        var result = await cmd.ExecuteScalarAsync();
        return result is null or DBNull ? null : result.ToString();
    }

    public async Task<List<(int Id, QCTestLogClassic.SheetHeader header, List<QCTestLogClassic.SheetRow> rows)>> GetAllSheetsAsync()
    {
        var result = new List<(int, QCTestLogClassic.SheetHeader, List<QCTestLogClassic.SheetRow>)>();

        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        var headers = new List<(int id, QCTestLogClassic.SheetHeader header)>();
        var headerSql = @"
            SELECT h.*, u.Name AS CreatedByName
            FROM QCTestLogHeaders h
            LEFT JOIN Users u ON u.Id = h.CreatedByUserId
            ORDER BY h.CreatedAt DESC";

        using (var cmd = new SqlCommand(headerSql, conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var header = new QCTestLogClassic.SheetHeader
                {
                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    QCTech = reader["QCTech"].ToString(),
                    Shift = reader["Shift"].ToString(),
                    Crew = reader["Crew"].ToString(),
                    Line = reader["Line"].ToString(),
                    FilmType = reader["FilmType"].ToString(),
                    Formula = reader["Formula"].ToString(),
                    RollWidth = reader["RollWidth"].ToString(),
                    Gauge = reader["Gauge"].ToString(),
                    USL1 = reader["USL1"].ToString(),
                    USL2 = reader["USL2"].ToString(),
                    USL3 = reader["USL3"].ToString(),
                    USL4 = reader["USL4"].ToString(),
                    USL5 = reader["USL5"].ToString(),
                    USL6 = reader["USL6"].ToString(),
                    USL7 = reader["USL7"].ToString(),
                    USL8 = reader["USL8"].ToString(),
                    LSL1 = reader["LSL1"].ToString(),
                    LSL2 = reader["LSL2"].ToString(),
                    LSL3 = reader["LSL3"].ToString(),
                    LSL4 = reader["LSL4"].ToString(),
                    LSL5 = reader["LSL5"].ToString(),
                    LSL6 = reader["LSL6"].ToString(),
                    LSL7 = reader["LSL7"].ToString(),
                    LSL8 = reader["LSL8"].ToString(),
                    Target1 = reader["Target1"].ToString(),
                    Target2 = reader["Target2"].ToString(),
                    Target3 = reader["Target3"].ToString(),
                    Target4 = reader["Target4"].ToString(),
                    Target5 = reader["Target5"].ToString(),
                    Target6 = reader["Target6"].ToString(),
                    Target7 = reader["Target7"].ToString(),
                    Target8 = reader["Target8"].ToString(),
                    Spec1 = reader["Spec1"].ToString(),
                    Spec2 = reader["Spec2"].ToString(),
                    Spec3 = reader["Spec3"].ToString(),
                    Spec4 = reader["Spec4"].ToString(),
                    Spec5 = reader["Spec5"].ToString(),
                    Spec6 = reader["Spec6"].ToString(),
                    FlagComment = reader["FlagComment"].ToString(),
                    CreatedByUserId = reader["CreatedByUserId"] is DBNull ? null : Convert.ToInt32(reader["CreatedByUserId"]),
                    CreatedByName = reader["CreatedByName"] is DBNull ? null : reader["CreatedByName"].ToString(),
                };
                headers.Add((reader.GetInt32(reader.GetOrdinal("Id")), header));
            }
        }

        foreach (var (id, header) in headers)
        {
            var rows = new List<QCTestLogClassic.SheetRow>();
            var rowSql = "SELECT * FROM QCTestLogRows WHERE HeaderId = @HeaderId";

            using var cmd = new SqlCommand(rowSql, conn);
            cmd.Parameters.AddWithValue("@HeaderId", id);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rows.Add(new QCTestLogClassic.SheetRow
                {
                    SampleTime = TimeOnly.FromTimeSpan((TimeSpan)reader["SampleTime"]),
                    TestTime = TimeOnly.FromTimeSpan((TimeSpan)reader["TestTime"]),
                    RollWidth = reader["RollWidth"].ToString(),
                    TestRollWidth = reader["TestRollWidth"].ToString(),
                    Position = reader["Position"].ToString(),
                    RollWeight = reader["RollWeight"].ToString(),
                    StretchP = reader["StretchP"].ToString(),
                    AvgStretch = reader["AvgStretch"].ToString(),
                    Unwind = reader["Unwind"].ToString(),
                    Puncture = reader["Puncture"].ToString(),
                    PW = reader["PW"].ToString(),
                    AvgThickness = reader["AvgThickness"].ToString(),
                    ThicknessOuter1 = reader["ThicknessOuter1"].ToString(),
                    ThicknessMiddle = reader["ThicknessMiddle"].ToString(),
                    ThicknessOuter2 = reader["ThicknessOuter2"].ToString(),
                    LtTransm = reader["LtTransm"].ToString(),
                    GelCount = reader["GelCount"].ToString(),
                    Remarks = reader["Remarks"].ToString(),
                    RollStatus = reader["RollStatus"].ToString(),
                    AvgOutCling = reader["AvgOutCling"].ToString(),
                    OutClingGrade = reader["OutClingGrade"].ToString(),
                    AvgInCling = reader["AvgInCling"].ToString(),
                    InClingGrade = reader["InClingGrade"].ToString(),
                    PunStdv = reader["PunStdv"].ToString(),
                    StretchForce = reader["StretchForce"].ToString(),
                    NumPunct = reader["NumPunct"].ToString(),
                    Tail = reader["Tail"].ToString(),
                    HazeStdv = reader["HazeStdv"].ToString(),
                    Barcode = reader["Barcode"].ToString(),
                    PalletNum = reader["PalletNum"].ToString(),
                    SpecSet = reader["SpecSet"].ToString(),
                });
            }

            result.Add((id, header, rows));
        }

        return result;
    }

    public async Task DeleteSheetAsync(int headerId)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var transaction = conn.BeginTransaction();

        using (var cmd = new SqlCommand("DELETE FROM QCTestLogRows WHERE HeaderId = @HeaderId", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = new SqlCommand("DELETE FROM QCTestLogHeaders WHERE Id = @Id", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Id", headerId);
            await cmd.ExecuteNonQueryAsync();
        }

        transaction.Commit();
    }
}
