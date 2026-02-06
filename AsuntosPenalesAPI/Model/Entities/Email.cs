using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace AsuntosPenalesAPI.Model.Entities
{
    public class Email    
   {
        public List<string> To { get; set; } = new();
        public List<string> Cc { get; set; } = new();
        public List<string> Bcc { get; set; } = new();
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public bool IsHtml { get; set; } = true;
        public MailPriority? Priority { get; set; }
        public List<EmailAttachment> Attachments { get; set; } = new();
    }
      public class EmailAttachment
    {
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string MediaType { get; set; } = MediaTypeNames.Application.Octet;
    }
}