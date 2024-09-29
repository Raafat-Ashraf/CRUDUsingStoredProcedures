using System.Data;
using CRUDUsingStoredProcedures.Api.Interfaces;
using CRUDUsingStoredProcedures.Api.Models;
using Microsoft.Data.SqlClient;

namespace CRUDUsingStoredProcedures.Api.Services;

public class ProductService(IConfiguration configuration) : IProductService
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        await using var connection = new SqlConnection(_connectionString);

        await using var command = new SqlCommand("GetProducts", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        List<Product> products = [];
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Price = reader.GetDecimal("Price")
            });
        }

        return products;
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("GetProductById", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Product
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Price = reader.GetDecimal("Price")
            };
        }

        return null;
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        await using var connection = new SqlConnection(_connectionString);

        await using var command = new SqlCommand("InsertProduct", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        var count = await GetProductCountAsync();

        product.Id = count;
        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        await using var connection = new SqlConnection(_connectionString);

        await using var command = new SqlCommand("UpdateProduct", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Id", product.Id);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("DeleteProduct", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }


    private async Task<int> GetProductCountAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("SELECT TOP 1 Id FROM Products ORDER BY Id DESC", connection);

        await connection.OpenAsync();
        return (int)await command.ExecuteScalarAsync();
    }}
