using DevAna.Business.Intefaces;
using DevAna.Business.Models;
using DevAna.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DevAna.Data.Repository
{
    public class FornecedorRepository : Repository<Fornecedor>, IFornecedorRepository
    {
        public FornecedorRepository(MeuDbContext context) : base(context)
        {
        }

        public async Task<Fornecedor> ObterFornecedorEndereco(Guid id) => 
            await Db.Fornecedores.AsNoTracking()
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Fornecedor> ObterFornecedorProdutosEndereco(Guid id) => 
            await Db.Fornecedores.AsNoTracking()
                .Include(c => c.Produtos)
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.Id == id);
    }
}