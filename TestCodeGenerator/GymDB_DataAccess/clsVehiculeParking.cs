
using System;
using System.Data.SqlClient;
using System.Data;
using GymDB_DataAccess;

namespace GymDB_DataLayer
{
    public class clsVehiculeParkingData
    {
        #nullable enable

        public static bool GetVehiculeParkingInfoByID(int ParkingID , ref int ParkingSpotID, ref int PersonID, ref string? LicensePlate, ref DateTime CheckInTime, ref DateTime? CheckOutTime, ref int? CreatedByUserID)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM VehiculeParking WHERE ParkingID = @ParkingID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ParkingID", ParkingID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        { 
                            if (reader.Read())
                            {

                                // The record was found
                                isFound = true;

                                ParkingSpotID = (int)reader["ParkingSpotID"];
                                PersonID = (int)reader["PersonID"];
                                LicensePlate = reader["LicensePlate"] != DBNull.Value ? reader["LicensePlate"].ToString() : null;
                                CheckInTime = (DateTime)reader["CheckInTime"];
                                CheckOutTime = reader["CheckOutTime"] != DBNull.Value ? (DateTime?)reader["CheckOutTime"] : null;
                                CreatedByUserID = reader["CreatedByUserID"] != DBNull.Value ? (int?)reader["CreatedByUserID"] : null;



                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllVehiculeParking()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM VehiculeParking";

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

         public static int? AddNewVehiculeParking(int ParkingSpotID, int PersonID, string? LicensePlate, DateTime CheckInTime, DateTime? CheckOutTime, int? CreatedByUserID)
        {
            int? ParkingID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into VehiculeParking ([ParkingSpotID],[PersonID],[LicensePlate],[CheckInTime],[CheckOutTime],[CreatedByUserID])
                            Values (@ParkingSpotID,@PersonID,@LicensePlate,@CheckInTime,@CheckOutTime,@CreatedByUserID)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ParkingSpotID", ParkingSpotID);
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@LicensePlate", LicensePlate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CheckInTime", CheckInTime);
                    command.Parameters.AddWithValue("@CheckOutTime", CheckOutTime ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID ?? (object)DBNull.Value);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        ParkingID = insertedID;
                    }
                }

            }
            return ParkingID;

        }


         public static bool UpdateVehiculeParkingByID(int ParkingID, int ParkingSpotID, int PersonID, string? LicensePlate, DateTime CheckInTime, DateTime? CheckOutTime, int? CreatedByUserID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update VehiculeParking
                                    set 
                                         [ParkingSpotID] = @ParkingSpotID,
                                         [PersonID] = @PersonID,
                                         [LicensePlate] = @LicensePlate,
                                         [CheckInTime] = @CheckInTime,
                                         [CheckOutTime] = @CheckOutTime,
                                         [CreatedByUserID] = @CreatedByUserID
                                  where [ParkingID]= @ParkingID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ParkingID", ParkingID);
                    command.Parameters.AddWithValue("@ParkingSpotID", ParkingSpotID);
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@LicensePlate", LicensePlate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CheckInTime", CheckInTime);
                    command.Parameters.AddWithValue("@CheckOutTime", CheckOutTime ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID ?? (object)DBNull.Value);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool DeleteVehiculeParking(int ParkingID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete VehiculeParking 
                        where ParkingID = @ParkingID";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ParkingID", ParkingID);


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
        string query = $@"select * from VehiculeParking
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
