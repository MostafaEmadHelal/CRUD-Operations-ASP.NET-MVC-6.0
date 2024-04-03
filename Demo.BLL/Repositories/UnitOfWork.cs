using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;

        public IDepartmentRepository DepartmentRepository { get; set;}
        public IEmployeeRepository EmployeeRepository { get; set; }

        public UnitOfWork(AppDbContext context)
        {
            EmployeeRepository = new EmployeeRepository(context);
            DepartmentRepository = new DepartmentRepository(context);
            this.context = context;
        }

        public int Complete()
        {

            return context.SaveChanges();
        }
    }

    
}
