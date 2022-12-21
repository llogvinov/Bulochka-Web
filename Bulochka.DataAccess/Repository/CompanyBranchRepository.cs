using Bulochka.DataAccess.Repository.IRepository;
using Bulochka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulochka.DataAccess.Repository
{
    public class CompanyBranchRepository : Repository<CompanyBranch>, ICompanyBranchRepository
    {
        private ApplicationDbContext _db;

        public CompanyBranchRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CompanyBranch branch)
        {
            _db.CompanyBranches.Update(branch);
        }
    }
}
