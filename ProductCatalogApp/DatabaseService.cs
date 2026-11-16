using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace ProductCatalogApp
{
    public class DatabaseService
    {
        private const string ConnectionString = "Data Source=products.db";

        public DatabaseService()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL
            )";
            command.ExecuteNonQuery();
        }

        public List<Product> GetProducts(string searchTerm = "")
        {
            var products = new List<Product>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            // Използване на параметризирана заявка за филтриране или показване на всички[cite: 3]
            command.CommandText = "SELECT Id, Name, Price FROM Products WHERE Name LIKE $term";
            command.Parameters.AddWithValue("$term", $"%{searchTerm}%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Price = reader.GetDouble(2)
                });
            }
            return products;
        }

        public void AddProduct(string name, double price)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Products (Name, Price) VALUES ($name, $price)";
            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$price", price);
            command.ExecuteNonQuery();
        }

        public void UpdateProduct(int id, string name, double price)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            // Обновяване с UPDATE + WHERE по Id[cite: 3]
            command.CommandText = "UPDATE Products SET Name = $name, Price = $price WHERE Id = $id";
            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$price", price);
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        public void DeleteProduct(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Products WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }
    }
}
