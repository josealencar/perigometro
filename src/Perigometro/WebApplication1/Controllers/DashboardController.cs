using Perigometro.Dominio1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: Teste
        public ActionResult TesteGrafico()
        {
            return View();
        }

        //GET: Estatisticas
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Estatisticas(List<int> anos)
        {
            int anoInicial = anos.ElementAt(0);
            int anoFinal = anos.ElementAt(1);
            var acidentes = db.Acidentes
                .Where(_ => _.Ano >= anoInicial && _.Ano <= anoFinal).ToList();

            #region Mortes
            var mortes = acidentes
                .Where(_ => _.Fatais > 0)
                .GroupBy(_ => _.Regiao)
                .Select(_ => new { 
                    Regiao = _.Key,
                    Qtde = _.Count()
                })
                .OrderByDescending(_ => _.Qtde);
            #endregion
            #region Clima
            var clima = acidentes
                .GroupBy(_ => _.Tempo)
                .Select(_ => new {
                    Clima = _.Key,
                    Qtde = _.Count()
                })
                .OrderByDescending(_ => _.Qtde)
                .Take(1);
            #endregion
            #region Regiao
            var regiao = acidentes
                .GroupBy(_ => _.Regiao)
                .Select(_ => new
                {
                    Regiao = _.Key,
                    Qtde = _.Count()
                })
                .OrderByDescending(_ => _.Qtde)
                .Take(1);               
            #endregion
            #region Dia
            var dia = acidentes
                .GroupBy(_ => _.Dia_Sem)
                .Select(_ => new 
                {
                    Dia = _.Key,
                    Qtde = _.Count()
                })
                .OrderByDescending(_ => _.Qtde)
                .Take(1);
            #endregion
            #region Veiculos
            var auto = new VeiculoCount
            { 
                Nome = "Automóvel",
                Qtde = acidentes
                       .Where(_ => _.Auto > 0)
                       .Count()
            };
            var taxi = new VeiculoCount
            {
                Nome = "Taxi",
                Qtde = acidentes
                       .Where(_ => _.Taxi > 0)
                       .Count()
            };
            var lotacao = new VeiculoCount
            {
                Nome = "Lotação",
                Qtde = acidentes
                       .Where(_ => _.Lotacao > 0)
                       .Count()
            };
            var onibus = new VeiculoCount
            {
                Nome = "Ônibus",
                Qtde = acidentes
                       .Where(_ => _.Onibus_Int > 0 || _.Onibus_Met > 0 || _.Onibus_Urb > 0)
                       .Count()
            };
            var caminhao = new VeiculoCount
            {
                Nome = "Caminhão",
                Qtde = acidentes
                       .Where(_ => _.Caminhao > 0)
                       .Count()
            };
            var moto = new VeiculoCount
            {
                Nome = "Moto",
                Qtde = acidentes
                       .Where(_ => _.Moto > 0)
                       .Count()
            };
            var bicicleta = new VeiculoCount
            {
                Nome = "Bicicleta",
                Qtde = acidentes
                       .Where(_ => _.Bicicleta > 0)
                       .Count()
            };
            

            List<VeiculoCount> veiculos = new List<VeiculoCount>();
            veiculos.Add(auto);
            veiculos.Add(taxi);
            veiculos.Add(lotacao);
            veiculos.Add(onibus);
            veiculos.Add(caminhao);
            veiculos.Add(moto);
            veiculos.Add(bicicleta);

            List<VeiculoCount> dashes = getMaiorMenor(veiculos);
            VeiculoCount seguro = dashes.ElementAt(1);
            VeiculoCount inseguro = dashes.ElementAt(0);
            #endregion            


            var jsonResult = Json(new {Clima = clima, Regiao = regiao, Dia = dia, Mortes = mortes, Seguro = seguro, Inseguro = inseguro}, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        private List<VeiculoCount> getMaiorMenor(List<VeiculoCount> veiculos)
        {
            List<VeiculoCount> dashes = new List<VeiculoCount>();
            var maior = new VeiculoCount
            {
                Nome = "",
                Qtde = 0
            };
            var menor = veiculos.ElementAt(0);
            foreach (var veiculo in veiculos)
            {
                if (veiculo.Qtde > maior.Qtde) maior = veiculo;
                if (veiculo.Qtde < menor.Qtde) menor = veiculo;
            }
            dashes.Add(maior);
            dashes.Add(menor);
            return dashes;
        }
    }
}