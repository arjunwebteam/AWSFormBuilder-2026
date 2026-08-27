using ArjunFormBuilder.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArjunFormBuilder.BLL
{
    public class FormBLL
    {
        ArjunFormBuilder.DAL.FormDAL _formDAL = new DAL.FormDAL();

        public Int64 SaveFormSchema(FormSaveRequest request, string createdBy, Int64 chapterId, ref int status)
        {
            Int64 newFormId = 0;
            newFormId = _formDAL.SaveFormSchema(request.FormId, request.Title, request.Schema, chapterId, createdBy, request.LogoUrl, request.LogoWidth, request.LogoHeight, request.Design, request.Conditions, ref status);  // ✅ CHANGED — passes Conditions through
            return newFormId;
        }

        public FormModel GetFormSchema(Int64 formId, ref int status)
        {
            DataTable dt = _formDAL.GetFormSchema(formId, ref status);
            FormModel objFormModel = new FormModel();
            if (status == 1 && dt.Rows.Count == 1)
            {
                objFormModel.FormId = Convert.ToInt64(dt.Rows[0]["FormId"]);
                objFormModel.Title = dt.Rows[0]["Title"].ToString();
                objFormModel.FormSchema = dt.Rows[0]["FormSchema"].ToString();
                objFormModel.LogoUrl = (dt.Rows[0].Table.Columns.Contains("LogoUrl") && dt.Rows[0]["LogoUrl"] != DBNull.Value)? dt.Rows[0]["LogoUrl"].ToString() : null;
                objFormModel.LogoWidth = (dt.Rows[0].Table.Columns.Contains("LogoWidth") && dt.Rows[0]["LogoWidth"] != DBNull.Value) ? Convert.ToInt32(dt.Rows[0]["LogoWidth"]): (int?)null;
                objFormModel.LogoHeight = (dt.Rows[0].Table.Columns.Contains("LogoHeight") && dt.Rows[0]["LogoHeight"] != DBNull.Value)? Convert.ToInt32(dt.Rows[0]["LogoHeight"]): (int?)null;
        
                objFormModel.DesignJson = (dt.Rows[0].Table.Columns.Contains("DesignJson") && dt.Rows[0]["DesignJson"] != DBNull.Value)? dt.Rows[0]["DesignJson"].ToString(): null;
                objFormModel.ConditionsJson = (dt.Rows[0].Table.Columns.Contains("ConditionsJson") && dt.Rows[0]["ConditionsJson"] != DBNull.Value)? dt.Rows[0]["ConditionsJson"].ToString(): null;  // ✅ ADDED
                objFormModel.ThankYouContent = (dt.Rows[0].Table.Columns.Contains("ThankYouContent") && dt.Rows[0]["ThankYouContent"] != DBNull.Value)? dt.Rows[0]["ThankYouContent"].ToString(): null;
                objFormModel.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);
                objFormModel.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                objFormModel.IsFormEnable = Convert.ToBoolean(dt.Rows[0]["IsFormEnable"]);
                objFormModel.CreatedDate = Convert.ToDateTime(dt.Rows[0]["CreatedDate"]);
                objFormModel.ModifiedDate = (dt.Rows[0]["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["ModifiedDate"]) : (DateTime?)null);
            }
            return objFormModel;
        }

        public bool FormHasPaymentField(string formSchemaJson)
        {
            if (string.IsNullOrWhiteSpace(formSchemaJson)) return false;

            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(formSchemaJson);
                if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var field in doc.RootElement.EnumerateArray())
                    {
                        if (field.TryGetProperty("type", out var t) &&
                            string.Equals(t.GetString(), "payment", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {

            }
            return false;
        }
        public List<string> GetFormFieldLabels(string formSchemaJson)
        {
            var labels = new List<string>();
            if (string.IsNullOrWhiteSpace(formSchemaJson)) return labels;

            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(formSchemaJson);
                if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var field in doc.RootElement.EnumerateArray())
                    {
                    
                        string fieldType = field.TryGetProperty("type", out var t) ? t.GetString() : null;
                        if (string.Equals(fieldType, "heading", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(fieldType, "paragraph", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(fieldType, "payment", StringComparison.OrdinalIgnoreCase))  
                        {
                            continue;
                        }
                        string label = null;
                        if (field.TryGetProperty("label", out var lbl)) label = lbl.GetString();
                        else if (field.TryGetProperty("name", out var nm)) label = nm.GetString();
                        else if (field.TryGetProperty("id", out var id)) label = id.GetString();

                        if (!string.IsNullOrWhiteSpace(label) && !labels.Contains(label))
                            labels.Add(label);
                    }
                }
            }
            catch
            {
            }
            return labels;
        }
        public List<FormSubmissionModel> GetFormSubmissionsList(long formId, string search, string sort, int pageNo, int items, ref int status)
        {
            try
            {
                var result = new List<FormSubmissionModel>();
                int Total = 0;

                DataTable dt = _formDAL.GetFormSubmissionsListWithPagination(formId, search, sort, pageNo, items, ref Total);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var submission = new FormSubmissionModel
                        {
                            SubmissionId = Convert.ToInt64(row["SubmissionId"]),
                            FormId = Convert.ToInt64(row["FormId"]),
                            SubmittedData = row["SubmittedData"].ToString(),
                            SubmittedBy = row["SubmittedBy"].ToString(),
                            SubmittedDate = Convert.ToDateTime(row["SubmittedDate"])
                        };
                        result.Add(submission);
                    }
                }

                status = Total;
                return result;
            }
            catch (Exception ex)
            {
                status = 0;
                throw ex;
            }
        }

        public FormSubmissionModel GetFormSubmissionDetail(long submissionId, ref int status)
        {
            FormSubmissionModel model = new FormSubmissionModel();
            DataTable dt = _formDAL.GetFormSubmissionDetail(submissionId, ref status);
            if (status == 1 && dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                model.SubmissionId = Convert.ToInt64(row["SubmissionId"]);
                model.FormId = Convert.ToInt64(row["FormId"]);
                model.SubmittedData = row["SubmittedData"].ToString();
                model.SubmittedBy = row["SubmittedBy"].ToString();
                model.SubmittedDate = Convert.ToDateTime(row["SubmittedDate"]);

                if (dt.Columns.Contains("PaymentStatus") && !row.IsNull("PaymentStatus"))
                    model.PaymentStatus = row["PaymentStatus"].ToString();

                if (dt.Columns.Contains("PaymentTxnId") && !row.IsNull("PaymentTxnId"))
                    model.PaymentTxnId = row["PaymentTxnId"].ToString();

                if (dt.Columns.Contains("PaymentGateway") && !row.IsNull("PaymentGateway"))
                    model.PaymentGateway = row["PaymentGateway"].ToString();

                if (dt.Columns.Contains("PaymentAmount") && !row.IsNull("PaymentAmount"))
                    model.PaymentAmount = Convert.ToDecimal(row["PaymentAmount"]);

                if (dt.Columns.Contains("PaymentCurrency") && !row.IsNull("PaymentCurrency"))
                    model.PaymentCurrency = row["PaymentCurrency"].ToString();
            }

            System.Diagnostics.Debug.WriteLine(
                "PaymentStatus: " + model.PaymentStatus);
            System.Diagnostics.Debug.WriteLine(
                "PaymentTxnId: " + model.PaymentTxnId);
            System.Diagnostics.Debug.WriteLine(
                "PaymentGateway: " + model.PaymentGateway);
            System.Diagnostics.Debug.WriteLine(
                "PaymentAmount: " + model.PaymentAmount);
            System.Diagnostics.Debug.WriteLine(
                "PaymentCurrency: " + model.PaymentCurrency);
            return model;
        }
        public List<FormModel> GetFormsListByVariable(Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<FormModel> lstForms = new List<FormModel>();
            DataTable dt = _formDAL.GetFormsListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _formDAL.GetFormsListByVariable(ChapterId, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    FormModel objForm = new FormModel();
                    objForm.RId = Convert.ToInt64(dr["Rid"].ToString());
                    objForm.FormId = Convert.ToInt64(dr["FormId"].ToString());
                    objForm.Title = dr["Title"].ToString();
                    objForm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objForm.IsFormEnable = Convert.ToBoolean(dr["IsFormEnable"]);
                    objForm.CreatedBy = (dr["CreatedBy"] != DBNull.Value ? dr["CreatedBy"].ToString() : null);
                    objForm.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    objForm.ModifiedDate = (dr["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(dr["ModifiedDate"]) : (DateTime?)null);
                    objForm.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objForm.SubmissionCount = (dr["SubmissionCount"] != DBNull.Value ? Convert.ToInt64(dr["SubmissionCount"]) : 0);
                    lstForms.Add(objForm);
                }
            }
            return lstForms;
        }

        public Int64 UpdateFormStatus(Int64 formId, ref int status)
        {
            return _formDAL.UpdateFormStatus(formId, ref status);
        }
        public Int64 UpdateFormEnable(Int64 formId, ref int status)
        {
            return _formDAL.UpdateFormStatus(formId, ref status);
        }
        public Int64 SaveThankYouContent(Int64 formId, string content, ref int status)
        {
            return _formDAL.SaveThankYouContent(formId, content, ref status);
        }
        public Int64 DeleteForm(Int64 formId, ref int status)
        {
            return _formDAL.DeleteForm(formId, ref status);
        }
        public Int64 DeleteFormSubmission(Int64 submissionId, ref int status)
        {
            return _formDAL.DeleteFormSubmission(submissionId, ref status);
        }
      
        public Int64 SaveFormSubmission(Int64 formId, string submittedData, string submittedBy,
            string paymentStatus, string paymentTxnId, string paymentGateway, decimal? paymentAmount, string paymentCurrency,
            ref int status)
        {
            Int64 newSubmissionId = 0;
            newSubmissionId = _formDAL.SaveFormSubmission(formId, submittedData, submittedBy,
                paymentStatus, paymentTxnId, paymentGateway, paymentAmount, paymentCurrency, ref status);
            return newSubmissionId;
        }

        public Int64 SaveFormSubmission(Int64 formId, string submittedData, string submittedBy, ref int status)
        {
            return SaveFormSubmission(formId, submittedData, submittedBy, null, null, null, null, null, ref status);
        }
    }
}