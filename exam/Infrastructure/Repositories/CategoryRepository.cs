using Domain.Models;
using Npgsql;

namespace Infrastructure.Repositories;

public class CategoryRepository
{
    public List<Category> GetAllCategories()
    {
        List<Category> categories = new List<Category>();
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM categories", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Category category = new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                        categories.Add(category);
                    }
                }
            }
        }
        return categories;
    }
    public Category GetCategoryById(int id)
    {
        Category category = null;
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM categories WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        category = new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                    }
                }
            }
        }
        return category;
    }
    public void AddCategory(Category category)
    {
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("INSERT INTO categories (name) VALUES (@name)", connection))
            {
                command.Parameters.AddWithValue("@name", category.Name);
                command.ExecuteNonQuery();
            }
        }
    }
    public void UpdateCategory(Category category)
    {
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("UPDATE categories SET name = @name WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@name", category.Name);
                command.Parameters.AddWithValue("@id", category.Id);
                command.ExecuteNonQuery();
            }
        }
    }
    public void DeleteCategory(int id)
    {
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("DELETE FROM categories WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
