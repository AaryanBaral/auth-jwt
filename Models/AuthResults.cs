using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Auth.Models
{
    public class AuthResults
    {
        public string? Token { get; set; }
        public bool? Result { get; set; }
        public List<string>? Errors { get; set; }
    }
}