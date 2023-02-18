using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulochka.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductRepository Product { get; }
        ICompanyBranchRepository CompanyBranch { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        void Save();
    }
}
