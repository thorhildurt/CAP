

using System;
using System.Collections.Generic;
using AutoMapper;
using CAcore.Data;
using CAcore.Dtos;
using CAcore.Models;
using Microsoft.AspNetCore.Mvc;

namespace CAcore.Controllers {
    [Route("/users/{uid}/certificates")]
    [ApiController]

    public class CertificateController: ControllerBase {
        private readonly ICAcoreRepo _repository; 
        private readonly IMapper _mapper; 
        public CertificateController(ICAcoreRepo repo, IMapper mapper) {
            _repository = repo; 
            _mapper = mapper; 
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserCertificateReadDto>> GetAllUserCertificates(string uid) {
            
            var certs = _repository.GetAllUserCertificates(uid);
            System.Console.WriteLine(certs);
            return Ok(_mapper.Map<IEnumerable<UserCertificateReadDto>>(certs));
            
        }

        [HttpPost]
        public ActionResult<UserCertificateReadDto> CreateCertificate(string uid) {
            Console.WriteLine("Create...");
            UserCertificate cert = _repository.CreateUserCertificate(uid);
            if (cert == null) {
                return null;
            }
            _repository.SaveChanges();
            UserCertificateReadDto readDto = _mapper.Map<UserCertificateReadDto>(cert); 
            Console.WriteLine("Create...");
            return CreatedAtRoute(nameof(GetUserCertificate), new {uid = uid, cid = cert.CertId}, readDto);
        }

        [HttpGet("{cid}", Name = "GetUserCertificate")]
        public ActionResult<IEnumerable<UserCertificate>> GetUserCertificate(string uid, string cid) {
            var cert = _repository.GetUserCertificate(uid, cid);
            return Ok(cert); 
        }



    }

}