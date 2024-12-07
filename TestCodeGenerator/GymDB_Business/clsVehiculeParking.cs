
using System;
using System.Data;
using GymDB_DataLayer;

namespace GymDB_BusinessLayer
{
    public class clsVehiculeParking
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ParkingID { get; set; }
        public int ParkingSpotID { get; set; }
        public clsParkingSpots ParkingSpotsInfo;
        public int PersonID { get; set; }
        public clsPeople PeopleInfo;
        public string? LicensePlate { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public int? CreatedByUserID { get; set; }
        public clsUsers UsersInfo;


public clsVehiculeParking()
{
    this.ParkingID = 0;
    this.ParkingSpotID = 0;
    this.PersonID = 0;
    this.LicensePlate = null;
    this.CheckInTime = DateTime.Now;
    this.CheckOutTime = null;
    this.CreatedByUserID = null;
    Mode = enMode.AddNew;
}


private clsVehiculeParking(
 int ParkingID, int ParkingSpotID, int PersonID, string LicensePlate, DateTime CheckInTime, DateTime CheckOutTime, int CreatedByUserID)
{
    this.ParkingID = ParkingID;
    this.ParkingSpotID = ParkingSpotID;
    this.ParkingSpotsInfo = clsParkingSpots.FindByParkingSpotID(this.ParkingSpotID);
    this.PersonID = PersonID;
    this.PeopleInfo = clsPeople.FindByPersonID(this.PersonID);
    this.LicensePlate = LicensePlate;
    this.CheckInTime = CheckInTime;
    this.CheckOutTime = CheckOutTime;
    this.CreatedByUserID = CreatedByUserID;
    this.UsersInfo = clsUsers.FindByCreatedByUserID(this.CreatedByUserID);
    Mode = enMode.Update;
}




    }
}
