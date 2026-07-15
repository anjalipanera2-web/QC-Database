using Microsoft.Data.SqlClient;

public class DailyQualityIssuesService
{
    private readonly string _conn;

    public static readonly string[] FixedLines =
    {
        "BE01", "BE02", "BE03", "BE04", "BE05", "BE06", "BE07", "BE08", "BE09", "BE10", "BP"
    };

    public DailyQualityIssuesService(IConfiguration config)
    {
        _conn = config.GetConnectionString("MyDB");
    }

    public class LineEntry
    {
        public string LineName { get; set; } = "";
        public string FilmType { get; set; } = "";
        public string QualityIssues { get; set; } = "";
        public string AuditNotes { get; set; } = "";
        public string Layers { get; set; } = "";
    }

    public class SheetHeader
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public string Shift { get; set; } = "";
        public string Crew { get; set; } = "";
    }

    private static async Task InsertLinesAsync(SqlConnection conn, SqlTransaction? transaction, int headerId, List<LineEntry> lines)
    {
        var sql = @"
            INSERT INTO DailyQualityIssuesLines (HeaderId, LineName, FilmType, QualityIssues, AuditNotes, Layers)
            VALUES (@HeaderId, @LineName, @FilmType, @QualityIssues, @AuditNotes, @Layers)";

        foreach (var line in lines)
        {
            using var cmd = new SqlCommand(sql, conn, transaction);
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            cmd.Parameters.AddWithValue("@LineName", line.LineName);
            cmd.Parameters.AddWithValue("@FilmType", line.FilmType);
            cmd.Parameters.AddWithValue("@QualityIssues", line.QualityIssues);
            cmd.Parameters.AddWithValue("@AuditNotes", line.AuditNotes);
            cmd.Parameters.AddWithValue("@Layers", line.Layers);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<int> SaveSheetAsync(SheetHeader header, List<LineEntry> lines)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        int headerId;
        using (var cmd = new SqlCommand(@"
            INSERT INTO DailyQualityIssuesHeaders ([Date], Shift, Crew)
            VALUES (@Date, @Shift, @Crew);
            SELECT CAST(SCOPE_IDENTITY() AS INT);", conn))
        {
            cmd.Parameters.AddWithValue("@Date", header.Date.Date);
            cmd.Parameters.AddWithValue("@Shift", header.Shift);
            cmd.Parameters.AddWithValue("@Crew", header.Crew);
            headerId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        await InsertLinesAsync(conn, null, headerId, lines);
        return headerId;
    }

    public async Task UpdateSheetAsync(int headerId, SheetHeader header, List<LineEntry> lines)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();
        using var transaction = conn.BeginTransaction();

        using (var cmd = new SqlCommand(@"
            UPDATE DailyQualityIssuesHeaders SET [Date] = @Date, Shift = @Shift, Crew = @Crew
            WHERE Id = @Id", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@Id", headerId);
            cmd.Parameters.AddWithValue("@Date", header.Date.Date);
            cmd.Parameters.AddWithValue("@Shift", header.Shift);
            cmd.Parameters.AddWithValue("@Crew", header.Crew);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = new SqlCommand("DELETE FROM DailyQualityIssuesLines WHERE HeaderId = @HeaderId", conn, transaction))
        {
            cmd.Parameters.AddWithValue("@HeaderId", headerId);
            await cmd.ExecuteNonQueryAsync();
        }

        await InsertLinesAsync(conn, transaction, headerId, lines);

        transaction.Commit();
    }

    public async Task<(int Id, SheetHeader header, List<LineEntry> lines)?> GetSheetByDateAsync(DateTime date)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        int id;
        SheetHeader header;
        using (var cmd = new SqlCommand("SELECT * FROM DailyQualityIssuesHeaders WHERE [Date] = @Date", conn))
        {
            cmd.Parameters.AddWithValue("@Date", date.Date);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            id = reader.GetInt32(reader.GetOrdinal("Id"));
            header = new SheetHeader
            {
                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                Shift = reader["Shift"].ToString() ?? "",
                Crew = reader["Crew"].ToString() ?? "",
            };
        }

        var lines = new List<LineEntry>();
        using (var cmd = new SqlCommand("SELECT * FROM DailyQualityIssuesLines WHERE HeaderId = @HeaderId", conn))
        {
            cmd.Parameters.AddWithValue("@HeaderId", id);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lines.Add(new LineEntry
                {
                    LineName = reader["LineName"].ToString() ?? "",
                    FilmType = reader["FilmType"].ToString() ?? "",
                    QualityIssues = reader["QualityIssues"].ToString() ?? "",
                    AuditNotes = reader["AuditNotes"].ToString() ?? "",
                    Layers = reader["Layers"].ToString() ?? "",
                });
            }
        }

        return (id, header, lines);
    }

    public static List<LineEntry> BuildDefaultLines() =>
        FixedLines.Select(name => new LineEntry { LineName = name }).ToList();
}
