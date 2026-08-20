//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ArjunFormBuilder.DAL;
//using System.Data;

//namespace ArjunFormBuilder.BLL
//{
//    public class RegistrationCategories
//    {
//        ArjunFormBuilder.DAL.RegistrationCategories _RegistrationCategories = new ArjunFormBuilder.DAL.RegistrationCategories();

//        #region Methods

//        public Int64 InsertRegistrationCategories(Entities.RegistrationCategories objRegistrationCategories)
//        {
//            Int64 _status = 0;
//            if (objRegistrationCategories != null)
//            {
//                _status = _RegistrationCategories.InsertRegistrationCategories(objRegistrationCategories);

//            }
//            return _status;
//        }

//        public Int64 DeleteRegistrationCategories(Int64 RegistrationCategoryId)
//        {
//            Int64 _status = 0;
//            _status = _RegistrationCategories.DeleteRegistrationCategoriesById(RegistrationCategoryId);
//            return _status;
//        }

//        public Int64 UpdateRegistrationCategoriesStatusById(Int64 RegistrationCategoryId)
//        {
//            Int64 _status = 0;
//            _status = _RegistrationCategories.UpdateRegistrationCategoriesStatusById(RegistrationCategoryId);
//            return _status;
//        }


//        public Int64 UpdateRegistrationCategoriesOrderNo(int OrderNo, Int64 RegistrationCategoryId)
//        {
//            Int64 _status = 0;
//            _status = _RegistrationCategories.UpdateRegistrationCategoriesOrderNo(OrderNo, RegistrationCategoryId);
//            return _status;
//        }

//        #endregion

//        #region Entities filling

//        public List<ArjunFormBuilder.Entities.RegistrationCategories> GetRegistrationCategoriesList(ref int status)
//        {
//            List<ArjunFormBuilder.Entities.RegistrationCategories> lstRegistrationCategories = new List<ArjunFormBuilder.Entities.RegistrationCategories>();
//            DataTable dt = _RegistrationCategories.GetRegistrationCategoriesList(ref status);

//            if (dt.Rows.Count != 0)
//            {
//                foreach (DataRow dr in dt.Rows)
//                {
//                    ArjunFormBuilder.Entities.RegistrationCategories objlstRegistrationCategories = new ArjunFormBuilder.Entities.RegistrationCategories();

                   
//                    objlstRegistrationCategories.RegistrationCategoryId = Convert.ToInt64(dr["RegistrationCategoryId"].ToString());
//                    objlstRegistrationCategories.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);
//                    objlstRegistrationCategories.Type = (dr["Type"] != DBNull.Value ? dr["Type"].ToString() : null);
//                    objlstRegistrationCategories.IsPayment = Convert.ToBoolean(dr["IsPayment"]);
//                    objlstRegistrationCategories.Description = (dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null);
//                    objlstRegistrationCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
//                    objlstRegistrationCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
//                    objlstRegistrationCategories.InstertedBy = dr["InstertedBy"].ToString();
//                    objlstRegistrationCategories.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());
//                    objlstRegistrationCategories.UpdatedBy = dr["UpdatedBy"].ToString();
//                    objlstRegistrationCategories.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());
//                    lstRegistrationCategories.Add(objlstRegistrationCategories);
//                }

//            }
//            return lstRegistrationCategories;
//        }

//        public ArjunFormBuilder.Entities.RegistrationCategories GetRegistrationCategoriesById(Int64 RegistrationCategoryId, ref int status)
//        {
//            ArjunFormBuilder.Entities.RegistrationCategories objRegistrationCategories = new ArjunFormBuilder.Entities.RegistrationCategories();
//            DataTable dt = new DataTable();
//            if (RegistrationCategoryId != 0)
//            {
//                dt = _RegistrationCategories.GetRegistrationCategoriesById(RegistrationCategoryId, ref status);
//                if (dt.Rows.Count == 1)
//                {

             
//                    objRegistrationCategories.RegistrationCategoryId = Convert.ToInt64(dt.Rows[0]["RegistrationCategoryId"].ToString());
//                    objRegistrationCategories.CategoryName = (dt.Rows[0]["CategoryName"] != DBNull.Value ? dt.Rows[0]["CategoryName"].ToString() : null);
//                    objRegistrationCategories.Type = (dt.Rows[0]["Type"] != DBNull.Value ? dt.Rows[0]["Type"].ToString() : null);
//                    objRegistrationCategories.IsPayment = Convert.ToBoolean(dt.Rows[0]["IsPayment"]);
//                    objRegistrationCategories.Description = (dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : null);
//                    objRegistrationCategories.OrderNo = (dt.Rows[0]["OrderNo"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["OrderNo"]) : 0);
//                    objRegistrationCategories.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
//                    objRegistrationCategories.InstertedBy = dt.Rows[0]["InstertedBy"].ToString();
//                    objRegistrationCategories.InsertedDate = Convert.ToDateTime(dt.Rows[0]["InsertedDate"].ToString());
//                    objRegistrationCategories.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
//                    objRegistrationCategories.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"].ToString());

//                }
//            }
//            return objRegistrationCategories;
//        }

//        public List<ArjunFormBuilder.Entities.RegistrationCategories> GetRegistrationCategoriesListByVariable(string Search, string Location, string Sort, int PageNo, int Items, ref int Total)
//        {
//            List<ArjunFormBuilder.Entities.RegistrationCategories> lstRegistrationCategories = new List<ArjunFormBuilder.Entities.RegistrationCategories>();
//            DataTable dt = _RegistrationCategories.GetRegistrationCategoriesListByVariable(Search, Sort, PageNo, Items, ref Total);
//            if (dt.Rows.Count == 0 && PageNo != 0)
//            {
//                dt = _RegistrationCategories.GetRegistrationCategoriesListByVariable(Search, Sort, PageNo - 1, Items, ref Total);
//            }
//            if (dt.Rows.Count != 0)
//            {
//                foreach (DataRow dr in dt.Rows)
//                {
//                    ArjunFormBuilder.Entities.RegistrationCategories objRegistrationCategories = new ArjunFormBuilder.Entities.RegistrationCategories();

//                    objRegistrationCategories.RId = Convert.ToInt64(dr["RId"].ToString());
//                    objRegistrationCategories.RegistrationCategoryId = Convert.ToInt64(dr["RegistrationCategoryId"].ToString());
//                    objRegistrationCategories.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);
//                    objRegistrationCategories.Type = (dr["Type"] != DBNull.Value ? dr["Type"].ToString() : null);
//                    objRegistrationCategories.Description = (dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null);
//                    objRegistrationCategories.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"]) : 0);
//                    objRegistrationCategories.IsActive = Convert.ToBoolean(dr["IsActive"]);
//                    objRegistrationCategories.InstertedBy = dr["InstertedBy"].ToString();
//                    objRegistrationCategories.InsertedDate = Convert.ToDateTime(dr["InsertedDate"].ToString());
//                    objRegistrationCategories.UpdatedBy = dr["UpdatedBy"].ToString();
//                    objRegistrationCategories.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"].ToString());
//                    objRegistrationCategories.Form1Count = (dr["Form1Count"] != DBNull.Value ? Convert.ToInt32(dr["Form1Count"]) : 0);
//                    objRegistrationCategories.Form2Count = (dr["Form2Count"] != DBNull.Value ? Convert.ToInt32(dr["Form2Count"]) : 0);
//                    objRegistrationCategories.Form3Count = (dr["Form3Count"] != DBNull.Value ? Convert.ToInt32(dr["Form3Count"]) : 0);
//                    objRegistrationCategories.Form4Count = (dr["Form4Count"] != DBNull.Value ? Convert.ToInt32(dr["Form4Count"]) : 0);
//                    objRegistrationCategories.Form5Count = (dr["Form5Count"] != DBNull.Value ? Convert.ToInt32(dr["Form5Count"]) : 0);
//                    objRegistrationCategories.Form6Count = (dr["Form6Count"] != DBNull.Value ? Convert.ToInt32(dr["Form6Count"]) : 0);
                    

//                    lstRegistrationCategories.Add(objRegistrationCategories);
//                }
//            }
//            return lstRegistrationCategories;
//        }


//        #endregion
//    }
//}
