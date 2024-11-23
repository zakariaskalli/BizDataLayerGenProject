
using System;
using System.Data.SqlClient;
using System.Data;

namespace GymDB_DataAccess
{
    public class clsPaymentsData
    {
        #nullable enable

        public static bool GetPaymentsInfoByID(int PaymentID , ref int MemberShipID, ref int MemberShipTypeID, ref DateTime? PaymentDate, ref bool Status, ref int CreatedByUserID)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM Payments WHERE PaymentID = @PaymentID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaymentID", PaymentID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        { 
                            if (reader.Read())
                            {

                                // The record was found
                                isFound = true;

                                MemberShipID = (int)reader["MemberShipID"];
                                MemberShipTypeID = (int)reader["MemberShipTypeID"];
                                PaymentDate = reader["PaymentDate"] != DBNull.Value ? (DateTime?)reader["PaymentDate"] : null;
                                Status = (bool)reader["Status"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];



                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllPayments()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM Payments";

        using (SqlCommand command = new SqlCommand(query, connection))
        {

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                    dt.Load(reader);
            }
        }
    }
    return dt;

}

         public static int? AddNewPayments(int MemberShipID, int MemberShipTypeID, DateTime? PaymentDate, bool Status, int CreatedByUserID)
        {
            int? PaymentID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into Payments ([MemberShipID],[MemberShipTypeID],[PaymentDate],[Status],[CreatedByUserID])
                            Values (@MemberShipID,@MemberShipTypeID,@PaymentDate,@Status,@CreatedByUserID)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MemberShipID", MemberShipID);
                    command.Parameters.AddWithValue("@MemberShipTypeID", MemberShipTypeID);
                    command.Parameters.AddWithValue("@PaymentDate", PaymentDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Status", Status);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        PaymentID = insertedID;
                    }
                }

            }
            return PaymentID;

        }


         public static bool UpdatePaymentsByID(int PaymentID, int MemberShipID, int MemberShipTypeID, DateTime? PaymentDate, bool Status, int CreatedByUserID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update Payments
                                    set 
                                         [MemberShipID] = @MemberShipID,
                                         [MemberShipTypeID] = @MemberShipTypeID,
                                         [PaymentDate] = @PaymentDate,
                                         [Status] = @Status,
                                         [CreatedByUserID] = @CreatedByUserID
                                  where [PaymentID]= @PaymentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PaymentID", PaymentID);
                    command.Parameters.AddWithValue("@MemberShipID", MemberShipID);
                    command.Parameters.AddWithValue("@MemberShipTypeID", MemberShipTypeID);
                    command.Parameters.AddWithValue("@PaymentDate", PaymentDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Status", Status);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool DeletePayments(int PaymentID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete Payments 
                        where PaymentID = @PaymentID";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@PaymentID", PaymentID);


            connection.Open();
            
            rowsAffected = command.ExecuteNonQuery();


        }

    }
    
    return (rowsAffected > 0);

}
        
        static public DataTable SearchData(string ColumnName, string Data)
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = $@"select * from Payments
                    where {ColumnName} Like '' + @Data + '%';";

        using (SqlCommand Command = new SqlCommand(query, connection))
        {
            Command.Parameters.AddWithValue("@Data", Data);


            connection.Open();

            using (SqlDataReader reader = Command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
        }
        
    }

    return dt;
}
    }
}
