
using System;
using System.Data;
using GymDB_DataLayer;

namespace GymDB_BusinessLayer
{
    public class clsParkingSpots
    {
        #nullable enable

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ParkingSpotID { get; set; }
        public int ParkingTypeID { get; set; }
        public clsVehiculeTypes VehiculeTypesInfo;
        public bool IsOccupied { get; set; }


public clsParkingSpots()
{
    this.ParkingSpotID = 0;
    this.ParkingTypeID = 0;
    this.IsOccupied = false;
    Mode = enMode.AddNew;
}


private clsParkingSpots(
 int ParkingSpotID, int ParkingTypeID, bool IsOccupied)
{
    this.ParkingSpotID = ParkingSpotID;
    this.ParkingTypeID = ParkingTypeID;
    this.VehiculeTypesInfo = clsVehiculeTypes.FindByParkingTypeID(this.ParkingTypeID);
    this.IsOccupied = IsOccupied;
    Mode = enMode.Update;
}




    }
}
