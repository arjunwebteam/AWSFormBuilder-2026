using ArjunFormBuilder.Areas.Admin.Models;
using ArjunFormBuilder.BLL;
using ArjunFormBuilder.DAL;
using ArjunFormBuilder.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using ClosedXML.Excel;
using System.Security.Claims;

namespace ArjunFormBuilder.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FormController : Controller
    {
        BLL.FormBLL _formBLL = new BLL.FormBLL();
        DAL.FormDAL _DFormSubmissions = new DAL.FormDAL();
        Entities.MailTemplates objMailTemplates = new Entities.MailTemplates();


        BLL.MailTemplates _mailTemplatesBLL = new BLL.MailTemplates();

        [Authorize]
        public ActionResult FormBuilder(Int64 id = 0)
        {
            ViewBag.FormId = id;

            if (id > 0)
            {
                int status = 0;
                var form = _formBLL.GetFormSchema(id, ref status);
                if (status == 1 && form != null)
                {
                    ViewBag.FormTitle = form.Title;
                    ViewBag.FormSchema = form.FormSchema;
                    ViewBag.LogoUrl = form.LogoUrl;
                    ViewBag.LogoWidth = form.LogoWidth;
                    ViewBag.LogoHeight = form.LogoHeight;
                    ViewBag.FormDesign = form.DesignJson;

                }
            }

            return View();
        }
        [Authorize]
        public ActionResult EditThankYouPage(Int64 id)
        {
            int status = 0;
            var form = _formBLL.GetFormSchema(id, ref status);
            if (status != 1 || form == null) return NotFound();

            ViewBag.FormId = id;
            ViewBag.FormTitle = form.Title;
            ViewBag.ThankYouContent = form.ThankYouContent;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditThankYouPage(Int64 FormId, string ThankYouContent)
        {
            int status = 0;
            try
            {
                _formBLL.SaveThankYouContent(FormId, ThankYouContent, ref status);
                TempData["message"] = status > 0
                    ? "<div class=\"alert alert-success\">Thank You Page updated successfully</div>"
                    : "<div class=\"alert alert-danger\">Failed to update Thank You Page</div>";
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger\">" + ex.Message + "</div>";
            }
            return RedirectToAction("EditThankYouPage", new { id = FormId }); 
        }

        [Authorize]
        [HttpPost]

        public JsonResult UploadFormLogo(IFormFile logo)
        {
            try
            {
                if (logo == null || logo.Length == 0)
                    return Json(new { success = false, message = "No file selected" });

                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                var objappinfo = _appinfo.GetAppInfoDetails(ref status1);

                string fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(logo.FileName);

                // Save path stays the same — this is correct
                string normalPath = Path.Combine(objappinfo.UploadPath, "FormLogos", "NormalImages", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(normalPath));

                using (var stream = new FileStream(normalPath, FileMode.Create))
                {
                    logo.CopyTo(stream);
                }

                string relativeUrl = objappinfo.BaseUrl + "Content/FormLogos/NormalImages/" + fileName;

                return Json(new { success = true, url = relativeUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult UploadFormTermsPdf(IFormFile termsPdf)
        {
            try
            {
                if (termsPdf == null || termsPdf.Length == 0)
                    return Json(new { success = false, message = "No file selected" });

                string ext = Path.GetExtension(termsPdf.FileName).ToLower();
                if (ext != ".pdf")
                    return Json(new { success = false, message = "Only PDF files are allowed" });

                int status1 = 0;
                BLL.AppInfo _appinfo = new BLL.AppInfo();
                var objappinfo = _appinfo.GetAppInfoDetails(ref status1);

                string fileName = Guid.NewGuid().ToString("N") + ext;

                string normalPath = Path.Combine(objappinfo.UploadPath, "uploads", "FormTerms", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(normalPath));

                using (var stream = new FileStream(normalPath, FileMode.Create))
                {
                    termsPdf.CopyTo(stream);
                }

                string relativeUrl = objappinfo.BaseUrl + "Content/uploads/FormTerms/" + fileName;

                return Json(new { success = true, url = relativeUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveFormSchema([FromBody] FormSaveRequest request)
        {
            int status = 0;
            try
            {
                string createdBy = HttpContext.User.Identity.Name;
                Int64 chapterId = (HttpContext.Session.GetString("chapterid") != null
                    ? Convert.ToInt64(HttpContext.Session.GetString("chapterid"))
                    : 1);

                Int64 formId = _formBLL.SaveFormSchema(request, createdBy, chapterId, ref status);

                return Json(new { success = formId > 0, formId = formId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ================= List page =================
        [Authorize]
        public ActionResult Index(Int64 ChapterId = 0)
        {
            ViewBag.ChapterId = ChapterId;
            return View();
        }

        [Authorize]
        public ActionResult FormsList(Int64 ChapterId = 0, string Search = "", string SortColumn = "CreatedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 25)
        {
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "");
            int Total = 0;
            List<FormModel> lstForms = new List<FormModel>();

            try
            {
                lstForms = _formBLL.GetFormsListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);
            }
            catch (Exception ex)
            {
                TempData["message"] = "<div class=\"alert alert-danger\">" + ex.Message + "</div>";
            }

            ViewBag.ChapterId = ChapterId;
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;
            ViewBag.lstForms = lstForms;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult FormStatus(Int64 FormId)
        {
            int status = 0;
            try
            {
                _formBLL.UpdateFormStatus(FormId, ref status);
                if (status == 1)
                    return Json(new { ok = true, messageType = "success", message = "Status updated successfully" });
                else
                    return Json(new { ok = false, messageType = "error", message = "Failed to update status" });
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteForm(Int64 FormId)
        {
            int status = 0;
            try
            {
                _formBLL.DeleteForm(FormId, ref status);
                if (status == 1)
                    return Json(new { ok = true, messageType = "success", message = "Form deleted successfully" });
                else
                    return Json(new { ok = false, messageType = "error", message = "Failed to delete form" });
            }
            catch
            {
                return Json(new { ok = false, messageType = "error", message = "Failed transaction." });
            }
        }

        [Authorize]
        public ActionResult RenderForm(Int64 id)
        {
            int status = 0;
            var form = _formBLL.GetFormSchema(id, ref status);
            if (status != 1 || form == null) return NotFound();

            try
            {
                var fields = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(form.FormSchema);
                var paymentField = fields?.FirstOrDefault(f =>
                    f.TryGetProperty("type", out var t) && t.GetString() == "payment");

                if (paymentField.HasValue && paymentField.Value.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                {
                    var pf = paymentField.Value;
                    ViewBag.PaymentEnabled = true;
                    ViewBag.PaymentGateway = pf.TryGetProperty("gateway", out var g) ? g.GetString() : "";
                    ViewBag.PaymentPublicKey = pf.TryGetProperty("publicKey", out var pk) ? pk.GetString() : "";
                    ViewBag.PaymentMode = pf.TryGetProperty("gatewayMode", out var m) ? m.GetString() : "sandbox";
                    ViewBag.PaymentCurrency = pf.TryGetProperty("currency", out var c) ? c.GetString() : "USD";
                }
                else
                {
                    ViewBag.PaymentEnabled = false;
                }

                var captchaField = fields?.FirstOrDefault(f =>
                    f.TryGetProperty("type", out var ct) && ct.GetString() == "captcha");

                if (captchaField.HasValue && captchaField.Value.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                {
                    var cf = captchaField.Value;
                    ViewBag.CaptchaEnabled = true;
                    ViewBag.CaptchaType = cf.TryGetProperty("captchaType", out var cty) ? cty.GetString() : "hcaptcha";
                    ViewBag.CaptchaSiteKey = cf.TryGetProperty("captchaSiteKey", out var csk) ? csk.GetString() : "";
                }
                else
                {
                    ViewBag.CaptchaEnabled = false;
                }
            }
            catch
            {
                ViewBag.PaymentEnabled = false; 
                ViewBag.CaptchaEnabled = false;
            }


            return View(form);
        }
        [HttpPost]
        public ActionResult SubmitForm(Int64 formId, [FromBody] Entities.FormSubmitRequest request)
        {
            int status = 0;
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== SUBMIT FORM CALLED ===");
                System.Diagnostics.Debug.WriteLine($"FormId: {formId}");
                System.Diagnostics.Debug.WriteLine($"Request is null: {request == null}");

                if (request != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Data count: {request.Data?.Count ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"Payment is null: {request.Payment == null}");

                    if (request.Payment != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Payment Status: {request.Payment.Status}");
                        System.Diagnostics.Debug.WriteLine($"Payment TxnId: {request.Payment.TxnId}");
                        System.Diagnostics.Debug.WriteLine($"Payment Gateway: {request.Payment.Gateway}");
                        System.Diagnostics.Debug.WriteLine($"Payment Amount: {request.Payment.Amount}");
                        System.Diagnostics.Debug.WriteLine($"Payment Currency: {request.Payment.Currency}");
                    }
                }

                var submittedData = request?.Data ?? new Dictionary<string, object>();
                string json = System.Text.Json.JsonSerializer.Serialize(submittedData);
                string submittedBy = HttpContext.User?.Identity?.Name ?? "anonymous";

                Int64 submissionId = _formBLL.SaveFormSubmission(
                    formId,
                    json,
                    submittedBy,
                    request?.Payment?.Status,
                    request?.Payment?.TxnId,
                    request?.Payment?.Gateway,
                    request?.Payment?.Amount,
                    request?.Payment?.Currency,
                    ref status);

                // ✅ ADDED — mail-template send, only runs after a successful save.
                if (submissionId > 0)
                {
                    try
                    {
                        int status1 = 0;
                        var formIsActive = _formBLL.GetFormSchema(formId, ref status1);

                        var fieldValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var kvp in submittedData)
                        {
                            fieldValues[kvp.Key] = kvp.Value?.ToString() ?? "";
                        }

                        if (request?.Payment != null)
                        {
                            if (!string.IsNullOrEmpty(request.Payment.Status)) fieldValues["PaymentStatus"] = request.Payment.Status;
                            if (!string.IsNullOrEmpty(request.Payment.TxnId)) fieldValues["PaymentTxnId"] = request.Payment.TxnId;
                            if (!string.IsNullOrEmpty(request.Payment.Gateway)) fieldValues["PaymentGateway"] = request.Payment.Gateway;
                            if (request.Payment.Amount.HasValue) fieldValues["PaymentAmount"] = request.Payment.Amount.Value.ToString("0.00");
                            if (!string.IsNullOrEmpty(request.Payment.Currency)) fieldValues["PaymentCurrency"] = request.Payment.Currency;
                        }
                        fieldValues["SubmissionId"] = submissionId.ToString();
                        fieldValues["SubmittedDate"] = DateTime.Now.ToString("MM/dd/yyyy");


                        string submitterEmail = FindEmailValue(fieldValues);
                        int status2 = 0;
                        var objMailTemplates = _mailTemplatesBLL.GetMailTemplateByFormId(formId, ref status1);

                        string adminEmail = objMailTemplates.BCC;
                        string siteBaseUrl = $"{Request.Scheme}://{Request.Host}";

                        if (formIsActive.IsActive == true)
                        {
                            if (!string.IsNullOrEmpty(submitterEmail) || !string.IsNullOrEmpty(adminEmail))
                            {
                                _mailTemplatesBLL.SendTemplateMailForForm(formId, fieldValues, submitterEmail, adminEmail, siteBaseUrl);
                            }
                        }
                    }
                    catch (Exception mailEx)
                    {

                        System.Diagnostics.Debug.WriteLine($"[SubmitForm] Mail send failed: {mailEx.Message}");
                    }
                }

                return Json(new { success = submissionId > 0, submissionId = submissionId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in SubmitForm: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = ex.Message });
            }
        }


        private string FindEmailValue(Dictionary<string, string> fieldValues)
        {
            string[] commonEmailKeys = { "Email", "EmailAddress", "UserEmail", "ContactEmail" };
            foreach (var key in commonEmailKeys)
            {
                if (fieldValues.TryGetValue(key, out string val) && !string.IsNullOrWhiteSpace(val))
                    return val;
            }
            return null;
        }

        [Authorize]

        public ActionResult FormSubmissions(Int64 formId, string Search = "", string SortColumn = "SubmittedDate", string SortOrder = "DESC", int PageNo = 1, int Items = 25)
        {
            int status = 0;
            var form = _formBLL.GetFormSchema(formId, ref status);
            if (status != 1 || form == null) return NotFound();

            var schemaFields = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(form.FormSchema);

            var summaryFields = schemaFields
                .Where(f => f["type"].ToString() != "heading" && f["type"].ToString() != "paragraph")
                .Take(3)
                .Select(f => new { Id = f["id"].ToString(), Label = f["label"].ToString() })
                .ToList();

            ViewBag.FormId = formId;
            ViewBag.FormTitle = form.Title; 
            ViewBag.SummaryFields = summaryFields;
            ViewBag.sortcolumn = SortColumn;
            ViewBag.sortorder = SortOrder.ToLower();

            int Total = 0;
            string Sort = (SortColumn != "" ? SortColumn + " " + SortOrder : "SubmittedDate DESC");

            var submissions = _formBLL.GetFormSubmissionsList(formId, Search, Sort, PageNo, Items, ref Total);

            ViewBag.lstSubmissions = submissions;
            ViewBag.total = Total;
            ViewBag.pageno = PageNo;
            ViewBag.items = Items;

            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult GetSubmissionDetail(Int64 id)
        {
            try
            {
                int status = 0;

                var submission = _formBLL.GetFormSubmissionDetail(id, ref status);

                if (submission == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Submission not found."
                    });
                }

                return Json(new
                {
                    success = true,

                    submissionId = submission.SubmissionId,
                    formId = submission.FormId,
                    submittedBy = submission.SubmittedBy,
                    submittedDate = submission.SubmittedDate.ToString("dd/MM/yyyy hh:mm tt"),
                    data = submission.SubmittedData,

                    // Payment details
                    paymentStatus = submission.PaymentStatus,
                    paymentTxnId = submission.PaymentTxnId,
                    paymentGateway = submission.PaymentGateway,
                    paymentAmount = submission.PaymentAmount,
                    paymentCurrency = submission.PaymentCurrency
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        public ActionResult ThankYou(Int64 id)
        {
            int status = 0;
            var form = _formBLL.GetFormSchema(id, ref status);
            ViewBag.LogoUrl = (form != null ? form.LogoUrl : null);
            ViewBag.Title = (form != null ? form.Title : "Form");
            ViewBag.ThankYouContent = (form != null ? form.ThankYouContent : null); 
            return View();
        }

        [Authorize]
        [Authorize]
        public IActionResult FormSubmissionsExportToExcel(string search = "", string sortColumn = "SubmittedDate", string sortOrder = "DESC", long formId = 0)
        {
            try
            {
                string sort = !string.IsNullOrEmpty(sortColumn)
                                ? $"{sortColumn} {sortOrder}"
                                : "";

                // 1. Get DataTable from DAL (Filtered by formId)
                DataTable dtRaw = _DFormSubmissions.FormSubmissionsExportToExcel(search, sort, formId);

                if (dtRaw == null || dtRaw.Rows.Count == 0)
                {
                    TempData["message"] = $"Export failed: No records found.";
                    return RedirectToAction("FormSubmissions", new { formId = formId });
                }


                DataTable dtFinal = new DataTable();
                dtFinal.Columns.Add("Submitted By", typeof(string));
                dtFinal.Columns.Add("Submitted Date", typeof(string));

                bool columnsAdded = false;

                foreach (DataRow row in dtRaw.Rows)
                {
                    string jsonData = row["SubmittedData"].ToString();


                    var submissionData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

                    if (submissionData != null && !columnsAdded)
                    {
                        foreach (var key in submissionData.Keys)
                        {
                            if (!dtFinal.Columns.Contains(key))
                            {
                                dtFinal.Columns.Add(key, typeof(string));
                            }
                        }
                        columnsAdded = true;
                    }

                    DataRow newRow = dtFinal.NewRow();
                    newRow["Submitted By"] = row["SubmittedBy"];
                    newRow["Submitted Date"] = Convert.ToDateTime(row["SubmittedDate"]).ToString("dd/MM/yyyy hh:mm tt");

                    if (submissionData != null)
                    {
                        foreach (var kvp in submissionData)
                        {
                            if (dtFinal.Columns.Contains(kvp.Key))
                            {
                                newRow[kvp.Key] = kvp.Value?.ToString() ?? "";
                            }
                        }
                    }
                    dtFinal.Rows.Add(newRow);
                }

                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("FormSubmissions-Export");

                    for (int col = 0; col < dtFinal.Columns.Count; col++)
                    {
                        var headerCell = worksheet.Cell(1, col + 1);
                        headerCell.Value = dtFinal.Columns[col].ColumnName;

                        headerCell.Style.Font.Bold = true;
                        headerCell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromArgb(173, 216, 230);
                    }

                    for (int row = 0; row < dtFinal.Rows.Count; row++)
                    {
                        for (int col = 0; col < dtFinal.Columns.Count; col++)
                        {
                            worksheet.Cell(row + 2, col + 1).Value = dtFinal.Rows[row][col]?.ToString() ?? "";
                        }
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        int status = 0;
                        var form = _formBLL.GetFormSchema(formId, ref status);
                        string formTitle = form != null ? form.Title : "Form";
                        string safeTitle = string.Join("_", formTitle.Split(Path.GetInvalidFileNameChars()));

                        string fileName = $"FormSubmissions-{safeTitle}-{DateTime.UtcNow:dd-MM-yyyy}.xlsx";

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Export failed: {ex.Message}";
                return RedirectToAction("FormSubmissions", new { formId = formId });
            }
        }
        public class AuthorizeAttribute : ActionFilterAttribute
        {
            BLL.Users _user = new BLL.Users();
            BLL.Roles _Roles = new BLL.Roles();

            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                string userRole = null;
                int status = 0;

                var user = filterContext.HttpContext.User;

                if (user?.Identity != null && user.Identity.IsAuthenticated)
                {
                    userRole = user.FindFirst(ClaimTypes.Role)?.Value;
                    string emailFromClaim = user.FindFirst(ClaimTypes.Email)?.Value;

                    if (!string.IsNullOrEmpty(emailFromClaim))
                    {
                        var objuser = _user.GetAdminUsersGetByEmail(emailFromClaim, ref status);

                        if (objuser != null)
                        {
                            filterContext.HttpContext.Session.SetString("UserName", objuser.UserName ?? "");
                            filterContext.HttpContext.Session.SetString("UserId", objuser.UserId.ToString());
                            filterContext.HttpContext.Session.SetString("UserEmail", emailFromClaim ?? "");
                            filterContext.HttpContext.Session.SetString("chapterid", objuser.ChapterId.ToString());
                            filterContext.HttpContext.Session.SetString("userrole", userRole ?? "");
                        }
                    }
                }

                if (string.IsNullOrEmpty(userRole))
                {
                    filterContext.Result = new RedirectToActionResult("LogOn", "Account", new { area = "Admin" });
                    return;
                }

                int roleStatus = 0;
                List<Entities.Roles> lstRoles = _Roles.GetRolesList(ref roleStatus);

                List<string> allowedRoles = lstRoles.Select(r => r.RoleName.Trim()).ToList();

                var userRoles = userRole
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .ToList();

                bool isAuthorized = userRoles.Any(r => allowedRoles.Contains(r));

                if (!isAuthorized)
                {
                    filterContext.Result = new RedirectToActionResult("Unauthorized", "Account", new { area = "Admin" });
                    return;
                }

                base.OnActionExecuting(filterContext);
            }
        }
    }
}