using Microsoft.Data.SqlClient;
using ProductionSheet.Components.Pages;

public class OperatorAuditService
{
    private readonly string _conn;

    public OperatorAuditService(IConfiguration config)
    {
        _conn = config.GetConnectionString("MyDB");
    }

    private static readonly string[] HeaderColumns = new[]
    {
        "Date", "OperatorName", "FormulaCode", "LineNumber", "Shift", "Crew", "OrderItem1", "OrderItem2",
        "ProductType1", "ProductType2", "Width1", "Width2", "Footage1", "Footage2", "WeightSpec1", "WeightSpec2",
        "BoxNumber", "PalletType", "Unit", "PalletTicket", "PalletSize", "Layers", "Product", "BoxLabel", "FlagComment"
    };

    private static void AddHeaderParameters(SqlCommand cmd, OperatorSelfInspection.SheetHeader header)
    {
        cmd.Parameters.AddWithValue("@FlagComment", header.FlagComment);
        cmd.Parameters.AddWithValue("@Date", header.Date);
        cmd.Parameters.AddWithValue("@OperatorName", header.OperatorName);
        cmd.Parameters.AddWithValue("@FormulaCode", header.FormulaCode);
        cmd.Parameters.AddWithValue("@LineNumber", header.LineNumber);
        cmd.Parameters.AddWithValue("@Shift", header.Shift);
        cmd.Parameters.AddWithValue("@Crew", header.Crew);
        cmd.Parameters.AddWithValue("@OrderItem1", header.OrderItem1);
        cmd.Parameters.AddWithValue("@OrderItem2", header.OrderItem2);
        cmd.Parameters.AddWithValue("@ProductType1", header.ProductType1);
        cmd.Parameters.AddWithValue("@ProductType2", header.ProductType2);
        cmd.Parameters.AddWithValue("@Width1", header.Width1);
        cmd.Parameters.AddWithValue("@Width2", header.Width2);
        cmd.Parameters.AddWithValue("@Footage1", header.Footage1);
        cmd.Parameters.AddWithValue("@Footage2", header.Footage2);
        cmd.Parameters.AddWithValue("@WeightSpec1", header.WeightSpec1);
        cmd.Parameters.AddWithValue("@WeightSpec2", header.WeightSpec2);
        cmd.Parameters.AddWithValue("@BoxNumber", header.BoxNumber);
        cmd.Parameters.AddWithValue("@PalletType", header.PalletType);
        cmd.Parameters.AddWithValue("@Unit", header.Unit);
        cmd.Parameters.AddWithValue("@PalletTicket", header.PalletTicket);
        cmd.Parameters.AddWithValue("@PalletSize", header.PalletSize);
        cmd.Parameters.AddWithValue("@Layers", header.Layers);
        cmd.Parameters.AddWithValue("@Product", header.Product);
        cmd.Parameters.AddWithValue("@BoxLabel", header.BoxLabel);
    }

    private static async Task InsertRowsAsync(SqlConnection conn, SqlTransaction? transaction, int headerId, List<OperatorSelfInspection.SheetRow> rows)
    {
        var rowSql = @"
            INSERT INTO OperatorAuditRows
            (HeaderId, Time, LineSpeed, Vacuum1, Vacuum2, TChiller1, TChiller2, PalletBarcode, PalletNum, CrushCore, Pos, Width, Weight, CoreProt, FilmRV, StretchP, Unwind, StretchF, HandCling, Puncture, LtTransm, Gels, PalletWrap, Grade)
            VALUES
            (@HeaderId, @Time, @LineSpeed, @Vacuum1, @Vacuum2, @TChiller1, @TChiller2, @PalletBarcode, @PalletNum, @CrushCore, @Pos, @Width, @Weight, @CoreProt, @FilmRV, @StretchP, @Unwind, @StretchF, @HandCling, @Puncture, @LtTransm, @Gels, @PalletWrap, @Grade)";

        foreach (var row in rows)
        {
            using var cmd = new SqlCommand(rowSql, conn, transaction);
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            cmd.Parameters.AddWithValue("@Time", row.Time.ToTimeSpan());
            cmd.Parameters.AddWithValue("@LineSpeed", row.LineSpeed);
            cmd.Parameters.AddWithValue("@Vacuum1", row.Vacuum1);
            cmd.Parameters.AddWithValue("@Vacuum2", row.Vacuum2);
            cmd.Parameters.AddWithValue("@TChiller1", row.TChiller1);
            cmd.Parameters.AddWithValue("@TChiller2", row.TChiller2);
            cmd.Parameters.AddWithValue("@PalletBarcode", row.PalletBarcode);
            cmd.Parameters.AddWithValue("@PalletNum", row.PalletNum);
            cmd.Parameters.AddWithValue("@CrushCore", row.CrushCore);
            cmd.Parameters.AddWithValue("@Pos", row.Pos);
            cmd.Parameters.AddWithValue("@Width", row.Width);
            cmd.Parameters.AddWithValue("@Weight", row.Weight);
            cmd.Parameters.AddWithValue("@CoreProt", row.CoreProt);
            cmd.Parameters.AddWithValue("@FilmRV", row.FilmRV);
            cmd.Parameters.AddWithValue("@StretchP", row.StretchP);
            cmd.Parameters.AddWithValue("@Unwind", row.Unwind);
            cmd.Parameters.AddWithValue("@StretchF", row.StretchF);
            cmd.Parameters.AddWithValue("@HandCling", row.HandCling);
            cmd.Parameters.AddWithValue("@Puncture", row.Puncture);
            cmd.Parameters.AddWithValue("@LtTransm", row.LtTransm);
            cmd.Parameters.AddWithValue("@Gels", row.Gels);
            cmd.Parameters.AddWithValue("@PalletWrap", row.PalletWrap);
            cmd.Parameters.AddWithValue("@Grade", row.Grade);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<int> SaveSheetAsync(OperatorSelfInspection.SheetHeader header, List<OperatorSelfInspection.SheetRow> rows, int? createdByUserId = null)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        var headerSql = $@"
            INSERT INTO OperatorAuditHeaders ({string.Join(", ", HeaderColumns)}, CreatedByUserId)
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

    public async Task UpdateSheetAsync(int headerId, OperatorSelfInspection.SheetHeader header, List<OperatorSelfInspection.SheetRow> rows)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var transaction = conn.BeginTransaction();

        var headerSql = $"UPDATE OperatorAuditHeaders SET {string.Join(", ", HeaderColumns.Select(c => $"{c}=@{c}"))} WHERE Id = @Id";
        using (var cmd = new SqlCommand(headerSql, conn, transaction))
        {
            AddHeaderParameters(cmd, header);
            cmd.Parameters.AddWithValue("@Id", headerId);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = new SqlCommand("DELETE FROM OperatorAuditRows WHERE HeaderId = @HeaderId", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            await cmd.ExecuteNonQueryAsync();
        }

        await InsertRowsAsync(conn, transaction, headerId, rows);

        transaction.Commit();
    }

    public async Task<List<(int Id, OperatorSelfInspection.SheetHeader header, List<OperatorSelfInspection.SheetRow> rows)>> GetAllSheetsAsync()
    {
        var result = new List<(int, OperatorSelfInspection.SheetHeader, List<OperatorSelfInspection.SheetRow>)>();

        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        var headers = new List<(int id, OperatorSelfInspection.SheetHeader header)>();
        var headerSql = @"
            SELECT h.*, u.Name AS CreatedByName
            FROM OperatorAuditHeaders h
            LEFT JOIN Users u ON u.Id = h.CreatedByUserId
            ORDER BY h.CreatedAt DESC";

        using (var cmd = new SqlCommand(headerSql, conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var header = new OperatorSelfInspection.SheetHeader
                {
                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    OperatorName = reader["OperatorName"].ToString(),
                    FormulaCode = reader["FormulaCode"].ToString(),
                    LineNumber = reader["LineNumber"].ToString(),
                    Shift = reader["Shift"].ToString(),
                    Crew = reader["Crew"].ToString(),
                    OrderItem1 = reader["OrderItem1"].ToString(),
                    OrderItem2 = reader["OrderItem2"].ToString(),
                    ProductType1 = reader["ProductType1"].ToString(),
                    ProductType2 = reader["ProductType2"].ToString(),
                    Width1 = reader["Width1"].ToString(),
                    Width2 = reader["Width2"].ToString(),
                    Footage1 = reader["Footage1"].ToString(),
                    Footage2 = reader["Footage2"].ToString(),
                    WeightSpec1 = reader["WeightSpec1"].ToString(),
                    WeightSpec2 = reader["WeightSpec2"].ToString(),
                    BoxNumber = reader["BoxNumber"].ToString(),
                    PalletType = reader["PalletType"].ToString(),
                    Unit = reader["Unit"].ToString(),
                    PalletTicket = reader["PalletTicket"].ToString(),
                    PalletSize = reader["PalletSize"].ToString(),
                    Layers = reader["Layers"].ToString(),
                    Product = reader["Product"].ToString(),
                    BoxLabel = reader["BoxLabel"].ToString(),
                    FlagComment = reader["FlagComment"].ToString(),
                    CreatedByUserId = reader["CreatedByUserId"] is DBNull ? null : Convert.ToInt32(reader["CreatedByUserId"]),
                    CreatedByName = reader["CreatedByName"] is DBNull ? null : reader["CreatedByName"].ToString(),
                };
                headers.Add((reader.GetInt32(reader.GetOrdinal("Id")), header));
            }
        }

        foreach (var (id, header) in headers)
        {
            var rows = new List<OperatorSelfInspection.SheetRow>();
            var rowSql = "SELECT * FROM OperatorAuditRows WHERE HeaderId = @HeaderId";

            using var cmd = new SqlCommand(rowSql, conn);
            cmd.Parameters.AddWithValue("@HeaderId", id);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rows.Add(new OperatorSelfInspection.SheetRow
                {
                    Time = TimeOnly.FromTimeSpan((TimeSpan)reader["Time"]),
                    LineSpeed = reader["LineSpeed"].ToString(),
                    Vacuum1 = reader["Vacuum1"].ToString(),
                    Vacuum2 = reader["Vacuum2"].ToString(),
                    TChiller1 = reader["TChiller1"].ToString(),
                    TChiller2 = reader["TChiller2"].ToString(),
                    PalletBarcode = reader["PalletBarcode"].ToString(),
                    PalletNum = reader["PalletNum"].ToString(),
                    CrushCore = reader["CrushCore"].ToString(),
                    Pos = reader["Pos"].ToString(),
                    Width = reader["Width"].ToString(),
                    Weight = reader["Weight"].ToString(),
                    CoreProt = reader["CoreProt"].ToString(),
                    FilmRV = reader["FilmRV"].ToString(),
                    StretchP = reader["StretchP"].ToString(),
                    Unwind = reader["Unwind"].ToString(),
                    StretchF = reader["StretchF"].ToString(),
                    HandCling = reader["HandCling"].ToString(),
                    Puncture = reader["Puncture"].ToString(),
                    LtTransm = reader["LtTransm"].ToString(),
                    Gels = reader["Gels"].ToString(),
                    PalletWrap = reader["PalletWrap"].ToString(),
                    Grade = reader["Grade"].ToString(),
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

        using (var cmd = new SqlCommand("DELETE FROM OperatorAuditRows WHERE HeaderId = @HeaderId", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = new SqlCommand("DELETE FROM OperatorAuditHeaders WHERE Id = @Id", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Id", headerId);
            await cmd.ExecuteNonQueryAsync();
        }

        transaction.Commit();
    }
}
