using System;

namespace CAwebapp.Models
{
    public class CreateCertResponse
    {   
        public string Message { get; set; } 

        public bool success { get; set; }

        public string cid { get; set; }

        public byte[] certBodyPkcs12 { get; set; }

    }
}