using AutoMapper;
using DevAna.Api.ViewModels;
using DevAna.Business.Intefaces;
using DevAna.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevAna.Api.Controllers
{
    [Route("api/produtos")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(
            IProdutoRepository produtoRepository,
            IProdutoService produtoService,
            IMapper mapper,
            INotificador notificador) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> GettAll() =>
            _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> GetById(Guid id)
        {
            var produtoViewModel = await GetProduto(id);

            if (produtoViewModel == null) return NotFound();

            return produtoViewModel;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Post([FromBody] ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgNome = $"{Guid.NewGuid()}_{produtoViewModel.Imagem} ";

            if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgNome)) return CustomResponse(produtoViewModel);

            produtoViewModel.Imagem = imgNome;
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        [HttpDelete("id:guid")]
        public async Task<ActionResult<ProdutoViewModel>> Delete(Guid id)
        {
            var produtoViewModel = await GetProduto(id);

            if (produtoViewModel == null) return NotFound();

            await _produtoService.Remover(id);

            return CustomResponse(produtoViewModel);
        }
        
        private async Task<ProdutoViewModel> GetProduto(Guid id) =>
            _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));

        private async Task<bool> UploadArquivo(string arquivo, string imgNome)
        {
            var imageDataByteArray = Convert.FromBase64String(arquivo);

            if(string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe uma imagem com este nome!");
                return false;
            }

            await System.IO.File.WriteAllBytesAsync(filePath, imageDataByteArray);

            return true;
        }
    }
}
