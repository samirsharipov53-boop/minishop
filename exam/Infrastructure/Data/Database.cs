using Npgsql;

public static class Database
{
    private static readonly string connectionString = "Host=localhost;Port=5432;Database=minishop;Username=postgres;Password=1234";
    public static NpgsqlConnection Connection()
    {
        return new NpgsqlConnection(connectionString);
    }
}