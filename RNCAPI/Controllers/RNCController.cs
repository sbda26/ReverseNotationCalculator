using Microsoft.AspNetCore.Mvc;
using RNC.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RNCAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RNCController : ControllerBase
    {
        // GET: api/<RNCController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RNCController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RNCController>
        [HttpPost]
        public ActionResult<double?> Post([FromBody] ReverseNotationCalculatorClass rnc)
        {
            var calc = new CalculateClass();
            try
            {
                double? result = calc.Calculate(rnc);
                return result;
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex.ToString());
            }
        }

        // PUT api/<RNCController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RNCController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
