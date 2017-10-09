using System;
using System.Collections.Generic;

namespace CRM.DAL.Entities
{
    public partial class Templates
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplatePath { get; set; }
    }
}
