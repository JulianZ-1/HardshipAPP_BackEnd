using HardshipAPI.Models;
using HardshipAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardshipAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HardshipsController : ControllerBase
    {
        private readonly IHardshipService _hardshipServices;
        private readonly IDebtService _debtService;

        public HardshipsController(IHardshipService hardshipService, IDebtService debtService)
        {
            _hardshipServices = hardshipService;
            _debtService = debtService;
        }

        /// (1) POST: Insert a new Hardship record.
        [HttpPost()]
        public async Task<ActionResult> CreateHardship([FromBody] HardshipManagementInsert request)
        {
            try
            {
                bool debtHasHardship = await _hardshipServices.DoesDebtHaveHardshipAsync(request.DebtID);

                if (debtHasHardship)
                {
                    return Problem($"DebtID {request.DebtID} already has a hardship.");
                }

                var hardshipDTO = new HardshipInsert {
                    Comments = request.Comments,
                    HardshipTypeID = request.HardshipTypeID,
                    DebtID = request.DebtID,
                };
                var hardship = await _hardshipServices.AddHardship(hardshipDTO);
                if (hardship > 0)
                {
                    var debtDTO = new Debt
                    {
                        DebtID = request.DebtID,
                        Name = request.Name,
                        DOB = request.DOB,
                        Income = request.Income,
                        Expenses = request.Expenses,
                    };
                    var debt = await _debtService.UpdateDebt(debtDTO);
                    if(debt > 0)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }

                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return Problem("An error occurred while creating a hardship.");
            }
        }

        [HttpPut("edit/{debtId}")]
        public async Task<ActionResult> UpdateHardship(
                 [FromRoute] long debtId,
                 [FromBody] HardshipManagementUpdate request)
        {
            try
            {
                // Verify existing hardship
                var existingHardship = await _hardshipServices.GetHardshipByDebtIdAsync(debtId);
                if (existingHardship == null)
                {
                    return NotFound($"No hardship found for DebtID={debtId}");
                }

                // Update hardship
                var hardshipResult = await _hardshipServices.UpdateHardship(new HardshipUpdate
                {
                    HardshipID = existingHardship.HardshipID,
                    HardshipTypeID = request.HardshipTypeID,
                    Comments = request.Comments
                });

                if (hardshipResult <= 0)
                {
                    return BadRequest("Failed to update hardship record.");
                }

                // Update debt
                var debtResult = await _debtService.UpdateDebt(new Debt
                {
                    DebtID = debtId,
                    Name = request.Name,
                    DOB = request.DOB,
                    Income = request.Income,
                    Expenses = request.Expenses
                });

                return debtResult > 0
                    ? Ok()
                    : BadRequest("Hardship updated but debt update failed.");
            }
            catch (Exception ex)
            {
                return Problem($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet()]
        public async Task<ActionResult> ViewAllHardships()
        {
            try
            {
                var allHardships = await _hardshipServices.ViewAllHardShips();
                if (allHardships.Length > 0)
                {
                    return Ok(allHardships);
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return Problem("An error occurred while creating a hardship.");
            }
        }

        [HttpGet("get-debt/{debtId}")]
        public async Task<ActionResult> GetHardshipByDebtId(long debtId)
        {
            try
            {
                var hardship = await _hardshipServices.GetHardshipByDebtIdAsync(debtId);
                if (hardship != null)
                {
                    return Ok(hardship);
                }
                else
                {
                    return NotFound($"No hardship found for DebtID={debtId}");
                }
            }
            catch (Exception ex)
            {
                return Problem($"An error occurred: {ex.Message}");
            }
        }
    }
}
