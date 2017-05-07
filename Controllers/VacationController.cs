using System;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using occ.Entities;
using occ.Models;
using occ.Repositories;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace occ.Controllers
{

    [Route("api/vacation")]
    public class VacationController : Controller
    {
        private const string ETag = "ETag";
        private const string IfMatch = "If-Match";

        [HttpGet]
        public IActionResult GetAllVacations()
        {
            var vacations = VacationRepository.GetAllVacations();
            if (vacations.Count == 0) { return NotFound("There is no vacations registered"); }

            return new OkObjectResult(vacations);
        }

        [HttpGet("{id:Guid}")]
        public IActionResult GetVacation(Guid id)
        {
            var vacation = VacationRepository.GetVacation(id);
            if (vacation == null) { return NotFound($"Vacation with id - [{id}] does not exist"); }

            AddHeader(HttpContext, ETag, GetHash(vacation));

            return Ok(vacation);
        }

        [HttpPost]
        public IActionResult AddVacation([FromBody]VacationUpsertModel vacation)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            var vacationToInsert = MapToVacation(vacation);

            VacationRepository.AddVacation(vacationToInsert);
            AddHeader(HttpContext, ETag, GetHash(vacationToInsert));

            return Ok(vacationToInsert);
        }

        [HttpPut("{id:Guid}")]
        public IActionResult UpdateVacation(Guid id, [FromBody]VacationUpsertModel vacation)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            if (!HttpContext.Request.Headers.Keys.Contains(IfMatch)) { return BadRequest($"{IfMatch} header needs to be specified"); }

            var receivedHash = HttpContext.Request.Headers[IfMatch].ToString();
            var vacationToUpdate = MapToVacation(vacation, id);
            var updateObjectHash = GetHash(vacationToUpdate);

            if (receivedHash.Equals(updateObjectHash)) { return StatusCode((int)HttpStatusCode.NotModified, "No changes found"); }

            var currentVacation = VacationRepository.GetVacation(id);
            var currentVacationHash = GetHash(currentVacation);
            if (!receivedHash.Equals(currentVacationHash)) { return StatusCode((int)HttpStatusCode.Conflict); }

            if (!VacationRepository.UpdateVacation(vacationToUpdate)) { return NotFound(); }

            AddHeader(HttpContext, ETag, updateObjectHash);

            return Ok(vacationToUpdate);
        }

        private Vacation MapToVacation(VacationUpsertModel vacation) => MapToVacation(vacation, Guid.NewGuid());

        private Vacation MapToVacation(VacationUpsertModel vacation, Guid id)
        {
            return new Vacation()
            {
                Id = id,
                Name = vacation.Name,
                Description = vacation.Description,
                StartDate = vacation.StartDate
            };
        }
        private string GetHash(Vacation vacation)
        {
            var sha = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(vacation));
            var hash = BitConverter.ToString(sha.ComputeHash(bytes))
                                   .Replace("-", "");
            return hash;
        }
        private void AddHeader(HttpContext context, string headerName, string value) =>
            context.Response.Headers.Add(headerName, value);
    }
}