using Microsoft.Data.SqlClient;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Data.Common;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Usuarios_Pruebas.Clases;

namespace Usuarios_Pruebas.Repositorios
{
    public class UsuarioRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;


        public UsuarioRepository(SqlConnection connection ,SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public Usuarios GetById(int id)
        {
            return _connection.Query<Usuarios>(where: e => e.Id == id,transaction: _transaction ).FirstOrDefault();
        }

        public int Insert(Usuarios usuario)
        {
            return (int)_connection.Insert(usuario, transaction: _transaction);
        }

        public IEnumerable<Usuarios> GetAll()
        {
            return _connection.QueryAll<Usuarios>(transaction: _transaction);
        }

        public int Update(Usuarios usuario, IEnumerable<Field> campos)
        {
            return _connection.Update<Usuarios>(usuario, fields: campos, transaction: _transaction);
        }

        public IEnumerable<Usuarios> GetByEstado(bool activo)
        {
            return _connection.Query<Usuarios>(u => u.Activo == activo, transaction: _transaction);
        }

        public Usuarios ObtenerPorCredenciales(string codigo, string claveEncriptada)
        {
            return _connection.Query<Usuarios>(
                u => u.Codigo == codigo && u.Clave == claveEncriptada,
                transaction: _transaction
            ).FirstOrDefault();
        }
    }
}
