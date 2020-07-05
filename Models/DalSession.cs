using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace protoUnitofWorkDapper.Models
{
    /* Ref:
     * https://dapper-tutorial.net/knowledge-base/31298235/how-to-implement-unit-of-work-pattern-with-dapper-
     * https://github.com/StackExchange/Dapper/tree/master/Dapper.Contrib
     */
    public class DalSession : IDisposable
    {
        private readonly IDbConnection _connection = null;

        public DalSession(IConfiguration config)
        {
            _connection = new SqlConnection(config.GetConnectionString("protoDb"));
            _connection.Open();
            UnitOfWork = new UnitOfWork(_connection);
        }

        public UnitOfWork UnitOfWork { get; } = null;

        public void Dispose()
        {
            UnitOfWork.Dispose();
            _connection.Dispose();
        }
    }

    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Begin();
        void Commit();
        void Rollback();
    }

    public class UnitOfWork : IUnitOfWork
    {
        IDbConnection _connection = null;
        IDbTransaction _transaction = null;
        Guid _id = Guid.Empty;

        internal UnitOfWork(IDbConnection connection)
        {
            _id = Guid.NewGuid();
            _connection = connection;
        }

        IDbConnection IUnitOfWork.Connection { get { return _connection; } }
        IDbTransaction IUnitOfWork.Transaction { get { return _transaction; } }
        Guid IUnitOfWork.Id { get { return _id; } }

        public void Begin() { _transaction = _connection.BeginTransaction(); }
        public void Commit() { _transaction.Commit(); Dispose(); }
        public void Rollback() { _transaction.Rollback(); Dispose(); }

        public void Dispose()
        {
            if (_transaction != null)
                _transaction.Dispose();

            _transaction = null;
        }
    }
}
