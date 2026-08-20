using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ArjunFormBuilder.Entities
{
    public class FormModel
    {
        public Int64 RId { get; set; }
        public Int64 FormId { get; set; }
        public string Title { get; set; }
        public string FormSchema { get; set; }
        public Int64 ChapterId { get; set; } = 1;
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string LogoUrl { get; set; }
        public int? LogoWidth { get; set; }   
        public int? LogoHeight { get; set; }   
        public string DesignJson { get; set; } 
        public string ThankYouContent { get; set; } 
    }
    public class FormSaveRequest
    {
        public int? FormId { get; set; }
        public string Title { get; set; }
        public string Schema { get; set; }
        public string LogoUrl { get; set; }
        public int? LogoWidth { get; set; }   
        public int? LogoHeight { get; set; }  
        public string Design { get; set; }    
    }

    public class FormSubmitRequest
    {
        public Dictionary<string, object> Data { get; set; }
        public PaymentSubmitInfo Payment { get; set; }
    }
    public class PaymentSubmitInfo
    {
        public string Status { get; set; }    
        public string TxnId { get; set; }     
        public string Gateway { get; set; }   
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
    }
}