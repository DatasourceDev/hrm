using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using HR.Common;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;

namespace HR.Models
{

    public class RelationshipService
    {
        public List<Relationship> LstRelationship(Nullable<int> pEmpID)
        {

            using (var db = new SBS2DBContext())
            {
                return (from a in db.Relationships
                        where a.Employee_Profile_ID == pEmpID
                        select a).ToList();
            }
        }

        public Relationship GetRelationship(Nullable<int> pRelationshipID)
        {

            using (var db = new SBS2DBContext())
            {
                return (from a in db.Relationships
                        where a.Relationship_ID == pRelationshipID
                        select a).FirstOrDefault();

            }
        }

        public bool UpdateRelationShip(Relationship pRelationShip)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pRelationShip != null && pRelationShip.Relationship_ID > 0 && pRelationShip.Relationship_ID > 0)
                    {
                        var current = (from a in db.Relationships
                                       where a.Relationship_ID == pRelationShip.Relationship_ID
                                       select a).FirstOrDefault();

                        if (current != null)
                        {
                            //Update
                            pRelationShip.Create_On = current.Create_On;
                            pRelationShip.Create_By = current.Create_By;
                            db.Entry(current).CurrentValues.SetValues(pRelationShip);
                        }
                        db.SaveChanges();
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public bool InsertRelationShip(Relationship pRelationShip)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pRelationShip != null)
                    {
                        //Insert                        
                        db.Relationships.Add(pRelationShip);
                        db.SaveChanges();
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
        public bool DeleteRelationShip(int pRelationShipID)
        {
            try
            {
                using (var db = new SBS2DBContext())
                {
                    if (pRelationShipID > 0)
                    {
                        var current = (from a in db.Relationships where a.Relationship_ID == pRelationShipID select a).FirstOrDefault();
                        if (current != null)
                        {
                            db.Relationships.Remove(current);
                            db.SaveChanges();
                        }

                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

    }

  


}
