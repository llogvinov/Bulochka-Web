using Bulochka.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulochka.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Product = new ProductRepository(_db);
            CompanyBranch = new CompanyBranchRepository(_db);
        }

        public IProductRepository Product { get; private set; }
        public ICompanyBranchRepository CompanyBranch { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
