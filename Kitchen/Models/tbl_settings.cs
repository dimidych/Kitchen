//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcStudy.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_settings
    {
        public int id { get; set; }
        public string ckey { get; set; }
        public string BaseHostAddress { get; set; }
        public string MailHost { get; set; }
        public string MailHostLogin { get; set; }
        public string MailHostPwd { get; set; }
        public Nullable<int> MailHostPort { get; set; }
        public Nullable<bool> MailHostUseSsl { get; set; }
        public string MailSubjectPattern { get; set; }
        public string MailBodyPattern { get; set; }
        public string CookieName { get; set; }
    }
}