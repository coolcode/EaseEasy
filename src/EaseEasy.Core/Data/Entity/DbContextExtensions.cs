using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data.Entity;
using EaseEasy.Linq;

namespace EaseEasy.Data.Entity {

    public static class DbContextExtensions {

        public static int Execute(this DbContext db, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) {
            return Do(db, cnn => cnn.Execute(sql, param, transaction, commandTimeout, commandType));
        }

        public static bool Exists(this DbContext db, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) {
            return db.Count(sql, param, transaction, buffered, commandTimeout, commandType) > 0;
        }

        public static int Count(this DbContext db, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) {
            return db.Query(sql, param, transaction, buffered, commandTimeout, commandType).Count();
        }

        public static dynamic Get(this DbContext db, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) {
            var list = db.Query(sql, param, transaction, buffered, commandTimeout, commandType).ToList();

            return list.Count > 0 ? list[0] : null;
        }

        public static T Get<T>(this DbContext db, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) {
            var list = db.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType).ToList();

            return list.Count > 0 ? list[0] : default(T);
        }

        public static IPaginatedList<T> Paging<T>(this DbContext db, PageParam page, string orderby, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) {
            orderby.ThrowIfNullOrEmpty("orderby");
            sql.ThrowIfNullOrEmpty("sql");
            string countSql = WrapCountSql(sql);
            string pagingSql = WrapPagingSql(sql, orderby, page);
            var totalRecords = db.Get<int>(countSql, param, transaction, buffered, commandTimeout, commandType);
            var pageResult = db.Query<T>(pagingSql, param, transaction, buffered, commandTimeout, commandType);

            return new PaginatedList<T>(pageResult, page.PageIndex, page.PageSize, totalRecords);
        }

        public static IPaginatedList<dynamic> Paging(this DbContext db, PageParam page, string orderby, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) {
            orderby.ThrowIfNullOrEmpty("orderby");
            sql.ThrowIfNullOrEmpty("sql");
            string countSql = WrapCountSql(sql);
            string pagingSql = WrapPagingSql(sql, orderby, page);
            var totalRecords = db.Get<int>(countSql, param, transaction, buffered, commandTimeout, commandType);
            var pageResult = db.Query(pagingSql, param, transaction, buffered, commandTimeout, commandType);

            return new PaginatedList<dynamic>(pageResult, page.PageIndex, page.PageSize, totalRecords);
        }

        private static string WrapCountSql(string sql) {
            return string.Format(@"select count(1) from (
                                {0}
                                ) as c 
                                /*gen by EaseEasy.Data.Entity*/", sql);
        }

        private static string WrapPagingSql(string sql, string orderby, PageParam page) {
            string pagingSql = string.Format(@"SELECT TOP ({3}) c.*
                    FROM (
                        SELECT c.*, row_number() OVER (ORDER BY c.{1}) AS [row_number] 
                        FROM (
                            {0}
                        ) AS c
                    ) AS c
                    WHERE c.[row_number] > {2}
                    ORDER BY c.{1}
                    /*gen by EaseEasy.Data.Entity*/",
               sql, SafeQuote(orderby), page.PageIndex * page.PageSize, page.PageSize);

            return pagingSql;
        }

        private static string SafeQuote(string sql) {
            return sql.Replace("'", "''");
        }

        public static IEnumerable<dynamic> Query(this DbContext db, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) {
            return Do(db, cnn => cnn.Query(sql, param, transaction, buffered, commandTimeout, commandType));
        }

        public static IEnumerable<T> Query<T>(this DbContext db, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) {
            return Do(db, cnn => cnn.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType));
        }

        public static SqlMapper.GridReader QueryMultiple(this DbContext db, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) {
            return Do(db, cnn => cnn.QueryMultiple(sql, param, transaction, commandTimeout, commandType));
        }

        private static TResult Do<TResult>(DbContext db, Func<DbConnection, TResult> func) {
            //2012-10-12:不能跟db复用同一个Connection
            using (var cnn = new SqlConnection(db.Database.Connection.ConnectionString)) {
                if (cnn.State != ConnectionState.Open) {
                    cnn.Open();
                }

                return func(cnn);
            }
        }
    }
}
