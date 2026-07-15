using Microsoft.Data.SqlClient;

public class AutosaveSettingsService
{
    private readonly string _conn;

    public AutosaveSettingsService(IConfiguration config)
    {
        _conn = config.GetConnectionString("MyDB");
    }

    public async Task<(string Mode, int IntervalSeconds)> GetSettingsAsync()
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand("SELECT Mode, IntervalSeconds FROM AutosaveSettings WHERE Id = 1", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return (reader["Mode"].ToString() ?? "Interval", Convert.ToInt32(reader["IntervalSeconds"]));
        }

        return ("Interval", 30);
    }

    public async Task SaveSettingsAsync(string mode, int intervalSeconds)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand(
            "UPDATE AutosaveSettings SET Mode = @Mode, IntervalSeconds = @IntervalSeconds WHERE Id = 1", conn);
        cmd.Parameters.AddWithValue("@Mode", mode);
        cmd.Parameters.AddWithValue("@IntervalSeconds", intervalSeconds);
        await cmd.ExecuteNonQueryAsync();
    }
}
