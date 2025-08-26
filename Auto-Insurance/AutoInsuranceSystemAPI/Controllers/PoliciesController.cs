using AutoInsuranceSystemAPI.Models;
using AutoInsuranceSystemAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoInsuranceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/admin/policies")]
    public class PoliciesController : ControllerBase
    {
        private readonly IPolicyService policyService;

        public PoliciesController(IPolicyService policyService)
        {
            this.policyService = policyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Policy>>> GetAll()
        {
            var items = await policyService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Policy>> GetById(int id)
        {
            var policy = await policyService.GetByIdAsync(id);
            if (policy == null) return NotFound();
            return Ok(policy);
        }

        [HttpPost]
        public async Task<ActionResult<Policy>> Create([FromBody] Policy policy)
        {
            var created = await policyService.CreateAsync(policy);
            return CreatedAtAction(nameof(GetById), new { id = created.PolicyId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Policy policy)
        {
            var ok = await policyService.UpdateAsync(id, policy);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await policyService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
} 