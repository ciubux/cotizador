
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class VehiculoBL
    {
        public Vehiculo getVehiculo(int idVehiculo)
        {
            using (VehiculoDAL dal = new VehiculoDAL())
            {
                Vehiculo vehiculo = dal.getVehiculo(idVehiculo);

                return vehiculo;
            }
        }

        public List<Vehiculo> getVehiculos(Vehiculo obj)
        {
            using (VehiculoDAL dal = new VehiculoDAL())
            {
                return dal.getVehiculos(obj);
            }
        }

        public Vehiculo insertVehiculo(Vehiculo obj)
        {
            using (VehiculoDAL dal = new VehiculoDAL())
            {
                return dal.insertVehiculo(obj);
            }
        }

        public Vehiculo updateVehiculo(Vehiculo obj)
        {
            using (VehiculoDAL dal = new VehiculoDAL())
            {
                return dal.updateVehiculo(obj);
            }
        }
    }
}
