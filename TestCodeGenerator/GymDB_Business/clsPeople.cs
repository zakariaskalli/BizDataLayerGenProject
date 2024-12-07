
using System;
using System.Data;
using GymDB_DataLayer;

namespace GymDB_BusinessLayer
{
    public class clsPeople
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? ThirdName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string? Address { get; set; }
        public int CityID { get; set; }
        public clsCity CityInfo;
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdate { get; set; }
        public string? ProfilePicture { get; set; }
        public int CreatedByUserID { get; set; }
        public clsUsers UsersInfo;


public clsPeople()
{
    this.PersonID = 0;
    this.FirstName = "";
    this.SecondName = null;
    this.ThirdName = null;
    this.LastName = "";
    this.Email = null;
    this.Phone = null;
    this.DateOfBirth = null;
    this.Gender = false;
    this.Address = null;
    this.CityID = 0;
    this.CreatedTime = DateTime.Now;
    this.LastUpdate = DateTime.Now;
    this.ProfilePicture = null;
    this.CreatedByUserID = 0;
    Mode = enMode.AddNew;
}


private clsPeople(
 int PersonID, string FirstName, string SecondName, string ThirdName, string LastName, string Email, string Phone, DateTime DateOfBirth, bool Gender, string Address, int CityID, DateTime CreatedTime, DateTime LastUpdate, string ProfilePicture, int CreatedByUserID)
{
    this.PersonID = PersonID;
    this.FirstName = FirstName;
    this.SecondName = SecondName;
    this.ThirdName = ThirdName;
    this.LastName = LastName;
    this.Email = Email;
    this.Phone = Phone;
    this.DateOfBirth = DateOfBirth;
    this.Gender = Gender;
    this.Address = Address;
    this.CityID = CityID;
    this.CityInfo = clsCity.FindByCityID(this.CityID);
    this.CreatedTime = CreatedTime;
    this.LastUpdate = LastUpdate;
    this.ProfilePicture = ProfilePicture;
    this.CreatedByUserID = CreatedByUserID;
    this.UsersInfo = clsUsers.FindByCreatedByUserID(this.CreatedByUserID);
    Mode = enMode.Update;
}




    }
}
