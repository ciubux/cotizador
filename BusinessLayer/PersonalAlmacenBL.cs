
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class PersonalAlmacenBL
    {
        public PersonalAlmacen getPersonalAlmacen(int idPersonalAlmacen)
        {
            using (PersonalAlmacenDAL dal = new PersonalAlmacenDAL())
            {
                PersonalAlmacen personal = dal.getPersonalAlmacen(idPersonalAlmacen);
                return personal;
            }
        }

        public List<PersonalAlmacen> getPersonalesAlmacen(PersonalAlmacen obj)
        {
            using (PersonalAlmacenDAL dal = new PersonalAlmacenDAL())
            {
                return dal.getPersonalesAlmacen(obj);
            }
        }

        public PersonalAlmacen insertPersonalAlmacen(PersonalAlmacen obj)
        {
            using (PersonalAlmacenDAL dal = new PersonalAlmacenDAL())
            {
                return dal.insertPersonalAlmacen(obj);
            }
        }

        public PersonalAlmacen updatePersonalAlmacen(PersonalAlmacen obj)
        {
            using (PersonalAlmacenDAL dal = new PersonalAlmacenDAL())
            {
                return dal.updatePersonalAlmacen(obj);
            }
        }
    }
}
