﻿using Demo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        //Department GetById(int? id);

        //IEnumerable<Department> GetAll();

        //int Add(Department employee);

        //int Update(Department employee);

        //int Delete(Department employee);
    }
}
