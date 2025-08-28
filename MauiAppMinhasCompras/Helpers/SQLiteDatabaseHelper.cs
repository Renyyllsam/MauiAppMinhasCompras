using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;
        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
        
        
        _conn.CreateTableAsync<Models.Produto>().Wait();
        }
        public Task<int> Insert(Models.Produto p)
        {
            return _conn.InsertAsync(p);
        }
        public Task<List<Models.Produto>> Update(Models.Produto p)
        {
            string sql = "UPDATE Produto SET Descricao=?, Quantidade=?, Preco=? WHERE Id=?";
            return _conn.QueryAsync<Models.Produto>(
            sql, p.Descricao, p.Quantidade, p.Preco, p.Id
            );
        }
        public Task<int> Delete(int id)
        {
            return _conn.Table<Models.Produto>().DeleteAsync(i => i.Id == id);
        }
        public Task<List<Models.Produto>> GetAll()
        {
            return _conn.Table<Models.Produto>().ToListAsync();
        }
        public Task<List<Models.Produto>> Search(string q)
        {
            string sql = "SELECT * Produto WHERE descricao LIKE '%" + q + "%'";
            return _conn.QueryAsync<Models.Produto>(sql);
        }
    }
}
    

