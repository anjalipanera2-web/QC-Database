using Microsoft.Data.SqlClient;

public class SpecOptionsService
{
    private readonly string _conn;

    public SpecOptionsService(IConfiguration config)
    {
        _conn = config.GetConnectionString("MyDB");
    }

    public async Task<List<(int Id, string Name)>> GetFilmTypesAsync()
    {
        var result = new List<(int Id, string Name)>();
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT Id, Name FROM FilmTypes ORDER BY Name", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add((reader.GetInt32(0), reader.GetString(1)));
        return result;
    }

    public async Task AddFilmTypeAsync(string name)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("INSERT INTO FilmTypes (Name) VALUES (@Name)", conn);
        cmd.Parameters.AddWithValue("@Name", name);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteFilmTypeAsync(int id)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("DELETE FROM FilmTypes WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
