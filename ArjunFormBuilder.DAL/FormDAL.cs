using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using SqlParameter = Microsoft.Data.SqlClient.SqlParameter;

namespace ArjunFormBuilder.DAL
{
    public class FormDAL
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;
        private readonly IConfiguration _configuration;

        public FormDAL()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public FormDAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DataTable GetFormSubmissionsList(Int64 formId, ref int status)
        {
            DataTable dt = new DataTable();
            try
            {
                _sqlP = new SqlParameter[]
                {
                    new SqlParameter("@FormId", formId),
                    new SqlParameter("@QStatus", 0)
                };
                _sqlP[1].Direction = ParameterDirection.Output;

                dt = _dbAccess.GetDataTable("GetFormSubmissionsList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                status = 0;
                throw ex;
            }
            return dt;
        }

        public DataTable GetFormSubmissionDetail(Int64 submissionId, ref int status)
        {
            DataTable dt = new DataTable();
            try
            {
                _sqlP = new SqlParameter[]
                {
                    new SqlParameter("@SubmissionId", submissionId),
                    new SqlParameter("@QStatus", 0)
                };
                _sqlP[1].Direction = ParameterDirection.Output;

                dt = _dbAccess.GetDataTable("GetFormSubmissionDetail", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                status = 0;
                throw ex;
            }
            return dt;
        }
     
        public DataTable GetFormSubmissionsListWithPagination(Int64 formId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new SqlParameter[]
                {
                    new SqlParameter("@FormId", formId),
                    new SqlParameter("@Search", Search ?? ""),
                    new SqlParameter("@Sort", Sort ?? "SubmittedDate DESC"),
                    new SqlParameter("@PageNo", PageNo),
                    new SqlParameter("@Items", Items),
                    new SqlParameter("@Total", 0)
                };
                _sqlP[5].Direction = ParameterDirection.Output;

                dt = _dbAccess.GetDataTable("GetFormSubmissionsListWithPagination", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #region Form Builder
        public DataTable GetFormsListByVariable(Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ChapterId", ChapterId),
                    new SqlParameter("@Search", Search),
                    new SqlParameter("@Sort", Sort),
                    new SqlParameter("@PageNo", PageNo),
                    new SqlParameter("@Items", Items),
                    new SqlParameter("@Total", Total)
                };
                _sqlP[5].Direction = ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FormsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 UpdateFormStatus(Int64 formId, ref int status)
        {
            Int64 result = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@FormId", formId),
                    new SqlParameter("@QStatus", 0)
                };
                _sqlP[1].Direction = ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("UpdateFormEnable", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
                result = status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public Int64 UpdateFormEnable(Int64 formId, ref int status)
        {
            Int64 result = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@FormId", formId),
                    new SqlParameter("@QStatus", 0)
                };
                _sqlP[1].Direction = ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("USP_FormStatusUpdate", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
                result = status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Int64 DeleteForm(Int64 formId, ref int status)
        {
            Int64 result = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@FormId", formId),
                    new SqlParameter("@QStatus", 0)
                };
                _sqlP[1].Direction = ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("USP_DeleteForm", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
                result = status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public Int64 DeleteFormSubmission(Int64 submissionId, ref int status)
         {
            Int64 result = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@submissionId", submissionId),
                    new SqlParameter("@QStatus", 0)
                };
                _sqlP[1].Direction = ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("DeleteFormSubmission", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
                result = status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Int64 SaveFormSchema(int? formId, string title, string formSchema, Int64 chapterId, string createdBy, string logoUrl, int? logoWidth, int? logoHeight, string designJson, string conditionsJson, ref int status)
        {
            Int64 newFormId = 0;
            try
            {
                _sqlP = new[]
                {
            new SqlParameter("@FormId", (object)formId ?? DBNull.Value),
            new SqlParameter("@Title", title),
            new SqlParameter("@FormSchema", formSchema),
            new SqlParameter("@ChapterId", chapterId),
            new SqlParameter("@CreatedBy", (object)createdBy ?? DBNull.Value),
            new SqlParameter("@LogoUrl", (object)logoUrl ?? DBNull.Value),
            new SqlParameter("@LogoWidth", (object)logoWidth ?? DBNull.Value),
            new SqlParameter("@LogoHeight", (object)logoHeight ?? DBNull.Value),
            new SqlParameter("@DesignJson", (object)designJson ?? DBNull.Value),
            new SqlParameter("@ConditionsJson", (object)conditionsJson ?? DBNull.Value),   // ✅ ADDED
            new SqlParameter("@QStatus", 0)
        };
                _sqlP[10].Direction = ParameterDirection.Output;   // ✅ CHANGED — index shifted to 10 (ConditionsJson param inserted before it)

                _dbAccess.SP_ExecuteScalar("USP_SaveFormSchema", ref _sqlP);
                status = Convert.ToInt32(_sqlP[10].Value);          // ✅ CHANGED — index shifted to 10
                newFormId = status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return newFormId;
        }
        public DataTable FormSubmissionsExportToExcel(string Search, string Sort, Int64 FormId)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
            new SqlParameter("@Search", Search),
            new SqlParameter("@Sort", Sort),
            new SqlParameter("@FormId", FormId)
        };

                dt = _dbAccess.GetDataTable("FormSubmissionsExportToExcel", ref _sqlP);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetFormSchema(Int64 formId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@FormId", formId),
                    new SqlParameter("@QStatus", 0)
                };
                _sqlP[1].Direction = ParameterDirection.Output;

                dt = _dbAccess.GetDataTable("USP_GetFormSchema", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetFormsListByChapterId(Int64 ChapterId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@ChapterId", ChapterId),
                    new SqlParameter("@QStatus", 0)
                };
                _sqlP[1].Direction = ParameterDirection.Output;

                dt = _dbAccess.GetDataTable("USP_GetFormsListByChapterId", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public Int64 SaveThankYouContent(Int64 formId, string content, ref int status)
        {
            Int64 result = 0;
            try
            {
                _sqlP = new[]
                {
            new SqlParameter("@FormId", formId),
            new SqlParameter("@ThankYouContent", (object)content ?? DBNull.Value),
            new SqlParameter("@QStatus", 0)
        };
                _sqlP[2].Direction = ParameterDirection.Output;

                _dbAccess.SP_ExecuteScalar("USP_SaveFormThankYouContent", ref _sqlP);
                status = Convert.ToInt32(_sqlP[2].Value);
                result = status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Int64 SaveFormSubmission(Int64 formId, string submittedData, string submittedBy,
            string paymentStatus, string paymentTxnId, string paymentGateway, decimal? paymentAmount, string paymentCurrency,
            ref int status)
        {
            Int64 newSubmissionId = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@FormId", formId),
                    new SqlParameter("@SubmittedData", submittedData),
                    new SqlParameter("@SubmittedBy", (object)submittedBy ?? DBNull.Value),
                    new SqlParameter("@PaymentStatus", (object)paymentStatus ?? DBNull.Value),
                    new SqlParameter("@PaymentTxnId", (object)paymentTxnId ?? DBNull.Value),
                    new SqlParameter("@PaymentGateway", (object)paymentGateway ?? DBNull.Value),
                    new SqlParameter("@PaymentAmount", (object)paymentAmount ?? DBNull.Value),
                    new SqlParameter("@PaymentCurrency", (object)paymentCurrency ?? DBNull.Value),
                    new SqlParameter("@QStatus", 0)
                };
                _sqlP[8].Direction = ParameterDirection.Output;

                _dbAccess.SP_ExecuteScalar("USP_SaveFormSubmission", ref _sqlP);
                status = Convert.ToInt32(_sqlP[8].Value);
                newSubmissionId = status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return newSubmissionId;
        }

        public Int64 SaveFormSubmission(Int64 formId, string submittedData, string submittedBy, ref int status)
        {
            return SaveFormSubmission(formId, submittedData, submittedBy, null, null, null, null, null, ref status);
        }

        #endregion
    }
}