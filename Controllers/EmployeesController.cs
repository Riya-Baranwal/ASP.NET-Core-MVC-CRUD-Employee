using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCCRUD.Data;
using MVCCRUD.Models;

namespace MVCCRUD.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeDbContext _dbContext;

        public EmployeesController(EmployeeDbContext empDbContext)
        {
            _dbContext = empDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _dbContext.Employees.ToListAsync();
            return View(employees);

        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeView addNewEmployee)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addNewEmployee.Name,
                Email = addNewEmployee.Email,
                Phone = addNewEmployee.Phone,
                Department = addNewEmployee.Department,
                DOJ = addNewEmployee.DOJ,
                
            };
            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ViewEmployee(Guid id)
        {

            var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee != null)
            {
                var viewModel = new UpdateEmployeeView()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    Department = employee.Department,
                    DOJ = employee.DOJ
                };
                return await Task.Run(() => View("ViewEmployee", viewModel));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ViewEmployee(UpdateEmployeeView model)
        {
            var employee = await _dbContext.Employees.FindAsync(model.Id);

            if (employee != null)
            {
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Phone = model.Phone;
                employee.Department = model.Department;
                employee.DOJ = model.DOJ;

                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(UpdateEmployeeView model)
        {
            var employee = await _dbContext.Employees.FindAsync(model.Id);

            if (employee != null)
            {
                _dbContext.Employees.Remove(employee);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
