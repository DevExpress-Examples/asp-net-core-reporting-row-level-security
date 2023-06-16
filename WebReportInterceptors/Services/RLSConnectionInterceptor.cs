using DevExpress.DataAccess.Sql;
using System.Data;

namespace WebReport.Services {
    public class RLSConnectionInterceptor : IDBConnectionInterceptor {
        readonly int employeeId;
        public RLSConnectionInterceptor(IUserService userService) {
            employeeId = userService.GetCurrentUserId();
        }

        public void ConnectionOpened(string sqlDataConnectionName, IDbConnection connection) {
            using(var command = connection.CreateCommand()) {
                command.CommandText = $"EXEC sp_set_session_context @key = N'EmployeeId', @value = {employeeId}";
                command.ExecuteNonQuery();
            }
        }

        public void ConnectionOpening(string sqlDataConnectionName, IDbConnection connection) { }
    }
}
