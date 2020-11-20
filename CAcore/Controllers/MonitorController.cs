using System;
using System.Collections.Generic;
using AutoMapper;
using CAcore.Data;
using CAcore.Dtos;
using CAcore.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CAcore.Controllers {

    [Route("/monitor")]
    [ApiController]
    public class MonitorController: ControllerBase 
    {
        private readonly ICAcoreRepo _repository; 
        private readonly IMapper _mapper; 
        public MonitorController(ICAcoreRepo repo, IMapper mapper) 
        {
            _repository = repo; 
            _mapper = mapper; 
        }

        [HttpGet]
        public ActionResult<IEnumerable<CertificatesStatusReadDto>> GetCertificateStatus(string uid) 
        {
            var status = new CertificatesStatusReadDto();
            var certs = _repository.GetAllCertificates();

            // Manually map the results to the dto
            status.NumberOfIssuedCertificates = certs.Count().ToString();
            status.NumberOfRevokedCertificates = certs.Count() > 0 ? certs.Count(x => x.Revoked).ToString() : "0";
            status.CurrentSerialNumber = certs.Count() > 0 ? certs.OrderBy(x => x.SerialInDecimal).LastOrDefault().CertId : "0";
            return Ok(status);
        }
    }
}