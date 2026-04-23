using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using crud_it15.Data;
using crud_it15.Models;

namespace crud_it15.Controllers
{
    public class PayrollsController : Controller
    {
        private readonly AppDbContext _context;

        public PayrollsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Payrolls
        public async Task<IActionResult> Index(int? employeeId)
        {
            // Build employee dropdown for filter
            var employees = await _context.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToListAsync();
            ViewBag.EmployeeList = new SelectList(employees, "EmployeeId", "FullName", employeeId);
            ViewBag.SelectedEmployeeId = employeeId;

            var payrolls = _context.Payrolls.Include(p => p.Employee).AsQueryable();

            if (employeeId.HasValue)
            {
                payrolls = payrolls.Where(p => p.EmployeeId == employeeId.Value);
            }

            var list = await payrolls.OrderByDescending(p => p.Date).ToListAsync();

            // Summary totals per employee (or filtered)
            ViewBag.TotalGross = list.Sum(p => p.GrossPay);
            ViewBag.TotalDeduction = list.Sum(p => p.Deduction);
            ViewBag.TotalNet = list.Sum(p => p.NetPay);

            return View(list);
        }

        // GET: Payrolls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);

            if (payroll == null) return NotFound();

            return View(payroll);
        }

        // GET: Payrolls/Create
        public async Task<IActionResult> Create(int? employeeId)
        {
            var employees = await _context.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToListAsync();
            ViewBag.EmployeeId = new SelectList(employees, "EmployeeId", "FullName", employeeId);
            return View(new Payroll { Date = DateTime.Today, EmployeeId = employeeId ?? 0 });
        }

        // POST: Payrolls/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Date,DaysWorked,Deduction")] Payroll payroll)
        {
            // Remove computed fields from validation
            ModelState.Remove("GrossPay");
            ModelState.Remove("NetPay");
            ModelState.Remove("Employee");

            if (ModelState.IsValid)
            {
                // Fetch employee to calculate GrossPay
                var employee = await _context.Employees.FindAsync(payroll.EmployeeId);
                if (employee == null)
                {
                    ModelState.AddModelError("EmployeeId", "Selected employee not found.");
                }
                else
                {
                    payroll.GrossPay = payroll.DaysWorked * employee.DailyRate;
                    payroll.NetPay = payroll.GrossPay - payroll.Deduction;

                    _context.Add(payroll);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payroll record was added successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var employees = await _context.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToListAsync();
            ViewBag.EmployeeId = new SelectList(employees, "EmployeeId", "FullName", payroll.EmployeeId);
            return View(payroll);
        }

        // GET: Payrolls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null) return NotFound();

            var employees = await _context.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToListAsync();
            ViewBag.EmployeeId = new SelectList(employees, "EmployeeId", "FullName", payroll.EmployeeId);
            return View(payroll);
        }

        // POST: Payrolls/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayrollId,EmployeeId,Date,DaysWorked,Deduction")] Payroll payroll)
        {
            if (id != payroll.PayrollId) return NotFound();

            ModelState.Remove("GrossPay");
            ModelState.Remove("NetPay");
            ModelState.Remove("Employee");

            if (ModelState.IsValid)
            {
                var employee = await _context.Employees.FindAsync(payroll.EmployeeId);
                if (employee == null)
                {
                    ModelState.AddModelError("EmployeeId", "Selected employee not found.");
                }
                else
                {
                    payroll.GrossPay = payroll.DaysWorked * employee.DailyRate;
                    payroll.NetPay = payroll.GrossPay - payroll.Deduction;

                    try
                    {
                        _context.Update(payroll);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Payroll record was updated successfully.";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PayrollExists(payroll.PayrollId)) return NotFound();
                        else throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }

            var employees = await _context.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToListAsync();
            ViewBag.EmployeeId = new SelectList(employees, "EmployeeId", "FullName", payroll.EmployeeId);
            return View(payroll);
        }

        // GET: Payrolls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);

            if (payroll == null) return NotFound();

            return View(payroll);
        }

        // POST: Payrolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll != null)
            {
                _context.Payrolls.Remove(payroll);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Payroll record was deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(e => e.PayrollId == id);
        }
    }
}
