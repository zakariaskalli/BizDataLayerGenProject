
using System;
using System.Data.SqlClient;
using System.Data;
using GymDB_DataAccess;

namespace GymDB_DataLayer
{
    public class clsPeopleData
    {
        #nullable enable

        public static bool GetPeopleInfoByID(int PersonID , ref string FirstName, ref string? SecondName, ref string? ThirdName, ref string LastName, ref string? Email, ref string? Phone, ref DateTime? DateOfBirth, ref bool Gender, ref string? Address, ref int CityID, ref DateTime CreatedTime, ref DateTime LastUpdate, ref string? ProfilePicture, ref int CreatedByUserID)
            {
                bool isFound = false;

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM People WHERE PersonID = @PersonID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        { 
                            if (reader.Read())
                            {

                                // The record was found
                                isFound = true;

                                FirstName = (string)reader["FirstName"];
                                SecondName = reader["Second Name"] != DBNull.Value ? reader["Second Name"].ToString() : null;
                                ThirdName = reader["ThirdName"] != DBNull.Value ? reader["ThirdName"].ToString() : null;
                                LastName = (string)reader["LastName"];
                                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null;
                                Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null;
                                DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? (DateTime?)reader["DateOfBirth"] : null;
                                Gender = (bool)reader["Gender"];
                                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null;
                                CityID = (int)reader["CityID"];
                                CreatedTime = (DateTime)reader["CreatedTime"];
                                LastUpdate = (DateTime)reader["LastUpdate"];
                                ProfilePicture = reader["ProfilePicture"] != DBNull.Value ? reader["ProfilePicture"].ToString() : null;
                                CreatedByUserID = (int)reader["CreatedByUserID"];



                            }
                        }

                    }
                }
                return isFound;

            }

        public static DataTable GetAllPeople()
{
    DataTable dt = new DataTable();

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = "SELECT * FROM People";

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

         public static int? AddNewPeople(string FirstName, string? SecondName, string? ThirdName, string LastName, string? Email, string? Phone, DateTime? DateOfBirth, bool Gender, string? Address, int CityID, DateTime CreatedTime, DateTime LastUpdate, string? ProfilePicture, int CreatedByUserID)
        {
            int? PersonID = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into People ([FirstName],[Second Name],[ThirdName],[LastName],[Email],[Phone],[DateOfBirth],[Gender],[Address],[CityID],[CreatedTime],[LastUpdate],[ProfilePicture],[CreatedByUserID])
                            Values (@FirstName,@SecondName,@ThirdName,@LastName,@Email,@Phone,@DateOfBirth,@Gender,@Address,@CityID,@CreatedTime,@LastUpdate,@ProfilePicture,@CreatedByUserID)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ThirdName", ThirdName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@Email", Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Gender", Gender);
                    command.Parameters.AddWithValue("@Address", Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CityID", CityID);
                    command.Parameters.AddWithValue("@CreatedTime", CreatedTime);
                    command.Parameters.AddWithValue("@LastUpdate", LastUpdate);
                    command.Parameters.AddWithValue("@ProfilePicture", ProfilePicture ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        PersonID = insertedID;
                    }
                }

            }
            return PersonID;

        }


         public static bool UpdatePeopleByID(int PersonID, string FirstName, string? SecondName, string? ThirdName, string LastName, string? Email, string? Phone, DateTime? DateOfBirth, bool Gender, string? Address, int CityID, DateTime CreatedTime, DateTime LastUpdate, string? ProfilePicture, int CreatedByUserID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update People
                                    set 
                                         [FirstName] = @FirstName,
                                         [Second Name] = @SecondName,
                                         [ThirdName] = @ThirdName,
                                         [LastName] = @LastName,
                                         [Email] = @Email,
                                         [Phone] = @Phone,
                                         [DateOfBirth] = @DateOfBirth,
                                         [Gender] = @Gender,
                                         [Address] = @Address,
                                         [CityID] = @CityID,
                                         [CreatedTime] = @CreatedTime,
                                         [LastUpdate] = @LastUpdate,
                                         [ProfilePicture] = @ProfilePicture,
                                         [CreatedByUserID] = @CreatedByUserID
                                  where [PersonID]= @PersonID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ThirdName", ThirdName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@Email", Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Gender", Gender);
                    command.Parameters.AddWithValue("@Address", Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CityID", CityID);
                    command.Parameters.AddWithValue("@CreatedTime", CreatedTime);
                    command.Parameters.AddWithValue("@LastUpdate", LastUpdate);
                    command.Parameters.AddWithValue("@ProfilePicture", ProfilePicture ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }

            }

            return (rowsAffected > 0);
        }


        public static bool DeletePeople(int PersonID)
{
    int rowsAffected = 0;

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        string query = @"Delete People 
                        where PersonID = @PersonID";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@PersonID", PersonID);


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
        string query = $@"select * from People
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
