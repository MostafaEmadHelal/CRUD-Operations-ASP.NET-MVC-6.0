using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Demo.PL.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        //private readonly IDepartmentRepository departmentRepository;
        private readonly ILogger<DepartmentController> logger;
        private readonly IMapper mapper;

        public DepartmentController(
            //IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            ILogger<DepartmentController> logger,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            //this.departmentRepository = departmentRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var departments = unitOfWork.DepartmentRepository.GetAll();
            return View(mapper.Map<IEnumerable<DepartmentViewModel>>(departments));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new DepartmentViewModel());
        }

        [HttpPost]
        public IActionResult Create(DepartmentViewModel departmentViewModel)
        {
            if (ModelState.IsValid)
            {
                var department = mapper.Map<Department>(departmentViewModel);

                unitOfWork.DepartmentRepository.Add(department);
                unitOfWork.Complete();

                TempData["MessageTemp"] = "Department Added Successfully!";

                return RedirectToAction(nameof(Index));
            }
            return View(departmentViewModel);
        }

        public IActionResult Details(int? id)
        {
            try
            {
                if (id is null)
                    return BadRequest();

                var department = unitOfWork.DepartmentRepository.GetById(id);

                if (department is null)
                    return NotFound();

                return View(mapper.Map<DepartmentViewModel>(department));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpGet]
        public IActionResult Update(int? id)
        {
            try
            {
                if (id == null)
                    return BadRequest();

                var department = unitOfWork.DepartmentRepository.GetById(id);

                if (department == null)
                    return NotFound();

                ViewBag.Departments = new SelectList(unitOfWork.DepartmentRepository.GetAll(), "Id", "Name");
                return View(mapper.Map<DepartmentViewModel>(department));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public IActionResult Update(DepartmentViewModel departmentViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var department = mapper.Map<Department>(departmentViewModel);

                    unitOfWork.DepartmentRepository.Update(department);
                    unitOfWork.Complete();

                    return RedirectToAction(nameof(Index));
                }

                return View(departmentViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                    return BadRequest();

                var department = unitOfWork.DepartmentRepository.GetById(id);

                if (department == null)
                    return NotFound();

                return View(mapper.Map<DepartmentViewModel>(department));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var department = unitOfWork.DepartmentRepository.GetById(id);

                if (department == null)
                    return NotFound();

                unitOfWork.DepartmentRepository.Delete(department);
                unitOfWork.Complete();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }



    }
}
