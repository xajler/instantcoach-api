using System.Data.SqlClient;

namespace Core
{
    public enum DatabaseError
    {
        UniqueConstraint,
        CannotInsertNull,
        MaxLength,
        NumericOverflow,
        Unhandled
    }
    public class SqlServerErrorManager
    {
        private const int CannotInsertNull = 515;
        private const int CannotInsertDuplicateKeyUniqueIndex = 2601;
        private const int CannotInsertDuplicateKeyUniqueConstraint = 2627;
        private const int ArithmeticOverflow = 8115;
        private const int StringOrBinaryDataWouldBeTruncated = 8152;

        public static DatabaseError GetError(SqlException dbException)
        {
            switch (dbException.Number)
            {
                case CannotInsertNull:
                    return DatabaseError.CannotInsertNull;
                case CannotInsertDuplicateKeyUniqueIndex:
                case CannotInsertDuplicateKeyUniqueConstraint:
                    return DatabaseError.UniqueConstraint;
                case ArithmeticOverflow:
                    return DatabaseError.NumericOverflow;
                case StringOrBinaryDataWouldBeTruncated:
                    return DatabaseError.MaxLength;
                default:
                    return DatabaseError.Unhandled;
            }
        }
    }
}