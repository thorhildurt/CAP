

using System;
using System.Collections.Generic;
using AutoMapper;
using CAcore.Data;
using CAcore.Dtos;
using CAcore.Models;
using Microsoft.AspNetCore.Mvc;
using CAcore.Helpers;

namespace CAcore.Controllers {
    [Route("/users/{uid}/certificates")]
    [ApiController]

    public class CertificateController: ControllerBase {
        private readonly ICAcoreRepo _repository; 
        private readonly IMapper _mapper; 
        private readonly FileHelper _fileHelper;
        public CertificateController(ICAcoreRepo repo, IMapper mapper) 
        {
            _repository = repo; 
            _mapper = mapper; 
            _fileHelper = new FileHelper();
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserCertificateReadDto>> GetAllUserCertificates(string uid) 
        {
            var certs = _repository.GetAllUserCertificates(uid);
            return Ok(_mapper.Map<IEnumerable<UserCertificateReadDto>>(certs));
        }

        [HttpPost]
        public ActionResult<UserCertificateReadDto> CreateAndDownloadCertificate(string uid) 
        {
            UserCertificate cert = _repository.CreateUserCertificate(uid);
            if (cert == null) 
            {
                return BadRequest("Failed to create user certificate. Check that user exists and that the root certificate is in the cert store.");
            }
            
            if(_repository.SaveChanges()) 
            {

                var file = _fileHelper.CreateAndWriteToFile(cert);
                UserCertificateReadDto readDto = _mapper.Map<UserCertificateReadDto>(cert); 
                // Return a file
                return CreatedAtRoute(nameof(GetUserCertificate), new {uid = uid, cid = cert.CertId}, readDto);
            }

            return BadRequest("Failed to save certificate to database");
        }

        [HttpGet("{cid}", Name = "GetUserCertificate")]
        public ActionResult<IEnumerable<UserCertificate>> GetUserCertificate(string uid, string cid) 
        {
            var cert = _repository.GetUserCertificate(uid, cid);
            if (cert != null) 
            {
                return Ok(_mapper.Map<UserCertificateReadDto>(cert));
            }
            return NotFound(); 
        }

        [HttpPut("{cid}/revoke")]
        public ActionResult RevokeCertificate(string uid, string cid) 
        {
            var cert = _repository.GetUserCertificate(uid, cid);
            if (cert == null) 
            {
                return BadRequest("Certificate does not exist");
            }
   
            _repository.RevokeUserCertificate(uid, cid);
            if(_repository.SaveChanges()) {
                return Ok("Certificate successfully revoked");
            }
            return BadRequest("Failed to revoke certificate");
        }
    }
}