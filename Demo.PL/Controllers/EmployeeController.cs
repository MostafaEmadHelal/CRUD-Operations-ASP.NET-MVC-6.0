using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace Demo.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public EmployeeController(IUnitOfWork unitOfWork , IMapper mapper) {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public IActionResult Index(string SearchValue = "")
        {
            IEnumerable<Employee> employees;
            IEnumerable<EmployeeViewModel> employeesViewModel;


            if (string.IsNullOrEmpty(SearchValue))
            {
                employees = unitOfWork.EmployeeRepository.GetAll();
                employeesViewModel = mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
            }
            else
            {
                employees = unitOfWork.EmployeeRepository.Search(SearchValue);
                employeesViewModel = mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
            }
              return View(employeesViewModel);
            
        }

        [HttpGet]
        public IActionResult Create() 
        {
            ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();

            return View(new EmployeeViewModel());
        }

        [HttpPost]

        public IActionResult Create(EmployeeViewModel employeeViewModel)
        {
            //ModelState["Department"].ValidationState = ModelValidationState.Valid;
            if (ModelState.IsValid)
            {
                //// Manual Mapping
                //Employee employee = new Employee
                //{
                //    Name = employeeViewModel.Name,
                //    Email = employeeViewModel.Email,
                //    Address = employeeViewModel.Address,
                //    DepartmentId = employeeViewModel.DepartmentId,
                //    HireDate = employeeViewModel.HireDate,
                //    Salary = employeeViewModel.Salary,
                //    IsActive = employeeViewModel.IsActive
                //};

                var employee = mapper.Map<Employee>(employeeViewModel);

                employee.ImageUrl = Path.Combine(DocumentSettings.UploadFile(employeeViewModel.Image, "Images"));

                unitOfWork.EmployeeRepository.Add(employee);

                unitOfWork.Complete();

                return RedirectToAction("Index");
            }           

            ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();

            return View(employeeViewModel);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var employee = unitOfWork.EmployeeRepository.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeViewModel = mapper.Map<EmployeeViewModel>(employee);

            return View(employee);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var employee = unitOfWork.EmployeeRepository.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeViewModel = mapper.Map<EmployeeViewModel>(employee);
            ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();
            return View(employeeViewModel);
        }

        [HttpPost]
        public IActionResult Update(EmployeeViewModel employeeViewModel)
        {
            if (ModelState.IsValid)
            {
                var employee = mapper.Map<Employee>(employeeViewModel);

                
                if (employeeViewModel.Image != null)
                {
                    if (!string.IsNullOrEmpty(employee.ImageUrl))
                    {
                        var existingImagePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/Files/Images", employee.ImageUrl);
                        if (System.IO.File.Exists(existingImagePath))
                        {
                            System.IO.File.Delete(existingImagePath);
                        }
                    }

                    employee.ImageUrl = DocumentSettings.UploadFile(employeeViewModel.Image, "Images");
                }

                unitOfWork.EmployeeRepository.Update(employee);
                unitOfWork.Complete();

                return RedirectToAction("Index");
            }

            ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();
            return View(employeeViewModel);
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            var employee = unitOfWork.EmployeeRepository.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }
            var employeeViewModel = mapper.Map<EmployeeViewModel>(employee);

            return View(employeeViewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var employee = unitOfWork.EmployeeRepository.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", "Images", employee.ImageUrl);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            unitOfWork.EmployeeRepository.Delete(employee);
            unitOfWork.Complete();

            TempData["MessageTemp"] = "Employee deleted successfully.";

            return RedirectToAction("Index");
        }
    }

    // DTO => Data Transfer Object
}
