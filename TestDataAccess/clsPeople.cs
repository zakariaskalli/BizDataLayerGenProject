
using System;
using System.Data.SqlClient;
using System.Data;

namespace GymDB_DataAccess
{
    public class clsPeopleData
    {

        public static bool GetPeopleInfoByID(int PersonID , ref string FirstName, ref string SecondName, ref string ThirdName, ref string LastName, ref string Email, ref string Phone, ref DateTime DateOfBirth, ref bool Gender, ref string Address, ref int CityID, ref DateTime CreatedTime, ref DateTime LastUpdate, ref string ProfilePicture, ref int CreatedByUserID)
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
                                SecondName = (string)reader["Second Name"];
                                ThirdName = (string)reader["ThirdName"];
                                LastName = (string)reader["LastName"];
                                Email = (string)reader["Email"];
                                Phone = (string)reader["Phone"];
                                DateOfBirth = (DateTime)reader["DateOfBirth"];
                                Gender = (bool)reader["Gender"];
                                Address = (string)reader["Address"];
                                CityID = (int)reader["CityID"];
                                CreatedTime = (DateTime)reader["CreatedTime"];
                                LastUpdate = (DateTime)reader["LastUpdate"];
                                ProfilePicture = (string)reader["ProfilePicture"];
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

         public static int AddNewPeople(string FirstName, string SecondName, string ThirdName, string LastName, string Email, string Phone, DateTime DateOfBirth, bool Gender, string Address, int CityID, DateTime CreatedTime, DateTime LastUpdate, string ProfilePicture, int CreatedByUserID)
        {
            int PersonID = -1;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into People ([FirstName],[Second Name],[ThirdName],[LastName],[Email],[Phone],[DateOfBirth],[Gender],[Address],[CityID],[CreatedTime],[LastUpdate],[ProfilePicture],[CreatedByUserID])
                            Values (@FirstName,@SecondName,@ThirdName,@LastName,@Email,@Phone,@DateOfBirth,@Gender,@Address,@CityID,@CreatedTime,@LastUpdate,@ProfilePicture,@CreatedByUserID)
                            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@ThirdName", ThirdName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", Gender);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@CityID", CityID);
                    command.Parameters.AddWithValue("@CreatedTime", CreatedTime);
                    command.Parameters.AddWithValue("@LastUpdate", LastUpdate);
                    command.Parameters.AddWithValue("@ProfilePicture", ProfilePicture);
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


    }
}
