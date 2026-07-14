using Microsoft.Data.SqlClient;

public class UserService
{
    private readonly string _conn;

    public UserService(IConfiguration config)
    {
        _conn = config.GetConnectionString("MyDB");
    }

    public class UserRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsMaster { get; set; }
    }

    public async Task<List<UserRecord>> GetAllUsersAsync()
    {
        var result = new List<UserRecord>();

        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand("SELECT Id, Name, IsMaster FROM Users ORDER BY Name", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new UserRecord
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader["Name"].ToString() ?? "",
                IsMaster = (bool)reader["IsMaster"],
            });
        }

        return result;
    }

    public async Task<UserRecord?> GetUserByIdAsync(int id)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand("SELECT Id, Name, IsMaster FROM Users WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new UserRecord
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader["Name"].ToString() ?? "",
            IsMaster = (bool)reader["IsMaster"],
        };
    }

    public async Task AddUserAsync(string name, bool isMaster)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand("INSERT INTO Users (Name, IsMaster) VALUES (@Name, @IsMaster)", conn);
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@IsMaster", isMaster);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SetMasterAsync(int id, bool isMaster)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand("UPDATE Users SET IsMaster = @IsMaster WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@IsMaster", isMaster);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        using var conn = new SqlConnection(_conn);
        await conn.OpenAsync();

        using var cmd = new SqlCommand("DELETE FROM Users WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
