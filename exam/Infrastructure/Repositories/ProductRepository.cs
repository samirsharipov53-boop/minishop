namespace Infrastructure.Repositories;
using Domain.Models;
using Npgsql;

public class ProductRepository
{
    public List<Product>GetAllProducts()
    {
        List<Product> products = new List<Product>();
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM products", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Quantity = reader.GetInt32(3),
                            CategoryId = reader.GetInt32(4)
                        };
                        products.Add(product);
                    }
                }
            }
        }
        return products;
    }

    public Product GetProductById(int id)
    {
        Product product = null;
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM products WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Quantity = reader.GetInt32(3),
                            CategoryId = reader.GetInt32(4)
                        };
                    }
                }
            }
        }
        return product;
    }
    public void AddProduct(Product product)
    {
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("INSERT INTO products (name, price, quantity, category_id) VALUES (@name, @price, @quantity, @category_id)", connection))
            {
                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@price", product.Price);
                command.Parameters.AddWithValue("@quantity", product.Quantity);
                command.Parameters.AddWithValue("@category_id", product.CategoryId);
                command.ExecuteNonQuery();
            }
        }
    }
    public void UpdateProduct(Product product)
    {
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("UPDATE products SET name = @name, price = @price, quantity = @quantity, category_id = @category_id WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", product.Id);
                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@price", product.Price);
                command.Parameters.AddWithValue("@quantity", product.Quantity);
                command.Parameters.AddWithValue("@category_id", product.CategoryId);
                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteProduct(int id)
    {
        using (var connection = Database.Connection())
        {
            connection.Open();
            using (var command = new NpgsqlCommand("DELETE FROM products WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
