

using System;
using System.Collections.Generic;
using AutoMapper;
using CAcore.Data;
using CAcore.Dtos;
using CAcore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace CAcore.Controllers {
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("/user/certificates")]
    [ApiController]

    public class CertificateController: ControllerBase {
        private readonly ICAcoreRepo _repository; 
        private readonly IMapper _mapper; 
        public CertificateController(ICAcoreRepo repo, IMapper mapper) 
        {
            _repository = repo; 
            _mapper = mapper; 
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserCertificateReadDto>> GetAllUserCertificates() {
            ClaimsPrincipal currentUser = this.User;
            var uid = currentUser.FindFirst(ClaimTypes.Name).Value;

            var certs = _repository.GetAllUserCertificates(uid);
            return Ok(_mapper.Map<IEnumerable<UserCertificateReadDto>>(certs));
        }

        [HttpPost]
        public ActionResult<UserCertificateReadDto> CreateCertificate() {
            ClaimsPrincipal currentUser = this.User;
            var uid = currentUser.FindFirst(ClaimTypes.Name).Value;

            UserCertificate cert = _repository.CreateUserCertificate(uid);
            if (cert == null) 
            {
                return BadRequest(new { message = "Error! Failed to create user certificate. Check that user exists and that the root certificate is in the cert store.", success = false });
            }
            
            if(_repository.SaveChanges()) 
            {
                UserCertificateReadDto readDto = _mapper.Map<UserCertificateReadDto>(cert); 
                return Ok(new {message = "Success! Certificate created", success = true});
            }

            return BadRequest(new { message = "Error! Failed to save certificate to database", success = false });
        }

        [HttpGet("{cid}", Name = "GetUserCertificate")]
        public ActionResult<IEnumerable<UserCertificate>> GetUserCertificate(string cid) {
            ClaimsPrincipal currentUser = this.User;
            var uid = currentUser.FindFirst(ClaimTypes.Name).Value;

            var cert = _repository.GetUserCertificate(uid, cid);
            if (cert != null) 
            {
                return Ok(_mapper.Map<UserCertificateReadDto>(cert));
            }
            return NotFound(); 
        }

        [HttpPut("{cid}/revoke")]
        public ActionResult RevokeCertificate(string cid) {
            ClaimsPrincipal currentUser = this.User;
            var uid = currentUser.FindFirst(ClaimTypes.Name).Value;

            var cert = _repository.GetUserCertificate(uid, cid);
            if (cert == null) 
            {
                return BadRequest(new { message = "Error! Certificate does not exist", success = false });
            }

            _repository.RevokeUserCertificate(uid, cid);
            if(_repository.SaveChanges()) {
                return Ok(new { message = "Success! Certificate revoked", success = true });
            }
            return BadRequest(new { message = "Error! Failed to revoke certificate", success = false });
        }
    }
}