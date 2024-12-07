
using System;
using System.Data.SqlClient;
using System.Data;
using GymDB_DataAccess;

namespace GymDB_DataLayer
{
    public class clsParkingSpotsData
    {
        #nullable enable

        public static bool GetParkingSpotsInfoByID(int ParkingSpotID , ref int ParkingTypeID, ref bool IsOccupied)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM ParkingSpots WHERE ParkingSpotID = @ParkingSpotID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ParkingSpotID", ParkingSpotID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        { 
                            if (reader.Read())
                            {

                                // The record was found
                                isFound = true;

                                ParkingTypeID = (int)reader["ParkingTypeID"];
                                IsOccupied = (bool)reader["IsOccupied"];



                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllParkingSpots()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM ParkingSpots";

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

         public static int? AddNewParkingSpots(int ParkingTypeID, bool IsOccupied)
        {
            int? ParkingSpotID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into ParkingSpots ([ParkingTypeID],[IsOccupied])
                            Values (@ParkingTypeID,@IsOccupied)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ParkingTypeID", ParkingTypeID);
                    command.Parameters.AddWithValue("@IsOccupied", IsOccupied);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        ParkingSpotID = insertedID;
                    }
                }

            }
            return ParkingSpotID;

        }


         public static bool UpdateParkingSpotsByID(int ParkingSpotID, int ParkingTypeID, bool IsOccupied)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update ParkingSpots
                                    set 
                                         [ParkingTypeID] = @ParkingTypeID,
                                         [IsOccupied] = @IsOccupied
                                  where [ParkingSpotID]= @ParkingSpotID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ParkingSpotID", ParkingSpotID);
                    command.Parameters.AddWithValue("@ParkingTypeID", ParkingTypeID);
                    command.Parameters.AddWithValue("@IsOccupied", IsOccupied);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool DeleteParkingSpots(int ParkingSpotID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete ParkingSpots 
                        where ParkingSpotID = @ParkingSpotID";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ParkingSpotID", ParkingSpotID);


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
        string query = $@"select * from ParkingSpots
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
